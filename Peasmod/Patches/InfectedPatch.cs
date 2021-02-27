using HarmonyLib;
using Hazel;
using System.Collections.Generic;
using System;
using UnhollowerBaseLib;
using Peasmod.Utility;
using Peasmod.GameModes;

namespace Peasmod.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSetInfected))]
    public static class SetInfectedPatch
    {
        public static void Postfix(PlayerControl __instance,
            [HarmonyArgument(0)] Il2CppReferenceArray<GameData.PlayerInfo> impostors)
        {
            if (PlayerControl.AllPlayerControls.Count == 1) return;
            if(!Peasmod.Settings.hotpotato.GetValue())
            {
                #region JesterMode
                JesterMode.Winner = null;
                JesterMode.JesterWon = false;
                JesterMode.Jesters.Clear();
                for (int i = 1; i <= Peasmod.Settings.jesteramount.GetValue() && Peasmod.crewmates.Count >= 1; i++)
                {
                    var jester = Peasmod.crewmates[Peasmod.random.Next(0, Peasmod.crewmates.Count)];
                    jester.RpcSetRole(Role.Jester);
                    Peasmod.crewmates.Remove(jester);
                }
                #endregion JesterMode
                #region DoctorMode
                DoctorMode.Doctors.Clear();
                Utils.Log("1");
                for (int i = 1; i <= Peasmod.Settings.doctoramount.GetValue() && Peasmod.crewmates.Count >= 1; i++)
                {
                    var doctor = Peasmod.crewmates[Peasmod.random.Next(0, Peasmod.crewmates.Count)];
                    Utils.Log("2: " + doctor.nameText.Text);
                    doctor.RpcSetRole(Role.Doctor);
                    Peasmod.crewmates.Remove(doctor);
                }
                #endregion DoctorMode
                #region MayorMode
                MayorMode.Mayors.Clear();
                for (int i = 1; i <= Peasmod.Settings.mayoramount.GetValue() && Peasmod.crewmates.Count >= 1; i++)
                {
                    var mayor = Peasmod.crewmates[Peasmod.random.Next(0, Peasmod.crewmates.Count)];
                    mayor.RpcSetRole(Role.Mayor);
                    Peasmod.crewmates.Remove(mayor);
                }
                #endregion MayorMode
                #region InspectorMode
                InspectorMode.Inspectors.Clear();
                for (int i = 1; i <= Peasmod.Settings.inspectoramount.GetValue() && Peasmod.crewmates.Count >= 1; i++)
                {
                    var inspector = Peasmod.crewmates[Peasmod.random.Next(0, Peasmod.crewmates.Count)];
                    inspector.RpcSetRole(Role.Inspector);
                    Peasmod.crewmates.Remove(inspector);
                }
                #endregion InspectorMode
                #region SheriffMode
                SheriffMode.Sheriffs.Clear();
                for (int i = 1; i <= Peasmod.Settings.sheriffamount.GetValue() && Peasmod.crewmates.Count >= 1; i++)
                {
                    var sheriff = Peasmod.crewmates[Peasmod.random.Next(0, Peasmod.crewmates.Count)];
                    sheriff.RpcSetRole(Role.Sheriff);
                    Peasmod.crewmates.Remove(sheriff);
                }
                #endregion SheriffMode
            }
        }
    }
}
