using System.Numerics;
using Robust.Shared.GameStates;
using Robust.Shared.Audio;
using Robust.Shared.Serialization;

namespace Content.Shared.Light.Components;

/// <summary>
/// Models the parts that make a handheld light readable as a physical object:
/// optics focus the beam, contacts add intermittent failures, the emitter heats up,
/// and the cell pays more current for a tighter cone. This keeps the horror light
/// visually strong without making it a cost-free floodlight.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class FlashlightMaintenanceComponent : Component
{
    [DataField, AutoNetworkedField]
    public float Focus = 0.72f;

    [DataField, AutoNetworkedField]
    public float Heat;

    [DataField, AutoNetworkedField]
    public bool Overheated;

    [DataField, AutoNetworkedField]
    public bool FaultLatched;

    [DataField, AutoNetworkedField]
    public FlashlightFault LastFault = FlashlightFault.None;

    [DataField, AutoNetworkedField]
    public FlashlightCircuitStep CircuitStep = FlashlightCircuitStep.Stable;

    [DataField, AutoNetworkedField]
    public float LensIntegrity = 1f;

    [DataField, AutoNetworkedField]
    public float EmitterIntegrity = 1f;

    [DataField, AutoNetworkedField]
    public float ContactIntegrity = 1f;

    [DataField, AutoNetworkedField]
    public float HeatSinkIntegrity = 1f;

    [DataField]
    public float WideRadius = 4.25f;

    [DataField]
    public float FocusedRadius = 8.5f;

    [DataField]
    public float WideEnergy = 1.15f;

    [DataField]
    public float FocusedEnergy = 2.65f;

    [DataField]
    public float WideFalloff = 4.2f;

    [DataField]
    public float FocusedFalloff = 8.6f;

    [DataField]
    public float Softness = 0.62f;

    [DataField]
    public Vector2 WideMaskScale = new(1.42f, 0.92f);

    [DataField]
    public Vector2 FocusedMaskScale = new(0.46f, 1.18f);

    [DataField]
    public float HeatPerSecond = 1.65f;

    [DataField]
    public float FocusHeatMultiplier = 1.2f;

    [DataField]
    public float CoolingPerSecond = 7.25f;

    [DataField]
    public float HeatCapacity = 100f;

    [DataField]
    public float OverheatThreshold = 92f;

    [DataField]
    public float ResumeThreshold = 48f;

    [DataField]
    public float ContactFlickerThreshold = 0.72f;

    [DataField]
    public float BatteryFocusCost = 0.82f;

    [DataField]
    public float InactiveCoolingMultiplier = 0.42f;

    [DataField]
    public float FailureCheckSeconds = 5.5f;

    [DataField]
    public float FailureCheckTimer;

    [DataField]
    public SoundSpecifier RepairSound = new SoundPathSpecifier("/Audio/Effects/multitool_pulse.ogg");

    [DataField]
    public SoundSpecifier FaultSound = new SoundPathSpecifier("/Audio/Effects/sparks2.ogg");

    [DataField]
    public SoundSpecifier SwitchSound = new SoundPathSpecifier("/Audio/Machines/button.ogg");
}

[Serializable, NetSerializable]
public enum FlashlightMaintenanceUiKey : byte
{
    Key,
}

[Serializable, NetSerializable]
public enum FlashlightModule : byte
{
    Lens,
    Emitter,
    Contacts,
    HeatSink,
}

[Serializable, NetSerializable]
public enum FlashlightFault : byte
{
    None,
    ContactDropout,
    ThermalRunaway,
    EmitterSag,
    GroundLeak,
}

[Serializable, NetSerializable]
public enum FlashlightCircuitStep : byte
{
    CheckContinuity,
    CorrectPolarity,
    BleedCapacitor,
    BridgeGround,
    MatchImpedance,
    CalibrateEmitter,
    Stable,
}

[Serializable, NetSerializable]
public enum FlashlightCircuitAction : byte
{
    ProbeContinuity,
    ReversePolarity,
    BleedCapacitor,
    BridgeGround,
    TrimResistor,
    CalibrateEmitter,
}

[Serializable, NetSerializable]
public sealed class FlashlightMaintenanceBoundUserInterfaceState(
    float focus,
    float heat,
    bool overheated,
    bool faultLatched,
    FlashlightFault lastFault,
    FlashlightCircuitStep circuitStep,
    float lensIntegrity,
    float emitterIntegrity,
    float contactIntegrity,
    float heatSinkIntegrity,
    float projectedRadius,
    float projectedEnergy)
    : BoundUserInterfaceState
{
    public float Focus = focus;
    public float Heat = heat;
    public bool Overheated = overheated;
    public bool FaultLatched = faultLatched;
    public FlashlightFault LastFault = lastFault;
    public FlashlightCircuitStep CircuitStep = circuitStep;
    public float LensIntegrity = lensIntegrity;
    public float EmitterIntegrity = emitterIntegrity;
    public float ContactIntegrity = contactIntegrity;
    public float HeatSinkIntegrity = heatSinkIntegrity;
    public float ProjectedRadius = projectedRadius;
    public float ProjectedEnergy = projectedEnergy;
}

[Serializable, NetSerializable]
public sealed class FlashlightSetFocusMessage(float focus) : BoundUserInterfaceMessage
{
    public float Focus { get; } = focus;
}

[Serializable, NetSerializable]
public sealed class FlashlightServiceModuleMessage(FlashlightModule module) : BoundUserInterfaceMessage
{
    public FlashlightModule Module { get; } = module;
}

[Serializable, NetSerializable]
public sealed class FlashlightCircuitActionMessage(FlashlightCircuitAction action) : BoundUserInterfaceMessage
{
    public FlashlightCircuitAction Action { get; } = action;
}
