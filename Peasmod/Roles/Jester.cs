using BepInEx.IL2CPP;
using HarmonyLib;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomEndReason;
using PeasAPI.Roles;
using UnityEngine;

namespace Peasmod.Roles
{
    [RegisterCustomRole]
    public class Jester : BaseRole
    {
        public Jester(BasePlugin plugin) : base(plugin) { }

        public override string Name => "Jester";

        public override string Description => "Trick the crew";

        public override string TaskText => "Trick the crew into voting you out";

        public override Color Color => ModdedPalette.JesterColor;

        public override int Limit => (int) Settings.JesterAmount.GetValue();

        public override Team Team => Team.Alone;

        public override Visibility Visibility => Visibility.NoOne;

        public override bool HasToDoTasks => false;

        public override void OnGameStart()
        {
            
        }

        public override void OnUpdate()
        {
            
        }

        public override void OnMeetingUpdate(MeetingHud meeting)
        {
            
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Exiled))]
        class PlayerControlExiledPatch
        {
            public static void Prefix(PlayerControl __instance)
            {
                if (__instance.IsRole<Jester>())
                    new CustomEndReason(__instance);
            }
        }
    }
}