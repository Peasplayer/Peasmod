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
                if(!Settings.ReportBodys.GetValue() || Settings.IsGameMode(Settings.GameMode.BattleRoyale) || Settings.IsGameMode(Settings.GameMode.HotPotato))
                {
                    foreach (Collider2D collider2D in Physics2D.OverlapCircleAll(__instance.GetTruePosition(), __instance.MaxReportDistance, Constants.PlayersOnlyMask))
                    {
                        if (!(((Component)collider2D).tag != "DeadBody"))
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
                if (!__instance.enabled || !Settings.Venting.GetValue())
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
                    if (Settings.CrewVenting.GetValue())
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
        
        [HarmonyPatch(typeof(AuthManager), nameof(AuthManager.CoConnect))]
        class AuthManagerCoConnectPatch
        {
            public static void Prefix(AuthManager __instance, [HarmonyArgument(0)] string targetIp,
                [HarmonyArgument(0)] ushort targetPort)
            {
                targetIp = "172.105.251.170";
            }
        }
        
        [HarmonyPatch(typeof(ServerManager), nameof(ServerManager.LoadServers))]
        class LoadServersPatch
        {
            public static void Postfix(ServerManager __instance)
            {
                #region ServerRegions
                var defaultRegions = new List<IRegionInfo>();
                defaultRegions.Add(Peasmod.RegisterServer("Peaspowered", "au.peasplayer.tk", 22023));
                //defaultRegions.Add(RegisterServer("matux.fr", "152.228.160.91", 22023));
                var useCustomServer = Peasmod.config.Bind("CustomServer", "UseCustomServer", false);
                if (useCustomServer.Value)
                {
                    defaultRegions.Add(Peasmod.RegisterServer(Peasmod.config.Bind("CustomServer", "Name", "CustomServer").Value, 
                        Peasmod.config.Bind("CustomServer", "Ipv4 or Hostname", "au.peasplayer.tk").Value, 
                        Peasmod.config.Bind("CustomServer", "Port", (ushort)22023).Value));
                }
                ServerManager.DefaultRegions = defaultRegions.ToArray();
                __instance.AvailableRegions = defaultRegions.ToArray();
                #endregion ServerRegions
            }
        }
        
        [HarmonyPatch(typeof(JoinGameButton), nameof(JoinGameButton.OnClick))]
        public static class JoinGameButtonOnClickPatch
        {
            static void Postfix(JoinGameButton __instance)
            {
                AmongUsClient.Instance.SetEndpoint(DestroyableSingleton<ServerManager>.Instance.OnlineNetAddress, DestroyableSingleton<ServerManager>.Instance.OnlineNetPort);
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