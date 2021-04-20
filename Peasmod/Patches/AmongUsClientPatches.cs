using System;
using System.Collections.Generic;
using System.Text;
using InnerNet;
using HarmonyLib;
using Hazel;
using Iced.Intel;
using Peasmod.Gamemodes;
using UnityEngine;
using Peasmod.Utility;
using UnhollowerRuntimeLib;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Peasmod.Patches
{
    class AmongUsClientPatches
    {
        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnDisconnected))]
        class DisconnectPatch
        {
            public static void Prefix(AmongUsClient __instance)
            { 
                DiscordManager.Instance.SetInMenus();
            }
        }

        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameJoined))]
        class JoinPatch
        {
            public static void Prefix(AmongUsClient __instance, [HarmonyArgument(0)] string gameIdString, [HarmonyArgument(1)] ClientData client)
            {
                BattleRoyaleMode.HasKilled = false;
            }
        }
        
        [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.Start))]
        class ClientStartPatch
        {
            public static void Prefix(InnerNetClient __instance)
            {
                DiscordManager.Instance.Start();
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Start))]
        class StartPatch
        {
            public static void Prefix(PlayerControl __instance)
            {
                if (PlayerControl.LocalPlayer == null || __instance.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                {
                   
                    __instance.SetRole(Role.Crewmate);
                }
            }
        }

        [HarmonyPatch(typeof(OptionsMenuBehaviour), nameof(OptionsMenuBehaviour.Open))]
        class OptionsMenuOpenPatch
        {
            public static void Prefix(OptionsMenuBehaviour __instance)
            {
                var resolutionSlider = __instance.Tabs[1].Content.GetComponent<ResolutionSlider>();
                if (resolutionSlider == null)
                    Utils.Log("null");
                //resolutionSlider.VSync.gameObject.transform.position +=
                //    new Vector3(0, 0.3f, 0);
                var test = GameObject.Instantiate(resolutionSlider.VSync, resolutionSlider.VSync.gameObject.transform);
                test.gameObject.transform.position -=
                    new Vector3(0, 0.6f, 0);
                var button = test.GetComponent<PassiveButton>();
                button.OnClick = new Button.ButtonClickedEvent();
                var buttonTextures = Peasmod.Instance.Config.Bind("Settings", "ButtonTextures", true, "If set to true the textures that Peasplayer made will be used. Else the textures of [ARTIST] will be used.");
                if (buttonTextures.Value)
                {
                    test.UpdateText(true);
                    test.Text.text = "Textures: Peasplayer";
                }
                else
                {
                    test.UpdateText(false);
                    test.Text.text = "Textures: [ARTIST]";
                }
                
                button.OnClick.AddListener((UnityAction)test1);
                void test1()
                {
                    if (test.onState)
                    {
                        buttonTextures.Value = false;
                        test.UpdateText(false);
                        test.Text.text = "Textures: [ARTIST]";
                    }
                    else
                    {
                        buttonTextures.Value = true;
                        test.UpdateText(true);
                        test.Text.text = "Textures: Peasplayer";
                    }
                }
            }
        }
    }
}
