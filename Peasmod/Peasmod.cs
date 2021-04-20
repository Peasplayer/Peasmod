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
        public const string PluginAuthor = "Peasplayerᵈᵉᵛ#2541";
        public const string PluginVersion = "2.1.0-beta6";
        public const string PluginPage = "peascord.tk";

        public static Peasmod Instance { get { return PluginSingleton<Peasmod>.Instance; } }
        
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
            public static CustomOptionHeader Header = CustomOption.AddHeader(StringColor.Green + "Peasmod" + StringColor.Reset, true, true, false);
            private static readonly CustomOption SectionGeneral = CustomOption.AddButton("˅ General", true, false, false);
            public static readonly CustomToggleOption Venting = CustomOption.AddToggle("venting", "• Venting", true, true);
            public static readonly CustomToggleOption ReportBodys = CustomOption.AddToggle("reportbodys", "• Body-Reporting", true, true);
            public static readonly CustomToggleOption Sabotaging = CustomOption.AddToggle("sabotaging", "• Sabotaging", true, true);
            private static readonly CustomOption SectionSpecial = CustomOption.AddButton("˅ Special", true, false, false);
            public static readonly CustomToggleOption CrewVenting = CustomOption.AddToggle("crewventing", "• Crew-Venting", true, false);
            public static readonly CustomToggleOption VentBuilding = CustomOption.AddToggle("ventbuilding", "• Vent-Building", true, false);
            public static readonly CustomNumberOption VentBuildingCooldown = CustomOption.AddNumber("ventbuildingcooldown", "╚══ Vent-Building-Cooldown", true, 7, 2, 30, 1);
            public static readonly CustomToggleOption BodyDragging = CustomOption.AddToggle("bodydragging", "• Body-Dragging", true, false);
            public static readonly CustomToggleOption Invisibility = CustomOption.AddToggle("invisibility", "• Invisibility", true, false);
            public static readonly CustomNumberOption InvisibilityCooldown = CustomOption.AddNumber("invisibilitycooldown", "╚══ Invisibility-Cooldown", true, 20, 2, 60, 2);
            public static readonly CustomNumberOption InvisibilityDuration = CustomOption.AddNumber("invisibilityduration", "╚══ Invisibility-Duration", true, 10, 2, 30, 1);
            public static readonly CustomToggleOption FreezeTime = CustomOption.AddToggle("freezetime", "• Time-Freezing", true, false);
            public static readonly CustomNumberOption FreezeTimeCooldown = CustomOption.AddNumber("freezetimecooldown", "╚══ Time-Freezing-Cooldown", true, 20, 2, 60, 2);
            public static readonly CustomNumberOption FreezeTimeDuration = CustomOption.AddNumber("freezetimeduration", "╚══ Time-Freezing-Duration", true, 10, 2, 30, 1);
            public static readonly CustomToggleOption Morphing = CustomOption.AddToggle("morphing", "• Morphing", true, false);
            public static readonly CustomNumberOption MorphingCooldown = CustomOption.AddNumber("morphingcooldown", "╚══ Morphing-Cooldown", true, 20, 2, 60, 2);
            private static readonly CustomOptionButton SectionRoles = CustomOption.AddButton("˅ Roles", true, false, false);
            //public static CustomToggleOption thanos = CustomOption.AddToggle("thanos", "Thanos", true, false);
            public static readonly CustomNumberOption JesterAmount = CustomOption.AddNumber("jesters", "• Jesters", true, 0, 0, 9, 1);
            public static readonly CustomNumberOption DoctorAmount = CustomOption.AddNumber("doctors", "• Doctors", true, 0, 0, 9, 1);
            public static readonly CustomNumberOption DoctorCooldown = CustomOption.AddNumber("doctorcooldown", "╚══ Revive-Cooldown", true, 10, 2, 60, 2);
            public static readonly CustomNumberOption MayorAmount = CustomOption.AddNumber("mayors", "• Mayors", true, 0, 0, 9, 1);
            public static readonly CustomNumberOption InspectorAmount = CustomOption.AddNumber("inspectors", "• Inspectors", true, 0, 0, 9, 1);
            public static readonly CustomNumberOption SheriffAmount = CustomOption.AddNumber("sheriffs", "• Sheriffs", true, 0, 0, 9, 1);
            public static readonly CustomNumberOption SheriffCooldown = CustomOption.AddNumber("sheriffcooldown", "╚══ Shoot-Cooldown", true, 10, 2, 60, 2);
            public static readonly CustomStringOption Gamemode = CustomOption.AddString("gamemode", "Gamemode", new string[] { "None", "HotPotato", "Battle Royale" });
            public static readonly CustomNumberOption HotPotatoTimer = CustomOption.AddNumber("hotpotatotimer", "HotPotato-Timer", true, 10, 2, 60, 2);
            
            public static void SectionGeneralListener(bool value)
            {
                Venting.MenuVisible = value;
                ReportBodys.MenuVisible = value;
                Sabotaging.MenuVisible = value;
            }
            
            public static void SectionSpecialListener(bool value)
            {
                CrewVenting.MenuVisible = value;
                VentBuilding.MenuVisible = value;
                VentBuildingCooldown.MenuVisible = value;
                BodyDragging.MenuVisible = value;
                Invisibility.MenuVisible = value;
                InvisibilityCooldown.MenuVisible = value;
                InvisibilityDuration.MenuVisible = value;
                FreezeTime.MenuVisible = value;
                FreezeTimeCooldown.MenuVisible = value;
                FreezeTimeDuration.MenuVisible = value;
                Morphing.MenuVisible = value;
                MorphingCooldown.MenuVisible = value;
            }
            
            public static void SectionRolesListener(bool value)
            {
                JesterAmount.MenuVisible = value;
                DoctorAmount.MenuVisible = value;
                DoctorCooldown.MenuVisible = value;
                MayorAmount.MenuVisible = value;
                InspectorAmount.MenuVisible = value;
                SheriffAmount.MenuVisible = value;
                SheriffCooldown.MenuVisible = value;
            }
            
            public enum GameMode
            {
                None = 0,
                HotPotato = 1,
                BattleRoyale = 2
            }

            public static bool IsGameMode(GameMode mode)
            {
                if (Gamemode.GetValue() == (int) mode)
                    return true;
                return false;
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
                    
                    SectionGeneral.SetValue(false);
                    SectionGeneral.OnValueChanged += (sender,args) => { SectionGeneralListener((bool)args.Value); };
                    
                    SectionSpecial.SetValue(false);
                    SectionSpecial.OnValueChanged += (sender,args) => { SectionSpecialListener((bool)args.Value); };
                    
                    SectionRoles.SetValue(false);
                    SectionRoles.OnValueChanged += (sender,args) => { SectionRolesListener((bool)args.Value); };
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
            defaultRegions.Add(new DnsRegionInfo(ServerName.Value, ServerName.Value, StringNames.NoTranslation, new[] {
                new ServerInfo($"{ServerName.Value}-Master-1", ip, port)
            }).Cast<IRegionInfo>());
            var UseLocalHost = Config.Bind("Server", "UseLocalHost", false);
            ServerPort = Config.Bind("Server", "LocalPort", (ushort)22023);
            if (UseLocalHost.Value)
            {
                ip = "127.0.0.1";
                port = ServerPort.Value;
                defaultRegions.Add(new DnsRegionInfo("Localhost", "Localhost", StringNames.NoTranslation, new[] {
                    new ServerInfo($"Localhost-Master-1", ip, port)
                }).Cast<IRegionInfo>());
            }
            ServerManager.DefaultRegions = defaultRegions.ToArray();
            Logger = this.Log;
            CustomOption.ShamelessPlug = false;
            #region OptionsHudVisibility
            Settings.SectionGeneralListener(false);
            Settings.SectionSpecialListener(false);
            Settings.SectionRolesListener(false);
            Settings.Venting.HudVisible = false;
            Settings.ReportBodys.HudVisible = false;
            Settings.Sabotaging.HudVisible = false;
            Settings.CrewVenting.HudVisible = false;
            Settings.VentBuilding.HudVisible = false;
            Settings.VentBuildingCooldown.HudVisible = false;
            Settings.BodyDragging.HudVisible = false;
            Settings.Invisibility.HudVisible = false;
            Settings.InvisibilityCooldown.HudVisible = false;
            Settings.InvisibilityDuration.HudVisible = false;
            Settings.FreezeTime.HudVisible = false;
            Settings.FreezeTimeCooldown.HudVisible = false;
            Settings.FreezeTimeDuration.HudVisible = false;
            Settings.Morphing.HudVisible = false;
            Settings.MorphingCooldown.HudVisible = false;
            Settings.JesterAmount.HudVisible = false;
            Settings.DoctorAmount.HudVisible = false;
            Settings.DoctorCooldown.HudVisible = false;
            Settings.MayorAmount.HudVisible = false;
            Settings.InspectorAmount.HudVisible = false;
            Settings.SheriffAmount.HudVisible = false;
            Settings.SheriffCooldown.HudVisible = false;
            Settings.Gamemode.HudVisible = false;
            Settings.HotPotatoTimer.HudVisible = false;
            #endregion OptionsHudVisibility
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
                if(!Settings.ReportBodys.GetValue() || Peasmod.Settings.IsGameMode(Settings.GameMode.BattleRoyale) || Peasmod.Settings.IsGameMode(Settings.GameMode.HotPotato))
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
                __instance.text.text += "\nReactor-Framework" + "\n" + PluginName + " v" + PluginVersion + " \nby " + StringColor.Green + PluginAuthor + " " + StringColor.Reset + PluginPage;
                __instance.transform.position -= new Vector3(0, 0.5f, 0);
                AccountManager.Instance.accountTab.gameObject.SetActive(false);
                foreach (var _object in GameObject.FindObjectsOfTypeAll(Il2CppType.Of<GameObject>()))
                    if (_object.name.Contains("ReactorVersion"))
                        GameObject.Destroy(_object);
                //if(UnityEngine.Object.FindObjectOfType<MainMenuManager>() != null && UnityEngine.Object.FindObjectOfType<MainMenuManager>().Announcement != null)
                //UnityEngine.Object.FindObjectOfType<MainMenuManager>().Announcement.gameObject.SetActive(true);
            }
        }
        
        [HarmonyPatch(typeof(MainMenuManager), "Start")]
        public static class MainMenuManagerStartPatch
        {
            public static void Postfix()
            {
                Texture2D tex = GUIExtensions.CreateEmptyTexture();
                Assembly assembly = Assembly.GetExecutingAssembly();
                Stream myStream = assembly.GetManifestResourceStream("Peasmod.Resources.Peasmod.png");
                byte[] buttonTexture = Reactor.Extensions.Extensions.ReadFully(myStream);
                ImageConversion.LoadImage(tex, buttonTexture, false);
                GameObject.Find("bannerLogo_AmongUs").GetComponent<SpriteRenderer>().sprite = GUIExtensions.CreateSprite(tex);
                GameObject.Find("AmongUsLogo").GetComponent<SpriteRenderer>().sprite = GUIExtensions.CreateSprite(tex);
                GameObject.Find("AmongUsLogo").transform.position += new Vector3(0.3f, 0, 0);
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
