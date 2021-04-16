using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Hazel;
using Reactor.Extensions;
using UnhollowerBaseLib;
using Peasmod.Utility;
using Peasmod.Gamemodes;
using System.Linq;

namespace Peasmod.Patches
{
    class MeetingPatch
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
        public static class VotePatch
        {
            public static bool Prefix(MeetingHud __instance, [HarmonyArgument(0)] sbyte suspect)
            {
                if (PlayerControl.LocalPlayer.IsRole(Role.Mayor) && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    __instance.CmdCastVote(PlayerControl.LocalPlayer.PlayerId, suspect);
                    return false;
                }
                return true;
            }

            /*[HarmonyPriority(Priority.First)]
            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.IsRole(Role.Mayor)) return;
                var role = Roles.Role.GetRole<Mayor>(PlayerControl.LocalPlayer);
                if (role.VoteBank > 0 && !role.SelfVote)
                {
                    __instance.SkipVoteButton.gameObject.SetActive(true);
                }
            }*/
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.OpenMeetingRoom))]
        public static class MeetingStartPatch
        {
            public static bool Prefix(HudManager __instance)
            {
                if(Peasmod.Settings.gamemode.GetValue() == (int)Peasmod.Settings.GameMode.HotPotato || Peasmod.Settings.gamemode.GetValue() == (int)Peasmod.Settings.GameMode.BattleRoyale)
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

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        public static class StartPatch
        {
            public static void Prefix(MeetingHud __instance)
            {
                if (DoctorMode.button != null)
                {
                    DoctorMode.button.Visibile = false;
                }
                if (SheriffMode.button != null)
                {
                    SheriffMode.button.Visibile = false;
                }
                if (BodyDragging.button != null)
                {
                    BodyDragging.button.Visibile = false;
                }
                if (InvisibilityMode.button != null)
                {
                    InvisibilityMode.button.Visibile = false;
                }
                if (MorphingMode.button != null)
                {
                    MorphingMode.button.Visibile = false;
                }
                if (TimeFreezing.button != null)
                {
                    TimeFreezing.button.Visibile = false;
                }
                if (VentBuilding.button != null)
                {
                    VentBuilding.button.Visibile = false;
                }
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
                        pstate.NameText.color = JesterMode.JesterColor;
                #endregion JesterMode
                #region DoctorMode
                foreach (var pstate in __instance.playerStates)
                    if (pstate.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId && PlayerControl.LocalPlayer.IsRole(Role.Doctor))
                        pstate.NameText.color = DoctorMode.DoctorColor;
                #endregion DoctorMode
                #region MayorMode
                foreach (var pstate in __instance.playerStates)
                    if (pstate.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId && PlayerControl.LocalPlayer.IsRole(Role.Mayor))
                        pstate.NameText.color = MayorMode.MayorColor;
                #endregion MayorMode
                #region InspectorMode
                foreach (var pstate in __instance.playerStates)
                    if (pstate.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId && PlayerControl.LocalPlayer.IsRole(Role.Inspector))
                        pstate.NameText.color = InspectorMode.InspectorColor;
                #endregion InspectorMode
                #region SheriffMode
                foreach (var pstate in __instance.playerStates)
                    if (pstate.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId && PlayerControl.LocalPlayer.IsRole(Role.Sheriff))
                        pstate.NameText.color = SheriffMode.SheriffColor;
                #endregion SheriffMode
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Close))]
        public static class ClosePatch
        {
            public static void Prefix(MeetingHud __instance)
            {
                if (DoctorMode.button != null)
                {
                    DoctorMode.button.Visibile = true;
                }
                if (SheriffMode.button != null)
                {
                    SheriffMode.button.Visibile = true;
                }
                if (BodyDragging.button != null)
                {
                    BodyDragging.button.Visibile = true;
                }
                if (InvisibilityMode.button != null)
                {
                    InvisibilityMode.button.Visibile = true;
                }
                if (MorphingMode.button != null)
                {
                    MorphingMode.button.Visibile = true;
                }
                if (TimeFreezing.button != null)
                {
                    TimeFreezing.button.Visibile = true;
                }
                if (VentBuilding.button != null)
                {
                    VentBuilding.button.Visibile = true;
                }
            }
        }
    }
}
