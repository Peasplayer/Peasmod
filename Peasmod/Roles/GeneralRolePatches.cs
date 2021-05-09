using HarmonyLib;
using Peasmod.Utility;
using UnhollowerBaseLib;

namespace Peasmod.Roles
{
    public class GeneralRolePatches
    {
        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
        public static class ShipStatusStartPatch
        {
            public static void Prefix(PlayerControl __instance)
            {
                if (Settings.IsGameMode(Settings.GameMode.Normal))
                {
                    //Roles.ResetRoles();
                    JesterRole.OnGameStart();
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSetInfected))]
        public static class PlayerControlRpcSetInfectedPatch
        {
            public static void Prefix(PlayerControl __instance)
            {
                if (Settings.IsGameMode(Settings.GameMode.Normal))
                {
                    foreach (var player in PlayerControl.AllPlayerControls)
                    {
                        if (player.Data.IsImpostor)
                            Roles.impostors.Add(player.PlayerId);
                        else
                            Roles.crewmates.Add(player.PlayerId);
                    }

                    JesterRole.OnRpcSetInfected();
                }
            }
        }

        [HarmonyPatch(typeof(IntroCutscene.Nested_0), nameof(IntroCutscene.Nested_0.MoveNext))]
        public static class IntroCutscenePatch
        {
            public static void Prefix(IntroCutscene.Nested_0 __instance)
            {
                if (Settings.IsGameMode(Settings.GameMode.Normal))
                {
                    JesterRole.OnIntroCutScenePrefix(__instance);
                }
            }

            public static void Postfix(IntroCutscene.Nested_0 __instance)
            {
                if (Settings.IsGameMode(Settings.GameMode.Normal))
                {
                    JesterRole.OnIntroCutScenePostfix(__instance);
                }
            }
        }

        [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString),
            new[] {typeof(StringNames), typeof(Il2CppReferenceArray<Il2CppSystem.Object>)})]
        public class TranslationControllerPatch
        {
            public static bool Prefix(ref string __result, [HarmonyArgument(0)] StringNames name)
            {
                if (ExileController.Instance != null && ExileController.Instance.exiled != null)
                {
                    if (name == StringNames.ExileTextPN || name == StringNames.ExileTextSN)
                    {
                        JesterRole.OnExiled(ref __result);
                        if (__result == null)
                        {
                            if (Peasmod.impostors.Count == 1)
                            {
                                __result = ExileController.Instance.exiled.PlayerName + " was not The Impostor.";
                            }
                            else
                            {
                                __result = ExileController.Instance.exiled.PlayerName + " was not An Impostor.";
                            }
                        }
                        return false;
                    }
                }
                return true;
            }
        }
        
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class HudManagerUpdatePatch
        {
            public static void Prefix(IntroCutscene.Nested_0 __instance)
            {
                if (Settings.IsGameMode(Settings.GameMode.Normal))
                {
                    JesterRole.OnUpdate();
                }
            }
        }
    }
}