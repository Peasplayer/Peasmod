using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using HarmonyLib;
using Hazel;
using Reactor;
using Reactor.Extensions;
using System.Collections.Generic;
using System;
using System.Net;
using Essentials.CustomOptions;
using UnityEngine;
using UnhollowerBaseLib;
using BepInEx.Logging;
using System.Linq;
using Peasmod.Utility;

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
        public const string PluginVersion = "1.7.0";

        public Harmony Harmony { get; } = new Harmony(Id);
        public static System.Random random = new System.Random();

        public static List<PlayerControl> crewmates = new List<PlayerControl>();
        public static List<PlayerControl> impostors = new List<PlayerControl>();
        
        public static List<CooldownButton> impostorbuttons = new List<CooldownButton>();

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
            public static CustomNumberOption jesteramount = CustomOption.AddNumber("jesters", "Jesters", true, 0, 0, 9, 1);
            public static CustomNumberOption doctoramount = CustomOption.AddNumber("doctors", "Doctors", true, 0, 0, 9, 1);
            public static CustomNumberOption doctorcooldown = CustomOption.AddNumber("doctorcooldown", "Revive-Cooldown", true, 10, 2, 60, 2);
            public static CustomNumberOption mayoramount = CustomOption.AddNumber("mayors", "Mayors", true, 0, 0, 9, 1);
            public static CustomNumberOption inspectoramount = CustomOption.AddNumber("inspectors", "Inspectors", true, 0, 0, 9, 1);
            public static CustomNumberOption sheriffamount = CustomOption.AddNumber("sheriffs", "Sheriffs", true, 0, 0, 9, 1);
            public static CustomNumberOption sheriffcooldown = CustomOption.AddNumber("sheriffcooldown", "Shoot-Cooldown", true, 10, 2, 60, 2);
            //public static CustomNumberOption engineeramount = CustomOption.AddNumber("engineers", "Engineers", true, 0, 0, 2, 1);

            [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Update))]
            class GameOptionsMenuUpdate
            {
                static void Postfix(ref GameOptionsMenu __instance)
                {
                    __instance.GetComponentInParent<Scroller>().YBounds.max = 18.5f;
                }
            }
        }

        public override void Load()
        {
            var ServerName = Config.Bind("Server", "Name", "Peaspowered");
            var ServerIp = Config.Bind("Server", "Ipv4 or Hostname", "au.peasplayer.tk");
            var ServerPort = Config.Bind("Server", "Port", (ushort)25995);
            var defaultRegions = ServerManager.DefaultRegions.ToList();
            var ip = ServerIp.Value;
            if (Uri.CheckHostName(ServerIp.Value).ToString() == "Dns")
            {
                try
                {
                    foreach (IPAddress address in Dns.GetHostAddresses(ServerIp.Value))
                        if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            ip = address.ToString();
                            break;
                        }
                }
                catch {}
            }
            var port = ServerPort.Value;
            defaultRegions.Insert(0, new RegionInfo(
                ServerName.Value, ip, new[] {
                    new ServerInfo($"{ServerName.Value}-Master-1", ip, port)
                })
            );
            ServerManager.DefaultRegions = defaultRegions.ToArray();
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

        [HarmonyPatch(typeof(UseButtonManager), nameof(UseButtonManager.SetTarget))]
        class SabotagePatch
        {
            static void Postfix(UseButtonManager __instance)
            {
                if (!Settings.sabotaging.GetValue())
                {
                    if(__instance.currentTarget == null)
                    {
                        __instance.UseButton.sprite = __instance.UseImage;
                        __instance.UseButton.color = UseButtonManager.DisabledColor;
                        __instance.enabled = false;
                    } 
                    else
                    {
                        __instance.UseButton.color = UseButtonManager.EnabledColor;
                        __instance.enabled = true;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.ReportClosest))]
        class BodyReportButtonPatch
        {
            static void Prefix(PlayerControl __instance)
            {   
                if(!Settings.reportbodys.GetValue())
                {
                    foreach (Collider2D collider2D in Physics2D.OverlapCircleAll(__instance.GetTruePosition(), __instance.MaxReportDistance, Constants.PlayersOnlyMask))
                    {
                        if (!(((Component)collider2D).tag != "DeadBody"))
                        {
                            DeadBody component = (DeadBody)((Component)collider2D).GetComponent<DeadBody>();
                            component.Reported = true;
                            DestroyableSingleton<ReportButtonManager>.Instance.renderer.color = Palette.DisabledColor;
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
                __instance.text.Text += "\n"+PluginName+" v"+PluginVersion+ "\n by " + StringColor.Green + PluginAuthor;
            }
        }

        [HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
        public static class VersionStartPatch
        {
            static void Postfix(VersionShower __instance)
            {
                __instance.text.Text += "\nReactor-Framework"+"\n" + PluginName + " v" + PluginVersion + " by " + StringColor.Green + PluginAuthor;
                UnityEngine.GameObject.Destroy(UnityEngine.GameObject.Find("ReactorVersion"));
                //if(UnityEngine.Object.FindObjectOfType<MainMenuManager>() != null && UnityEngine.Object.FindObjectOfType<MainMenuManager>().Announcement != null)
                    //UnityEngine.Object.FindObjectOfType<MainMenuManager>().Announcement.gameObject.SetActive(true);
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
