using Content.Server.Radio.Components;
using Content.Shared.Radio;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server.Radio.EntitySystems;

public sealed partial class RadioSystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    private TimeSpan _nextWhisperGlobal = TimeSpan.Zero;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (_timing.CurTime < _nextWhisperGlobal)
            return;

        _nextWhisperGlobal = _timing.CurTime + TimeSpan.FromSeconds(_random.Next(45, 120));

        var query = EntityQueryEnumerator<CorruptedRadioComponent, RadioSpeakerComponent, ActiveRadioComponent>();
        while (query.MoveNext(out var uid, out var corruption, out var speaker, out _))
        {
            if (!_random.Prob(corruption.WhisperChance))
                continue;

            if (!speaker.Enabled)
                continue;

            var message = _random.Pick(corruption.WhisperMessages);
            var spoofName = _random.Pick(corruption.SpoofNames);

            var channel = speaker.Channels.FirstOrDefault();
            if (channel == default)
                continue;

            SendRadioMessage(uid, message, channel, uid, escapeMarkup: false);
        }
    }
}
