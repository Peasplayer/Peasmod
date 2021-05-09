using HarmonyLib;
using System.Collections.Generic;
using System;
using Peasmod.Utility;
using Peasmod.Gamemodes;

namespace Peasmod.Patches
{
    [HarmonyPatch(typeof(IntroCutscene.Nested_0), nameof(IntroCutscene.Nested_0.MoveNext))]
    public static class StartScreenPatch
    {
        public static void Prefix(IntroCutscene.Nested_0 __instance)
        {
            #region HotPotatoMode
            if(Settings.IsGameMode(Settings.GameMode.HotPotato))
            {
                var yourTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
                foreach (var player in PlayerControl.AllPlayerControls)
                    if (player.Data.IsImpostor)
                        yourTeam.Add(player);
                __instance.yourTeam = yourTeam;
            }
            #endregion HotPotatoMode
            #region JesterMode
            else if (PlayerControl.LocalPlayer.IsRole(Role.Jester))
            {
                var yourTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
                yourTeam.Add(PlayerControl.LocalPlayer);
                __instance.yourTeam = yourTeam;
            }
            #endregion JesterMode
        }
        public static void Postfix(IntroCutscene.Nested_0 __instance)
        {
            #region HotPotatoMode
            if (Settings.IsGameMode(Settings.GameMode.HotPotato))
            {
                var inst = __instance.__this;
                inst.Title.text = "HotPotato";
                inst.Title.color = HotPotatoMode.color;
                inst.ImpostorText.text = "Watch out for the Potato";
                inst.BackgroundBar.material.color = HotPotatoMode.color;
            }
            #endregion HotPotatoMode
            #region BatttleRoyaleMode
            else if (Settings.IsGameMode(Settings.GameMode.BattleRoyale))
            {
                var inst = __instance.__this;
                inst.Title.text = "Battle Royale";
                inst.Title.color = Palette.ImpostorRed;
                inst.ImpostorText.text = "Be the last person alive";
                inst.BackgroundBar.material.color = Palette.ImpostorRed;
                PlayerControl.LocalPlayer.Data.IsImpostor = true;
            }
            #endregion BattleRoyaleMode
            #region JesterMode
            if (PlayerControl.LocalPlayer.IsRole(Role.Jester))
            {
                var inst = __instance.__this;
                inst.Title.text = "Jester";
                inst.Title.color = JesterMode.JesterColor;
                inst.ImpostorText.text = "Get voted out";
                inst.BackgroundBar.material.color = JesterMode.JesterColor;
            }
            #endregion JesterMode
            #region DoctorMode
            if (PlayerControl.LocalPlayer.IsRole(Role.Doctor))
            {
                var inst = __instance.__this;
                inst.Title.text = "Doctor";
                inst.Title.color = DoctorMode.DoctorColor;
                inst.ImpostorText.text = "Revive dead crewmates";
                inst.BackgroundBar.material.color = DoctorMode.DoctorColor;
            }
            #endregion DoctorMode
            #region MayorMode
            if (PlayerControl.LocalPlayer.IsRole(Role.Mayor))
            {
                var inst = __instance.__this;
                inst.Title.text = "Mayor";
                inst.Title.color = MayorMode.MayorColor;
                inst.ImpostorText.text = "Your vote counts twice";
                inst.BackgroundBar.material.color = MayorMode.MayorColor;
            }
            #endregion MayorMode
            #region InspectorMode
            if (PlayerControl.LocalPlayer.IsRole(Role.Inspector))
            {
                var inst = __instance.__this;
                inst.Title.text = "Inspector";
                inst.Title.color = InspectorMode.InspectorColor;
                inst.ImpostorText.text = "Find evidence against the impostor";
                inst.BackgroundBar.material.color = InspectorMode.InspectorColor;
            }
            #endregion InspectorMode
            #region SheriffMode
            if (PlayerControl.LocalPlayer.IsRole(Role.Sheriff))
            {
                var inst = __instance.__this;
                inst.Title.text = "Sheriff";
                inst.Title.color = SheriffMode.SheriffColor;
                inst.ImpostorText.text = "Shoot the impostor";
                inst.BackgroundBar.material.color = SheriffMode.SheriffColor;
            }
            #endregion SheriffMode
            #region ThanosMode
            if(PlayerControl.LocalPlayer.IsRole(Role.Thanos))
            {
                var inst = __instance.__this;
                inst.Title.text = "Thanos";
                inst.Title.color = ThanosMode.ThanosColor;
                inst.BackgroundBar.material.color = ThanosMode.ThanosColor;
            }
            #endregion ThanosMode
        }
    }
}
