using HarmonyLib;
using Hazel;
using System.Collections.Generic;
using System;
using Reactor.Extensions;
using UnhollowerBaseLib;
using Peasmod.Utility;
using Peasmod.Gamemodes;

namespace Peasmod.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSetInfected))]
    public static class SetInfectedPatch
    {
        public static void Prefix(PlayerControl __instance,
            [HarmonyArgument(0)] Il2CppReferenceArray<GameData.PlayerInfo> impostors)
        {
            if (PlayerControl.AllPlayerControls.Count == 1) return;
            if (Peasmod.Settings.gamemode.GetValue() == (int)Peasmod.Settings.GameMode.HotPotato)
            {

            }
            else if(Peasmod.Settings.gamemode.GetValue() == (int)Peasmod.Settings.GameMode.BattleRoyale)
            {
                foreach (var _player in PlayerControl.AllPlayerControls)
                    _player.Data.IsImpostor = false;
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.BattleRoyaleInit, Hazel.SendOption.None, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Il2CppStructArray<byte> _impostors = new byte[1];
                PlayerControl.LocalPlayer.SetInfected(_impostors);
                PlayerControl.LocalPlayer.Data.IsImpostor = true;
                HudManager.Instance.KillButton.gameObject.SetActive(true);
            }
        }

        public static void Postfix(PlayerControl __instance,
            [HarmonyArgument(0)] Il2CppReferenceArray<GameData.PlayerInfo> impostors)
        {
            if (PlayerControl.AllPlayerControls.Count == 1) return;
            if(Peasmod.Settings.gamemode.GetValue() == (int)Peasmod.Settings.GameMode.None)
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
                for (int i = 1; i <= Peasmod.Settings.doctoramount.GetValue() && Peasmod.crewmates.Count >= 1; i++)
                {
                    var doctor = Peasmod.crewmates[Peasmod.random.Next(0, Peasmod.crewmates.Count)];
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
                #region ThanosMode
                /*foreach(var impostorinfo in impostors)
                {
                    var impostor = impostorinfo.Object;
                    impostor.RpcSetRole(Role.Thanos);
                    ThanosMode.Thanos.Add(impostor);
                }*/
                #endregion ThanosMode
            }
            else if (Peasmod.Settings.gamemode.GetValue() == (int)Peasmod.Settings.GameMode.BattleRoyale)
            {
                Il2CppReferenceArray<GameData.PlayerInfo> _impostors = new GameData.PlayerInfo[PlayerControl.AllPlayerControls._size + 1];
                int i = 0;
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    player.Data.IsImpostor = true;
                    _impostors[i] = player.Data;
                    i++;
                }
                impostors = _impostors;
                //PlayerControl.LocalPlayer.RpcSetInfected(_impostors);
            }
        }
    }
}
