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
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.Radio.EntitySystems;

public sealed class HandheldRadioSystem : SharedRadioDeviceSystem
{
    [Dependency] private readonly ChatSystem _chat = default!;
    [Dependency] private readonly ContainerSystem _container = default!;
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly RadioSystem _radio = default!;
    [Dependency] private readonly RadioDeviceSystem _radioDevice = default!;
    [Dependency] private readonly UserInterfaceSystem _ui = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    private static readonly string[] StaticPhrases =
    {
        "tem alguem respirando no canal",
        "nao abra a porta vermelha",
        "a luz piscou tres vezes e a pele veio junto",
        "eles estao contando os dentes no balde",
        "o sangue esta no fio terra e ainda conduz",
        "voce ainda esta transmitindo",
        "tem carne presa no microfone",
        "a garganta no armario acabou de chamar seu nome",
        "nao e chiado, sao unhas dentro da parede",
        "a maca esta vazia mas o osso continua falando",
        "a frequencia lembra um corredor",
    };

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

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<HandheldRadioComponent>();
        while (query.MoveNext(out var uid, out var handheld))
        {
            handheld.NextCongestionUpdate -= frameTime;
            if (handheld.NextCongestionUpdate > 0f)
                continue;

            handheld.NextCongestionUpdate = handheld.CongestionUpdateSeconds + _random.NextFloat(-20f, 40f);
            var channelBias = handheld.SelectedChannel is { } channel
                ? GetChannelCongestion(channel, handheld.Drift)
                : 0f;

            handheld.Drift = Math.Clamp(handheld.Drift + _random.NextFloat(-0.018f, 0.022f), 0f, 1f);
            handheld.Congestion = Math.Clamp((handheld.Congestion * 0.92f) + (channelBias * 0.08f) + _random.NextFloat(-0.018f, 0.020f), 0f, 1f);
            handheld.SignalQuality = Math.Clamp(1f - handheld.Congestion * 0.74f - MathF.Abs(handheld.Drift - 0.5f) * 0.18f, 0.05f, 1f);

            if (handheld.SelectedChannel is { } active)
            {
                handheld.AdjacentCongestionLow = active > handheld.MinChannel ? GetChannelCongestion(active - 1, handheld.Drift) : 1f;
                handheld.AdjacentCongestionHigh = active < handheld.MaxChannel ? GetChannelCongestion(active + 1, handheld.Drift) : 1f;
            }
            else
            {
                handheld.AdjacentCongestionLow = 0f;
                handheld.AdjacentCongestionHigh = 0f;
            }

            if (_random.Prob(0.35f + handheld.Congestion * 0.45f))
            {
                handheld.LastStaticPhrase = _random.Pick(StaticPhrases);
                if (handheld.SpeakerEnabled)
                    _audio.PlayPvs(new SoundPathSpecifier(handheld.StaticSound), uid, AudioParams.Default.WithVolume(-9f));
            }

            Dirty(uid, handheld);
            UpdateUi((uid, handheld));
        }
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
        else if (ent.Comp.SpeakerEnabled)
        {
            activeRadio = EnsureComp<ActiveRadioComponent>(ent);
            activeRadio.Channels.Clear();
            activeRadio.Channels.Add(radioChannel);
            Dirty(ent.Owner, activeRadio);
        }
        else
        {
            RemCompDeferred<ActiveRadioComponent>(ent);
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
                ent.Comp.SpeakerEnabled,
                ent.Comp.Congestion,
                ent.Comp.AdjacentCongestionLow,
                ent.Comp.AdjacentCongestionHigh,
                ent.Comp.SignalQuality,
                ent.Comp.Drift,
                ent.Comp.LastStaticPhrase));
    }

    private ProtoId<RadioChannelPrototype> GetChannelId(int channel)
    {
        return new ProtoId<RadioChannelPrototype>($"Channel{channel}");
    }

    private static float GetChannelCongestion(int channel, float drift)
    {
        var slowBand = MathF.Sin(channel * 0.37f + drift * 2.1f);
        var carrierBeat = MathF.Sin(channel * 1.71f + drift * 0.9f);
        return Math.Clamp(0.48f + slowBand * 0.31f + carrierBeat * 0.17f, 0f, 1f);
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
