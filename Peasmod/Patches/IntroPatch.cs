using HarmonyLib;
using System.Collections.Generic;
using System;

namespace Peasmod.Patches
{
    [HarmonyPatch(typeof(IntroCutscene.CoBegin__d), nameof(IntroCutscene.CoBegin__d.MoveNext))]
    public static class StartScreenPatch
    {
        public static void Prefix(IntroCutscene.CoBegin__d __instance)
        {
            #region JesterMode
            if (JesterMode.Jester1 != null)
            {
                if (JesterMode.Jester1.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                {
                    var yourTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
                    yourTeam.Add(PlayerControl.LocalPlayer);
                    __instance.yourTeam = yourTeam;
                }
            }
            if (JesterMode.Jester2 != null)
            {
                if (JesterMode.Jester2.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                {
                    var yourTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
                    yourTeam.Add(PlayerControl.LocalPlayer);
                    __instance.yourTeam = yourTeam;
                }
            }
            #endregion JesterMode
        }
        public static void Postfix(IntroCutscene.CoBegin__d __instance)
        {
            #region JesterMode
            if (JesterMode.Jester1 != null)
            {
                if (JesterMode.Jester1.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                {
                    var inst = __instance.__this;
                    inst.Title.Text = "Jester";
                    inst.Title.Color = JesterMode.JesterColor;
                    inst.ImpostorText.Text = "Get voted out";
                    inst.BackgroundBar.material.color = JesterMode.JesterColor;
                }
            }
            if (JesterMode.Jester2 != null)
            {
                if (JesterMode.Jester2.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                {
                    var inst = __instance.__this;
                    inst.Title.Text = "Jester";
                    inst.Title.Color = JesterMode.JesterColor;
                    inst.ImpostorText.Text = "Get voted out";
                    inst.BackgroundBar.material.color = JesterMode.JesterColor;
                }
            }
            #endregion JesterMode
            #region DoctorMode
            if (DoctorMode.Doctor1 != null)
            {
                if (DoctorMode.Doctor1.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                {
                    var inst = __instance.__this;
                    inst.Title.Text = "Doctor";
                    inst.Title.Color = DoctorMode.DoctorColor;
                    inst.ImpostorText.Text = "Revive dead crewmates";
                    inst.BackgroundBar.material.color = DoctorMode.DoctorColor;
                }
            }
            if (DoctorMode.Doctor2 != null)
            {
                if (DoctorMode.Doctor2.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                {
                    var inst = __instance.__this;
                    inst.Title.Text = "Doctor";
                    inst.Title.Color = DoctorMode.DoctorColor;
                    inst.ImpostorText.Text = "Revive dead crewmates";
                    inst.BackgroundBar.material.color = DoctorMode.DoctorColor;
                }
            }
            #endregion DoctorMode
            #region MayorMode
            if (MayorMode.Mayor1 != null)
            {
                if (MayorMode.Mayor1.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                {
                    var inst = __instance.__this;
                    inst.Title.Text = "Mayor";
                    inst.Title.Color = MayorMode.MayorColor;
                    inst.ImpostorText.Text = "Your vote counts twice";
                    inst.BackgroundBar.material.color = MayorMode.MayorColor;
                }
            }
            if (MayorMode.Mayor2 != null)
            {
                if (MayorMode.Mayor2.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                {
                    var inst = __instance.__this;
                    inst.Title.Text = "Mayor";
                    inst.Title.Color = MayorMode.MayorColor;
                    inst.ImpostorText.Text = "Your vote counts twice";
                    inst.BackgroundBar.material.color = MayorMode.MayorColor;
                }
            }
            #endregion MayorMode
            #region InspectorMode
            if (InspectorMode.Inspector1 != null)
            {
                if (InspectorMode.Inspector1.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                {
                    var inst = __instance.__this;
                    inst.Title.Text = "Inspector";
                    inst.Title.Color = InspectorMode.InspectorColor;
                    inst.ImpostorText.Text = "Find evidence against the impostor";
                    inst.BackgroundBar.material.color = InspectorMode.InspectorColor;
                }
            }
            if (InspectorMode.Inspector2 != null)
            {
                if (InspectorMode.Inspector2.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                {
                    var inst = __instance.__this;
                    inst.Title.Text = "Inspector";
                    inst.Title.Color = InspectorMode.InspectorColor;
                    inst.ImpostorText.Text = "Find evidence against the impostor";
                    inst.BackgroundBar.material.color = InspectorMode.InspectorColor;
                }
            }
            #endregion InspectorMode
            #region SheriffMode
            if (SheriffMode.Sheriff1 != null)
            {
                if (SheriffMode.Sheriff1.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                {
                    var inst = __instance.__this;
                    inst.Title.Text = "Sheriff";
                    inst.Title.Color = SheriffMode.SheriffColor;
                    inst.ImpostorText.Text = "Shot the impostor";
                    inst.BackgroundBar.material.color = SheriffMode.SheriffColor;
                }
            }
            if (SheriffMode.Sheriff2 != null)
            {
                if (SheriffMode.Sheriff2.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                {
                    var inst = __instance.__this;
                    inst.Title.Text = "Sheriff";
                    inst.Title.Color = SheriffMode.SheriffColor;
                    inst.ImpostorText.Text = "Shot the impostor";
                    inst.BackgroundBar.material.color = SheriffMode.SheriffColor;
                }
            }
            #endregion SheriffMode
        }
    }
}
