using System.Collections.Generic;
using HarmonyLib;
using PeasAPI.GameModes;
using Peasmod.GameModes;
using UnityEngine;

namespace Peasmod.Patches
{
    public class GeneralPatches
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        class ActivateVentButtonPatch
        {
            static void Postfix(PlayerControl __instance)
            {
                var player = PlayerControl.LocalPlayer;
                
                if (Settings.CrewVenting.Value && !player.Data.Role.IsImpostor && PeasAPI.PeasAPI.GameStarted && !MeetingHud.Instance)
                {
                    HudManager.Instance.ImpostorVentButton.gameObject.SetActive(true);
                    if (player.CanMove && !player.Data.IsDead)
                        HudManager.Instance.ImpostorVentButton.SetEnabled();
                    else
                        HudManager.Instance.ImpostorVentButton.SetDisabled();
                }

                if (!Settings.Venting.Value && player.Data.Role.IsImpostor && PeasAPI.PeasAPI.GameStarted &&
                    !MeetingHud.Instance)
                    HudManager.Instance.ImpostorVentButton.gameObject.SetActive(false);
                
                if (!Settings.Sabotaging.Value && PeasAPI.PeasAPI.GameStarted && !MeetingHud.Instance)
                    HudManager.Instance.SabotageButton.gameObject.SetActive(false);
                
                if (!Settings.ReportBodys.Value && PeasAPI.PeasAPI.GameStarted && !MeetingHud.Instance)
                    HudManager.Instance.ReportButton.gameObject.SetActive(false);
            }
        }
        
        [HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
        [HarmonyPriority(Priority.Last)]
        public static class VentCanUsePostfix
        {
            public static void Postfix(Vent __instance, [HarmonyArgument(1)] ref bool canUse,
                [HarmonyArgument(2)] ref bool couldUse, ref float __result)
            {
                if (Settings.CrewVenting.Value && !PlayerControl.LocalPlayer.Data.Role.IsImpostor)
                {
                    couldUse = !PlayerControl.LocalPlayer.Data.IsDead;
                    canUse = couldUse;
                    var distance = Vector2.Distance(PlayerControl.LocalPlayer.GetTruePosition(), __instance.transform.position);
                    canUse &= distance <= __instance.UsableDistance;
                    __result = distance;
                }
                
                if (!Settings.Venting.Value && PlayerControl.LocalPlayer.Data.Role.IsImpostor)
                {
                    var distance = Vector2.Distance(PlayerControl.LocalPlayer.GetTruePosition(), __instance.transform.position);
                    canUse = couldUse = false;
                    __result = distance;
                }
            }
        }
        
        [HarmonyPatch(typeof(StatsManager), nameof(StatsManager.AmBanned), MethodType.Getter)]
        public static class AmBannedPatch
        {
            public static void Postfix(out bool __result)
            {
                __result = false;
            }
        }
    }
}