using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Hazel;
using Reactor.Extensions;
using UnhollowerBaseLib;
using Peasmod.Utility;
using Peasmod.Gamemodes;

namespace Peasmod.Patches
{
    class MeetingPatch
    {
        [HarmonyPatch(typeof(MeetingHud), "CCEPEINGBCN")]
        class VotePatch
        {
            public static bool Prefix(MeetingHud __instance, ref Il2CppStructArray<byte> __result)
            {
                byte[] numArray = new byte[13];
                for (int index1 = 0; index1 < __instance.playerStates.Length; ++index1)
                {
                    PlayerVoteArea playerState = __instance.playerStates[index1];
                    if (playerState.didVote)
                    {
                        int index2 = (int)playerState.votedFor + 1;
                        if (index2 >= 0 && index2 < numArray.Length)
                            ++numArray[index2];
                        if (Utils.GetPlayer((byte)playerState.TargetPlayerId).IsRole(Role.Mayor))
                            ++numArray[index2];
                    }
                }
                __result = numArray;
                return false;
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.OpenMeetingRoom))]
        public static class MeetingStartPatch
        {
            public static bool Prefix(HudManager __instance)
            {
                if(Peasmod.Settings.hotpotato.GetValue())
                {
                    return false;
                }
                #region MorphingMode
                if (PlayerControl.LocalPlayer.IsMorphed())
                    MorphingMode.OnLabelClick(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer, false);
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.MorphBack, Hazel.SendOption.None, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                #endregion MorphingMode
                return true;
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        public static class MeetingUpdatePatch
        {
            public static void Postfix(MeetingHud __instance)
            {
                #region JesterMode
                foreach (var pstate in __instance.playerStates)
                    if (pstate.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId && PlayerControl.LocalPlayer.IsRole(Role.Jester))
                        pstate.NameText.Color = JesterMode.JesterColor;
                #endregion JesterMode
                #region DoctorMode
                foreach (var pstate in __instance.playerStates)
                    if (pstate.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId && PlayerControl.LocalPlayer.IsRole(Role.Doctor))
                        pstate.NameText.Color = DoctorMode.DoctorColor;
                #endregion DoctorMode
                #region MayorMode
                foreach (var pstate in __instance.playerStates)
                    if (pstate.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId && PlayerControl.LocalPlayer.IsRole(Role.Mayor))
                        pstate.NameText.Color = MayorMode.MayorColor;
                #endregion MayorMode
                #region InspectorMode
                foreach (var pstate in __instance.playerStates)
                    if (pstate.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId && PlayerControl.LocalPlayer.IsRole(Role.Inspector))
                        pstate.NameText.Color = InspectorMode.InspectorColor;
                #endregion InspectorMode
                #region SheriffMode
                foreach (var pstate in __instance.playerStates)
                    if (pstate.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId && PlayerControl.LocalPlayer.IsRole(Role.Sheriff))
                        pstate.NameText.Color = SheriffMode.SheriffColor;
                #endregion SheriffMode
            }
        }
    }
}
