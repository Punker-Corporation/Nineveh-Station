using System.Linq;
using System.Numerics;
using System.Threading;
using Content.Client.Verbs;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Input;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Verbs;
using JetBrains.Annotations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Input.Binding;
using Robust.Shared.Map;
using Robust.Shared.Utility;
using static Content.Shared.Interaction.SharedInteractionSystem;
using static Robust.Client.UserInterface.Controls.BoxContainer;
using Direction = Robust.Shared.Maths.Direction;

namespace Content.Client.Examine
{
    [UsedImplicitly]
    public sealed class ExamineSystem : ExamineSystemShared
    {
        [Dependency] private readonly IUserInterfaceManager _userInterfaceManager = default!;
        [Dependency] private readonly IPlayerManager _playerManager = default!;
        [Dependency] private readonly IEyeManager _eyeManager = default!;
        [Dependency] private readonly VerbSystem _verbSystem = default!;
        [Dependency] private readonly SpriteSystem _sprite = default!;

        private List<Verb> _verbList = new();

        public const string StyleClassEntityTooltip = "entity-tooltip";

        private EntityUid _examinedEntity;
        private Popup? _examineTooltipOpen;
        private ScreenCoordinates _popupPos;
        private CancellationTokenSource? _requestCancelTokenSource;
        private int _idCounter;

        public override void Initialize()
        {
            base.Initialize();

            UpdatesOutsidePrediction = true;

            SubscribeLocalEvent<GetVerbsEvent<ExamineVerb>>(AddExamineVerb);

            SubscribeNetworkEvent<ExamineSystemMessages.ExamineInfoResponseMessage>(OnExamineInfoResponse);

            SubscribeLocalEvent<ItemComponent, DroppedEvent>(OnExaminedItemDropped);

            CommandBinds.Builder
                .Bind(ContentKeyFunctions.ExamineEntity, new PointerInputCmdHandler(HandleExamine, outsidePrediction: true))
                .Register<ExamineSystem>();

            _idCounter = 0;
        }

        private void OnExaminedItemDropped(EntityUid item, ItemComponent comp, DroppedEvent args)
        {
            if (!args.User.Valid)
                return;
            if (_examineTooltipOpen == null)
                return;

            if (item == _examinedEntity && args.User == _playerManager.LocalEntity)
                CloseTooltip();
        }

        public override void Update(float frameTime)
        {
            if (_examineTooltipOpen is not {Visible: true}) return;
            if (!_examinedEntity.Valid || _playerManager.LocalEntity is not { } player) return;

            if (!CanExamine(player, _examinedEntity))
                CloseTooltip();
        }

        public override void Shutdown()
        {
            CommandBinds.Unregister<ExamineSystem>();
            base.Shutdown();
        }

        public override bool CanExamine(EntityUid examiner, MapCoordinates target, Ignored? predicate = null, EntityUid? examined = null, ExaminerComponent? examinerComp = null)
        {
            if (!Resolve(examiner, ref examinerComp, false))
                return false;

            if (examinerComp.SkipChecks)
                return true;

            if (examinerComp.CheckInRangeUnOccluded)
            {
                // TODO fix this. This should be using the examiner's eye component, not eye manager.
                var b = _eyeManager.GetWorldViewbounds();
                if (!b.Contains(target.Position))
                    return false;
            }

            return base.CanExamine(examiner, target, predicate, examined, examinerComp);
        }

        private bool HandleExamine(in PointerInputCmdHandler.PointerInputCmdArgs args)
        {
            var entity = args.EntityUid;

            if (!args.EntityUid.IsValid() || !Exists(entity))
            {
                return false;
            }

            if (_playerManager.LocalEntity is not { } player ||
                !CanExamine(player, entity))
            {
                return false;
            }

            DoExamine(entity);
            return true;
        }

        private void AddExamineVerb(GetVerbsEvent<ExamineVerb> args)
        {
            if (!CanExamine(args.User, args.Target))
                return;

            // Basic examine verb.
            ExamineVerb verb = new();
            verb.Category = VerbCategory.Examine;
            verb.Priority = 10;
            // Center it on the entity if they use the verb instead.
            verb.Act = () => DoExamine(args.Target, false);
            verb.Text = Loc.GetString("examine-verb-name");
            verb.Icon = new SpriteSpecifier.Texture(new ("/Textures/Interface/VerbIcons/examine.svg.192dpi.png"));
            verb.ShowOnExamineTooltip = false;
            verb.ClientExclusive = true;
            args.Verbs.Add(verb);
        }

        private void OnExamineInfoResponse(ExamineSystemMessages.ExamineInfoResponseMessage ev)
        {
            var player = _playerManager.LocalEntity;
            if (player == null)
                return;

            // Prevent updating a new tooltip.
            if (ev.Id != 0 && ev.Id != _idCounter)
                return;

            // Tooltips coming in from the server generally prioritize
            // opening at the old tooltip rather than the cursor/another entity,
            // since there's probably one open already if it's coming in from the server.
            var entity = GetEntity(ev.EntityUid);

            // Fire added start
            if (TerminatingOrDeleted(entity))
            {
                CloseTooltip();
                return;
            }
            // Fire added end

            OpenTooltip(player.Value, entity, ev.CenterAtCursor, ev.OpenAtOldTooltip, ev.KnowTarget);
            UpdateTooltipInfo(player.Value, entity, ev.Message, ev.Verbs, getVerbs: false);
        }

        public override void SendExamineTooltip(EntityUid player, EntityUid target, FormattedMessage message, bool getVerbs, bool centerAtCursor)
        {
            OpenTooltip(player, target, centerAtCursor);
            UpdateTooltipInfo(player, target, message, getVerbs: getVerbs);
        }

        /// <summary>
        ///     Abre o dossiê visual antes de preencher os detalhes vindos do cliente e do servidor.
        /// </summary>
        public void OpenTooltip(EntityUid player, EntityUid target, bool centeredOnCursor=true, bool openAtOldTooltip=true, bool knowTarget = true)
        {
            // Mantém a posição anterior para que exames consecutivos pareçam uma inspeção contínua.
            ScreenCoordinates? oldTooltipPos = _examineTooltipOpen != null ? _popupPos : null;
            CloseTooltip();

            _examinedEntity = target;

            const float minWidth = 340;

            if (openAtOldTooltip && oldTooltipPos != null)
            {
                _popupPos = oldTooltipPos.Value;
            }
            else if (centeredOnCursor)
            {
                _popupPos = _userInterfaceManager.MousePositionScaled;
            }
            else
            {
                _popupPos = _eyeManager.CoordinatesToScreen(Transform(target).Coordinates);
                _popupPos = _userInterfaceManager.ScreenToUIPosition(_popupPos);
            }

            _examineTooltipOpen = new Popup { MaxWidth = 460 };
            _userInterfaceManager.ModalRoot.AddChild(_examineTooltipOpen);
            var panel = new PanelContainer() { Name = "ExaminePopupPanel" };
            panel.AddStyleClass(StyleClassEntityTooltip);
            panel.ModulateSelfOverride = Color.FromHex("#111015").WithAlpha(0.96f);
            _examineTooltipOpen.AddChild(panel);

            var vBox = new BoxContainer
            {
                Name = "ExaminePopupVbox",
                Orientation = LayoutOrientation.Vertical,
                MaxWidth = _examineTooltipOpen.MaxWidth
            };
            panel.AddChild(vBox);

            var dossierLabel = new RichTextLabel
            {
                Margin = new Thickness(8, 6, 8, 2)
            };
            dossierLabel.SetMessage(FormattedMessage.FromMarkupPermissive(
                $"[color=#b94747][bold]{Loc.GetString("examine-ui-title")}[/bold][/color]"));
            vBox.AddChild(dossierLabel);

            var hBox = new BoxContainer
            {
                Orientation = LayoutOrientation.Horizontal,
                SeparationOverride = 5,
                Margin = new Thickness(8, 0, 8, 4)
            };

            vBox.AddChild(hBox);

            if (HasComp<SpriteComponent>(target))
            {
                var spriteView = new SpriteView
                {
                    OverrideDirection = Direction.South,
                    SetSize = new Vector2(42, 42)
                };
                spriteView.SetEntity(target);
                hBox.AddChild(spriteView);
            }

            if (knowTarget)
            {
                var itemName = FormattedMessage.EscapeText(Identity.Name(target, EntityManager, player));
                var labelMessage = FormattedMessage.FromMarkupPermissive($"[bold]{itemName}[/bold]");
                var label = new RichTextLabel();
                label.SetMessage(labelMessage);
                hBox.AddChild(label);
            }
            else
            {
                var label = new RichTextLabel();
                label.SetMessage(FormattedMessage.FromMarkupPermissive(
                    $"[bold]{Loc.GetString("examine-ui-unknown")}[/bold]"));
                hBox.AddChild(label);
            }

            var contentPanel = new PanelContainer
            {
                Name = "ExamineContentPanel",
                Margin = new Thickness(8, 2, 8, 8),
            };
            contentPanel.ModulateSelfOverride = Color.FromHex("#070709").WithAlpha(0.86f);
            vBox.AddChild(contentPanel);

            var contentBox = new BoxContainer
            {
                Name = "ExamineContentVbox",
                Orientation = LayoutOrientation.Vertical,
                Margin = new Thickness(8, 6, 8, 8),
                MaxWidth = _examineTooltipOpen.MaxWidth - 24
            };
            contentPanel.AddChild(contentBox);

            panel.Measure(Vector2Helpers.Infinity);
            var size = Vector2.Max(new Vector2(minWidth, 0), panel.DesiredSize);

            _examineTooltipOpen.Open(UIBox2.FromDimensions(_popupPos.Position, size));
        }

        /// <summary>
        ///     Preenche o dossiê com observações físicas e verbos contextuais.
        /// </summary>
        public void UpdateTooltipInfo(EntityUid player, EntityUid target, FormattedMessage message, List<Verb>? verbs=null, bool getVerbs = true)
        {
            var vBox = _examineTooltipOpen?.GetChild(0).GetChild(0);
            if (vBox == null)
            {
                return;
            }

            var contentBox = vBox.Children.FirstOrDefault(c => c.Name == "ExamineContentVbox") as BoxContainer;
            contentBox ??= (BoxContainer)vBox;
            ClearChildren(contentBox);

            contentBox.AddChild(BuildSectionLabel("examine-ui-observations"));
            var summaryLabel = new RichTextLabel { Margin = new Thickness(0, 2, 0, 6) };
            summaryLabel.SetMessage(BuildObservationSummary(player, target));
            contentBox.AddChild(summaryLabel);

            var pushedDescription = false;
            foreach (var msg in message.Nodes)
            {
                if (msg.Name != null)
                    continue;

                var text = msg.Value.StringValue ?? "";

                if (string.IsNullOrWhiteSpace(text))
                    continue;

                contentBox.AddChild(BuildSectionLabel("examine-ui-physical-record"));
                var richLabel = new RichTextLabel() { Margin = new Thickness(0, 2, 0, 4)};
                richLabel.SetMessage(message);
                contentBox.AddChild(richLabel);
                pushedDescription = true;
                break;
            }

            if (!pushedDescription)
            {
                var emptyLabel = new RichTextLabel { Margin = new Thickness(0, 2, 0, 4) };
                emptyLabel.SetMessage(FormattedMessage.FromMarkupPermissive(
                    $"[color=#888888]{Loc.GetString("examine-ui-no-visible-details")}[/color]"));
                contentBox.AddChild(emptyLabel);
            }

            var totalVerbs = _verbSystem.GetLocalVerbs(target, player, typeof(ExamineVerb));

            // We still need client-exclusive verbs even when the server sends its data in so if that's the case
            // we remove any non-client-exclusive verbs.
            if (!getVerbs)
            {
                _verbList.AddRange(totalVerbs);

                foreach (var verb in _verbList)
                {
                    if (!verb.ClientExclusive)
                    {
                        totalVerbs.Remove(verb);
                    }
                }

                _verbList.Clear();
            }

            if (verbs != null)
            {
                totalVerbs.UnionWith(verbs);
            }

            AddVerbsToTooltip(totalVerbs);
        }

        private FormattedMessage BuildObservationSummary(EntityUid player, EntityUid target)
        {
            var distance = (Transform(target).WorldPosition - Transform(player).WorldPosition).Length();
            var distanceKey = distance switch
            {
                <= 1.25f => "examine-ui-distance-touch",
                <= 3.0f => "examine-ui-distance-close",
                <= 7.0f => "examine-ui-distance-mid",
                _ => "examine-ui-distance-far"
            };

            var detailKey = IsInDetailsRange(player, target)
                ? "examine-ui-details-rich"
                : "examine-ui-details-poor";

            var summary = new FormattedMessage();
            summary.AddMarkupPermissive(
                $"[color=#c9c0b8]{Loc.GetString(distanceKey)}[/color]\n[color=#8d8580]{Loc.GetString(detailKey)}[/color]");
            return summary;
        }

        private RichTextLabel BuildSectionLabel(string locId)
        {
            var label = new RichTextLabel
            {
                Margin = new Thickness(0, 4, 0, 1)
            };
            label.SetMessage(FormattedMessage.FromMarkupPermissive(
                $"[color=#b94747][bold]{Loc.GetString(locId)}[/bold][/color]"));
            return label;
        }

        private static void ClearChildren(Control control)
        {
            foreach (var child in control.Children.ToArray())
            {
                child.Dispose();
            }
        }

        private void AddVerbsToTooltip(IEnumerable<Verb> verbs)
        {
            if (_examineTooltipOpen == null)
                return;

            var buttonsHBox = new BoxContainer
            {
                Name = "ExamineButtonsHBox",
                Orientation = LayoutOrientation.Horizontal,
                HorizontalAlignment = Control.HAlignment.Stretch,
                VerticalAlignment = Control.VAlignment.Bottom,
            };

            var hoverExamineBox = new BoxContainer
            {
                Name = "HoverExamineHBox",
                Orientation = LayoutOrientation.Horizontal,
                HorizontalAlignment = Control.HAlignment.Left,
                VerticalAlignment = Control.VAlignment.Center,
                HorizontalExpand = true
            };

            var clickExamineBox = new BoxContainer
            {
                Name = "ClickExamineHBox",
                Orientation = LayoutOrientation.Horizontal,
                HorizontalAlignment = Control.HAlignment.Right,
                VerticalAlignment = Control.VAlignment.Center,
                HorizontalExpand = true
            };

            // Examine button time
            foreach (var verb in verbs)
            {
                if (verb is not ExamineVerb examine)
                    continue;

                if (examine.Icon == null)
                    continue;

                if (!examine.ShowOnExamineTooltip)
                    continue;

                var button = new ExamineButton(examine, _sprite);

                if (examine.HoverVerb)
                {
                    hoverExamineBox.AddChild(button);
                }
                else
                {
                    button.OnPressed += VerbButtonPressed;
                    clickExamineBox.AddChild(button);
                }
            }

            var vbox = _examineTooltipOpen?.GetChild(0).GetChild(0);
            if (vbox == null)
            {
                buttonsHBox.Dispose();
                return;
            }

            // Remove any existing buttons hbox, in case we generated it from the client
            // then received ones from the server
            var hbox = vbox.Children.Where(c => c.Name == "ExamineButtonsHBox").ToArray();
            if (hbox.Any())
            {
                vbox.Children.Remove(hbox.First());
            }
            buttonsHBox.AddChild(hoverExamineBox);
            buttonsHBox.AddChild(clickExamineBox);
            vbox.AddChild(buttonsHBox);
        }

        public void VerbButtonPressed(BaseButton.ButtonEventArgs obj)
        {
            if (obj.Button is ExamineButton button)
            {
                _verbSystem.ExecuteVerb(_examinedEntity, button.Verb);
                if (button.Verb.CloseMenu ?? button.Verb.CloseMenuDefault)
                    CloseTooltip();
            }
        }

        public void DoExamine(EntityUid entity, bool centeredOnCursor = true, EntityUid? userOverride = null)
        {
            var playerEnt = userOverride ?? _playerManager.LocalEntity;
            if (playerEnt == null)
                return;

            FormattedMessage message;

            OpenTooltip(playerEnt.Value, entity, centeredOnCursor, false);

            // Always update tooltip info from client first.
            // If we get it wrong, server will correct us later anyway.
            // This will usually be correct (barring server-only components, which generally only adds, not replaces text)
            message = GetExamineText(entity, playerEnt);
            UpdateTooltipInfo(playerEnt.Value, entity, message);

            if (!IsClientSide(entity))
            {
                // Ask server for extra examine info.
                unchecked
                {
                    _idCounter += 1;
                }
                RaiseNetworkEvent(new ExamineSystemMessages.RequestExamineInfoMessage(GetNetEntity(entity), _idCounter, true));
            }

            RaiseLocalEvent(entity, new ClientExaminedEvent(entity, playerEnt.Value));
        }

        private void CloseTooltip()
        {
            if (_examineTooltipOpen != null)
            {
                foreach (var control in _examineTooltipOpen.Children)
                {
                    if (control is ExamineButton button)
                    {
                        button.OnPressed -= VerbButtonPressed;
                    }
                }
                _examineTooltipOpen.Dispose();
                _examineTooltipOpen = null;
            }

            if (_requestCancelTokenSource != null)
            {
                _requestCancelTokenSource.Cancel();
                _requestCancelTokenSource = null;
            }
        }
    }

    /// <summary>
    /// An entity was examined on the client.
    /// </summary>
    public sealed class ClientExaminedEvent : EntityEventArgs
    {
        /// <summary>
        ///     The entity performing the examining.
        /// </summary>
        public readonly EntityUid Examiner;

        /// <summary>
        ///     Entity being examined, for broadcast event purposes.
        /// </summary>
        public readonly EntityUid Examined;

        public ClientExaminedEvent(EntityUid examined, EntityUid examiner)
        {
            Examined = examined;
            Examiner = examiner;
        }
    }
}
