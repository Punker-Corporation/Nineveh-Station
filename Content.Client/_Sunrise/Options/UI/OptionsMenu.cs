namespace Content.Client.Options.UI;

public sealed partial class OptionsMenu
{
    [Dependency] private readonly ILocalizationManager _loc = default!;

    private void SetTabsName()
    {
        Title = _loc.GetString("ui-options-title");

        // Início das abas customizadas.
        Tabs.SetTabTitle(0, Loc.GetString("ui-options-tab-scp"));


        Tabs.SetTabTitle(1, _loc.GetString("ui-options-tab-extra"));
        Tabs.SetTabTitle(2, _loc.GetString("ui-options-tab-misc"));
        Tabs.SetTabTitle(3, _loc.GetString("ui-options-tab-graphics"));
        Tabs.SetTabTitle(4, _loc.GetString("ui-options-tab-controls"));
        Tabs.SetTabTitle(5, _loc.GetString("ui-options-tab-audio"));
        Tabs.SetTabTitle(6, _loc.GetString("ui-options-tab-accessibility"));
        Tabs.SetTabTitle(7, _loc.GetString("ui-options-tab-admin"));
        // Fim das abas customizadas.
    }
}
