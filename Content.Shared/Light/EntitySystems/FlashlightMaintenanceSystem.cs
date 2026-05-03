using Content.Shared.Examine;
using Content.Shared.Light.Components;
using Content.Shared.UserInterface;
using Content.Shared.Verbs;
using Robust.Shared.Maths;
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
    [Dependency] private readonly SharedUserInterfaceSystem _ui = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<FlashlightMaintenanceComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<FlashlightMaintenanceComponent, ExaminedEvent>(OnExamined);
        SubscribeLocalEvent<FlashlightMaintenanceComponent, GetVerbsEvent<ActivationVerb>>(OnGetActivationVerbs);
        SubscribeLocalEvent<FlashlightMaintenanceComponent, BoundUIOpenedEvent>(OnUiOpened);
        SubscribeLocalEvent<FlashlightMaintenanceComponent, FlashlightSetFocusMessage>(OnSetFocus);
        SubscribeLocalEvent<FlashlightMaintenanceComponent, FlashlightServiceModuleMessage>(OnServiceModule);
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
        ent.Comp.Focus = Math.Clamp(args.Focus, 0f, 1f);
        Dirty(ent);
        ApplyOptics(ent.Owner, IsActiveLight(ent.Owner), ent.Comp);
        UpdateUi(ent);
    }

    private void OnServiceModule(Entity<FlashlightMaintenanceComponent> ent, ref FlashlightServiceModuleMessage args)
    {
        // Field service deliberately gives small, bounded gains. The player can stabilize a tool,
        // but not turn a cheap light into a permanent industrial spotlight mid-round.
        const float serviceAmount = 0.18f;
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

        Dirty(ent);
        ApplyOptics(ent.Owner, IsActiveLight(ent.Owner), ent.Comp);
        UpdateUi(ent);
    }

    public bool CanTurnOn(EntityUid uid, FlashlightMaintenanceComponent? comp = null)
    {
        return !Resolve(uid, ref comp, false) || !comp.Overheated;
    }

    public float GetBatteryDrainMultiplier(EntityUid uid, FlashlightMaintenanceComponent? comp = null)
    {
        if (!Resolve(uid, ref comp, false))
            return 1f;

        var focusCost = 1f + comp.Focus * comp.Focus * comp.BatteryFocusCost;
        var contactLoss = 1f + (1f - comp.ContactIntegrity) * 0.35f;
        var emitterLoss = 1f + (1f - comp.EmitterIntegrity) * 0.25f;
        return Math.Clamp(focusCost * contactLoss * emitterLoss, 0.65f, 2.4f);
    }

    public bool UpdateActiveLight(EntityUid uid, bool active, float frameTime, FlashlightMaintenanceComponent? comp = null)
    {
        if (!Resolve(uid, ref comp, false))
            return true;

        var oldHeat = comp.Heat;
        var oldOverheated = comp.Overheated;
        var oldLens = comp.LensIntegrity;
        var oldEmitter = comp.EmitterIntegrity;
        var oldContacts = comp.ContactIntegrity;
        var oldHeatSink = comp.HeatSinkIntegrity;

        if (active)
        {
            var heatSinkCooling = 0.55f + comp.HeatSinkIntegrity * 0.45f;
            var focusHeat = 1f + comp.Focus * comp.Focus * comp.FocusHeatMultiplier;
            var emitterLoss = 1f + (1f - comp.EmitterIntegrity) * 0.65f;
            comp.Heat += comp.HeatPerSecond * focusHeat * emitterLoss / heatSinkCooling * frameTime;
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
            var thermalStress = Math.Clamp((GetHeatRatio(comp) - 0.62f) / 0.38f, 0f, 1f);
            if (thermalStress > 0f)
            {
                comp.EmitterIntegrity = Math.Clamp(comp.EmitterIntegrity - thermalStress * 0.0015f * frameTime, 0f, 1f);
                comp.ContactIntegrity = Math.Clamp(comp.ContactIntegrity - thermalStress * 0.0010f * frameTime, 0f, 1f);
                comp.HeatSinkIntegrity = Math.Clamp(comp.HeatSinkIntegrity - thermalStress * 0.0007f * frameTime, 0f, 1f);
            }
        }

        ApplyOptics(uid, active && !comp.Overheated, comp);

        if (MathF.Abs(comp.Heat - oldHeat) > 1.5f ||
            oldOverheated != comp.Overheated ||
            PartChanged(oldLens, comp.LensIntegrity) ||
            PartChanged(oldEmitter, comp.EmitterIntegrity) ||
            PartChanged(oldContacts, comp.ContactIntegrity) ||
            PartChanged(oldHeatSink, comp.HeatSinkIntegrity))
        {
            Dirty(uid, comp);
            UpdateUi((uid, comp));
        }

        return !comp.Overheated;
    }

    public void ApplyOptics(EntityUid uid, bool active, FlashlightMaintenanceComponent? comp = null)
    {
        if (!Resolve(uid, ref comp, false) || !_lights.TryGetLight(uid, out var light))
            return;

        var radius = GetProjectedRadius(comp);
        var energy = GetProjectedEnergy(comp);
        var falloff = MathHelper.Lerp(comp.WideFalloff, comp.FocusedFalloff, comp.Focus);
        var heatRatio = GetHeatRatio(comp);
        var color = Color.InterpolateBetween(Color.FromHex("#F8E6B8"), Color.FromHex("#FF8A42"), heatRatio * 0.45f);

        if (active && comp.ContactIntegrity < comp.ContactFlickerThreshold)
        {
            var loss = 1f - comp.ContactIntegrity;
            var seconds = (float) _timing.CurTime.TotalSeconds;
            var intermittent = 0.72f + 0.28f * MathF.Sin(seconds * (19f + loss * 31f));
            energy *= Math.Clamp(intermittent - loss * 0.24f, 0.35f, 1f);
        }

        _lights.SetRadius(uid, radius, light);
        _lights.SetEnergy(uid, active ? energy : comp.WideEnergy, light);
        _lights.SetFalloff(uid, falloff, light);
        _lights.SetSoftness(uid, comp.Softness, light);
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
        return MathHelper.Lerp(comp.WideRadius, comp.FocusedRadius, comp.Focus) * lens;
    }

    private static float GetProjectedEnergy(FlashlightMaintenanceComponent comp)
    {
        var lens = 0.62f + comp.LensIntegrity * 0.38f;
        var emitter = 0.48f + comp.EmitterIntegrity * 0.52f;
        var focusGain = MathHelper.Lerp(comp.WideEnergy, comp.FocusedEnergy, comp.Focus);
        var heatSag = MathHelper.Lerp(1f, 0.72f, GetHeatRatio(comp));
        return focusGain * lens * emitter * heatSag;
    }

    private static float GetHeatRatio(FlashlightMaintenanceComponent comp)
    {
        return comp.HeatCapacity <= 0f ? 0f : Math.Clamp(comp.Heat / comp.HeatCapacity, 0f, 1f);
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
