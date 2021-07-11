using BepInEx.IL2CPP;
using Hazel;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.Roles;
using Peasmod.Utility;
using UnityEngine;

namespace Peasmod.Roles
{
    [RegisterCustomRole]
    public class Demon : BaseRole
    {
        public Demon(BasePlugin plugin) : base(plugin) { }

        public override string Name => "Demon";

        public override string Description => "Swap into the afterlife";

        public override string TaskText => "Swap into the afterlife";

        public override Color Color => new Color(255/255f, 46/255f, 0/255f);

        public override int Limit => (int) Settings.DemonAmount.GetValue();

        public override Team Team => Team.Crewmate;

        public override Visibility Visibility => Visibility.NoOne;

        public override bool HasToDoTasks => true;

        public RoleButton Button;

        public override void OnGameStart()
        {
            Button = new RoleButton(() =>
                {
                    PlayerControl.LocalPlayer.Die(DeathReason.Kill);
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.DemonAbility, SendOption.None, -1);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(true);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }, Settings.DemonCooldown.GetValue(), Utils.CreateSprite("Buttons.Button1.png"), Vector2.zero, true, 
                Settings.DemonDuration.GetValue(), () =>
                {
                    PlayerControl.LocalPlayer.Revive();
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.DemonAbility, SendOption.None, -1);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(false);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }, this);
        }

        public override void OnUpdate()
        {
            
        }

        public override void OnMeetingUpdate(MeetingHud meeting)
        {
            
        }
    }
}