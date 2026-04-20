using Content.Server._Nineveh.GameTicking.Components;
using Content.Server.Communications;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Rules;
using Content.Shared.GameTicking.Components;

namespace Content.Server._Nineveh.GameTicking;

public sealed class NinevehRoundTimerRuleSystem : GameRuleSystem<NinevehRoundTimerRuleComponent>
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CommunicationConsoleCallShuttleAttemptEvent>(OnShuttleCallAttempt);
    }

    protected override void Started(EntityUid uid,
        NinevehRoundTimerRuleComponent component,
        GameRuleComponent gameRule,
        GameRuleStartedEvent args)
    {
        base.Started(uid, component, gameRule, args);

        component.StartTime = Timing.CurTime;
        component.EndTime = component.StartTime + component.RoundDuration;
        component.RoundEnding = false;
    }

    protected override void ActiveTick(EntityUid uid,
        NinevehRoundTimerRuleComponent component,
        GameRuleComponent gameRule,
        float frameTime)
    {
        base.ActiveTick(uid, component, gameRule, frameTime);

        if (component.RoundEnding || Timing.CurTime < component.EndTime)
            return;

        component.RoundEnding = true;
        GameTicker.EndRound(Loc.GetString("rule-time-has-run-out"));
    }

    protected override void AppendRoundEndText(EntityUid uid,
        NinevehRoundTimerRuleComponent component,
        GameRuleComponent gameRule,
        ref RoundEndTextAppendEvent args)
    {
        args.AddLine(Loc.GetString("nineveh-round-timer-round-end-summary",
            ("duration", component.RoundDuration.ToString(@"hh\:mm\:ss"))));
    }

    private void OnShuttleCallAttempt(ref CommunicationConsoleCallShuttleAttemptEvent ev)
    {
        var query = QueryActiveRules();
        while (query.MoveNext(out _, out _, out var timer, out _))
        {
            if (timer.RoundEnding || Timing.CurTime >= timer.EndTime)
                continue;

            var remaining = timer.EndTime - Timing.CurTime;
            ev.Cancelled = true;
            ev.Reason = Loc.GetString("nineveh-round-timer-shuttle-unavailable",
                ("time", remaining.ToString(@"hh\:mm\:ss")));
            return;
        }
    }
}
