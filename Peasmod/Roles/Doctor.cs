using System.Collections.Generic;
using System.Linq;
using BepInEx.IL2CPP;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.Roles;
using Reactor.Extensions;
using Reactor.Networking.MethodRpc;
using UnityEngine;

namespace Peasmod.Roles
{
    [RegisterCustomRole]
    public class Doctor : BaseRole
    {
        public Doctor(BasePlugin plugin) : base(plugin) { }

        public override string Name => "Doctor";
        public override string Description => "Revive dead crewmates";
        public override string TaskText => "Revive dead crewmates";
        public override Color Color => ModdedPalette.DoctorColor;
        public override Visibility Visibility => Visibility.NoOne;
        public override Team Team => Team.Crewmate;
        public override bool HasToDoTasks => true;
        public override int Limit => (int)Settings.DoctorAmount.Value;

        public CustomButton Button;

        public override void OnGameStart()
        {
            Button = CustomButton.AddRoleButton(() =>
                {
                    List<DeadBody> _bodys = Physics2D
                        .OverlapCircleAll(PlayerControl.LocalPlayer.GetTruePosition(),
                            PlayerControl.LocalPlayer.MaxReportDistance - 2f, Constants.PlayersOnlyMask)
                        .Where(collider => collider.CompareTag("DeadBody")).ToList().ConvertAll(
                            collider => collider.GetComponent<DeadBody>());

                    if (_bodys.Count != 0)
                    {
                        RpcRevive(_bodys[0].ParentId.GetPlayer());

                        var player = _bodys[0].ParentId.GetPlayer();
                        player.Revive();
                        player.transform.position = _bodys[0].transform.position;
                        _bodys[0].gameObject.Destroy();
                    }
                }, Settings.DoctorCooldown.Value,
                Utility.CreateSprite("Peasmod.Resources.Buttons.Revive.png", 803f), this, text: "<size=40%>Revive");
        }

        public override void OnUpdate()
        {
            if (Button != null)
            {
                if (Button.KillButtonManager.graphic != null)
                {
                    List<DeadBody> bodys = Physics2D
                        .OverlapCircleAll(PlayerControl.LocalPlayer.GetTruePosition(),
                            PlayerControl.LocalPlayer.MaxReportDistance - 2f, Constants.PlayersOnlyMask)
                        .Where(collider => collider.CompareTag("DeadBody")).ToList().ConvertAll(
                            collider => collider.GetComponent<DeadBody>());

                    if (bodys.Count == 0)
                    {
                        Button.KillButtonManager.graphic.color = Palette.DisabledClear;
                        Button.Usable = false;
                    }
                    else
                    {
                        Button.KillButtonManager.graphic.color = Palette.EnabledColor;
                        Button.Usable = true;
                    }
                }
            }
        }

        [MethodRpc((uint) CustomRpcCalls.DoctorAbility)]
        public static void RpcRevive(PlayerControl sender)
        {
            sender.Revive();
            sender.transform.position = Object.FindObjectsOfType<DeadBody>().Where(body => body.ParentId == sender.PlayerId).ToList()[0].transform.position;
            Object.FindObjectsOfType<DeadBody>().Where(body => body.ParentId == sender.PlayerId).ToList()[0].gameObject.Destroy();
        }
    }
}