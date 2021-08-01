using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using HarmonyLib;
using InnerNet;
using Reactor;
using BepInEx.Logging;
using Essentials;
using Hazel.Udp;
using PeasAPI;
using PeasAPI.Components;
using Peasmod.ApiExtension.Gamemodes;
using Peasmod.Utility;
using UnityEngine;
using Random = System.Random;

namespace Peasmod
{
    [BepInPlugin(Id)]
    [BepInProcess("Among Us.exe")]
    [BepInDependency(ReactorPlugin.Id)]
    [BepInDependency(EssentialsPlugin.Id)]
    [BepInDependency(PeasApi.Id)]
    public class PeasmodPlugin : BasePlugin
    {
        public const string Id = "tk.peasplayer.peasmod";

        public const string PluginName = "Peasmod";
        public const string PluginAuthor = "Peasplayer#2541";
        public const string PluginVersion = "2.2.0";

        public Harmony Harmony { get; } = new Harmony(Id);
        
        public static ManualLogSource Logger { get; private set; }
        
        public static ConfigFile ConfigFile { get; private set; }

        public static readonly Random Random = new Random();

        public static bool GameStarted =>
            GameData.Instance && ShipStatus.Instance && AmongUsClient.Instance &&
            (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started ||
             AmongUsClient.Instance.GameMode == GameModes.FreePlay);

        public override void Load()
        {
            Logger = this.Log;
            ConfigFile = Config;

            Watermark.PingText = $" | {PluginName} v{PluginVersion}\n{StringColor.Green} by {PluginAuthor}";
            Watermark.PingTextOffset = new Vector3(-0.9f, 0f);
            Watermark.VersionText = $" | {PluginName} v{PluginVersion} {StringColor.Green} by {PluginAuthor}";
            Watermark.VersionTextOffset = new Vector3(0f, -0.3f);
            PeasApi.AccountTabOffset = new Vector3(0f, -0.3f);

            CustomServerManager.RegisterServer("Peaspowered", "au.peasplayer.tk", 22023);
            CustomServerManager.RegisterServer("matux.fr", "152.228.160.91", 22023);

            Settings.Load();

            RegisterCustomRoleAttribute.Register(this);
            RegisterCustomGameModeAttribute.Register(this);

            Harmony.Unpatch(typeof(UdpConnection).GetMethod("HandleSend"), HarmonyPatchType.Prefix, ReactorPlugin.Id);
            Harmony.PatchAll();
        }
    }
}