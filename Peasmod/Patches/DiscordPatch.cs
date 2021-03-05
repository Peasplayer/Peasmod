using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Reactor;
using Discord;
using InnerNet;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Peasmod.Patches
{
    [HarmonyPatch(typeof(DiscordManager), nameof(DiscordManager.Start))]
    class DiscordManagerPatch
    {
        public static bool Prefix(DiscordManager __instance)
        {
            __instance.presence = new Discord.Discord(787398232540053544, 1UL);
            __instance.presence.GetImageManager();
            var activityManager = __instance.presence.GetActivityManager();
            activityManager.RegisterSteam(945360U);
            //Activity activity = null;
            //activity.State = "Being cool";
            //activityManager.UpdateActivity(activity, DiscordManager.c.field_Public_Static_UpdateActivityHandler_0);
            return false;
        }
    }

    [HarmonyPatch(typeof(DiscordManager), nameof(DiscordManager.FixedUpdate))]
    class DiscordManagerJoinRequestPatch
    {
        public static void Prefix(DiscordManager __instance)
        {
            
        }
    }
}
