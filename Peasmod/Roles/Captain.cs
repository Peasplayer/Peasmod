using BepInEx.IL2CPP;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.Roles;
using UnityEngine;

namespace Peasmod.Roles
{
    [RegisterCustomRole]
    public class Captain : BaseRole
    {
        public Captain(BasePlugin plugin) : base(plugin) { }

        public override string Name => "Captain";
        public override string Description => "Keep your crew safe";
        public override string TaskText => "Keep your crew safe";
        public override Color Color => ModdedPalette.CaptainColor;
        public override Visibility Visibility => Visibility.NoOne;
        public override Team Team => Team.Crewmate;
        public override bool HasToDoTasks => true;
        public override int Limit => (int) Settings.CaptainAmount.Value;

        public CustomButton Button;

        public override void OnGameStart()
        {
            Button = CustomButton.AddRoleButton(() =>
                {
                    PlayerControl.LocalPlayer.CmdReportDeadBody(null);
                }, Settings.CaptainCooldown.Value,
                PeasAPI.Utility.CreateSprite("Peasmod.Resources.Buttons.CallMeeting.png", 650f), Vector2.zero, false, this, "<size=40%>Call");
        }
    }
}