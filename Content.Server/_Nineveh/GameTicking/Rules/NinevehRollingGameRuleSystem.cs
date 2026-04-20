using Content.Server.Antag.Components;
using Content.Server._Nineveh.Antag;
using Content.Server._Nineveh.GameTicking.Components;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Rules;
using Content.Shared.GameTicking;
using Content.Shared.GameTicking.Components;

namespace Content.Server._Nineveh.GameTicking.Rules;

public sealed class NinevehRollingGameRuleSystem : GameRuleSystem<NinevehRollingGameRuleComponent>
{
    [Dependency] private readonly GameTicker _gameTicker = default!;
    [Dependency] private readonly NinevehAntagSelectionSystem _ninevehAntag = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RulePlayerSpawningEvent>(OnRulePlayerSpawning);
        SubscribeLocalEvent<RoundEndedEvent>(OnRoundEnded);
    }

    private void OnRulePlayerSpawning(RulePlayerSpawningEvent args)
    {
        var query = EntityQueryEnumerator<NinevehRollingGameRuleComponent, GameRuleComponent, AntagSelectionComponent>();
        while (query.MoveNext(out var uid, out var rolling, out var gameRule, out var antagSelection))
        {
            if (!GameTicker.IsGameRuleActive(uid, gameRule) || rolling.CivilianHandoffComplete)
                continue;

            _ninevehAntag.TryRunNinevehRoundStart(uid, rolling, antagSelection, args.PlayerPool, args.Profiles);
        }
    }

    private void OnRoundEnded(RoundEndedEvent args)
    {
        _gameTicker.SetGamePreset("NinevehSurvival");
    }
}
