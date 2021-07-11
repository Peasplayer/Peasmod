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
    public class Troll : BaseRole
    {
        public Troll(BasePlugin plugin) : base(plugin) { }

        public override string Name => "Troll";

        public override string Description => "Get killed by an impostor";

        public override string TaskText => "Get killed by an impostor";

        public override Color Color => Palette.AcceptedGreen;

        public override int Limit => (int) Settings.TrollAmount.GetValue();

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

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
        class PlayerControlExiledPatch
        {
            public static void Postifx(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl victim)
            {
                if (victim.IsRole<Troll>())
                    new CustomEndReason(victim);
            }
        }
    }
}