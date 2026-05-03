using Content.Client.Items;
using Content.Client.Light.Components;
using Content.Shared.Light;
using Content.Shared.Light.Components;
using Content.Shared.Toggleable;
using Content.Shared.UserInterface;
using Robust.Client.GameObjects;
using Content.Client.Light.EntitySystems;
using Content.Shared.Light.EntitySystems;

namespace Content.Client.Light;

public sealed class HandheldLightSystem : SharedHandheldLightSystem
{
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly LightBehaviorSystem _lightBehavior = default!;
    [Dependency] private readonly FlashlightMaintenanceSystem _maintenance = default!;
    [Dependency] private readonly SharedUserInterfaceSystem _ui = default!;

    public override void Initialize()
    {
        base.Initialize();

        Subs.ItemStatus<HandheldLightComponent>(ent => new HandheldLightStatus(ent));
        SubscribeLocalEvent<HandheldLightComponent, AppearanceChangeEvent>(OnAppearanceChange);
        SubscribeLocalEvent<FlashlightMaintenanceComponent, AfterAutoHandleStateEvent>(OnMaintenanceAfterHandleState);
    }

    /// <remarks>
    ///     TODO: Not properly predicted yet. Don't call this function if you want a the actual return value!
    /// </remarks>
    public override bool TurnOff(Entity<HandheldLightComponent> ent, bool makeNoise = true)
    {
        return true;
    }

    /// <remarks>
    ///     TODO: Not properly predicted yet. Don't call this function if you want a the actual return value!
    /// </remarks>
    public override bool TurnOn(EntityUid user, Entity<HandheldLightComponent> uid)
    {
        return true;
    }

    private void OnAppearanceChange(EntityUid uid, HandheldLightComponent? component, ref AppearanceChangeEvent args)
    {
        if (!Resolve(uid, ref component))
        {
            return;
        }

        if (!_appearance.TryGetData<bool>(uid, ToggleableVisuals.Enabled, out var enabled, args.Component))
        {
            return;
        }

        if (!_appearance.TryGetData<HandheldLightPowerStates>(uid, HandheldLightVisuals.Power, out var state, args.Component))
        {
            return;
        }

        if (TryComp<FlashlightMaintenanceComponent>(uid, out var maintenance))
            _maintenance.ApplyOptics(uid, enabled && !maintenance.Overheated && !maintenance.FaultLatched, maintenance);

        if (TryComp<LightBehaviourComponent>(uid, out var lightBehaviour))
        {
            // Reset any running behaviour to reset the animated properties back to the original value, to avoid conflicts between resets
            if (_lightBehavior.HasRunningBehaviours((uid, lightBehaviour)))
            {
                _lightBehavior.StopLightBehaviour((uid, lightBehaviour), resetToOriginalSettings: true);
            }

            if (!enabled)
            {
                return;
            }

            switch (state)
            {
                case HandheldLightPowerStates.FullPower:
                    break; // We just needed to reset all behaviours
                case HandheldLightPowerStates.LowPower:
                    _lightBehavior.StartLightBehaviour((uid, lightBehaviour), component.RadiatingBehaviourId);
                    break;
                case HandheldLightPowerStates.Dying:
                    _lightBehavior.StartLightBehaviour((uid, lightBehaviour), component.BlinkingBehaviourId);
                    break;
            }
        }
    }

    private void OnMaintenanceAfterHandleState(Entity<FlashlightMaintenanceComponent> ent, ref AfterAutoHandleStateEvent args)
    {
        _maintenance.ApplyOptics(ent.Owner, IsMaintenanceLightActive(ent.Owner, ent.Comp), ent.Comp);

        if (_ui.TryGetOpenUi<FlashlightMaintenanceBoundUserInterface>(ent.Owner, FlashlightMaintenanceUiKey.Key, out var bui))
            bui.Reload();
    }

    private bool IsMaintenanceLightActive(EntityUid uid, FlashlightMaintenanceComponent comp)
    {
        return !comp.Overheated &&
               !comp.FaultLatched &&
               TryComp<HandheldLightComponent>(uid, out var handheld) &&
               handheld.Activated;
    }
}
