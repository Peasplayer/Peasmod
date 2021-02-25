using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Hazel;
using Reactor.Extensions;
using UnityEngine;

namespace Peasmod.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
    class KillPatch
    {
        public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
        {
            if (__instance == SheriffMode.Sheriff1 || __instance == SheriffMode.Sheriff2)
                __instance.Data.IsImpostor = true;
        }

        public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
        {
            if (__instance == SheriffMode.Sheriff1 || __instance == SheriffMode.Sheriff2)
                __instance.Data.IsImpostor = false;
        }
    }

    [HarmonyPatch(typeof(KillAnimation), nameof(KillAnimation.CoPerformKill))]
    class KillAnimationPatch
    {
        public static void Prefix(KillAnimation __instance, [HarmonyArgument(0)] PlayerControl killer, [HarmonyArgument(1)] PlayerControl target)
        {
            #region InvisibilityMode
            if (InvisibilityMode.invisplayers.Contains(killer.PlayerId))
            {
                killer.myRend.color = Palette.DisabledColor;
                killer.HatRenderer.color = Palette.DisabledColor;
                killer.MyPhysics.Skin.layer.color = Palette.DisabledColor;
            }
            #endregion InvisibilityMode
        }
    }
}
