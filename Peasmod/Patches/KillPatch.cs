using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Hazel;
using Reactor.Extensions;
using UnityEngine;
using UnhollowerBaseLib;
using Peasmod.Utility;
using Peasmod.GameModes;

namespace Peasmod.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
    class KillPatch
    {
        public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
        {
            if(Peasmod.Settings.hotpotato.GetValue())
            {
                target.Data.IsImpostor = true;
                __instance.Data.IsImpostor = false;
                __instance.nameText.Color = Palette.White;
                if (__instance.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                {
                    HotPotatoMode.timer.GetComponent<TextRenderer>().Text = string.Empty;
                    HudManager.Instance.KillButton.gameObject.SetActive(false);
                }
                if (target.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                {
                    HotPotatoMode.timer.GetComponent<TextRenderer>().Text = "Timer";
                    HotPotatoMode.Timer = Peasmod.Settings.hotpotatotimer.GetValue();
                    HudManager.Instance.KillButton.gameObject.SetActive(true);
                }
            }
            else
            {
                if (__instance.IsRole(Role.Sheriff))
                    __instance.Data.IsImpostor = true;
            }
        }

        public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
        {
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
                killer.myRend.color = Palette.DisabledColor;
                killer.HatRenderer.color = Palette.DisabledColor;
                killer.MyPhysics.Skin.layer.color = Palette.DisabledColor;
            }
            #endregion InvisibilityMode
        }
    }
}
