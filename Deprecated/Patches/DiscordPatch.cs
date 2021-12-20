using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Discord;
using InnerNet;

namespace Peasmod.Patches
{
    [HarmonyPatch(typeof(DiscordManager), nameof(DiscordManager.Start))]
    class DiscordManagerPatch
    {
        public static bool Prefix(DiscordManager __instance)
        {
            try
			{
				__instance.Field_1 = new Discord.Discord(787398232540053544, 1UL);
				ActivityManager activityManager = __instance.Field_1.GetActivityManager();
				activityManager.RegisterSteam(945360U);
				__instance.SetInMenus();
			}
			catch
			{
			}
			return false;
        }
    }
    
    [HarmonyPatch(typeof(DiscordManager), nameof(DiscordManager.HandleJoinRequest))]
    class HandleJoinPatch
    {
        public static bool Prefix(DiscordManager __instance, [HarmonyArgument((0))] string joinSecret)
        {
	        if (!joinSecret.StartsWith("join"))
	        {
		        return false;
	        }
	        if (!AmongUsClient.Instance)
	        {
		        return false;
	        }
	        if (!DestroyableSingleton<DiscordManager>.InstanceExists)
	        {
		        return false;
	        }
	        if (AmongUsClient.Instance.mode != MatchMakerModes.None)
	        {
		        return false;
	        }
	        AmongUsClient.Instance.GameMode = (GameModes)1;
	        AmongUsClient.Instance.GameId = GameCode.GameNameToInt(joinSecret.Substring(4));
	        AmongUsClient.Instance.SetEndpoint(ServerManager.Instance.OnlineNetAddress, ServerManager.Instance.OnlineNetPort);
	        AmongUsClient.Instance.MainMenuScene = "MMOnline";
	        AmongUsClient.Instance.OnlineScene = "OnlineGame";
	        DestroyableSingleton<DiscordManager>.Instance.StopAllCoroutines();
	        AmongUsClient.Instance.Connect((MatchMakerModes)1);
	        __instance.StartCoroutine(__instance.CoJoinGame());
			return false;
        }
    }
}
