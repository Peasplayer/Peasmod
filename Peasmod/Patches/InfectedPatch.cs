using HarmonyLib;
using Hazel;
using System.Collections.Generic;
using System;
using UnhollowerBaseLib;

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
            JesterMode.Jester1 = null;
            JesterMode.Jester2 = null;
            if (Peasmod.Settings.jesteramount.GetValue() >= 1 && Peasmod.crewmates.Count >= 1)
            {
                JesterMode.Jester1 = Peasmod.crewmates[Peasmod.random.Next(0, Peasmod.crewmates.Count)];
                Peasmod.crewmates.Remove(JesterMode.Jester1);
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(__instance.NetId, (byte)CustomRpc.SetJester1, Hazel.SendOption.None, -1);
                writer.Write(JesterMode.Jester1.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
            if (Peasmod.Settings.jesteramount.GetValue() == 2 && Peasmod.crewmates.Count >= 1)
            {
                JesterMode.Jester2 = Peasmod.crewmates[Peasmod.random.Next(0, Peasmod.crewmates.Count)];
                Peasmod.crewmates.Remove(JesterMode.Jester2);
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(__instance.NetId, (byte)CustomRpc.SetJester2, Hazel.SendOption.None, -1);
                writer.Write(JesterMode.Jester2.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
            #endregion JesterMode
            #region DoctorMode
            DoctorMode.Doctor1 = null;
            DoctorMode.Doctor2 = null;
            if (Peasmod.Settings.doctoramount.GetValue() >= 1 && Peasmod.crewmates.Count >= 1)
            {
                DoctorMode.Doctor1 = Peasmod.crewmates[Peasmod.random.Next(0, Peasmod.crewmates.Count)];
                Peasmod.crewmates.Remove(DoctorMode.Doctor1);
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(__instance.NetId, (byte)CustomRpc.SetDoctor1, Hazel.SendOption.None, -1);
                writer.Write(DoctorMode.Doctor1.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
            if (Peasmod.Settings.doctoramount.GetValue() == 2 && Peasmod.crewmates.Count >= 1)
            {
                DoctorMode.Doctor2 = Peasmod.crewmates[Peasmod.random.Next(0, Peasmod.crewmates.Count)];
                Peasmod.crewmates.Remove(DoctorMode.Doctor2);
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(__instance.NetId, (byte)CustomRpc.SetDoctor2, Hazel.SendOption.None, -1);
                writer.Write(DoctorMode.Doctor2.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
            #endregion DoctorMode
            #region MayorMode
            MayorMode.Mayor1 = null;
            MayorMode.Mayor2 = null;
            if (Peasmod.Settings.mayoramount.GetValue() >= 1 && Peasmod.crewmates.Count >= 1)
            {
                MayorMode.Mayor1 = Peasmod.crewmates[Peasmod.random.Next(0, Peasmod.crewmates.Count)];
                Peasmod.crewmates.Remove(MayorMode.Mayor1);
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(__instance.NetId, (byte)CustomRpc.SetMayor1, Hazel.SendOption.None, -1);
                writer.Write(MayorMode.Mayor1.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
            if (Peasmod.Settings.mayoramount.GetValue() == 2 && Peasmod.crewmates.Count >= 1)
            {
                MayorMode.Mayor2 = Peasmod.crewmates[Peasmod.random.Next(0, Peasmod.crewmates.Count)];
                Peasmod.crewmates.Remove(MayorMode.Mayor2);
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(__instance.NetId, (byte)CustomRpc.SetMayor2, Hazel.SendOption.None, -1);
                writer.Write(MayorMode.Mayor2.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
            #endregion MayorMode
            #region InspectorMode
            InspectorMode.Inspector1 = null;
            InspectorMode.Inspector2 = null;
            if (Peasmod.Settings.inspectoramount.GetValue() >= 1 && Peasmod.crewmates.Count >= 1)
            {
                InspectorMode.Inspector1 = Peasmod.crewmates[Peasmod.random.Next(0, Peasmod.crewmates.Count)];
                Peasmod.crewmates.Remove(InspectorMode.Inspector1);
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(__instance.NetId, (byte)CustomRpc.SetInspector1, Hazel.SendOption.None, -1);
                writer.Write(InspectorMode.Inspector1.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
            if (Peasmod.Settings.inspectoramount.GetValue() == 2 && Peasmod.crewmates.Count >= 1)
            {
                InspectorMode.Inspector2 = Peasmod.crewmates[Peasmod.random.Next(0, Peasmod.crewmates.Count)];
                Peasmod.crewmates.Remove(InspectorMode.Inspector2);
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(__instance.NetId, (byte)CustomRpc.SetInspector2, Hazel.SendOption.None, -1);
                writer.Write(InspectorMode.Inspector2.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
            #endregion InspectorMode
            #region SheriffMode
            SheriffMode.Sheriff1 = null;
            SheriffMode.Sheriff2 = null;
            if (Peasmod.Settings.sheriffamount.GetValue() >= 1 && Peasmod.crewmates.Count >= 1)
            {
                SheriffMode.Sheriff1 = Peasmod.crewmates[Peasmod.random.Next(0, Peasmod.crewmates.Count)];
                Peasmod.crewmates.Remove(SheriffMode.Sheriff1);
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(__instance.NetId, (byte)CustomRpc.SetSheriff1, Hazel.SendOption.None, -1);
                writer.Write(SheriffMode.Sheriff1.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
            if (Peasmod.Settings.sheriffamount.GetValue() == 2 && Peasmod.crewmates.Count >= 1)
            {
                SheriffMode.Sheriff2 = Peasmod.crewmates[Peasmod.random.Next(0, Peasmod.crewmates.Count)];
                Peasmod.crewmates.Remove(SheriffMode.Sheriff2);
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(__instance.NetId, (byte)CustomRpc.SetSheriff2, Hazel.SendOption.None, -1);
                writer.Write(SheriffMode.Sheriff2.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
            #endregion SheriffMode
        }
    }
}
