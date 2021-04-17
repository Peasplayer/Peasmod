using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Hazel;
using Reactor.Extensions;
using UnityEngine;
using UnhollowerBaseLib;
using Peasmod.Utility;
using Peasmod.Gamemodes;

namespace Peasmod.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
    class KillPatch
    {
        public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
        {
            if (__instance.IsRole(Role.Sheriff))
                __instance.Data.IsImpostor = true;
            if (Peasmod.Settings.IsGameMode(Peasmod.Settings.GameMode.HotPotato))
                return false;
            return true;
        }

        public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
        {
            if (__instance.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                BattleRoyaleMode.HasKilled = true;
            if (__instance.IsRole(Role.Sheriff))
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
                killer.myRend.color = Palette.DisabledClear;
                killer.HatRenderer.color = Palette.DisabledClear;
                killer.MyPhysics.Skin.layer.color = Palette.DisabledClear;
            }
            #endregion InvisibilityMode
        }
    }
}
