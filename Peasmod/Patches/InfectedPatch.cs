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
            #region JesterMode
            JesterMode.Winner = null;
            JesterMode.JesterWon = false;
            JesterMode.Jesters.Clear();
            for (int i = 1; i <= Peasmod.Settings.jesteramount.GetValue() && Peasmod.crewmates.Count >= 1; i++)
            {
                var jester = Peasmod.crewmates[Peasmod.random.Next(0, Peasmod.crewmates.Count)];
                jester.RpcSetRole(Role.Jester);
                //JesterMode.Jesters.Add(jester);
                Peasmod.crewmates.Remove(jester);
                /*MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(__instance.NetId, (byte)CustomRpc.AddMayor, Hazel.SendOption.None, -1);
                writer.Write(jester.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);*/
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
                //DoctorMode.Doctors.Add(doctor);
                Peasmod.crewmates.Remove(doctor);
                /*MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(__instance.NetId, (byte)CustomRpc.AddMayor, Hazel.SendOption.None, -1);
                writer.Write(doctor.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);*/
            }
            #endregion DoctorMode
            #region MayorMode
            MayorMode.Mayors.Clear();
            for (int i = 1; i <= Peasmod.Settings.mayoramount.GetValue() && Peasmod.crewmates.Count >= 1; i++)
            {
                var mayor = Peasmod.crewmates[Peasmod.random.Next(0, Peasmod.crewmates.Count)];
                mayor.RpcSetRole(Role.Mayor);
                //MayorMode.Mayors.Add(mayor);
                Peasmod.crewmates.Remove(mayor);
                /*MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(__instance.NetId, (byte)CustomRpc.AddMayor, Hazel.SendOption.None, -1);
                writer.Write(mayor.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);*/
            }
            #endregion MayorMode
            #region InspectorMode
            InspectorMode.Inspectors.Clear();
            for (int i = 1; i <= Peasmod.Settings.inspectoramount.GetValue() && Peasmod.crewmates.Count >= 1; i++)
            {
                var inspector = Peasmod.crewmates[Peasmod.random.Next(0, Peasmod.crewmates.Count)];
                inspector.RpcSetRole(Role.Inspector);
                //InspectorMode.Inspectors.Add(inspector);
                Peasmod.crewmates.Remove(inspector);
                /*MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(__instance.NetId, (byte)CustomRpc.AddMayor, Hazel.SendOption.None, -1);
                writer.Write(inspector.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);*/
            }
            #endregion InspectorMode
            #region SheriffMode
            SheriffMode.Sheriffs.Clear();
            for (int i = 1; i <= Peasmod.Settings.sheriffamount.GetValue() && Peasmod.crewmates.Count >= 1; i++)
            {
                var sheriff = Peasmod.crewmates[Peasmod.random.Next(0, Peasmod.crewmates.Count)];
                sheriff.RpcSetRole(Role.Sheriff);
                //SheriffMode.Sheriffs.Add(sheriff);
                Peasmod.crewmates.Remove(sheriff);
                /*MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(__instance.NetId, (byte)CustomRpc.AddMayor, Hazel.SendOption.None, -1);
                writer.Write(sheriff.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);*/
            }
            #endregion SheriffMode
        }
    }
}
