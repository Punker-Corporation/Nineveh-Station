using Content.Server.Chat.Systems;
using Content.Shared.Chat;
using Content.Shared.Radio;
using Content.Shared.Radio.Components;
using Content.Shared.Speech.Components;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server.Radio.EntitySystems;

public sealed partial class RadioSystem : EntitySystem
{
    [Dependency] private readonly ChatSystem _chat = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly IRobustRandom _robustRandom = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    private TimeSpan _nextWhisperGlobal = TimeSpan.Zero;
    private readonly Dictionary<string, int> _recentWordCounts = new();
    private TimeSpan _lastWordCleanup = TimeSpan.Zero;

    public override void Initialize()
    {
        base.Initialize();
    }

    public void SendRadioMessage(
        EntityUid messageSource,
        string message,
        RadioChannelPrototype channel,
        EntityUid radioSource,
        bool escapeMarkup = true)
    {
        var sendAttempt = new RadioSendAttemptEvent(channel, radioSource);
        RaiseLocalEvent(ref sendAttempt);
        if (sendAttempt.Cancelled)
            return;

        if (!TryApplyCorruption(messageSource, ref message, radioSource))
            return;

        var speech = _chat.GetSpeechVerb(messageSource, message);
        var nameEv = new TransformSpeakerNameEvent(messageSource, Name(messageSource));
        RaiseLocalEvent(messageSource, nameEv);

        var speakerName = nameEv.VoiceName;
        if (nameEv.SpeechVerb != null && _prototypeManager.Resolve(nameEv.SpeechVerb, out var overrideVerb))
            speech = overrideVerb;

        var wrappedMessage = Loc.GetString(
            speech.Bold ? "chat-radio-message-wrap-bold" : "chat-radio-message-wrap",
            ("color", channel.Color),
            ("fontType", speech.FontId),
            ("fontSize", speech.FontSize),
            ("verb", Loc.GetString(_robustRandom.Pick(speech.SpeechVerbStrings))),
            ("channel", $"[{channel.LocalizedName}]"),
            ("name", FormattedMessage.EscapeText(speakerName)),
            ("message", escapeMarkup ? FormattedMessage.EscapeText(message) : message));

        var chatMessage = new ChatMessage(
            ChatChannel.Radio,
            message,
            wrappedMessage,
            GetNetEntity(messageSource),
            null);

        var netMessage = new MsgChatMessage { Message = chatMessage };
        var receivers = new List<EntityUid>();

        var sourceMap = Transform(radioSource).MapUid;
        var query = EntityQueryEnumerator<ActiveRadioComponent, TransformComponent>();
        while (query.MoveNext(out var uid, out var activeRadio, out var xform))
        {
            var canReceiveChannel = activeRadio.ReceiveAllChannels || activeRadio.Channels.Contains(channel.ID);
            if (!canReceiveChannel)
                continue;

            var crossMapAllowed = activeRadio.GlobalReceive || channel.LongRange;
            if (!crossMapAllowed && (sourceMap == null || xform.MapUid != sourceMap))
                continue;

            var receiveAttempt = new RadioReceiveAttemptEvent(channel, radioSource, uid);
            RaiseLocalEvent(ref receiveAttempt);
            if (receiveAttempt.Cancelled)
                continue;

            var ev = new RadioReceiveEvent(message, messageSource, channel, radioSource, netMessage, receivers);
            RaiseLocalEvent(uid, ref ev);
        }

        RaiseLocalEvent(new RadioSpokeEvent(messageSource, message, receivers.ToArray()));
    }

    public void SendRadioMessage(
        EntityUid messageSource,
        string message,
        ProtoId<RadioChannelPrototype> channelId,
        EntityUid radioSource,
        bool escapeMarkup = true)
    {
        SendRadioMessage(messageSource, message, _prototypeManager.Index(channelId), radioSource, escapeMarkup);
    }

    private bool TryApplyCorruption(
        EntityUid messageSource,
        ref string message,
        EntityUid radioSource,
        CorruptedRadioComponent? corruption = null)
    {
        if (!Resolve(radioSource, ref corruption, false))
            return true;

        var random = _robustRandom;

        if (random.Prob(corruption.SilenceChance))
            return false;

        if (random.Prob(corruption.DeadAirChance))
        {
            message = "[estática intensa]";
            return true;
        }

        if (random.Prob(corruption.SpoofChance))
        {
            _ = random.Pick(corruption.SpoofNames);
        }

        if (random.Prob(corruption.CorruptionChance))
        {
            message = ApplyCorruptionFilter(message, corruption);
        }

        if (random.Prob(corruption.JamaisVuChance))
        {
            message = ApplyJamaisVu(message, corruption);
        }

        ProcessSemanticSatiation(message, corruption);

        if (random.Prob(corruption.InfrasoundChance))
        {
            TriggerInfrasound(radioSource);
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
        _audio.PlayPvs("/Audio/Effects/infrasound_rumble.ogg", source);
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
