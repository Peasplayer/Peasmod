using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Hazel;
using UnityEngine;
using UnhollowerBaseLib;
using Peasmod.Gamemodes;
using Peasmod.Utility;

namespace Peasmod.Patches
{
    [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), new Type[] { typeof(StringNames), typeof(Il2CppReferenceArray<Il2CppSystem.Object>) })]
    static class TranslationPatch
    {
        static void Postfix(ref string __result, StringNames HKOIECMDOKL, Il2CppReferenceArray<Il2CppSystem.Object> EBKIKEILMLF)
        {
            if (ExileController.Instance != null && ExileController.Instance.exiled != null)
            {
                if (HKOIECMDOKL == StringNames.ExileTextPN || HKOIECMDOKL == StringNames.ExileTextSN)
                {
                    #region JesterMode
                    if (ExileController.Instance.exiled.Object.IsRole(Role.Jester))
                        __result = ExileController.Instance.exiled.PlayerName + " was The Jester.";
                    #endregion JesterMode
                    #region DoctorMode
                    if (ExileController.Instance.exiled.Object.IsRole(Role.Doctor))
                        __result = ExileController.Instance.exiled.PlayerName + " was The Doctor.";
                    #endregion DoctorMode
                    #region MayorMode
                    if (ExileController.Instance.exiled.Object.IsRole(Role.Mayor))
                        __result = ExileController.Instance.exiled.PlayerName + " was The Mayor.";
                    #endregion MayorMode
                    #region InspectorMode
                    if (ExileController.Instance.exiled.Object.IsRole(Role.Inspector))
                        __result = ExileController.Instance.exiled.PlayerName + " was The Inspector.";
                    #endregion InspectorMode
                    #region SheriffMode
                    if (ExileController.Instance.exiled.Object.IsRole(Role.Sheriff))
                        __result = ExileController.Instance.exiled.PlayerName + " was The Sheriff.";
                    #endregion SheriffMode
                    if (__result == null)
                        if (Peasmod.impostors.Count == 1)
                            __result = ExileController.Instance.exiled.PlayerName + " was not The Impostor.";
                        else
                            __result = ExileController.Instance.exiled.PlayerName + " was not An Impostor.";
                }
                if (HKOIECMDOKL == StringNames.ImpostorsRemainP || HKOIECMDOKL == StringNames.ImpostorsRemainS)
                {
                    #region JesterMode
                    if (ExileController.Instance.exiled.Object.IsRole(Role.Jester))
                        __result = "";
                    #endregion JesterMode
                }
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Exiled))]
    public static class PlayerControlWinPatch
    {
        public static void Prefix(PlayerControl __instance)
        {
            #region MorphingMode
            if(PlayerControl.LocalPlayer.IsMorphed())
                MorphingMode.OnLabelClick(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer, false);
            #endregion MorphingMode
            #region JesterMode
            if (__instance.IsRole(Role.Jester))
            {
                ShipStatus.Instance.enabled = false;
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.JesterWin, Hazel.SendOption.None, -1);
                writer.Write(__instance.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                JesterMode.Winner = __instance;
                JesterMode.JesterWon = true;
                JesterMode.Winner.Data.IsImpostor = true;
                JesterMode.Winner.infectedSet = true;
                foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                {
                    if (player.PlayerId != JesterMode.Winner.PlayerId)
                    {
                        player.RemoveInfected();
                        player.Data.IsImpostor = false;
                    }
                }
                HandleWinRpc();
                #if ITCH
                    ShipStatus.Method_34(GameOverReason.ImpostorByVote, false);
                #elif STEAM
                    ShipStatus.RpcEndGame(GameOverReason.ImpostorByVote, false);
                #endif
            }
            #endregion JesterMode
        }

        public static void HandleWinRpc()
        {
            #region JesterMode
            if(JesterMode.JesterWon)
            {
                if (PlayerControl.LocalPlayer.PlayerId == JesterMode.Winner.PlayerId)
                {
                    EndGameScreenPatch.Text = "Victory";
                    EndGameScreenPatch.TextColor = Palette.CrewmateBlue;
                    EndGameScreenPatch.Winner = "crew";
                }
                else
                {
                    EndGameScreenPatch.Text = "Defeat";
                    EndGameScreenPatch.TextColor = Palette.ImpostorRed;
                    EndGameScreenPatch.Winner = "impostor";
                }
                EndGameScreenPatch.BGColor = JesterMode.JesterColor;
            }
            #endregion JesterMode
        }
    }

    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
    public static class GameEndPatch
    {
        public static void Prefix()
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                player.Visible = true;
                player.moveable = true;
            }
            PlayerControlExtensions.ResetRoles();
            #region JesterMode
            if (JesterMode.JesterWon)
            {
                Il2CppSystem.Collections.Generic.List<WinningPlayerData> _winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
                JesterMode.Winner.Data.IsDead = false;
                _winners.Add(new WinningPlayerData(JesterMode.Winner.Data));
                TempData.winners = _winners;
            }
            #endregion JesterMode
            ShipstatusOnEnablePatch.gameStarted = false;
        }
    }

    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public static class EndGameScreenPatch
    {
        public static string Text;
        public static Color? TextColor;

        public static Color? BGColor;
        public static string Winner;

        static void Prefix(EndGameManager __instance)
        {
            Patch(__instance, false);
        }

        public static void Postfix(EndGameManager __instance)
        {
            Patch(__instance, true);
        }

        private static void Patch(EndGameManager __instance, bool removeState)
        {
            __instance.DisconnectStinger = Winner switch
            {
                "crew" => __instance.CrewStinger,
                "impostor" => __instance.ImpostorStinger,
                _ => __instance.DisconnectStinger
            };
            if (Text != null)
            {
                __instance.WinText.Text = Text;
            }

            if (TextColor != null)
            {
                __instance.WinText.Color = (Color)TextColor;
            }

            if (BGColor != null)
            {
                __instance.BackgroundBar.material.color = (Color)BGColor;
            }

            if (removeState)
            {
                Text = null;
                TextColor = null;
                BGColor = null;
            }
        }
    }

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CheckEndCriteria))]
    [HarmonyPriority(Priority.First)]
    public static class CheckEndCriteriaPatch
    {
        public static bool Prefix()
        {
            if(Peasmod.Settings.hotpotato.GetValue())
            {
                var impostors = 0;
                var crewmates = 0;
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if(!player.Data.IsDead)
                    {
                        if (player.Data.IsImpostor)
                            impostors++;
                        else
                            crewmates++;
                    }
                }
                if (impostors > crewmates)
                    return true;
                else
                    return false;
            }
            return true;
        }
    }
}
