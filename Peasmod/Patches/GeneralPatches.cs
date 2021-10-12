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
                    foreach (Collider2D collider2D in Physics2D.OverlapCircleAll(__instance.GetTruePosition(), __instance.MaxReportDistance, Constants.PlayersOnlyMask))
                    {
                        if (((Component)collider2D).CompareTag("DeadBody"))
                        {
                            DeadBody component = (DeadBody)((Component)collider2D).GetComponent<DeadBody>();
                            component.Reported = true;
                            DestroyableSingleton<ReportButtonManager>.Instance.renderer.color = Palette.DisabledClear;
                            DestroyableSingleton<ReportButtonManager>.Instance.renderer.material.SetFloat("_Desat", 1f);
                        }
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
                if(localPlayer.Data.IsImpostor)
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
    }
}