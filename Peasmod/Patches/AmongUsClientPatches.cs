using System;
using System.Collections.Generic;
using System.Text;
using InnerNet;
using HarmonyLib;
using Hazel;
using UnityEngine;
using Peasmod.Utility;
using UnhollowerRuntimeLib;

namespace Peasmod.Patches
{
    class AmongUsClientPatches
    {
        /*[HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.OnDisconnect))]
        class DisconnectPatch
        {
            public static void Prefix(InnerNetClient __instance)
            {
                Peasmod.GameStarted = false;
            }
        }*/

        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameJoined))]
        class JoinPatch
        {
            public static void Prefix(InnerNetClient __instance, [HarmonyArgument(0)] string gameIdString, [HarmonyArgument(1)] ClientData client)
            {
                
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Start))]
        class StartPatch
        {
            public static void Prefix(PlayerControl __instance)
            {
                if (PlayerControl.LocalPlayer == null || __instance.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                {
                    HudManager.Instance.GameMenu.gameObject.SetActive(true);
                    __instance.SetRole(Role.Crewmate);
                }
            }
        }
    }
}
