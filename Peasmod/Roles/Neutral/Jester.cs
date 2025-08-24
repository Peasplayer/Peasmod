using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomEndReason;
using PeasAPI.Roles;
using UnityEngine;

namespace Peasmod.Roles.Neutral
{
    [RegisterCustomRole]
    public class Jester : BaseRole
    {
        public Jester(BasePlugin plugin) : base(plugin) { }

        public override string Name => "Jester";
        public override string Description => "Trick the crew";
        public override string LongDescription => "";
        public override string TaskText => "Trick the crew into voting you out";
        public override Color Color => ModdedPalette.JesterColor;
        public override Visibility Visibility => Visibility.NoOne;
        public override Team Team => Team.Alone;
        public override bool HasToDoTasks => false;

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Exiled))]
        public static class PlayerControlExiledPatch
        {
            public static void Prefix(PlayerControl __instance)
            {
                if (__instance.IsCustomRole<Jester>() && __instance.IsLocal())
                    new CustomEndReason(__instance);
            }
        }
    }
}