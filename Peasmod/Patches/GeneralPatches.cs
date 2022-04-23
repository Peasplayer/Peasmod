using System;
using System.Linq;
using HarmonyLib;
using PeasAPI;
using PeasAPI.Managers;
using Reactor.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Peasmod.Patches
{
    [HarmonyPatch]
    public static class GeneralPatches
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        [HarmonyPostfix]
        private static void ManageButtons(PlayerControl __instance)
        {
            var player = PlayerControl.LocalPlayer;
            if (player == null || player.Data == null || player.Data.Role == null)
                return;

            if (Settings.CrewVenting.Value && player.Data.Role && !player.Data.Role.IsImpostor &&
                PeasAPI.PeasAPI.GameStarted &&
                !MeetingHud.Instance)
            {
                HudManager.Instance.ImpostorVentButton.gameObject.SetActive(true);
                if (player.CanMove && !player.Data.IsDead)
                    HudManager.Instance.ImpostorVentButton.SetEnabled();
                else
                    HudManager.Instance.ImpostorVentButton.SetDisabled();
            }

            if (!Settings.Venting.Value && player.Data.Role.IsImpostor && PeasAPI.PeasAPI.GameStarted)
                HudManager.Instance.ImpostorVentButton.gameObject.SetActive(false);

            if (!Settings.Sabotaging.Value && PeasAPI.PeasAPI.GameStarted)
                HudManager.Instance.SabotageButton.gameObject.SetActive(false);

            if (!Settings.ReportBodys.Value && PeasAPI.PeasAPI.GameStarted)
            {
                HudManager.Instance.ReportButton.gameObject.SetActive(false);
                Physics2D.OverlapCircleAll(PlayerControl.LocalPlayer.GetTruePosition(),
                        PlayerControl.LocalPlayer.MaxReportDistance + 2f, Constants.PlayersOnlyMask)
                    .Where(collider => collider.CompareTag("DeadBody"))
                    .Do(collider => collider.GetComponent<DeadBody>().Reported = true);
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
        [HarmonyPostfix]
        public static void AddSpectatorButtonPatch(PlayerControl __instance)
        {
            if (__instance.IsLocal() && HudManager.Instance.transform.FindChild("Buttons").FindChild("TopRight").FindChild("SpectatorButton") ==
                null)
            {
                var spectatorButton = Object.Instantiate(HudManager.Instance.transform.FindChild("Buttons")
                    .FindChild("TopRight").FindChild("MapButton").gameObject, HudManager.Instance.transform.FindChild("Buttons")
                    .FindChild("TopRight"));
                spectatorButton.name = "SpectatorButton";
                var aspect = spectatorButton.GetComponent<AspectPosition>();
                aspect.DistanceFromEdge = new Vector3(0.4f, 1.7f);
                aspect.AdjustPosition();
                var button = spectatorButton.GetComponent<ButtonBehavior>();
                button.OnClick.RemoveAllListeners();
                button.OnClick.AddListener((UnityEngine.Events.UnityAction) Listener);
                void Listener()
                {
                    PlayerMenuManager.OpenPlayerMenu(
                        Utility.GetAllPlayers().Where(player => !player.IsLocal()).ToList()
                            .ConvertAll(player => player.PlayerId), player => PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(player.transform.position), () => { });
                }
                spectatorButton.GetComponent<SpriteRenderer>().sprite =
                    Utility.CreateSprite("Peasmod.Resources.Buttons.SpectatorButton.png", 100f);
            }
            else
            {
                HudManager.Instance.transform.FindChild("Buttons").FindChild("TopRight").FindChild("SpectatorButton").gameObject.SetActive(true);
            }
        }
        
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Revive))]
        [HarmonyPostfix]
        public static void HideSpectatorButtonPatch(PlayerControl __instance)
        {
            if (__instance.IsLocal() && HudManager.Instance.transform.FindChild("Buttons").FindChild("TopRight").FindChild("SpectatorButton") !=
                null)
            {
                HudManager.Instance.transform.FindChild("Buttons").FindChild("TopRight").FindChild("SpectatorButton").gameObject.SetActive(false);
            }
        }
        
        [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.ShowSabotageMap))]
        [HarmonyPrefix]
        public static bool DisableSabotagesPatch(MapBehaviour __instance)
        {
            if (PeasAPI.PeasAPI.GameStarted && !Settings.Sabotaging.Value)
            {
                HudManager.Instance.ShowMap((Action<MapBehaviour>)(map =>
                {
                    foreach (MapRoom mapRoom in map.infectedOverlay.rooms.ToArray())
                    {
                        mapRoom.gameObject.SetActive(false);
                    }
                }));

                //return false;
            }

            return true;
        }

        [HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
        [HarmonyPriority(Priority.Last)]
        [HarmonyPostfix]
        private static void CrewVentingPatch(Vent __instance, [HarmonyArgument(1)] ref bool canUse,
            [HarmonyArgument(2)] ref bool couldUse, ref float __result)
        {
            if (Settings.CrewVenting.Value && !PlayerControl.LocalPlayer.Data.Role.IsImpostor)
            {
                couldUse = !PlayerControl.LocalPlayer.Data.IsDead;
                canUse = couldUse;
                var distance = Vector2.Distance(PlayerControl.LocalPlayer.GetTruePosition(),
                    __instance.transform.position);
                canUse &= distance <= __instance.UsableDistance;
                __result = distance;
            }

            if (!Settings.Venting.Value && PlayerControl.LocalPlayer.Data.Role.IsImpostor)
            {
                var distance = Vector2.Distance(PlayerControl.LocalPlayer.GetTruePosition(),
                    __instance.transform.position);
                canUse = couldUse = false;
                __result = distance;
            }
        }
        
        public static bool UseHorseMode;
        
        [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
        [HarmonyPostfix]
        public static void HorseModeButtonPatch()
        {
            if (GameObject.Find("CreditsButton") != null)
            {
                var horseModeButton = Object.Instantiate(GameObject.Find("CreditsButton"), GameObject.Find("CreditsButton").transform.parent);
                horseModeButton.name = "HorseModeButton";
                horseModeButton.GetComponent<SpriteRenderer>().sprite = Utility.CreateSprite("Peasmod.Resources.Buttons.HorseModeButton.png", 95f);
                horseModeButton.GetComponent<ButtonRolloverHandler>().Destroy();
                var button = horseModeButton.GetComponent<PassiveButton>();
                button.OnClick = new Button.ButtonClickedEvent();
                button.OnClick.AddListener((UnityAction) OnClick);

                void OnClick()
                {
                    UseHorseMode = !UseHorseMode;
                    horseModeButton.GetComponent<SpriteRenderer>().material.SetColor("_Color", UseHorseMode ? Color.green : Color.white);
                }
            }
            
            if (GameObject.Find("HorseModeButton") != null)
            {
                var horseModeButton = GameObject.Find("HorseModeButton");
                horseModeButton.name = "HorseModeButton";
                horseModeButton.GetComponent<SpriteRenderer>().material
                    .SetColor("_Color", UseHorseMode ? Color.green : Color.white);
            }
        }

        [HarmonyPatch(typeof(Constants), nameof(Constants.ShouldHorseAround))]
        [HarmonyPrefix]
        public static bool UseHorseModePatch(out bool __result)
        {
            __result = UseHorseMode;
            return false;
        }
        
        [HarmonyPatch]
        public static class CustomAnnouncementServerPatch
        {
            [HarmonyPatch(typeof(AnnouncementPopUp), nameof(AnnouncementPopUp.Init))]
            [HarmonyPrefix]
            static void ChangeAnnouncementServerToCustomPatch(AnnouncementPopUp __instance)
            {
                //Replaces the URL with your own so it requests the announcement from your server
                Constants.BaseEndpoint = "https://api.peasplayer.tk/amongus/";
            }
            
            [HarmonyPatch(typeof(AnnouncementPopUp), nameof(AnnouncementPopUp.Init))]
            [HarmonyPostfix]
            static void ChangeAnnouncementServerBackPatch(AnnouncementPopUp __instance)
            {
                //Show the announcement
                __instance.Show();
                
                //Resets it back to the default URL
                Constants.BaseEndpoint = "https://backend.innersloth.com/api/";
            }
        }

        [HarmonyPatch(typeof(StatsManager), nameof(StatsManager.AmBanned), MethodType.Getter)]
        [HarmonyPostfix]
        public static void UnbanPatch(out bool __result)
        {
            __result = false;
        }
    }
}