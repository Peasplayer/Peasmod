using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using HarmonyLib;
using Hazel;
using InnerNet;
using Reactor;
using Reactor.Extensions;
using System.Collections.Generic;
using System;
using System.IO;
using System.Net;
using Essentials.Options;
using UnityEngine;
using UnhollowerBaseLib;
using BepInEx.Logging;
using System.Linq;
using System.Reflection;
using Hazel.Udp;
using Peasmod.Roles;
using Peasmod.Utility;
//using Peasmod.Utility;
using UnhollowerRuntimeLib;

namespace Peasmod
{
    [BepInPlugin(Id)]
    [BepInProcess("Among Us.exe")]
    [BepInDependency(ReactorPlugin.Id)]
    public class Peasmod : BasePlugin
    {
        public const string Id = "tk.peasplayer.peasmod";
        public const string PluginName = "Peasmod";
        public const string PluginAuthor = "Peasplayerᵈᵉᵛ#2541";
        public const string PluginVersion = "2.1.0";
        public const string PluginPage = "peascord.tk";

        public static Peasmod Instance { get { return PluginSingleton<Peasmod>.Instance; } }
        
        public Harmony Harmony { get; } = new Harmony(Id);
        public static ManualLogSource Logger { get; private set; }
        public static ConfigFile config { get; private set; }
        public static System.Random random = new System.Random();

        public static List<PlayerControl> crewmates = new List<PlayerControl>();
        public static List<PlayerControl> impostors = new List<PlayerControl>();
        
        //public static List<CooldownButton> impostorbuttons = new List<CooldownButton>();

        public static bool GameStarted { get { 
                return GameData.Instance && ShipStatus.Instance && AmongUsClient.Instance && (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started || AmongUsClient.Instance.GameMode == GameModes.FreePlay);
            } }
        
        public static IRegionInfo RegisterServer(string name, string ip, ushort port)
        {
            if (Uri.CheckHostName(ip).ToString() == "Dns")
            {
                try
                {
                    foreach (IPAddress address in Dns.GetHostAddresses(ip))
                        if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            ip = address.ToString();
                            break;
                        }
                }
                catch {}
            }
            return new DnsRegionInfo(ip, name, StringNames.NoTranslation, ip, port)
                .Cast<IRegionInfo>();
        }
        
        public override void Load()
        {
            Logger = this.Log;
            config = Config;
            
            Settings.Load();
            
            Harmony.Unpatch(typeof(UdpConnection).GetMethod("HandleSend"), HarmonyPatchType.Prefix, ReactorPlugin.Id);
            
            Harmony.PatchAll();
        }
    }
}
