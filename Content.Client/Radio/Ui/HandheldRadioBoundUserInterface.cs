using Content.Shared.Radio.Components;
using JetBrains.Annotations;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;

namespace Content.Client.Radio.Ui;

[UsedImplicitly]
public sealed class HandheldRadioBoundUserInterface : BoundUserInterface
{
    [ViewVariables]
    private HandheldRadioWindow? _window;

    public HandheldRadioBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<HandheldRadioWindow>();

        if (EntMan.TryGetComponent(Owner, out HandheldRadioComponent? radio))
            _window.SetState(radio.SelectedChannel, radio.MinChannel, radio.MaxChannel);

        _window.ApplyButton.OnPressed += _ =>
        {
            var text = _window.ChannelLineEdit.Text.Trim();
            if (string.Equals(text, "null", StringComparison.OrdinalIgnoreCase))
            {
                SendMessage(new HandheldRadioSetChannelMessage(null));
                _window.Close();
                return;
            }

            if (int.TryParse(text, out var channel))
            {
                SendMessage(new HandheldRadioSetChannelMessage(channel));
                _window.Close();
            }
        };

        _window.NullButton.OnPressed += _ =>
        {
            SendMessage(new HandheldRadioSetChannelMessage(null));
            _window.Close();
        };
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        if (_window == null || state is not HandheldRadioBoundUserInterfaceState cast)
            return;

        _window.SetState(cast.SelectedChannel, cast.MinChannel, cast.MaxChannel);
    }
}
