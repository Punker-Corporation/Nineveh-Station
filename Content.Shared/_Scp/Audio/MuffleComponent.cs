using Robust.Shared.GameStates;

namespace Content.Shared._Scp.Audio;

/// <summary>
/// Unified acoustic muffling component for entities and local zones.
/// It models masks, hands over mouths, walls, vents, bags, and any gameplay source that should absorb air and highs.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MuffleComponent : Component
{
    /// <summary>
    /// Normalized low-pass cutoff control. Lower values remove more high-frequency content.
    /// </summary>
    [DataField, AutoNetworkedField]
    public float Cutoff = 0.62f;

    /// <summary>
    /// Resonance added around the cutoff region. Kept bounded to avoid harsh filter ringing.
    /// </summary>
    [DataField, AutoNetworkedField]
    public float Resonance = 0.18f;

    /// <summary>
    /// Additional air absorption scalar applied as gain attenuation.
    /// </summary>
    [DataField, AutoNetworkedField]
    public float AirAbsorption = 0.25f;

    /// <summary>
    /// Wet/dry mix between direct sound and muffled transfer path.
    /// </summary>
    [DataField, AutoNetworkedField]
    public float Wet = 0.75f;
}
