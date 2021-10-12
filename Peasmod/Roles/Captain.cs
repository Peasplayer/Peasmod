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

        public override int Limit => (int) Settings.CaptainAmount.Value;

        public override Team Team => Team.Crewmate;

        public override Visibility Visibility => Visibility.NoOne;

        public override bool HasToDoTasks => true;

        public CustomButton Button;

        public override void OnGameStart()
        {
            Button = CustomButton.AddRoleButton(() =>
                {
                    PlayerControl.LocalPlayer.CmdReportDeadBody(null);
                }, PlayerControl.GameOptions.EmergencyCooldown,
                PeasAPI.Utility.CreateSprite("Peasmod.Resources.Buttons.CallMeeting.png", 650f), Vector2.zero, false, this, "<size=40%>Call", new Vector2(0f, -0.6f));
        }

        public override void OnUpdate()
        {
            
        }

        public override void OnMeetingUpdate(MeetingHud meeting)
        {
            
        }
    }
}