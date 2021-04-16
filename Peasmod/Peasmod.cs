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
using System.Net;
using Essentials.Options;
using UnityEngine;
using UnhollowerBaseLib;
using BepInEx.Logging;
using System.Linq;
using Peasmod.Utility;
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
        public const string PluginAuthor = "Peasplayer";
        public const string PluginVersion = "2.1.0-beta3";
        public const string PluginPage = "peascord.tk";

        public Harmony Harmony { get; } = new Harmony(Id);
        public static ManualLogSource Logger { get; private set; }
        public static System.Random random = new System.Random();

        public static List<PlayerControl> crewmates = new List<PlayerControl>();
        public static List<PlayerControl> impostors = new List<PlayerControl>();
        
        public static List<CooldownButton> impostorbuttons = new List<CooldownButton>();

        public static bool GameStarted { get { 
                return GameData.Instance && ShipStatus.Instance && AmongUsClient.Instance && (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started || AmongUsClient.Instance.GameMode == GameModes.FreePlay);
            } }

        public static class Settings
        {
            public static CustomStringOption section1 = CustomOption.AddString("section", "Section", new string[] { "Peasmod" });
            public static CustomToggleOption venting = CustomOption.AddToggle("venting", "Venting", true, true);
            public static CustomToggleOption reportbodys = CustomOption.AddToggle("reportbodys", "Body-Reporting", true, true);
            public static CustomToggleOption sabotaging = CustomOption.AddToggle("sabotaging", "Sabotaging", true, true);
            public static CustomToggleOption crewventing = CustomOption.AddToggle("crewventing", "Crew-Venting", true, false);
            public static CustomToggleOption ventbuilding = CustomOption.AddToggle("ventbuilding", "Vent-Building", true, false);
            public static CustomNumberOption ventbuildingcooldown = CustomOption.AddNumber("ventbuildingcooldown", "Vent-Building-Cooldown", true, 7, 2, 30, 1);
            public static CustomToggleOption bodydragging = CustomOption.AddToggle("bodydragging", "Body-Dragging", true, false);
            public static CustomToggleOption invisibility = CustomOption.AddToggle("invisibility", "Invisibility", true, false);
            public static CustomNumberOption invisibilitycooldown = CustomOption.AddNumber("invisibilitycooldown", "Invisibility-Cooldown", true, 20, 2, 60, 2);
            public static CustomNumberOption invisibilityduration = CustomOption.AddNumber("invisibilityduration", "Invisibility-Duration", true, 10, 2, 30, 1);
            public static CustomToggleOption freezetime = CustomOption.AddToggle("freezetime", "Time-Freezing", true, false);
            public static CustomNumberOption freezetimecooldown = CustomOption.AddNumber("freezetimecooldown", "Time-Freezing-Cooldown", true, 20, 2, 60, 2);
            public static CustomNumberOption freezetimeduration = CustomOption.AddNumber("freezetimeduration", "Time-Freezing-Duration", true, 10, 2, 30, 1);
            public static CustomToggleOption morphing = CustomOption.AddToggle("morphing", "Morphing", true, false);
            public static CustomNumberOption morphingcooldown = CustomOption.AddNumber("morphingcooldown", "Morphing-Cooldown", true, 20, 2, 60, 2);
            public static CustomStringOption section2 = CustomOption.AddString("section", "Section", new string[] { "Roles" });
            //public static CustomToggleOption thanos = CustomOption.AddToggle("thanos", "Thanos", true, false);
            public static CustomNumberOption jesteramount = CustomOption.AddNumber("jesters", "Jesters", true, 0, 0, 9, 1);
            public static CustomNumberOption doctoramount = CustomOption.AddNumber("doctors", "Doctors", true, 0, 0, 9, 1);
            public static CustomNumberOption doctorcooldown = CustomOption.AddNumber("doctorcooldown", "Revive-Cooldown", true, 10, 2, 60, 2);
            public static CustomNumberOption mayoramount = CustomOption.AddNumber("mayors", "Mayors", true, 0, 0, 9, 1);
            public static CustomNumberOption inspectoramount = CustomOption.AddNumber("inspectors", "Inspectors", true, 0, 0, 9, 1);
            public static CustomNumberOption sheriffamount = CustomOption.AddNumber("sheriffs", "Sheriffs", true, 0, 0, 9, 1);
            public static CustomNumberOption sheriffcooldown = CustomOption.AddNumber("sheriffcooldown", "Shoot-Cooldown", true, 10, 2, 60, 2);
            //public static CustomNumberOption engineeramount = CustomOption.AddNumber("engineers", "Engineers", true, 0, 0, 2, 1);
            public static CustomStringOption gamemode = CustomOption.AddString("gamemode", "Gamemode", new string[] { "None", "HotPotato", "Battle Royale" });
            public static CustomNumberOption hotpotatotimer = CustomOption.AddNumber("hotpotatotimer", "HotPotato-Timer", true, 10, 2, 60, 2);

            public enum GameMode
            {
                None = 0,
                HotPotato = 1,
                BattleRoyale =2
            }

            [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Update))]
            class GameOptionsMenuUpdate
            {
                static void Postfix(ref GameOptionsMenu __instance)
                {
                    __instance.GetComponentInParent<Scroller>().YBounds.max = 21.5f;
                }
            }

            [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.OnEnable))]
            public static class GameSettingMenuPatch
            {
                static void Prefix(GameSettingMenu __instance)
                {
                    __instance.HideForOnline = new Il2CppReferenceArray<Transform>(0);
                }
            }
        }

        public override void Load()
        {
            var ServerName = Config.Bind("Server", "Name", "Peaspowered");
            var ServerIp = Config.Bind("Server", "Ipv4 or Hostname", "au.peasplayer.tk");
            var ServerPort = Config.Bind("Server", "Port", (ushort)25911);
            var ip = ServerIp.Value;
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
            var port = ServerPort.Value;
            var defaultRegions = new List<IRegionInfo>();
            defaultRegions.Insert(0, new DnsRegionInfo(ServerName.Value, ServerName.Value, StringNames.NoTranslation, new[] {
                new ServerInfo($"{ServerName.Value}-Master-1", ip, port)
            }).Cast<IRegionInfo>());
            var UseLocalHost = Config.Bind("Server", "UseLocalHost", false);
            ServerPort = Config.Bind("Server", "LocalPort", (ushort)22023);
            if (UseLocalHost.Value)
            {
                ip = "127.0.0.1";
                port = ServerPort.Value;
                defaultRegions.Insert(1, new DnsRegionInfo("Localhost", "Localhost", StringNames.NoTranslation, new[] {
                    new ServerInfo($"Localhost-Master-1", ip, port)
                }).Cast<IRegionInfo>());
            }
            ServerManager.DefaultRegions = defaultRegions.ToArray();
            Logger = this.Log;
            Essentials.Options.CustomOption.ShamelessPlug = false;
            /*
             * I disabled the credits in game. I provide credit here and on my repository. If there is any problem with the author of this library feel free to contact me via email: peasplayer@peasplayer.t
             * Essentials: https://github.com/DorCoMaNdO/Reactor-Essentials
             * Author: DorComando (https://github.com/DorCoMaNdO)
             */
            Harmony.PatchAll();
        }

        //[HarmonyPatch(typeof(), nameof())]
        /*
         * [HarmonyPatch(typeof(), nameof())]
         * class SomePatch {
         *      public static void Prefix() {
         *      
         *      }
         * }
         */

        /* TODO-Liste:
         * 
         * Security-Guard: Kann jemanden beschützen, Admin/Security öffnen
         * Seher: Kann einen Crewmate revealen
         * Clone: Kann sich klonen
         * Sabotage: Crewmates kriegen mehr tasks
         * Engineer: Kann Sabotage fixen
         * Demon: Kann sich in einen Geist verwandeln
         * Helper: Kann anderen bei Tasks helfen
         * Zombie: Kann Leute infizieren, muss alle infizieren
         * Captain: Kann jederzeit Meeting callen
         * Traitor: Wenn er alle seine Tasks macht wird er Impostor
         * Summoner: Kann einen rausgevoteten Spieler reviven.
         * Assassin: Impostor geben ihm einmal einen Kill-Auftrag
         * Snitch: Wenn er alle Tasks gemacht hat sieht er die Impostor, ab 2 übrigen Tasks sehen die Impostor ihn
         * President: Kann jemandem zum Major machen
         * 
         * Gamemodes:
         * Freeze-Tag
         * Zombie
         * Räuber und Gendarm
         * 
         */


        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetInfected))]
        class SetInfectedPatch
        {
            public static void Postfix()
            {
                if (PlayerControl.AllPlayerControls.Count == 1) return;
                
                Peasmod.crewmates.Clear();
                Peasmod.impostors.Clear();
                foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                    if (player.Data.IsImpostor)
                        Peasmod.impostors.Add(player);
                    else
                        Peasmod.crewmates.Add(player);
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.ReportClosest))]
        class BodyReportButtonPatch
        {
            static void Prefix(PlayerControl __instance)
            {   
                if(!Settings.reportbodys.GetValue() || Peasmod.Settings.gamemode.GetValue() == (int)Peasmod.Settings.GameMode.BattleRoyale || Peasmod.Settings.gamemode.GetValue() == (int)Peasmod.Settings.GameMode.HotPotato)
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
        public static class VentPatch
        {
            public static bool Prefix(Vent __instance, ref float __result, [HarmonyArgument(0)] GameData.PlayerInfo pc, [HarmonyArgument(1)] out bool canUse, [HarmonyArgument(2)] out bool couldUse)
            {   
                float distance = float.MaxValue;
                if (!__instance.enabled || !Settings.venting.GetValue())
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
                    if (Settings.crewventing.GetValue())
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

        [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
        public static class PingPatch
        {
            public static void Postfix(PingTracker __instance)
            {
                __instance.text.text += "\n"+PluginName+" v"+PluginVersion+ "\n by " + StringColor.Green + PluginAuthor;
            }
        }

        [HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
        public static class VersionStartPatch
        {
            static void Postfix(VersionShower __instance)
            {
                __instance.text.text += "\nReactor-Framework" + "\n" + PluginName + " v" + PluginVersion + " by " + StringColor.Green + PluginAuthor + " " + StringColor.Reset + PluginPage;
                foreach (var _object in GameObject.FindObjectsOfTypeAll(Il2CppType.Of<GameObject>()))
                    if (_object.name.Contains("ReactorVersion"))
                        GameObject.Destroy(_object);
                //if(UnityEngine.Object.FindObjectOfType<MainMenuManager>() != null && UnityEngine.Object.FindObjectOfType<MainMenuManager>().Announcement != null)
                //UnityEngine.Object.FindObjectOfType<MainMenuManager>().Announcement.gameObject.SetActive(true);
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

        /*[HarmonyPatch(typeof(AnnouncementPopUp), nameof(AnnouncementPopUp.Show))]
        public static class AnnouncementPatch
        {

            public static string Announcement = "uh";

            private static bool IsSuccess(AnnouncementPopUp.AnnounceState state)
            {
                return state == AnnouncementPopUp.AnnounceState.Success || state == AnnouncementPopUp.AnnounceState.Cached;
            }

            public static bool Prefix(AnnouncementPopUp __instance)
            {
                float timer = 0f;
                while (__instance.AskedForUpdate == AnnouncementPopUp.AnnounceState.Fetching && timer < 6f)
                {
                    timer += Time.deltaTime;
                }
                if (!IsSuccess(__instance.AskedForUpdate))
                {
                    Announcement lastAnnouncement = SaveManager.LastAnnouncement;
                    if (lastAnnouncement.Id == 0U)
                    {
                        __instance.AnnounceText.Text = Announcement;
                    }
                    else
                    {
                        __instance.AnnounceText.Text = Announcement;
                    }
                }
                else if (__instance.announcementUpdate.Id != SaveManager.LastAnnouncement.Id)
                {
                    if (__instance.AskedForUpdate != AnnouncementPopUp.AnnounceState.Cached)
                    {
                        __instance.gameObject.SetActive(true);
                    }
                    if (__instance.announcementUpdate.Id == 0U)
                    {
                        __instance.announcementUpdate = SaveManager.LastAnnouncement;
                        __instance.announcementUpdate.DateFetched = Il2CppSystem.DateTime.UtcNow;
                    }
                    SaveManager.LastAnnouncement = __instance.announcementUpdate;
                    SaveManager.LastAnnouncement.AnnounceText = Announcement;
                    __instance.AnnounceText.Text = Announcement;
                }
                return false;
            }
        }*/

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
