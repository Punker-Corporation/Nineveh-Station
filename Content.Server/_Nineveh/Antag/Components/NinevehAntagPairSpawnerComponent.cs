using Content.Server.Antag.Components;
using Content.Server.Spawners.Components;
using Content.Shared.Roles;
using Robust.Shared.Prototypes;

namespace Content.Server._Nineveh.Antag.Components;

/// <summary>
/// Stores the data for a single Nineveh antag/counter-antag pair scenario.
/// The component lives on a prototype selected by the Nineveh pair entity table.
/// </summary>
[RegisterComponent]
public sealed partial class NinevehAntagPairSpawnerComponent : Component
{
    [DataField("pairId", required: true)]
    public string PairId = string.Empty;

    [DataField("primaryMob", required: true)]
    public EntProtoId PrimaryMob;

    [DataField("secondaryMob", required: true)]
    public EntProtoId SecondaryMob;

    [DataField("primaryMarker")]
    public EntProtoId? PrimaryMarker;

    [DataField("secondaryMarker")]
    public EntProtoId? SecondaryMarker;

    [DataField("primaryFallbackJob")]
    public ProtoId<JobPrototype>? PrimaryFallbackJob;

    [DataField("secondaryFallbackJob")]
    public ProtoId<JobPrototype>? SecondaryFallbackJob;

    [DataField("primaryDefinition", required: true)]
    public AntagSelectionDefinition PrimaryDefinition = new();

    [DataField("secondaryDefinition", required: true)]
    public AntagSelectionDefinition SecondaryDefinition = new();
}
