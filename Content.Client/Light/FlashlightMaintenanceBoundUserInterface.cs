using Content.Shared.Light.Components;
using Robust.Shared.Maths;

namespace Content.Client.Light;

public sealed class FlashlightMaintenanceBoundUserInterface : BoundUserInterface
{
    private FlashlightMaintenanceWindow? _window;

    public FlashlightMaintenanceBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = new FlashlightMaintenanceWindow();
        _window.OnClose += Close;
        _window.OpenCentered();
        _window.OnFocusChanged += focus => SendMessage(new FlashlightSetFocusMessage(focus));
        _window.OnServiceModule += module => SendMessage(new FlashlightServiceModuleMessage(module));
        Reload();
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        if (state is FlashlightMaintenanceBoundUserInterfaceState maintenance)
            _window?.UpdateState(maintenance);
    }

    public void Reload()
    {
        if (_window == null || !EntMan.TryGetComponent(Owner, out FlashlightMaintenanceComponent? comp))
            return;

        _window.UpdateState(new FlashlightMaintenanceBoundUserInterfaceState(
            comp.Focus,
            comp.HeatCapacity <= 0f ? 0f : comp.Heat / comp.HeatCapacity,
            comp.Overheated,
            comp.LensIntegrity,
            comp.EmitterIntegrity,
            comp.ContactIntegrity,
            comp.HeatSinkIntegrity,
            MathHelper.Lerp(comp.WideRadius, comp.FocusedRadius, comp.Focus),
            MathHelper.Lerp(comp.WideEnergy, comp.FocusedEnergy, comp.Focus)));
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (!disposing || _window == null)
            return;

        _window.OnClose -= Close;
        _window.Orphan();
        _window = null;
    }
}
