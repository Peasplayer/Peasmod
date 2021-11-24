using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace Peasmod.Patches
{
    public class GeneralPatches
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.ReportClosest))]
        class ReportBodyButtonPatch
        {
            static void Prefix(PlayerControl __instance)
            {   
                if(!Settings.ReportBodys.Value || Settings.IsGameMode(Settings.GameMode.BattleRoyale) || Settings.IsGameMode(Settings.GameMode.HotPotato))
                {
                    foreach (var body in Object.FindObjectsOfType<DeadBody>())
                    {
                        body.Reported = true;
                        DestroyableSingleton<ReportButton>.Instance.SetActive(false);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Vent), "CanUse")]
        public static class VentCanUsePatch
        {
            public static bool Prefix(Vent __instance, ref float __result, [HarmonyArgument(0)] GameData.PlayerInfo pc, [HarmonyArgument(1)] out bool canUse, [HarmonyArgument(2)] out bool couldUse)
            {   
                float distance = float.MaxValue;
                if (!__instance.enabled || !Settings.Venting.Value)
                {
                    canUse = false;
                    couldUse = false;
                    return false;
                }
                PlayerControl localPlayer = pc.Object;
                if(localPlayer.Data.Role.IsImpostor)
                {
                    couldUse = !localPlayer.Data.IsDead;
                    canUse = couldUse;
                    distance = Vector2.Distance(localPlayer.GetTruePosition(), __instance.transform.position);
                    canUse &= distance <= __instance.UsableDistance;
                    __result = distance;
                }
                else
                {
                    if (Settings.CrewVenting.Value)
                    {
                        couldUse = !localPlayer.Data.IsDead;
                        canUse = couldUse;
                        distance = Vector2.Distance(localPlayer.GetTruePosition(), __instance.transform.position);
                        canUse &= distance <= __instance.UsableDistance;
                        __result = distance;
                    }
                    else
                    {
                        couldUse = false;
                        canUse = false;
                    }
                }
                return false;
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