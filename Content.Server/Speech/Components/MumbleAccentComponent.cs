using Robust.Shared.Audio;

namespace Content.Server.Speech.Components;

[RegisterComponent]
public sealed partial class MumbleAccentComponent : Component
{
    /// <summary>
    /// This modifies the audio parameters of emote sounds, screaming, laughing, etc.
    /// By default, it reduces the volume and distance of emote sounds.
    /// </summary>
    [DataField]
    public AudioParams EmoteAudioParams = AudioParams.Default
        .WithVolume(-8f)
        .WithMaxDistance(7f)
        .WithBus(AudioBus.Voice)
        .WithPsychoacousticProfile(PsychoacousticProfile.WhisperOccluded, 0.25f)
        .WithAcousticMaterial(AcousticMaterialProfile.Flesh);
}
