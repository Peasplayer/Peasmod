using HarmonyLib;
using System.Collections.Generic;
using System;
using Peasmod.Utility;
using Peasmod.Gamemodes;

namespace Peasmod.Patches
{
    [HarmonyPatch(typeof(IntroCutscene.CoBegin__d), nameof(IntroCutscene.CoBegin__d.MoveNext))]
    public static class StartScreenPatch
    {
        public static void Prefix(IntroCutscene.CoBegin__d __instance)
        {
            #region HotPotatoMode
            if(Peasmod.Settings.hotpotato.GetValue())
            {
                var yourTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
                foreach (var player in PlayerControl.AllPlayerControls)
                    if (player.Data.IsImpostor)
                        yourTeam.Add(player);
                __instance.yourTeam = yourTeam;
            }
            #endregion HotPotatoMode
            #region JesterMode
            if (PlayerControl.LocalPlayer.IsRole(Role.Jester))
            {
                var yourTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
                yourTeam.Add(PlayerControl.LocalPlayer);
                __instance.yourTeam = yourTeam;
            }
            #endregion JesterMode
        }
        public static void Postfix(IntroCutscene.CoBegin__d __instance)
        {
            #region HotPotatoMode
            if (Peasmod.Settings.hotpotato.GetValue())
            {
                var inst = __instance.__this;
                inst.Title.Text = "HotPotato";
                inst.Title.Color = HotPotatoMode.color;
                inst.ImpostorText.Text = "Watch out for the Potato";
                inst.BackgroundBar.material.color = HotPotatoMode.color;
            }
            #endregion HotPotatoMode
            #region JesterMode
            if (PlayerControl.LocalPlayer.IsRole(Role.Jester))
            {
                var inst = __instance.__this;
                inst.Title.Text = "Jester";
                inst.Title.Color = JesterMode.JesterColor;
                inst.ImpostorText.Text = "Get voted out";
                inst.BackgroundBar.material.color = JesterMode.JesterColor;
            }
            #endregion JesterMode
            #region DoctorMode
            if (PlayerControl.LocalPlayer.IsRole(Role.Doctor))
            {
                var inst = __instance.__this;
                inst.Title.Text = "Doctor";
                inst.Title.Color = DoctorMode.DoctorColor;
                inst.ImpostorText.Text = "Revive dead crewmates";
                inst.BackgroundBar.material.color = DoctorMode.DoctorColor;
            }
            #endregion DoctorMode
            #region MayorMode
            if (PlayerControl.LocalPlayer.IsRole(Role.Mayor))
            {
                var inst = __instance.__this;
                inst.Title.Text = "Mayor";
                inst.Title.Color = MayorMode.MayorColor;
                inst.ImpostorText.Text = "Your vote counts twice";
                inst.BackgroundBar.material.color = MayorMode.MayorColor;
            }
            #endregion MayorMode
            #region InspectorMode
            if (PlayerControl.LocalPlayer.IsRole(Role.Inspector))
            {
                var inst = __instance.__this;
                inst.Title.Text = "Inspector";
                inst.Title.Color = InspectorMode.InspectorColor;
                inst.ImpostorText.Text = "Find evidence against the impostor";
                inst.BackgroundBar.material.color = InspectorMode.InspectorColor;
            }
            #endregion InspectorMode
            #region SheriffMode
            if (PlayerControl.LocalPlayer.IsRole(Role.Sheriff))
            {
                var inst = __instance.__this;
                inst.Title.Text = "Sheriff";
                inst.Title.Color = SheriffMode.SheriffColor;
                inst.ImpostorText.Text = "Shot the impostor";
                inst.BackgroundBar.material.color = SheriffMode.SheriffColor;
            }
            #endregion SheriffMode
        }
    }
}
