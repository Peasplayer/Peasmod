using System;
using System.Linq;
using BepInEx.IL2CPP;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.Managers;
using PeasAPI.Roles;
using Reactor.Networking;
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
        public override string TaskText => "Use your abilities to kill every crewmate";
        public override Color Color => Palette.ImpostorRed;
        public override Visibility Visibility => Visibility.Impostor;
        public override Team Team => Team.Impostor;
        public override bool HasToDoTasks => true;
        public override Type[] GameModeWhitelist { get; } = { typeof(Peasmod.GameModes.GodImpostor) };
        public override int Limit => 1;
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
            MorphButton = CustomButton.AddRoleButton(
                () =>
                {
                    PlayerMenuManager.OpenPlayerMenu(PlayerControl.AllPlayerControls.ToArray().ToList(),
                        player => PlayerControl.LocalPlayer.RpcShapeshift(player, false));
                }, Settings.MorphingCooldown.Value,
                PeasAPI.Utility.CreateSprite("Peasmod.Resources.Buttons.Morph.png", 737f), Vector2.zero, false, this,
                "<size=40%>Morph");
            MorphButton.Enabled = MorphButton.Visible = Settings.Morphing.Value;

            VentBuildButton = CustomButton.AddRoleButton(
                () =>
                {
                    var pos = PlayerControl.LocalPlayer.transform.position;
                    Builder.RpcCreateVent(PlayerControl.LocalPlayer, pos.x, pos.y, pos.z);
                },
                Settings.VentBuildingCooldown.Value,
                PeasAPI.Utility.CreateSprite("Peasmod.Resources.Buttons.CreateVent.png", 552f), Vector2.zero, false,
                this, "<size=40%>Build");
            VentBuildButton.Enabled = VentBuildButton.Visible = Settings.VentBuilding.Value;

            DragBodyButton = CustomButton.AddRoleButton(() =>
                {
                    if (Undertaker.Instance.CarryingBody)
                    {
                        Undertaker.Instance.CarryingBody = false;
                        DragBodyButton.SetImage(Utility.CreateSprite("Peasmod.Resources.Buttons.DragBody.png", 702f));
                        DragBodyButton.Text = "<size=40%>Drag";
                        Undertaker.RpcDragBody(PlayerControl.LocalPlayer, false, byte.MaxValue);
                    }
                    else
                    {
                        Undertaker.Instance.CarryingBody = true;
                        DragBodyButton.SetImage(Utility.CreateSprite("Peasmod.Resources.Buttons.DropBody.png", 803f));
                        DragBodyButton.Text = "<size=40%>Drop";
                        Undertaker.RpcDragBody(PlayerControl.LocalPlayer, true,
                            Undertaker.Instance.TargetBody.GetComponent<DeadBody>().ParentId);
                    }
                }, 0f, Utility.CreateSprite("Peasmod.Resources.Buttons.DragBody.png", 702f), Vector2.zero, false, this,
                "<size=40%>Drag");
            DragBodyButton.Enabled = DragBodyButton.Visible = Settings.BodyDragging.Value;

            InvisibilityButton = CustomButton.AddRoleButton(
                () => { Ninja.RpcGoInvisible(PlayerControl.LocalPlayer, true); }, Settings.InvisibilityCooldown.Value,
                Utility.CreateSprite("Peasmod.Resources.Buttons.Hide.png", 794f), Vector2.zero, false, this,
                Settings.InvisibilityDuration.Value, () => { Ninja.RpcGoInvisible(PlayerControl.LocalPlayer, false); },
                "<size=40%>Hide");
            InvisibilityButton.Enabled = InvisibilityButton.Visible = Settings.Invisibility.Value;

            FreezeTimeButton = CustomButton.AddRoleButton(
                () => { Glaciater.RpcFreeze(PlayerControl.LocalPlayer, true); }, Settings.FreezeCooldown.Value,
                Utility.CreateSprite("Peasmod.Resources.Buttons.Freezing.png", 851f), Vector2.zero, false, this,
                Settings.FreezeDuration.Value, () => { Glaciater.RpcFreeze(PlayerControl.LocalPlayer, false); },
                "<size=40%>Freeze");
            FreezeTimeButton.Enabled = FreezeTimeButton.Visible = Settings.Freeze.Value;
        }

        public override void OnUpdate()
        {
            if (VentBuildButton == null)
                return;
            if (Object.FindObjectOfType<Vent>() == null)
                return;
            var vent = Object.FindObjectOfType<Vent>().gameObject;
            var ventSize = Vector2.Scale(vent.GetComponent<BoxCollider2D>().size, vent.transform.localScale) * 0.75f;
            var hits = Physics2D.OverlapBoxAll(PlayerControl.LocalPlayer.transform.position, ventSize, 0).Where(c => (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer != 8 && c.gameObject.layer != 5).ToArray();
            VentBuildButton.Usable = hits.Length == 0;
            
            if (DragBodyButton == null)
                return;

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