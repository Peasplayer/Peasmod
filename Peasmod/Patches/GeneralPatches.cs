using System.Collections.Generic;
using HarmonyLib;
using PeasAPI.GameModes;
using Peasmod.GameModes;
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
                if(!Settings.ReportBodys.Value || GameModeManager.IsGameModeActive(BattleRoyale.Instance)) //TO-DO: Add potato mode exception
                {
                    foreach (var body in Object.FindObjectsOfType<DeadBody>())
                    {
                        body.Reported = true;
                        DestroyableSingleton<ReportButton>.Instance.SetActive(false);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        class ActivateVentButtonPatch
        {
            static void Postfix(PlayerControl __instance)
            {
                if (Settings.CrewVenting.Value && PeasAPI.PeasAPI.GameStarted && !MeetingHud.Instance)
                {
                    HudManager.Instance.ImpostorVentButton.gameObject.SetActive(true);
                    if (__instance.CanMove && !__instance.Data.IsDead)
                    {
                        HudManager.Instance.ImpostorVentButton.SetEnabled();
                    }
                    else
                    {
                        HudManager.Instance.ImpostorVentButton.SetDisabled();
                    }
                }
            }
        }
        
        [HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
        [HarmonyPriority(Priority.Last)]
        public static class VentCanUsePostfix
        {
            public static void Postfix(Vent __instance, [HarmonyArgument(1)] ref bool canUse,
                [HarmonyArgument(2)] ref bool couldUse, ref float __result)
            {
                if (Settings.CrewVenting.Value)
                {
                    couldUse = !PlayerControl.LocalPlayer.Data.IsDead;
                    canUse = couldUse;
                    var distance = Vector2.Distance(PlayerControl.LocalPlayer.GetTruePosition(), __instance.transform.position);
                    canUse &= distance <= __instance.UsableDistance;
                    __result = distance;
                }
            }
        }

        [HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
        public static class VentCanUsePatch
        {
            public static bool Prefix(Vent __instance, ref float __result, [HarmonyArgument(0)] GameData.PlayerInfo pc, [HarmonyArgument(1)] out bool canUse, [HarmonyArgument(2)] out bool couldUse)
            {
                canUse = couldUse = false;
                float num = float.MaxValue;
                PlayerControl @object = pc.Object;
                couldUse = ((pc.Role.CanVent && pc.Role.CanUse(__instance.Cast<IUsable>()) || Settings.CrewVenting.Value) && !@object.MustCleanVent(__instance.Id) && !pc.IsDead && (@object.CanMove || @object.inVent));
                if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Ventilation))
                {
                    VentilationSystem ventilationSystem = ShipStatus.Instance.Systems[SystemTypes.Ventilation].Cast<VentilationSystem>();
                    if (ventilationSystem != null && ventilationSystem.IsVentCurrentlyBeingCleaned(__instance.Id))
                    {
                        couldUse = false;
                    }
                }
                canUse = couldUse;
                if (canUse)
                {
                    Vector3 center = @object.Collider.bounds.center;
                    Vector3 position = __instance.transform.position;
                    num = Vector2.Distance(center, position);
                    canUse &= (num <= __instance.UsableDistance && !PhysicsHelpers.AnythingBetween(@object.Collider, center, position, Constants.ShipOnlyMask, false));
                }
                __result = num;
                float distance = float.MaxValue;
                if (!__instance.enabled || !Settings.Venting.Value)
                {
                    canUse = false;
                    couldUse = false;
                    return false;
                }
                PlayerControl localPlayer = pc.Object;
                if (localPlayer.Data.Role.IsImpostor)
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