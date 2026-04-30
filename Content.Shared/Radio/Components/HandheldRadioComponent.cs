using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Radio.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class HandheldRadioComponent : Component
{
    [DataField, AutoNetworkedField]
    public int? SelectedChannel = 1;

    [DataField]
    public int MinChannel = 1;

    [DataField]
    public int MaxChannel = 36;
}

[Serializable, NetSerializable]
public sealed class HandheldRadioSetChannelMessage : BoundUserInterfaceMessage
{
    public int? Channel;

    public HandheldRadioSetChannelMessage(int? channel)
    {
        Channel = channel;
    }
}

[Serializable, NetSerializable]
public sealed class HandheldRadioBoundUserInterfaceState : BoundUserInterfaceState
{
    public int? SelectedChannel;
    public int MinChannel;
    public int MaxChannel;

    public HandheldRadioBoundUserInterfaceState(int? selectedChannel, int minChannel, int maxChannel)
    {
        SelectedChannel = selectedChannel;
        MinChannel = minChannel;
        MaxChannel = maxChannel;
    }
}

[Serializable, NetSerializable]
public enum HandheldRadioUiKey : byte
{
    Key,
}
