using System.Collections.Generic;
using System.Linq;
using BepInEx.IL2CPP;
using Hazel;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.Roles;
using Reactor;
using Reactor.Extensions;
using Reactor.Networking;
using UnityEngine;

namespace Peasmod.Roles
{
    [RegisterCustomRole]
    public class Doctor : BaseRole
    {
        public Doctor(BasePlugin plugin) : base(plugin)
        {
        }

        public override string Name => "Doctor";

        public override string Description => "Revive dead crewmates";

        public override string TaskText => "Revive dead crewmates";

        public override Color Color => ModdedPalette.DoctorColor;

        public override int Limit => (int)Settings.DoctorAmount.Value;

        public override Team Team => Team.Crewmate;

        public override Visibility Visibility => Visibility.NoOne;

        public override bool HasToDoTasks => true;

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
                        Rpc<DoctorAbilityRpc>.Instance.Send(new DoctorAbilityRpc.Data(_bodys[0].ParentId.GetPlayer()));

                        var player = _bodys[0].ParentId.GetPlayer();
                        player.Revive();
                        player.transform.position = _bodys[0].transform.position;
                        _bodys[0].gameObject.Destroy();
                    }
                }, Settings.DoctorCooldown.Value,
                PeasAPI.Utility.CreateSprite("Peasmod.Resources.Buttons.Revive.png", 803f), Vector2.zero, false, this, "<size=40%>Revive");
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

        public override void OnMeetingUpdate(MeetingHud meeting)
        {
        }

        [RegisterCustomRpc((uint)CustomRpcCalls.DoctorAbility)]
        public class DoctorAbilityRpc : PlayerCustomRpc<PeasmodPlugin, DoctorAbilityRpc.Data>
        {
            public DoctorAbilityRpc(PeasmodPlugin plugin, uint id) : base(plugin, id)
            {
            }

            public readonly struct Data
            {
                public readonly PlayerControl Player;

                public Data(PlayerControl player)
                {
                    Player = player;
                }
            }

            public override RpcLocalHandling LocalHandling => RpcLocalHandling.None;

            public override void Write(MessageWriter writer, Data data)
            {
                writer.Write(data.Player.PlayerId);
            }

            public override Data Read(MessageReader reader)
            {
                return new Data(reader.ReadByte().GetPlayer());
            }

            public override void Handle(PlayerControl innerNetObject, Data data)
            {
                data.Player.Revive();
                data.Player.transform.position = Object.FindObjectsOfType<DeadBody>().Where(body => body.ParentId == data.Player.PlayerId).ToList()[0].transform.position;
                Object.FindObjectsOfType<DeadBody>().Where(body => body.ParentId == data.Player.PlayerId).ToList()[0].gameObject.Destroy();
            }
        }
    }
}