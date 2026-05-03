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

    [DataField, AutoNetworkedField]
    public float Congestion;

    [DataField, AutoNetworkedField]
    public float AdjacentCongestionLow;

    [DataField, AutoNetworkedField]
    public float AdjacentCongestionHigh;

    [DataField, AutoNetworkedField]
    public float SignalQuality = 1f;

    [DataField, AutoNetworkedField]
    public float Drift;

    [DataField, AutoNetworkedField]
    public string LastStaticPhrase = string.Empty;

    [DataField]
    public float CongestionUpdateSeconds = 95f;

    [DataField]
    public float NextCongestionUpdate;

    [DataField]
    public string StaticSound = "/Audio/_Scp/Effects/Radio/static.ogg";
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
    public float Congestion;
    public float AdjacentCongestionLow;
    public float AdjacentCongestionHigh;
    public float SignalQuality;
    public float Drift;
    public string LastStaticPhrase;

    public HandheldRadioBoundUserInterfaceState(
        int? selectedChannel,
        int minChannel,
        int maxChannel,
        bool microphoneEnabled,
        bool speakerEnabled,
        float congestion,
        float adjacentCongestionLow,
        float adjacentCongestionHigh,
        float signalQuality,
        float drift,
        string lastStaticPhrase)
    {
        SelectedChannel = selectedChannel;
        MinChannel = minChannel;
        MaxChannel = maxChannel;
        MicrophoneEnabled = microphoneEnabled;
        SpeakerEnabled = speakerEnabled;
        Congestion = congestion;
        AdjacentCongestionLow = adjacentCongestionLow;
        AdjacentCongestionHigh = adjacentCongestionHigh;
        SignalQuality = signalQuality;
        Drift = drift;
        LastStaticPhrase = lastStaticPhrase;
    }
}

[Serializable, NetSerializable]
public enum HandheldRadioUiKey : byte
{
    Key,
}
