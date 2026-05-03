using System.Numerics;
using Content.Shared.Examine;
using Content.Shared.Light.Components;
using Content.Shared.UserInterface;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Maths;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared.Light.EntitySystems;

/// <summary>
/// Applies the handheld-light "mechanical truth" layer. The renderer already supports cone masks;
/// this system drives those masks with optics, heat and contact state so the flashlight becomes a
/// survival tool instead of a binary sprite.
/// </summary>
public sealed class FlashlightMaintenanceSystem : EntitySystem
{
    [Dependency] private readonly SharedPointLightSystem _lights = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedUserInterfaceSystem _ui = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<FlashlightMaintenanceComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<FlashlightMaintenanceComponent, ExaminedEvent>(OnExamined);
        SubscribeLocalEvent<FlashlightMaintenanceComponent, GetVerbsEvent<ActivationVerb>>(OnGetActivationVerbs);
        SubscribeLocalEvent<FlashlightMaintenanceComponent, BoundUIOpenedEvent>(OnUiOpened);
        SubscribeLocalEvent<FlashlightMaintenanceComponent, FlashlightSetFocusMessage>(OnSetFocus);
        SubscribeLocalEvent<FlashlightMaintenanceComponent, FlashlightServiceModuleMessage>(OnServiceModule);
        SubscribeLocalEvent<FlashlightMaintenanceComponent, FlashlightCircuitActionMessage>(OnCircuitAction);
    }

    private void OnMapInit(Entity<FlashlightMaintenanceComponent> ent, ref MapInitEvent args)
    {
        ApplyOptics(ent.Owner, false, ent.Comp);
        UpdateUi(ent);
    }

    private void OnExamined(Entity<FlashlightMaintenanceComponent> ent, ref ExaminedEvent args)
    {
        var comp = ent.Comp;
        args.PushMarkup(Loc.GetString(
            "flashlight-maintenance-examine",
            ("focus", Percent(comp.Focus)),
            ("heat", Percent(GetHeatRatio(comp))),
            ("lens", Percent(comp.LensIntegrity)),
            ("emitter", Percent(comp.EmitterIntegrity)),
            ("contacts", Percent(comp.ContactIntegrity)),
            ("heatsink", Percent(comp.HeatSinkIntegrity))));

        if (comp.Overheated)
            args.PushMarkup(Loc.GetString("flashlight-maintenance-overheated-examine"));

        if (comp.FaultLatched)
            args.PushMarkup(Loc.GetString(
                "flashlight-maintenance-fault-examine",
                ("fault", Loc.GetString(GetFaultLoc(comp.LastFault)))));
    }

    private void OnGetActivationVerbs(Entity<FlashlightMaintenanceComponent> ent, ref GetVerbsEvent<ActivationVerb> args)
    {
        if (!args.CanAccess || !args.CanInteract)
            return;

        var user = args.User;
        args.Verbs.Add(new ActivationVerb
        {
            Text = Loc.GetString("flashlight-maintenance-verb"),
            Icon = new SpriteSpecifier.Texture(new("/Textures/Interface/VerbIcons/examine.svg.192dpi.png")),
            Priority = -2,
            Act = () => _ui.TryOpenUi(ent.Owner, FlashlightMaintenanceUiKey.Key, user),
        });
    }

    private void OnUiOpened(Entity<FlashlightMaintenanceComponent> ent, ref BoundUIOpenedEvent args)
    {
        UpdateUi(ent);
    }

    private void OnSetFocus(Entity<FlashlightMaintenanceComponent> ent, ref FlashlightSetFocusMessage args)
    {
        var newFocus = Math.Clamp(args.Focus, 0f, 1f);
        if (MathF.Abs(ent.Comp.Focus - newFocus) < 0.005f)
            return;

        ent.Comp.Focus = newFocus;
        _audio.PlayPvs(ent.Comp.SwitchSound, ent.Owner, AudioParams.Default.WithVolume(-6f));
        Dirty(ent);
        ApplyOptics(ent.Owner, IsActiveLight(ent.Owner), ent.Comp);
        UpdateUi(ent);
    }

    private void OnServiceModule(Entity<FlashlightMaintenanceComponent> ent, ref FlashlightServiceModuleMessage args)
    {
        // Field service deliberately gives small, bounded gains. The player can stabilize a tool,
        // but not turn a cheap light into a permanent industrial spotlight mid-round.
        const float serviceAmount = 0.12f;
        switch (args.Module)
        {
            case FlashlightModule.Lens:
                ent.Comp.LensIntegrity = Math.Clamp(ent.Comp.LensIntegrity + serviceAmount, 0f, 1f);
                break;
            case FlashlightModule.Emitter:
                ent.Comp.EmitterIntegrity = Math.Clamp(ent.Comp.EmitterIntegrity + serviceAmount, 0f, 1f);
                break;
            case FlashlightModule.Contacts:
                ent.Comp.ContactIntegrity = Math.Clamp(ent.Comp.ContactIntegrity + serviceAmount, 0f, 1f);
                break;
            case FlashlightModule.HeatSink:
                ent.Comp.HeatSinkIntegrity = Math.Clamp(ent.Comp.HeatSinkIntegrity + serviceAmount, 0f, 1f);
                ent.Comp.Heat = MathF.Max(0f, ent.Comp.Heat - ent.Comp.HeatCapacity * 0.12f);
                break;
        }

        if (ent.Comp.Overheated && ent.Comp.Heat <= ent.Comp.ResumeThreshold)
            ent.Comp.Overheated = false;

        _audio.PlayPvs(ent.Comp.RepairSound, ent.Owner, AudioParams.Default.WithVolume(-4f));
        Dirty(ent);
        ApplyOptics(ent.Owner, IsActiveLight(ent.Owner), ent.Comp);
        UpdateUi(ent);
    }

    private void OnCircuitAction(Entity<FlashlightMaintenanceComponent> ent, ref FlashlightCircuitActionMessage args)
    {
        if (!ent.Comp.FaultLatched || ent.Comp.CircuitStep == FlashlightCircuitStep.Stable)
            return;

        var expected = GetExpectedAction(ent.Comp.CircuitStep);
        if (args.Action != expected)
        {
            Miswire(ent);
            return;
        }

        _audio.PlayPvs(ent.Comp.RepairSound, ent.Owner, AudioParams.Default.WithVolume(-3f));
        AdvanceCircuit(ent.Comp);
        if (ent.Comp.CircuitStep == FlashlightCircuitStep.Stable)
        {
            ent.Comp.FaultLatched = false;
            ent.Comp.LastFault = FlashlightFault.None;
            ent.Comp.ContactIntegrity = Math.Clamp(ent.Comp.ContactIntegrity + 0.18f, 0f, 1f);
            ent.Comp.EmitterIntegrity = Math.Clamp(ent.Comp.EmitterIntegrity + 0.10f, 0f, 1f);
            ent.Comp.HeatSinkIntegrity = Math.Clamp(ent.Comp.HeatSinkIntegrity + 0.10f, 0f, 1f);
            ent.Comp.Heat = MathF.Max(0f, ent.Comp.Heat - ent.Comp.HeatCapacity * 0.18f);
        }

        Dirty(ent);
        ApplyOptics(ent.Owner, IsActiveLight(ent.Owner), ent.Comp);
        UpdateUi(ent);
    }

    public bool CanTurnOn(EntityUid uid, FlashlightMaintenanceComponent? comp = null)
    {
        return !Resolve(uid, ref comp, false) || (!comp.Overheated && !comp.FaultLatched);
    }

    public string GetTurnOnBlocker(EntityUid uid, FlashlightMaintenanceComponent? comp = null)
    {
        if (!Resolve(uid, ref comp, false))
            return string.Empty;

        if (comp.FaultLatched)
            return "flashlight-maintenance-fault-popup";

        if (comp.Overheated)
            return "flashlight-maintenance-overheated-popup";

        return string.Empty;
    }

    public float GetBatteryDrainMultiplier(EntityUid uid, FlashlightMaintenanceComponent? comp = null)
    {
        if (!Resolve(uid, ref comp, false))
            return 1f;

        var focusCurve = GetFocusCurve(comp);
        var focusCost = MathHelper.Lerp(0.82f, 1f + comp.BatteryFocusCost * 1.85f, focusCurve);
        var contactLoss = 1f + (1f - comp.ContactIntegrity) * 0.35f;
        var emitterLoss = 1f + (1f - comp.EmitterIntegrity) * 0.25f;
        return Math.Clamp(focusCost * contactLoss * emitterLoss, 0.65f, 3.25f);
    }

    public void UpdateInactiveLights(float frameTime)
    {
        var query = EntityQueryEnumerator<FlashlightMaintenanceComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (Paused(uid) || IsActiveLight(uid) || comp.Heat <= 0f)
                continue;

            CoolInactiveLight(uid, comp, frameTime);
        }
    }

    public bool UpdateActiveLight(EntityUid uid, bool active, float frameTime, FlashlightMaintenanceComponent? comp = null)
    {
        if (!Resolve(uid, ref comp, false))
            return true;

        var oldHeat = comp.Heat;
        var oldOverheated = comp.Overheated;
        var oldFaultLatched = comp.FaultLatched;
        var oldLastFault = comp.LastFault;
        var oldCircuitStep = comp.CircuitStep;
        var oldLens = comp.LensIntegrity;
        var oldEmitter = comp.EmitterIntegrity;
        var oldContacts = comp.ContactIntegrity;
        var oldHeatSink = comp.HeatSinkIntegrity;

        if (active)
        {
            var heatSinkCooling = 0.65f + comp.HeatSinkIntegrity * 0.55f;
            var focusCurve = GetFocusCurve(comp);
            var focusHeat = MathHelper.Lerp(0.72f, 1f + comp.FocusHeatMultiplier * 2.15f, focusCurve);
            var emitterLoss = 1f + (1f - comp.EmitterIntegrity) * 0.65f;
            comp.Heat += comp.HeatPerSecond * focusHeat * emitterLoss / heatSinkCooling * frameTime;
            UpdateFailureTimer(uid, comp, frameTime);
        }
        else
        {
            var coolRate = comp.CoolingPerSecond * (0.55f + comp.HeatSinkIntegrity * 0.45f);
            comp.Heat -= coolRate * frameTime;
        }

        comp.Heat = Math.Clamp(comp.Heat, 0f, comp.HeatCapacity);

        if (comp.Heat >= comp.OverheatThreshold)
            comp.Overheated = true;
        else if (comp.Overheated && comp.Heat <= comp.ResumeThreshold)
            comp.Overheated = false;

        if (active)
        {
            var thermalStress = Math.Clamp((GetHeatRatio(comp) - 0.72f) / 0.28f, 0f, 1f);
            if (thermalStress > 0f)
            {
                comp.EmitterIntegrity = Math.Clamp(comp.EmitterIntegrity - thermalStress * 0.00055f * frameTime, 0f, 1f);
                comp.ContactIntegrity = Math.Clamp(comp.ContactIntegrity - thermalStress * 0.00045f * frameTime, 0f, 1f);
                comp.HeatSinkIntegrity = Math.Clamp(comp.HeatSinkIntegrity - thermalStress * 0.00035f * frameTime, 0f, 1f);
            }
        }

        ApplyOptics(uid, active && !comp.Overheated && !comp.FaultLatched, comp);

        if (MathF.Abs(comp.Heat - oldHeat) > 1.5f ||
            oldOverheated != comp.Overheated ||
            oldFaultLatched != comp.FaultLatched ||
            oldLastFault != comp.LastFault ||
            oldCircuitStep != comp.CircuitStep ||
            PartChanged(oldLens, comp.LensIntegrity) ||
            PartChanged(oldEmitter, comp.EmitterIntegrity) ||
            PartChanged(oldContacts, comp.ContactIntegrity) ||
            PartChanged(oldHeatSink, comp.HeatSinkIntegrity))
        {
            Dirty(uid, comp);
            UpdateUi((uid, comp));
        }

        return !comp.Overheated && !comp.FaultLatched;
    }

    public void ApplyOptics(EntityUid uid, bool active, FlashlightMaintenanceComponent? comp = null)
    {
        if (!Resolve(uid, ref comp, false) || !_lights.TryGetLight(uid, out var light))
            return;

        var radius = GetProjectedRadius(comp);
        var energy = GetProjectedEnergy(comp);
        var focusCurve = GetFocusCurve(comp);
        var falloff = MathHelper.Lerp(comp.WideFalloff, comp.FocusedFalloff, focusCurve);
        var heatRatio = GetHeatRatio(comp);
        var contactNoise = GetContactNoise(comp);
        var color = Color.InterpolateBetween(Color.FromHex("#FFF1C9"), Color.FromHex("#FF7246"), heatRatio * 0.55f);
        const float edgeSoftness = 0f;
        const float edgeFalloff = 0f;
        const float edgeScale = 1f;

        if (active && comp.ContactIntegrity < comp.ContactFlickerThreshold)
        {
            var loss = 1f - comp.ContactIntegrity;
            var intermittent = 0.72f + 0.28f * contactNoise;
            energy *= Math.Clamp(intermittent - loss * 0.24f, 0.35f, 1f);
        }

        if (comp.FaultLatched)
            energy *= 0.18f;

        _lights.SetRadius(uid, active ? radius : comp.WideRadius * 0.45f, light);
        _lights.SetEnergy(uid, active ? energy : comp.WideEnergy * 0.35f, light);
        _lights.SetFalloff(uid, falloff * (1f + edgeFalloff), light);
        _lights.SetSoftness(uid, Math.Clamp(MathHelper.Lerp(1.2f, 0.42f, focusCurve) + edgeSoftness + (1f - comp.LensIntegrity) * 0.35f + heatRatio * 0.10f, 0.35f, 1.45f), light);
        _lights.SetCurveFactor(uid, MathHelper.Lerp(0.72f, 1.32f, focusCurve), light);
        _lights.SetMaskScale(uid, GetProjectedMaskScale(comp) * edgeScale, light);
        _lights.SetColor(uid, color, light);
    }

    private bool IsActiveLight(EntityUid uid)
    {
        return TryComp<HandheldLightComponent>(uid, out var handheld) && handheld.Activated;
    }

    private void UpdateUi(Entity<FlashlightMaintenanceComponent> ent)
    {
        if (!_ui.IsUiOpen(ent.Owner, FlashlightMaintenanceUiKey.Key))
            return;

        _ui.SetUiState(ent.Owner, FlashlightMaintenanceUiKey.Key, new FlashlightMaintenanceBoundUserInterfaceState(
            ent.Comp.Focus,
            GetHeatRatio(ent.Comp),
            ent.Comp.Overheated,
            ent.Comp.FaultLatched,
            ent.Comp.LastFault,
            ent.Comp.CircuitStep,
            ent.Comp.LensIntegrity,
            ent.Comp.EmitterIntegrity,
            ent.Comp.ContactIntegrity,
            ent.Comp.HeatSinkIntegrity,
            GetProjectedRadius(ent.Comp),
            GetProjectedEnergy(ent.Comp)));
    }

    private static float GetProjectedRadius(FlashlightMaintenanceComponent comp)
    {
        var lens = 0.55f + comp.LensIntegrity * 0.45f;
        var focusCurve = GetFocusCurve(comp);
        var intensityReach = MathHelper.Lerp(0.88f, 1.42f, focusCurve);
        return MathHelper.Lerp(comp.WideRadius, comp.FocusedRadius, focusCurve) * lens * intensityReach;
    }

    private static float GetProjectedEnergy(FlashlightMaintenanceComponent comp)
    {
        var lens = 0.62f + comp.LensIntegrity * 0.38f;
        var emitter = 0.48f + comp.EmitterIntegrity * 0.52f;
        var focusCurve = GetFocusCurve(comp);
        var focusGain = MathHelper.Lerp(comp.WideEnergy * 0.72f, comp.FocusedEnergy * 1.75f, focusCurve);
        var heatSag = MathHelper.Lerp(1f, 0.72f, GetHeatRatio(comp));
        return focusGain * lens * emitter * heatSag;
    }

    private static Vector2 GetProjectedMaskScale(FlashlightMaintenanceComponent comp)
    {
        var focusCurve = GetFocusCurve(comp);
        return new Vector2(
            MathHelper.Lerp(comp.WideMaskScale.X, comp.FocusedMaskScale.X, focusCurve),
            MathHelper.Lerp(comp.WideMaskScale.Y, comp.FocusedMaskScale.Y, focusCurve));
    }

    private void CoolInactiveLight(EntityUid uid, FlashlightMaintenanceComponent comp, float frameTime)
    {
        var oldHeat = comp.Heat;
        var oldOverheated = comp.Overheated;
        var heatSinkCooling = 0.55f + comp.HeatSinkIntegrity * 0.45f;
        var coolRate = comp.CoolingPerSecond * comp.InactiveCoolingMultiplier * heatSinkCooling;
        comp.Heat = Math.Clamp(comp.Heat - coolRate * frameTime, 0f, comp.HeatCapacity);

        if (comp.Overheated && comp.Heat <= comp.ResumeThreshold)
            comp.Overheated = false;

        if (MathF.Abs(oldHeat - comp.Heat) <= 1.5f && oldOverheated == comp.Overheated)
            return;

        Dirty(uid, comp);
        UpdateUi((uid, comp));
    }

    private void UpdateFailureTimer(EntityUid uid, FlashlightMaintenanceComponent comp, float frameTime)
    {
        if (comp.FaultLatched)
            return;

        comp.FailureCheckTimer -= frameTime;
        if (comp.FailureCheckTimer > 0f)
            return;

        comp.FailureCheckTimer = comp.FailureCheckSeconds + _random.NextFloat(-1.5f, 4.5f);
        var heatRatio = GetHeatRatio(comp);
        var chance = 0.006f
            + MathF.Pow(GetFocusCurve(comp), 1.35f) * 0.026f
            + heatRatio * heatRatio * 0.018f
            + (1f - comp.ContactIntegrity) * 0.035f
            + (1f - comp.EmitterIntegrity) * 0.025f
            + (1f - comp.HeatSinkIntegrity) * 0.020f;

        if (!_random.Prob(Math.Clamp(chance, 0f, 0.22f)))
            return;

        comp.FaultLatched = true;
        comp.LastFault = PickFault(comp);
        comp.CircuitStep = GetFirstStep(comp.LastFault);
        _audio.PlayPvs(comp.FaultSound, uid, AudioParams.Default.WithVolume(-1f));
    }

    private FlashlightFault PickFault(FlashlightMaintenanceComponent comp)
    {
        var roll = _random.NextFloat();
        if (comp.ContactIntegrity < 0.68f || roll < 0.32f)
            return FlashlightFault.ContactDropout;

        if (GetHeatRatio(comp) > 0.70f || roll < 0.55f)
            return FlashlightFault.ThermalRunaway;

        if (comp.EmitterIntegrity < 0.72f || roll < 0.78f)
            return FlashlightFault.EmitterSag;

        return FlashlightFault.GroundLeak;
    }

    private static FlashlightCircuitStep GetFirstStep(FlashlightFault fault)
    {
        return fault switch
        {
            FlashlightFault.ContactDropout => FlashlightCircuitStep.CheckContinuity,
            FlashlightFault.ThermalRunaway => FlashlightCircuitStep.BleedCapacitor,
            FlashlightFault.EmitterSag => FlashlightCircuitStep.MatchImpedance,
            FlashlightFault.GroundLeak => FlashlightCircuitStep.BridgeGround,
            _ => FlashlightCircuitStep.CheckContinuity,
        };
    }

    private static FlashlightCircuitAction GetExpectedAction(FlashlightCircuitStep step)
    {
        return step switch
        {
            FlashlightCircuitStep.CheckContinuity => FlashlightCircuitAction.ProbeContinuity,
            FlashlightCircuitStep.CorrectPolarity => FlashlightCircuitAction.ReversePolarity,
            FlashlightCircuitStep.BleedCapacitor => FlashlightCircuitAction.BleedCapacitor,
            FlashlightCircuitStep.BridgeGround => FlashlightCircuitAction.BridgeGround,
            FlashlightCircuitStep.MatchImpedance => FlashlightCircuitAction.TrimResistor,
            FlashlightCircuitStep.CalibrateEmitter => FlashlightCircuitAction.CalibrateEmitter,
            _ => FlashlightCircuitAction.ProbeContinuity,
        };
    }

    private static void AdvanceCircuit(FlashlightMaintenanceComponent comp)
    {
        comp.CircuitStep = comp.CircuitStep switch
        {
            FlashlightCircuitStep.CheckContinuity => FlashlightCircuitStep.CorrectPolarity,
            FlashlightCircuitStep.CorrectPolarity => FlashlightCircuitStep.BleedCapacitor,
            FlashlightCircuitStep.BleedCapacitor => FlashlightCircuitStep.BridgeGround,
            FlashlightCircuitStep.BridgeGround => FlashlightCircuitStep.MatchImpedance,
            FlashlightCircuitStep.MatchImpedance => FlashlightCircuitStep.CalibrateEmitter,
            FlashlightCircuitStep.CalibrateEmitter => FlashlightCircuitStep.Stable,
            _ => FlashlightCircuitStep.Stable,
        };
    }

    private void Miswire(Entity<FlashlightMaintenanceComponent> ent)
    {
        ent.Comp.Heat = Math.Clamp(ent.Comp.Heat + ent.Comp.HeatCapacity * 0.08f, 0f, ent.Comp.HeatCapacity);
        ent.Comp.ContactIntegrity = Math.Clamp(ent.Comp.ContactIntegrity - 0.035f, 0f, 1f);
        ent.Comp.EmitterIntegrity = Math.Clamp(ent.Comp.EmitterIntegrity - 0.020f, 0f, 1f);
        _audio.PlayPvs(ent.Comp.FaultSound, ent.Owner, AudioParams.Default.WithVolume(-1f));
        Dirty(ent);
        UpdateUi(ent);
    }

    private float GetContactNoise(FlashlightMaintenanceComponent comp)
    {
        var seconds = (float) _timing.CurTime.TotalSeconds;
        var loss = 1f - comp.ContactIntegrity;
        var fast = MathF.Sin(seconds * (16f + loss * 22f));
        var slow = MathF.Sin(seconds * (2.7f + loss * 4f));
        return Math.Clamp(0.5f + fast * 0.32f + slow * 0.18f, 0f, 1f);
    }

    private static string GetFaultLoc(FlashlightFault fault)
    {
        return fault switch
        {
            FlashlightFault.ContactDropout => "flashlight-maintenance-fault-contact-dropout",
            FlashlightFault.ThermalRunaway => "flashlight-maintenance-fault-thermal-runaway",
            FlashlightFault.EmitterSag => "flashlight-maintenance-fault-emitter-sag",
            FlashlightFault.GroundLeak => "flashlight-maintenance-fault-ground-leak",
            _ => "flashlight-maintenance-fault-none",
        };
    }

    private static float GetHeatRatio(FlashlightMaintenanceComponent comp)
    {
        return comp.HeatCapacity <= 0f ? 0f : Math.Clamp(comp.Heat / comp.HeatCapacity, 0f, 1f);
    }

    private static float GetFocusCurve(FlashlightMaintenanceComponent comp)
    {
        var focus = Math.Clamp(comp.Focus, 0f, 1f);
        return focus * focus * (3f - 2f * focus);
    }

    private static int Percent(float value)
    {
        return (int) MathF.Round(Math.Clamp(value, 0f, 1f) * 100f);
    }

    private static bool PartChanged(float oldValue, float newValue)
    {
        return MathF.Abs(oldValue - newValue) > 0.01f;
    }
}
