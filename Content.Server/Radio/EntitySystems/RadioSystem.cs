using System.Reflection;
using Content.Shared.Chat;
using Content.Shared.Radio;
using HarmonyLib;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server.Radio.EntitySystems;

public sealed partial class RadioSystem
{
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly IRobustRandom _robustRandom = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    private TimeSpan _nextWhisperGlobal = TimeSpan.Zero;
    private readonly Dictionary<string, int> _recentWordCounts = new();
    private TimeSpan _lastWordCleanup = TimeSpan.Zero;

    public void InitializeCorruption()
    {
        var harmony = new Harmony("com.nineveh.radiocorruption");
        var original = typeof(RadioSystem).GetMethod("SendRadioMessage",
            new[] { typeof(EntityUid), typeof(string), typeof(RadioChannelPrototype), typeof(EntityUid), typeof(bool) });

        var prefix = typeof(RadioSystem).GetMethod(nameof(SendRadioMessagePrefix), BindingFlags.NonPublic | BindingFlags.Instance);
        harmony.Patch(original, new HarmonyMethod(prefix));
    }

    private static bool SendRadioMessagePrefix(
        EntityUid messageSource,
        ref string message,
        RadioChannelPrototype channel,
        EntityUid radioSource,
        bool escapeMarkup,
        RadioSystem __instance)
    {
        if (!__instance.TryComp<CorruptedRadioComponent>(radioSource, out var corruption))
            return true;

        var random = __instance._robustRandom;

        if (random.Prob(corruption.SilenceChance))
            return false;

        if (random.Prob(corruption.DeadAirChance))
        {
            message = "[estática intensa]";
            return true;
        }

        if (random.Prob(corruption.SpoofChance))
        {
            var spoofedName = random.Pick(corruption.SpoofNames);
            var nameEv = new TransformSpeakerNameEvent(messageSource, spoofedName);
            __instance.RaiseLocalEvent(messageSource, nameEv);
        }

        if (random.Prob(corruption.CorruptionChance))
        {
            message = __instance.ApplyCorruptionFilter(message, corruption);
        }

        if (random.Prob(corruption.JamaisVuChance))
        {
            message = __instance.ApplyJamaisVu(message, corruption);
        }

        __instance.ProcessSemanticSatiation(message, corruption);

        if (random.Prob(corruption.InfrasoundChance))
        {
            __instance.TriggerInfrasound(radioSource);
        }

        return true;
    }

    private string ApplyCorruptionFilter(string input, CorruptedRadioComponent corruption)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var chars = input.ToCharArray();
        var replacements = corruption.CorruptionReplacements.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            if (_robustRandom.Prob(corruption.CorruptionIntensity))
            {
                chars[i] = _robustRandom.Pick(replacements);
            }
            else if (_robustRandom.Prob(0.1f))
            {
                chars[i] = char.ToUpperInvariant(chars[i]);
            }
        }
        return new string(chars);
    }

    private string ApplyJamaisVu(string input, CorruptedRadioComponent corruption)
    {
        var words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < words.Length; i++)
        {
            if (words[i].Length > 3 && _robustRandom.Prob(0.3f))
            {
                words[i] = _robustRandom.Pick(corruption.WhisperMessages).Replace("...", "");
            }
        }
        return string.Join(' ', words);
    }

    private void ProcessSemanticSatiation(string message, CorruptedRadioComponent corruption)
    {
        var words = message.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        foreach (var word in words)
        {
            if (word.Length <= 3)
                continue;

            if (!_recentWordCounts.ContainsKey(word))
                _recentWordCounts[word] = 0;
            _recentWordCounts[word]++;

            if (_recentWordCounts[word] >= corruption.SemanticSatiationThreshold)
            {
                if (_robustRandom.Prob(0.5f))
                {
                    _recentWordCounts[word] = 0;
                }
            }
        }

        if (_gameTiming.CurTime - _lastWordCleanup > TimeSpan.FromSeconds(30))
        {
            _recentWordCounts.Clear();
            _lastWordCleanup = _gameTiming.CurTime;
        }
    }

    private void TriggerInfrasound(EntityUid source)
    {
        var filter = Filter.Pvs(source);
        _audio.PlayStatic("/Audio/Effects/infrasound_rumble.ogg", filter, source, false);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (_gameTiming.CurTime < _nextWhisperGlobal)
            return;

        _nextWhisperGlobal = _gameTiming.CurTime + TimeSpan.FromSeconds(_robustRandom.Next(45, 120));

        var query = EntityQueryEnumerator<CorruptedRadioComponent, RadioMicrophoneComponent, ActiveRadioComponent>();
        while (query.MoveNext(out var uid, out var corruption, out var microphone, out var activeRadio))
        {
            if (!_robustRandom.Prob(corruption.WhisperChance))
                continue;

            var channelId = microphone.BroadcastChannel;
            if (!_prototypeManager.TryIndex<RadioChannelPrototype>(channelId, out var channelProto))
                continue;

            var message = _robustRandom.Pick(corruption.WhisperMessages);
            var spoofName = _robustRandom.Pick(corruption.SpoofNames);

            var chatMessage = new ChatMessage(
                ChatChannel.Radio,
                message,
                Loc.GetString("chat-radio-message-wrap",
                    ("color", channelProto.Color),
                    ("fontType", "Default"),
                    ("fontSize", 12),
                    ("verb", "sussurra"),
                    ("channel", $"\\[{channelProto.LocalizedName}\\]"),
                    ("name", $"[???] {spoofName}"),
                    ("message", message)),
                GetNetEntity(uid),
                null);

            var ev = new RadioReceiveEvent(message, uid, channelProto, uid, new MsgChatMessage { Message = chatMessage }, new List<EntityUid>());
            RaiseLocalEvent(uid, ref ev);
        }
    }
}
