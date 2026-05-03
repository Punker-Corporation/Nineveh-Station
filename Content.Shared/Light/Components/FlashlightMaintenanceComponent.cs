using Robust.Shared.GameStates;
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
    public float LensIntegrity = 1f;

    [DataField, AutoNetworkedField]
    public float EmitterIntegrity = 1f;

    [DataField, AutoNetworkedField]
    public float ContactIntegrity = 1f;

    [DataField, AutoNetworkedField]
    public float HeatSinkIntegrity = 1f;

    [DataField]
    public float WideRadius = 5.1f;

    [DataField]
    public float FocusedRadius = 11.4f;

    [DataField]
    public float WideEnergy = 1.35f;

    [DataField]
    public float FocusedEnergy = 3.35f;

    [DataField]
    public float WideFalloff = 4.2f;

    [DataField]
    public float FocusedFalloff = 8.6f;

    [DataField]
    public float Softness = 0.52f;

    [DataField]
    public float HeatPerSecond = 1.65f;

    [DataField]
    public float FocusHeatMultiplier = 2.35f;

    [DataField]
    public float CoolingPerSecond = 3.35f;

    [DataField]
    public float HeatCapacity = 100f;

    [DataField]
    public float OverheatThreshold = 96f;

    [DataField]
    public float ResumeThreshold = 42f;

    [DataField]
    public float ContactFlickerThreshold = 0.72f;

    [DataField]
    public float BatteryFocusCost = 0.82f;

    [DataField, AutoNetworkedField]
    public float CalibrationTarget = 0.63f;
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
public sealed class FlashlightMaintenanceBoundUserInterfaceState(
    float focus,
    float heat,
    bool overheated,
    float lensIntegrity,
    float emitterIntegrity,
    float contactIntegrity,
    float heatSinkIntegrity,
    float calibrationTarget,
    float projectedRadius,
    float projectedEnergy)
    : BoundUserInterfaceState
{
    public float Focus = focus;
    public float Heat = heat;
    public bool Overheated = overheated;
    public float LensIntegrity = lensIntegrity;
    public float EmitterIntegrity = emitterIntegrity;
    public float ContactIntegrity = contactIntegrity;
    public float HeatSinkIntegrity = heatSinkIntegrity;
    public float CalibrationTarget = calibrationTarget;
    public float ProjectedRadius = projectedRadius;
    public float ProjectedEnergy = projectedEnergy;
}

[Serializable, NetSerializable]
public sealed class FlashlightSetFocusMessage(float focus) : BoundUserInterfaceMessage
{
    public float Focus { get; } = focus;
}

[Serializable, NetSerializable]
public sealed class FlashlightServiceModuleMessage(FlashlightModule module, float calibration) : BoundUserInterfaceMessage
{
    public FlashlightModule Module { get; } = module;
    public float Calibration { get; } = calibration;
}
