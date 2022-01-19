using System;
using System.Linq;
using BepInEx.IL2CPP;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.Managers;
using PeasAPI.Options;
using PeasAPI.Roles;
using Peasmod.Roles.Impostor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Peasmod.Roles.GameModes
{
    [RegisterCustomRole]
    public class GodImpostor : BaseRole
    {
        public GodImpostor(BasePlugin plugin) : base(plugin)
        {
            Instance = this;
        }

        public override string Name => "God Impostor";
        public override string Description => "Use your abilities to kill every crewmate";
        public override string LongDescription => "";
        public override string TaskText => "Use your abilities to kill every crewmate";
        public override Color Color => Palette.ImpostorRed;
        public override Visibility Visibility => Visibility.Impostor;
        public override Team Team => Team.Impostor;
        public override bool HasToDoTasks => true;
        public override bool CreateRoleOption => false;
        public override Type[] GameModeWhitelist { get; } = { typeof(Peasmod.GameModes.GodImpostor) };
        public override int Count => 1;
        public override int MaxCount => 1;
        public override bool CanVent => true;
        public override bool CanKill(PlayerControl victim = null) => !victim || victim.Data.Role.IsImpostor;
        public override bool CanSabotage(SystemTypes? sabotage) => true;

        public static GodImpostor Instance;

        public CustomButton MorphButton;
        public CustomButton VentBuildButton;
        public CustomButton DragBodyButton;
        public CustomButton InvisibilityButton;
        public CustomButton FreezeTimeButton;

        public override void OnGameStart()
        {
            MorphButton = CustomButton.AddButton(
                () =>
                {
                    PlayerMenuManager.OpenPlayerMenu(PlayerControl.AllPlayerControls.ToArray().ToList().ConvertAll(p => p.PlayerId),
                        player => PlayerControl.LocalPlayer.RpcShapeshift(player, false), () => MorphButton.SetCoolDown(0));
                }, Settings.MorphingCooldown.Value,
                PeasAPI.Utility.CreateSprite("Peasmod.Resources.Buttons.Morph.png", 737f), p => p.IsRole(this) && !p.Data.IsDead, _ => true,
                text: "<size=40%>Morph");
            MorphButton.Enabled = MorphButton.Visible = Settings.Morphing.Value;

            VentBuildButton = CustomButton.AddButton(
                () =>
                { PlayerControl.LocalPlayer.RpcCreateVent(); },
                ((CustomNumberOption) PeasAPI.Roles.RoleManager.GetRole<EvilBuilder>().AdvancedOptions["VentBuildingCooldown"]).Value,
                PeasAPI.Utility.CreateSprite("Peasmod.Resources.Buttons.CreateVent.png", 552f), p => p.IsRole(this) && !p.Data.IsDead, _ => true, text: "<size=40%>Build");
            VentBuildButton.Enabled = VentBuildButton.Visible = Settings.VentBuilding.Value;

            DragBodyButton = CustomButton.AddButton(() =>
                {
                    if (Undertaker.Instance.CarryingBody)
                    {
                        Undertaker.Instance.CarryingBody = false;
                        DragBodyButton.SetImage(Utility.CreateSprite("Peasmod.Resources.Buttons.DragBody.png", 702f));
                        DragBodyButton.Text = "<size=40%>Drag";
                        PlayerControl.LocalPlayer.RpcDropBody();
                    }
                    else
                    {
                        Undertaker.Instance.CarryingBody = true;
                        DragBodyButton.SetImage(Utility.CreateSprite("Peasmod.Resources.Buttons.DropBody.png", 803f));
                        DragBodyButton.Text = "<size=40%>Drop";
                        PlayerControl.LocalPlayer.RpcDragBody(Undertaker.Instance.TargetBody.GetComponent<DeadBody>().ParentId);
                    }
                }, 0f, Utility.CreateSprite("Peasmod.Resources.Buttons.DragBody.png", 702f), p => p.IsRole(this) && !p.Data.IsDead, _ => true,
                text: "<size=40%>Drag");
            DragBodyButton.Enabled = DragBodyButton.Visible = Settings.BodyDragging.Value;

            InvisibilityButton = CustomButton.AddButton(
                () => { PlayerControl.LocalPlayer.RpcGoInvisible(true); }, ((CustomNumberOption) PeasAPI.Roles.RoleManager.GetRole<Ninja>().AdvancedOptions["InvisibilityCooldown"]).Value,
                Utility.CreateSprite("Peasmod.Resources.Buttons.Hide.png", 794f), p => p.IsRole(this) && !p.Data.IsDead, _ => true,
                effectDuration: ((CustomNumberOption) PeasAPI.Roles.RoleManager.GetRole<Ninja>().AdvancedOptions["InvisibilityDuration"]).Value, onEffectEnd: () => { PlayerControl.LocalPlayer.RpcGoInvisible(false); },
                text: "<size=40%>Hide");
            InvisibilityButton.Enabled = InvisibilityButton.Visible = Settings.Invisibility.Value;

            FreezeTimeButton = CustomButton.AddButton(
                () => { PlayerControl.LocalPlayer.RpcFreeze(true); }, ((CustomNumberOption) Glaciater.Instance.AdvancedOptions["FreezeCooldown"]).Value,
                Utility.CreateSprite("Peasmod.Resources.Buttons.Freezing.png", 851f), p => p.IsRole(this) && !p.Data.IsDead, _ => true,
                effectDuration: ((CustomNumberOption) Glaciater.Instance.AdvancedOptions["FreezeDuration"]).Value, onEffectEnd: () => { PlayerControl.LocalPlayer.RpcFreeze(false); },
                text: "<size=40%>Freeze");
            FreezeTimeButton.Enabled = FreezeTimeButton.Visible = Settings.Freeze.Value;
        }

        public override void OnUpdate()
        {
            if (VentBuildButton != null && Object.FindObjectOfType<Vent>() != null)
            {
                var vent = Object.FindObjectOfType<Vent>().gameObject;
                var ventSize = Vector2.Scale(vent.GetComponent<BoxCollider2D>().size, vent.transform.localScale) * 0.75f;
                var hits = Physics2D.OverlapBoxAll(PlayerControl.LocalPlayer.transform.position, ventSize, 0).Where(c => (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer != 8 && c.gameObject.layer != 5).ToArray();
                VentBuildButton.Usable = hits.Length == 0;
            }

            if (DragBodyButton != null)
            {
                if (Undertaker.Instance.CarriedBodys != null)
                {
                    for (int i = 0; i < Undertaker.Instance.CarriedBodys.Count; i++)
                    {
                        var player = Undertaker.Instance.CarriedBodys.Keys.ToArray()[i].GetPlayer();
                        var body = Undertaker.Instance.CarriedBodys.Values.ToArray()[i];

                        var bodyObject = UnityEngine.Object.FindObjectsOfType<DeadBody>()
                            .Where(_body => _body.ParentId == body).ToList();
                        if (bodyObject.Count == 0)
                        {
                            if (player.IsLocal())
                            {
                                Undertaker.Instance.CarryingBody = false;
                                DragBodyButton.SetImage(Utility.CreateSprite("Peasmod.Resources.Buttons.DragBody.png"));
                                Undertaker.RpcDragBody(PlayerControl.LocalPlayer, false, byte.MaxValue);
                            }

                            continue;
                        }

                        Undertaker.MoveBody(player, body);
                    }
                }
                
                if (Undertaker.Instance.CarryingBody)
                {
                    DragBodyButton.Usable = true;
                }
                else
                {
                    Undertaker.Instance.TargetBody = null;
                    DragBodyButton.Usable = false;

                    var bodys = Physics2D
                        .OverlapCircleAll(PlayerControl.LocalPlayer.GetTruePosition(),
                            PlayerControl.LocalPlayer.MaxReportDistance - 2f, Constants.PlayersOnlyMask)
                        .Where(collider => collider.CompareTag("DeadBody")).ToList();
                    if (bodys.Count != 0)
                    {
                        Undertaker.Instance.TargetBody = bodys[0].gameObject;
                        DragBodyButton.Usable = true;
                    }
                }
            }
        }
    }
}