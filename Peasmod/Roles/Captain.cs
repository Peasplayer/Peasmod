using BepInEx.IL2CPP;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.Roles;
using Peasmod.Utility;
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

        public override Color Color => Palette.LightBlue;

        public override int Limit => (int) Settings.CaptainAmount.GetValue();

        public override Team Team => Team.Crewmate;

        public override Visibility Visibility => Visibility.NoOne;

        public override bool HasToDoTasks => true;

        public RoleButton Button;

        public override void OnGameStart()
        {
            Button = new RoleButton(() =>
                {
                    PlayerControl.LocalPlayer.CmdReportDeadBody(null);
                }, PlayerControl.GameOptions.EmergencyCooldown,
                Utils.CreateSprite("Buttons.Button1.png"), Vector2.zero, false, this);
        }

        public override void OnUpdate()
        {
            
        }

        public override void OnMeetingUpdate(MeetingHud meeting)
        {
            
        }
    }
}