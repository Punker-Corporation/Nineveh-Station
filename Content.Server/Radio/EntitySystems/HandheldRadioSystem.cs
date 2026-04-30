using Content.Server.Popups;
using Content.Server.Chat.Systems;
using Content.Shared.Chat;
using Content.Shared._Sunrise.TTS;
using Content.Shared.Mobs.Components;
using Content.Shared.Radio;
using Content.Shared.Radio.Components;
using Content.Shared.Radio.EntitySystems;
using Content.Shared.Speech;
using Content.Shared.UserInterface;
using Robust.Server.Containers;
using Robust.Server.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Server.Radio.EntitySystems;

public sealed class HandheldRadioSystem : SharedRadioDeviceSystem
{
    [Dependency] private readonly ChatSystem _chat = default!;
    [Dependency] private readonly ContainerSystem _container = default!;
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly RadioSystem _radio = default!;
    [Dependency] private readonly RadioDeviceSystem _radioDevice = default!;
    [Dependency] private readonly UserInterfaceSystem _ui = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<HandheldRadioComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<HandheldRadioComponent, AfterActivatableUIOpenEvent>(OnAfterUiOpen);
        SubscribeLocalEvent<HandheldRadioComponent, HandheldRadioSetChannelMessage>(OnSetChannel);
        SubscribeLocalEvent<HandheldRadioComponent, HandheldRadioToggleMicrophoneMessage>(OnToggleMicrophone);
        SubscribeLocalEvent<HandheldRadioComponent, HandheldRadioToggleSpeakerMessage>(OnToggleSpeaker);
        SubscribeLocalEvent<HandheldRadioComponent, RadioReceiveEvent>(OnRadioReceive);
        SubscribeLocalEvent<EntitySpokeEvent>(OnEntitySpoke);
    }

    private void OnStartup(Entity<HandheldRadioComponent> ent, ref ComponentStartup args)
    {
        ApplyChannel(ent);
    }

    private void OnAfterUiOpen(Entity<HandheldRadioComponent> ent, ref AfterActivatableUIOpenEvent args)
    {
        UpdateUi(ent);
    }

    private void OnSetChannel(Entity<HandheldRadioComponent> ent, ref HandheldRadioSetChannelMessage args)
    {
        if (args.Channel is { } channel && (channel < ent.Comp.MinChannel || channel > ent.Comp.MaxChannel))
            return;

        ent.Comp.SelectedChannel = args.Channel;
        Dirty(ent);
        ApplyChannel(ent);
        UpdateUi(ent);

        if (args.Actor is { Valid: true } user)
        {
            var channelName = args.Channel?.ToString() ?? Loc.GetString("handheld-radio-channel-null");
            _popup.PopupEntity(Loc.GetString("handheld-radio-channel-set", ("channel", channelName)), ent.Owner, user);
        }
    }

    private void OnToggleMicrophone(Entity<HandheldRadioComponent> ent, ref HandheldRadioToggleMicrophoneMessage args)
    {
        ent.Comp.MicrophoneEnabled = args.Enabled;
        Dirty(ent);
        ApplyChannel(ent);
        UpdateUi(ent);
    }

    private void OnToggleSpeaker(Entity<HandheldRadioComponent> ent, ref HandheldRadioToggleSpeakerMessage args)
    {
        ent.Comp.SpeakerEnabled = args.Enabled;
        Dirty(ent);
        ApplyChannel(ent);
        UpdateUi(ent);
    }

    private void OnEntitySpoke(EntitySpokeEvent args)
    {
        var sentChannels = new HashSet<ProtoId<RadioChannelPrototype>>();
        var query = EntityQueryEnumerator<HandheldRadioComponent, RadioMicrophoneComponent>();

        while (query.MoveNext(out var uid, out var handheld, out var microphone))
        {
            if (!handheld.MicrophoneEnabled || handheld.SelectedChannel is not { } channel)
                continue;

            if (GetUser(uid) != args.Source)
                continue;

            var radioChannel = GetChannelId(channel);
            if (!sentChannels.Add(radioChannel))
                continue;

            microphone.BroadcastChannel = radioChannel;
            Dirty(uid, microphone);
            _radio.SendRadioMessage(args.Source, args.Message, radioChannel, uid);
        }
    }

    private void OnRadioReceive(Entity<HandheldRadioComponent> ent, ref RadioReceiveEvent args)
    {
        if (ent.Owner == args.RadioSource)
            return;

        var receiver = GetUser(ent);
        var relayName = GetRelayName(ent, args.MessageSource);

        if (TryComp(receiver, out ActorComponent? actor))
        {
            _chat.TrySendInGameICMessage(ent,
                args.Message,
                InGameICChatType.Whisper,
                hideChat: true,
                hideLog: true,
                nameOverride: relayName,
                checkRadioPrefix: false,
                ignoreActionBlocker: true);

            _net.ServerSendMessage(args.ChatMsg, actor.PlayerSession.Channel);

            if (receiver != args.MessageSource && HasComp<TTSComponent>(args.MessageSource) && !args.Receivers.Contains(receiver))
                args.Receivers.Add(receiver);

            return;
        }

        _chat.TrySendInGameICMessage(ent,
            args.Message,
            InGameICChatType.Whisper,
            ChatTransmitRange.GhostRangeLimit,
            nameOverride: relayName,
            checkRadioPrefix: false,
            ignoreActionBlocker: true);
    }

    private string GetRelayName(Entity<HandheldRadioComponent> ent, EntityUid messageSource)
    {
        var nameEv = new TransformSpeakerNameEvent(messageSource, Name(messageSource));
        RaiseLocalEvent(messageSource, nameEv);

        return Loc.GetString("speech-name-relay",
            ("speaker", Name(ent)),
            ("originalName", nameEv.VoiceName));
    }

    private void ApplyChannel(Entity<HandheldRadioComponent> ent)
    {
        if (ent.Comp.SelectedChannel is not { } channel)
        {
            SetNullChannel(ent);
            return;
        }

        var radioChannel = GetChannelId(channel);

        if (TryComp(ent, out RadioMicrophoneComponent? microphone))
        {
            microphone.BroadcastChannel = radioChannel;
            _radioDevice.SetMicrophoneEnabled(ent, null, ent.Comp.MicrophoneEnabled, true, microphone);
            Dirty(ent.Owner, microphone);
        }

        if (TryComp(ent, out RadioSpeakerComponent? speaker))
        {
            speaker.Channels.Clear();
            speaker.Channels.Add(radioChannel);
            _radioDevice.SetSpeakerEnabled(ent, null, ent.Comp.SpeakerEnabled, true, speaker);
            Dirty(ent.Owner, speaker);
        }

        if (ent.Comp.SpeakerEnabled && TryComp(ent, out ActiveRadioComponent? activeRadio))
        {
            activeRadio.Channels.Clear();
            activeRadio.Channels.Add(radioChannel);
            Dirty(ent.Owner, activeRadio);
        }

        if (TryComp(ent, out EncryptionKeyHolderComponent? keyHolder))
        {
            keyHolder.DefaultChannel = radioChannel;
            Dirty(ent.Owner, keyHolder);
        }
    }

    private void SetNullChannel(Entity<HandheldRadioComponent> ent)
    {
        if (TryComp(ent, out RadioMicrophoneComponent? microphone))
        {
            _radioDevice.SetMicrophoneEnabled(ent, null, false, true, microphone);
            Dirty(ent.Owner, microphone);
        }

        if (TryComp(ent, out RadioSpeakerComponent? speaker))
        {
            _radioDevice.SetSpeakerEnabled(ent, null, false, true, speaker);
            speaker.Channels.Clear();
            Dirty(ent.Owner, speaker);
        }

        RemCompDeferred<ActiveRadioComponent>(ent);
    }

    private void UpdateUi(Entity<HandheldRadioComponent> ent)
    {
        _ui.SetUiState(ent.Owner,
            HandheldRadioUiKey.Key,
            new HandheldRadioBoundUserInterfaceState(
                ent.Comp.SelectedChannel,
                ent.Comp.MinChannel,
                ent.Comp.MaxChannel,
                ent.Comp.MicrophoneEnabled,
                ent.Comp.SpeakerEnabled));
    }

    private ProtoId<RadioChannelPrototype> GetChannelId(int channel)
    {
        return new ProtoId<RadioChannelPrototype>($"Channel{channel}");
    }

    private EntityUid GetUser(EntityUid radio)
    {
        var current = radio;

        for (var i = 0; i < 8; i++)
        {
            if (!_container.TryGetContainingContainer((current, null, null), out var container))
                break;

            current = container.Owner;

            if (HasComp<MobStateComponent>(current))
                return current;
        }

        var parent = Transform(radio).ParentUid;
        return parent.IsValid() ? parent : radio;
    }
}
