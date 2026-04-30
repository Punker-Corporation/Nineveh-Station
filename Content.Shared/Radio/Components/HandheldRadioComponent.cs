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

    [DataField, AutoNetworkedField]
    public bool MicrophoneEnabled = true;

    [DataField, AutoNetworkedField]
    public bool SpeakerEnabled = true;
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
public sealed class HandheldRadioToggleMicrophoneMessage : BoundUserInterfaceMessage
{
    public bool Enabled;

    public HandheldRadioToggleMicrophoneMessage(bool enabled)
    {
        Enabled = enabled;
    }
}

[Serializable, NetSerializable]
public sealed class HandheldRadioToggleSpeakerMessage : BoundUserInterfaceMessage
{
    public bool Enabled;

    public HandheldRadioToggleSpeakerMessage(bool enabled)
    {
        Enabled = enabled;
    }
}

[Serializable, NetSerializable]
public sealed class HandheldRadioBoundUserInterfaceState : BoundUserInterfaceState
{
    public int? SelectedChannel;
    public int MinChannel;
    public int MaxChannel;
    public bool MicrophoneEnabled;
    public bool SpeakerEnabled;

    public HandheldRadioBoundUserInterfaceState(
        int? selectedChannel,
        int minChannel,
        int maxChannel,
        bool microphoneEnabled,
        bool speakerEnabled)
    {
        SelectedChannel = selectedChannel;
        MinChannel = minChannel;
        MaxChannel = maxChannel;
        MicrophoneEnabled = microphoneEnabled;
        SpeakerEnabled = speakerEnabled;
    }
}

[Serializable, NetSerializable]
public enum HandheldRadioUiKey : byte
{
    Key,
}
