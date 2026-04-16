using Content.Shared.Radio;
using Robust.Server.GameObjects;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server.Radio;

public sealed partial class RadioSystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;

    private TimeSpan _nextWhisperTime = TimeSpan.Zero;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (_timing.CurTime < _nextWhisperTime)
            return;

        _nextWhisperTime = _timing.CurTime + TimeSpan.FromSeconds(_random.Next(60, 180));

        foreach (var (radio, corrupted) in EntityManager.EntityQuery<RadioMicrophoneComponent, CorruptedRadioComponent>())
        {
            if (!corrupted.WhisperMessages.Any())
                continue;

            if (!_random.Prob(corrupted.WhisperChance))
                continue;

            var message = _random.Pick(corrupted.WhisperMessages);
            var senderName = _random.Pick(corrupted.SpoofNames);

            var ev = new RadioReceiveEvent(
                message,
                senderName,
                radio.BroadcastChannel,
                EntityUid.Invalid);

            RaiseLocalEvent(radio.Owner, ref ev);
        }
    }

    private void OnSendRadioMessage(EntityUid uid, RadioMicrophoneComponent radio, ref RadioSendEvent args)
    {
        if (!TryComp<CorruptedRadioComponent>(uid, out var corruption))
            return;

        if (_random.Prob(corruption.DeadAirChance))
        {
            args.Message = "[estática intensa]";
            return;
        }

        if (_random.Prob(corruption.SpoofChance))
        {
            args.SenderName = _random.Pick(corruption.SpoofNames);
        }

        if (_random.Prob(corruption.CorruptionChance))
        {
            args.Message = CorruptMessage(args.Message);
        }
    }

    private string CorruptMessage(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var chars = input.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            if (_random.Prob(0.25f))
            {
                chars[i] = _random.Pick("!@#$%&*_?. ");
            }
            else if (_random.Prob(0.05f))
            {
                chars[i] = char.ToUpperInvariant(chars[i]);
            }
        }
        return new string(chars);
    }
}
