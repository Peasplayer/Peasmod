using System;
using Discord;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Peasmod.Patches
{
    [HarmonyPatch]
    public class DiscordPatches
    {
        [HarmonyPatch(typeof(DiscordManager), nameof(DiscordManager.Start))]
        [HarmonyPrefix]
        public static bool CustomDiscordRPCPrefix(DiscordManager __instance)
        {
            if (DestroyableSingleton<DiscordManager>.Instance != __instance)
            {
                return false;
            }
            try
            {
                __instance.presence = new Discord.Discord(787398232540053544L, 1UL); //477175586805252107L
                ActivityManager activityManager = __instance.presence.GetActivityManager();
                activityManager.RegisterSteam(945360U);
                activityManager.OnActivityJoin = (Action<string>)delegate (string joinSecret)
                {
                    if (!joinSecret.StartsWith("join"))
                    {
                        Debug.LogWarning("DiscordManager: Invalid join secret: " + joinSecret);
                        return;
                    }
                    __instance.StopAllCoroutines();
                    __instance.StartCoroutine(__instance.CoJoinGame(joinSecret));
                };
                __instance.SetInMenus();
                SceneManager.sceneLoaded = (Action<Scene, LoadSceneMode>)delegate (Scene scene, LoadSceneMode mode)
                {
                    __instance.OnSceneChange(scene.name);
                };
            }
            catch
            {
                Debug.LogWarning("DiscordManager: Discord messed up");
            }

            return false;
        }

        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.FixedUpdate))]
        [HarmonyPostfix]
        public static void FixAccountTabPatch(ShipStatus __instance)
        {
            DestroyableSingleton<AccountTab>.Instance.gameObject.SetActive(false);
        }
        
        /*[HarmonyPatch(typeof(DiscordManager), nameof(DiscordManager.Start))]
        [HarmonyPostfix]
        public static void CustomDiscordRPCPatch(DiscordManager __instance)
        {
            if (DestroyableSingleton<DiscordManager>.Instance != __instance)
            {
                return;
            }
            try
            {
                __instance.presence = new Discord.Discord(787398232540053544L, 1UL); //477175586805252107L
                ActivityManager activityManager = __instance.presence.GetActivityManager();
                activityManager.RegisterSteam(945360U);
                activityManager.OnActivityJoin = (Action<string>)delegate (string joinSecret)
                {
                    if (!joinSecret.StartsWith("join"))
                    {
                        Debug.LogWarning("DiscordManager: Invalid join secret: " + joinSecret);
                        return;
                    }
                    __instance.StopAllCoroutines();
                    __instance.StartCoroutine(__instance.CoJoinGame(joinSecret));
                };
                __instance.SetInMenus();
                SceneManager.sceneLoaded = (Action<Scene, LoadSceneMode>)delegate (Scene scene, LoadSceneMode mode)
                {
                    __instance.OnSceneChange(scene.name);
                };
            }
            catch
            {
                Debug.LogWarning("DiscordManager: Discord messed up");
            }
        }*/
    }
}