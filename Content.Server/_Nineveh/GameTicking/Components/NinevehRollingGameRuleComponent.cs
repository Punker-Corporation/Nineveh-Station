using Content.Server._Nineveh.Antag;
using Content.Shared.EntityTable;
using Robust.Shared.Prototypes;

namespace Content.Server._Nineveh.GameTicking.Components;

/// <summary>
/// Drives the custom Nineveh rolling-mode start flow.
/// </summary>
[RegisterComponent, Access(typeof(Rules.NinevehRollingGameRuleSystem), typeof(NinevehAntagSelectionSystem))]
public sealed partial class NinevehRollingGameRuleComponent : Component
{
    [DataField("pairTable", required: true)]
    public ProtoId<EntityTablePrototype> PairTable;

    [ViewVariables]
    public EntProtoId? SelectedPairSpawner;

    [ViewVariables]
    public bool PairSelectionComplete;

    [ViewVariables]
    public bool CivilianHandoffComplete;
}
