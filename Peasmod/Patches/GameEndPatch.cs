using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Hazel;
using UnityEngine;
using UnhollowerBaseLib;

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
                    if (JesterMode.Jester1 != null && ExileController.Instance.exiled.Object.PlayerId == JesterMode.Jester1.PlayerId
                        || JesterMode.Jester2 != null && ExileController.Instance.exiled.Object.PlayerId == JesterMode.Jester2.PlayerId)
                        __result = ExileController.Instance.exiled.PlayerName + " was The Jester.";
                    #endregion JesterMode
                    #region DoctorMode
                    if (DoctorMode.Doctor1 != null && ExileController.Instance.exiled.Object.PlayerId == DoctorMode.Doctor1.PlayerId
                        || DoctorMode.Doctor2 != null && ExileController.Instance.exiled.Object.PlayerId == DoctorMode.Doctor2.PlayerId)
                        __result = ExileController.Instance.exiled.PlayerName + " was The Doctor.";
                    #endregion DoctorMode
                    #region MayorMode
                    if (MayorMode.Mayor1 != null && ExileController.Instance.exiled.Object.PlayerId == MayorMode.Mayor1.PlayerId
                        || MayorMode.Mayor2 != null && ExileController.Instance.exiled.Object.PlayerId == MayorMode.Mayor2.PlayerId)
                        __result = ExileController.Instance.exiled.PlayerName + " was The Mayor.";
                    #endregion MayorMode
                    #region InspectorMode
                    if (InspectorMode.Inspector1 != null && ExileController.Instance.exiled.Object.PlayerId == InspectorMode.Inspector1.PlayerId
                        || InspectorMode.Inspector2 != null && ExileController.Instance.exiled.Object.PlayerId == InspectorMode.Inspector2.PlayerId)
                        __result = ExileController.Instance.exiled.PlayerName + " was The Inspector.";
                    #endregion InspectorMode
                    #region SheriffMode
                    if (SheriffMode.Sheriff1 != null && ExileController.Instance.exiled.Object.PlayerId == SheriffMode.Sheriff1.PlayerId
                        || SheriffMode.Sheriff2 != null && ExileController.Instance.exiled.Object.PlayerId == SheriffMode.Sheriff2.PlayerId)
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
                    if (JesterMode.Jester1 != null && ExileController.Instance.exiled.Object.PlayerId == JesterMode.Jester1.PlayerId)
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
            foreach(var player in PlayerControl.AllPlayerControls)
                MorphingMode.OnLabelClick(player, player, false);
            #endregion MorphingMode
            #region JesterMode
            if (JesterMode.Jester1 != null)
            {
                if (AmongUsClient.Instance.AmHost && __instance.PlayerId == JesterMode.Jester1.PlayerId)
                {

                    ShipStatus.Instance.enabled = false;
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.JesterWin, Hazel.SendOption.None, -1);
                    writer.Write(JesterMode.Jester1.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    JesterMode.Winner = JesterMode.Jester1;
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
            }
            if (JesterMode.Jester2 != null)
            {
                if (AmongUsClient.Instance.AmHost && __instance.PlayerId == JesterMode.Jester2.PlayerId)
                {
                    ShipStatus.Instance.enabled = false;
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.JesterWin, Hazel.SendOption.None, -1);
                    writer.Write(JesterMode.Jester2.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    JesterMode.Winner = JesterMode.Jester2;
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
}
