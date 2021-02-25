using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using System.Linq;
using UnityEngine;
using System.Timers;
using Hazel;
using Reactor.Extensions;
using Reactor;
using System.Reflection;
using System.IO;
using Reactor.Unstrip;

namespace Peasmod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerPatch
    {

        private static float Timer = 0f;
        private static float MaxTimer = 0.125f;
        private static Vector3 lastPosition;
        public static Dictionary<GameObject, float> dots = new Dictionary<GameObject, float>();

        public static void Postfix(HudManager __instance)
        {
            #region GameSettingsText
            if (__instance.GameSettings != null && !__instance.GameSettings.Text.Contains("Peasplayer") && __instance.GameSettings.Text.Contains("Peasmod"))
            {
                string[] settings = __instance.GameSettings.Text.Split("\nSection");
                __instance.GameSettings.Text = StringColor.Green + "Peasplayer" + StringColor.Lime + " feat. DrSchwammkopf" + StringColor.Reset + "\n" + settings[0];
                string roles = "\nRoles: ";
                if (Peasmod.Settings.jesteramount.GetValue() > 0)
                {
                    if (roles == "\nRoles: ")
                        roles += Peasmod.Settings.jesteramount.GetValue() + " Jester";
                    else
                        roles += ", " + Peasmod.Settings.jesteramount.GetValue() + " Jester"; ;
                }
                if (Peasmod.Settings.doctoramount.GetValue() > 0)
                {
                    if (roles == "\nRoles: ")
                        roles += Peasmod.Settings.doctoramount.GetValue() + " Doctor";
                    else
                        roles += ", " + Peasmod.Settings.doctoramount.GetValue() + " Doctor"; ;
                }
                if (Peasmod.Settings.mayoramount.GetValue() > 0)
                {
                    if (roles == "\nRoles: ")
                        roles += Peasmod.Settings.mayoramount.GetValue() + " Mayor";
                    else
                        roles += ", " + Peasmod.Settings.mayoramount.GetValue() + " Mayor"; ;
                }
                if (Peasmod.Settings.inspectoramount.GetValue() > 0)
                {
                    if (roles == "\nRoles: ")
                        roles += Peasmod.Settings.inspectoramount.GetValue() + " Inspector";
                    else
                        roles += ", " + Peasmod.Settings.inspectoramount.GetValue() + " Inspector"; ;
                }
                if (Peasmod.Settings.sheriffamount.GetValue() > 0)
                {
                    if (roles == "\nRoles: ")
                        roles += Peasmod.Settings.sheriffamount.GetValue() + " Sheriff";
                    else
                        roles += ", " + Peasmod.Settings.sheriffamount.GetValue() + " Sheriff"; ;
                }
                __instance.GameSettings.Text += roles;
                string special = "\nSpecial: ";
                var specials = 0;
                if (Peasmod.Settings.crewventing.GetValue())
                {
                    if(special == "\nSpecial: ")
                        special += "Crew-Venting";
                    else
                        special += ", Crew-Venting";
                    ++specials;
                }
                if (Peasmod.Settings.ventbuilding.GetValue())
                {
                    if (special == "\nSpecial: ")
                        special += "Vent-Building";
                    else
                        special += ", Vent-Building";
                    ++specials;
                }
                if (Peasmod.Settings.bodydragging.GetValue())
                {
                    if (special == "\nSpecial: ")
                        special += "Body-Dragging";
                    else
                        special += ", Body-Dragging";
                    ++specials;
                }
                if (Peasmod.Settings.invisibility.GetValue())
                {
                    if (special == "\nSpecial: ")
                        special += "Invisibility";
                    else if(specials/3 > 0)
                    {
                        special += "\n Invisibility";
                        specials -= specials / 3 * 3;
                    }
                    else
                        special += ", Invisibility";
                    ++specials;
                }
                if (Peasmod.Settings.freezetime.GetValue())
                {
                    if (special == "\nSpecial: ")
                        special += "Time-Freezing";
                    else if (specials / 3 > 0)
                    {
                        special += "\n Time-Freezing";
                        specials -= specials / 3 * 3;
                    }
                    else
                        special += ", Time-Freezing";
                    ++specials;
                }
                if (Peasmod.Settings.morphing.GetValue())
                {
                    if (special == "\nSpecial: ")
                        special += "Morphing";
                    else if (specials / 3 > 0)
                    {
                        special += "\n Morphing";
                        specials -= specials / 3 * 3;
                    }
                    else
                        special += ", Morphing";
                    ++specials;
                }
                __instance.GameSettings.Text += special;
            }
            #endregion GameSettingsText
            if (!PlayerControl.LocalPlayer) return;
            #region JesterMode
            if (JesterMode.Jester1 != null)
                if (PlayerControl.LocalPlayer.PlayerId == JesterMode.Jester1.PlayerId)
                    PlayerControl.LocalPlayer.nameText.Color = JesterMode.JesterColor;
            if (JesterMode.Jester2 != null)
                if (PlayerControl.LocalPlayer.PlayerId == JesterMode.Jester2.PlayerId)
                    PlayerControl.LocalPlayer.nameText.Color = JesterMode.JesterColor;
            #endregion JesterMode
            #region DoctorMode
            if (DoctorMode.Doctor1 != null)
                if (PlayerControl.LocalPlayer.PlayerId == DoctorMode.Doctor1.PlayerId)
                    PlayerControl.LocalPlayer.nameText.Color = DoctorMode.DoctorColor;
            if (DoctorMode.Doctor2 != null)
                if (PlayerControl.LocalPlayer.PlayerId == DoctorMode.Doctor2.PlayerId)
                    PlayerControl.LocalPlayer.nameText.Color = DoctorMode.DoctorColor;
            #endregion JesterMode
            #region MayorMode
            if (MayorMode.Mayor1 != null)
                if (PlayerControl.LocalPlayer.PlayerId == MayorMode.Mayor1.PlayerId)
                    PlayerControl.LocalPlayer.nameText.Color = MayorMode.MayorColor;
            if (MayorMode.Mayor2 != null)
                if (PlayerControl.LocalPlayer.PlayerId == MayorMode.Mayor2.PlayerId)
                    PlayerControl.LocalPlayer.nameText.Color = MayorMode.MayorColor;
            #endregion MayorMode
            #region InspectorMode
            if (InspectorMode.Inspector1 != null)
                if (PlayerControl.LocalPlayer.PlayerId == InspectorMode.Inspector1.PlayerId)
                    PlayerControl.LocalPlayer.nameText.Color = InspectorMode.InspectorColor;
            if (InspectorMode.Inspector2 != null)
                if (PlayerControl.LocalPlayer.PlayerId == InspectorMode.Inspector2.PlayerId)
                    PlayerControl.LocalPlayer.nameText.Color = InspectorMode.InspectorColor;
            #endregion InspectorMode
            #region SheriffMode
            if (SheriffMode.Sheriff1 != null)
                if (PlayerControl.LocalPlayer.PlayerId == SheriffMode.Sheriff1.PlayerId)
                    PlayerControl.LocalPlayer.nameText.Color = SheriffMode.SheriffColor;
            if (SheriffMode.Sheriff2 != null)
                if (PlayerControl.LocalPlayer.PlayerId == SheriffMode.Sheriff2.PlayerId)
                    PlayerControl.LocalPlayer.nameText.Color = SheriffMode.SheriffColor;
            #endregion SheriffMode
            #region VentBuilding
            CooldownButton.HudUpdate();
            if(Peasmod.Settings.ventbuilding.GetValue())
            {
                if (VentBuilding.button != null)
                {
                    if (VentBuilding.button.killButtonManager.renderer != null)
                    {
                        var hits = Physics2D.OverlapBoxAll(PlayerControl.LocalPlayer.transform.position, VentBuilding.VentSize, 0);
                        hits = hits.ToArray().Where((c) => (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer != 8 && c.gameObject.layer != 5).ToArray();
                        if (hits.Count() != 0)
                        {
                            VentBuilding.button.killButtonManager.renderer.color = Palette.DisabledColor;
                            VentBuilding.button.enabled = false;
                        }
                        else
                        {
                            VentBuilding.button.killButtonManager.renderer.color = Palette.EnabledColor;
                            VentBuilding.button.enabled = true;
                        }
                    }
                }
            }
            #endregion VentBuilding
            #region BodyDragging
            if (BodyDragging.button != null)
            {
                if (BodyDragging.button.killButtonManager.renderer != null)
                {
                    if (BodyDragging.draggers.Contains(PlayerControl.LocalPlayer.PlayerId))
                    {
                        BodyDragging.button.killButtonManager.renderer.color = Palette.EnabledColor;
                        BodyDragging.button.enabled = true;
                    }
                    else
                    {
                        List<DeadBody> bodys = new List<DeadBody>();
                        foreach (Collider2D collider2D in Physics2D.OverlapCircleAll(PlayerControl.LocalPlayer.GetTruePosition(), PlayerControl.LocalPlayer.MaxReportDistance - 2f, Constants.PlayersOnlyMask))
                        {
                            if (collider2D.tag == "DeadBody")
                            {
                                DeadBody body = (DeadBody)((Component)collider2D).GetComponent<DeadBody>();
                                bodys.Add(body);
                            }
                        }
                        if (bodys.Count == 0)
                        {
                            BodyDragging.button.killButtonManager.renderer.color = Palette.DisabledColor;
                            BodyDragging.button.enabled = false;
                        }
                        else
                        {
                            BodyDragging.button.killButtonManager.renderer.color = Palette.EnabledColor;
                            BodyDragging.button.enabled = true;
                        }
                    }
                }
            }
            #endregion BodyDragging
            #region DoctorMode
            if (DoctorMode.button != null)
            {
                if (DoctorMode.button.killButtonManager.renderer != null)
                {
                    List<DeadBody> bodys = new List<DeadBody>();
                    foreach (Collider2D collider2D in Physics2D.OverlapCircleAll(PlayerControl.LocalPlayer.GetTruePosition(), PlayerControl.LocalPlayer.MaxReportDistance - 2f, Constants.PlayersOnlyMask))
                    {
                        if (collider2D.tag == "DeadBody")
                        {
                            DeadBody body = (DeadBody)((Component)collider2D).GetComponent<DeadBody>();
                            bodys.Add(body);
                        }
                    }
                    if (bodys.Count == 0)
                    {
                        DoctorMode.button.killButtonManager.renderer.color = Palette.DisabledColor;
                        DoctorMode.button.enabled = false;
                    }
                    else
                    {
                        DoctorMode.button.killButtonManager.renderer.color = Palette.EnabledColor;
                        DoctorMode.button.enabled = true;
                    }
                }
            }
            #endregion DoctorMode
            #region SheriffMode
            if (SheriffMode.button != null)
            {
                if (SheriffMode.button.killButtonManager.renderer != null && (PlayerControl.LocalPlayer == SheriffMode.Sheriff1 || PlayerControl.LocalPlayer == SheriffMode.Sheriff2) && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    PlayerControl target = Utils.GetClosestPlayer(PlayerControl.LocalPlayer);
                    if (Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, target) > GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance])
                        target = null;
                    if (target == null)
                    {
                        SheriffMode.button.killButtonManager.renderer.color = Palette.DisabledColor;
                        SheriffMode.button.killButtonManager.SetTarget(target);
                        SheriffMode.CurrentTarget = target;
                        SheriffMode.button.enabled = false;
                    }
                    else
                    {
                        SheriffMode.button.killButtonManager.renderer.color = Palette.EnabledColor;
                        SheriffMode.button.killButtonManager.SetTarget(target);
                        SheriffMode.CurrentTarget = target;
                        SheriffMode.button.enabled = true;
                    }
                }
            }
            #endregion SheriffMode
            #region InvisibilityMode
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (InvisibilityMode.invisplayers.Contains(player.PlayerId))
                {
                    if (PlayerControl.LocalPlayer.PlayerId == player.PlayerId || PlayerControl.LocalPlayer.Data.IsImpostor)
                    {
                        player.myRend.color = Palette.DisabledColor;
                        player.HatRenderer.color = Palette.DisabledColor;
                        player.MyPhysics.Skin.layer.color = Palette.DisabledColor;
                    }
                    else
                    {
                        player.Visible = false;
                    }
                }
            }
            #endregion InvisibilityMode
            #region TimeFreezing
            if (TimeFreezing.timeIsFroozen && !PlayerControl.LocalPlayer.Data.IsImpostor)
            {
                if(!PlayerControl.LocalPlayer.Data.IsDead)
                {
                    PlayerControl.LocalPlayer.moveable = false;
                    if (Minigame.Instance != null)
                        Minigame.Instance.ForceClose();
                    PlayerControl.LocalPlayer.MyPhysics.ResetAnim();
                    PlayerControl.LocalPlayer.MyPhysics.body.velocity = Vector2.zero;
                }
            }
            #endregion TimeFreezing
            if (ShipstatusOnEnablePatch.gameStarted)
            {
                #region InspectorMode
                if (Timer < 0f)
                {
                    if(lastPosition == null && !PlayerControl.LocalPlayer.Data.IsDead)
                    {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.CreateDot, Hazel.SendOption.None, -1);
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        lastPosition = PlayerControl.LocalPlayer.GetTruePosition();
                    }
                    else
                    {
                        if (!(lastPosition.x == PlayerControl.LocalPlayer.GetTruePosition().x && lastPosition.y == PlayerControl.LocalPlayer.GetTruePosition().y) && !PlayerControl.LocalPlayer.Data.IsDead)
                        {
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.CreateDot, Hazel.SendOption.None, -1);
                            writer.Write(PlayerControl.LocalPlayer.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            lastPosition = PlayerControl.LocalPlayer.GetTruePosition();
                        }
                    }
                    Timer = MaxTimer;
                }
                else
                    Timer -= Time.deltaTime;
                for (int i = 0; i < dots.Count; i++)
                {
                    var _dot = dots.ElementAt(i).Key;
                    var time = dots.ElementAt(i).Value;
                    if (time + 2f <= Time.time)
                    {
                        Color c = _dot.GetComponent<SpriteRenderer>().material.color;
                        c.a -= 0.2f;
                        _dot.GetComponent<SpriteRenderer>().material.color = c;
                        if (c.a <= 0f)
                            dots.Remove(_dot);
                        else
                        {
                            time += 2f;
                            dots.Remove(_dot);
                            dots.Add(_dot, time);
                        }
                    }
                }
                #endregion InspectorMode
                #region MorphingMode
                if (MorphingMode.menuOpen)
                {
                    MorphingMode.button.killButtonManager.renderer.enabled = false;
                    MorphingMode.button.killButtonManager.SetCoolDown(0, 0);
                    MorphingMode.button.killButtonManager.enabled = false;
                    MorphingMode.button.killButtonManager.gameObject.active = false;
                }
                #endregion MorphingMode
            }
            else
            {
                TimeFreezing.timeIsFroozen = false;
                foreach(var player in PlayerControl.AllPlayerControls)
                    player.Visible = true;
            }
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.OpenMeetingRoom))]
    public static class MeetingStartPatch
    {
        public static void Prefix(HudManager __instance)
        {
            #region MorphingMode
            foreach (var player in PlayerControl.AllPlayerControls)
                MorphingMode.OnLabelClick(player, player, false);
            #endregion MorphingMode
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
    public static class MeetingUpdatePatch
    {
        public static void Postfix(MeetingHud __instance)
        {
            #region JesterMode
            if (JesterMode.Jester1 != null)
                if (PlayerControl.LocalPlayer.PlayerId == JesterMode.Jester1.PlayerId)
                    foreach (var pstate in __instance.playerStates)
                        if (pstate.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId &&
                        PlayerControl.LocalPlayer.PlayerId == JesterMode.Jester1.PlayerId)
                            pstate.NameText.Color = JesterMode.JesterColor;
            if (JesterMode.Jester2 != null)
                if (PlayerControl.LocalPlayer.PlayerId == JesterMode.Jester2.PlayerId)
                    foreach (var pstate in __instance.playerStates)
                        if (pstate.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId &&
                        PlayerControl.LocalPlayer.PlayerId == JesterMode.Jester2.PlayerId)
                            pstate.NameText.Color = JesterMode.JesterColor;
            #endregion JesterMode
            #region DoctorMode
            if (DoctorMode.Doctor1 != null)
                if (PlayerControl.LocalPlayer.PlayerId == DoctorMode.Doctor1.PlayerId)
                    foreach (var pstate in __instance.playerStates)
                        if (pstate.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId &&
                        PlayerControl.LocalPlayer.PlayerId == DoctorMode.Doctor1.PlayerId)
                            pstate.NameText.Color = DoctorMode.DoctorColor;
            if (DoctorMode.Doctor2 != null)
                if (PlayerControl.LocalPlayer.PlayerId == DoctorMode.Doctor2.PlayerId)
                    foreach (var pstate in __instance.playerStates)
                        if (pstate.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId &&
                        PlayerControl.LocalPlayer.PlayerId == DoctorMode.Doctor2.PlayerId)
                            pstate.NameText.Color = DoctorMode.DoctorColor;
            #endregion DoctorMode
            #region MayorMode
            if (MayorMode.Mayor1 != null)
                if (PlayerControl.LocalPlayer.PlayerId == MayorMode.Mayor1.PlayerId)
                    foreach (var pstate in __instance.playerStates)
                        if (pstate.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId &&
                        PlayerControl.LocalPlayer.PlayerId == MayorMode.Mayor1.PlayerId)
                            pstate.NameText.Color = MayorMode.MayorColor;
            if (MayorMode.Mayor2 != null)
                if (PlayerControl.LocalPlayer.PlayerId == MayorMode.Mayor2.PlayerId)
                    foreach (var pstate in __instance.playerStates)
                        if (pstate.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId &&
                        PlayerControl.LocalPlayer.PlayerId == MayorMode.Mayor2.PlayerId)
                            pstate.NameText.Color = MayorMode.MayorColor;
            #endregion MayorMode
            #region InspectorMode
            if (InspectorMode.Inspector1 != null)
                if (PlayerControl.LocalPlayer.PlayerId == InspectorMode.Inspector1.PlayerId)
                    foreach (var pstate in __instance.playerStates)
                        if (pstate.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId &&
                        PlayerControl.LocalPlayer.PlayerId == InspectorMode.Inspector1.PlayerId)
                            pstate.NameText.Color = InspectorMode.InspectorColor;
            if (InspectorMode.Inspector2 != null)
                if (PlayerControl.LocalPlayer.PlayerId == InspectorMode.Inspector2.PlayerId)
                    foreach (var pstate in __instance.playerStates)
                        if (pstate.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId &&
                        PlayerControl.LocalPlayer.PlayerId == InspectorMode.Inspector2.PlayerId)
                            pstate.NameText.Color = InspectorMode.InspectorColor;
            #endregion InspectorMode
            #region SheriffMode
            if (SheriffMode.Sheriff1 != null)
                if (PlayerControl.LocalPlayer.PlayerId == SheriffMode.Sheriff1.PlayerId)
                    foreach (var pstate in __instance.playerStates)
                        if (pstate.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId &&
                        PlayerControl.LocalPlayer.PlayerId == SheriffMode.Sheriff1.PlayerId)
                            pstate.NameText.Color = SheriffMode.SheriffColor;
            if (SheriffMode.Sheriff2 != null)
                if (PlayerControl.LocalPlayer.PlayerId == SheriffMode.Sheriff2.PlayerId)
                    foreach (var pstate in __instance.playerStates)
                        if (pstate.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId &&
                        PlayerControl.LocalPlayer.PlayerId == SheriffMode.Sheriff2.PlayerId)
                            pstate.NameText.Color = SheriffMode.SheriffColor;
            #endregion SheriffMode
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    class PlayerUpdate
    {
        static void Prefix(PlayerControl __instance)
        {
            #region VentBuilding
            if (__instance.AmOwner && VentBuilding.button != null && VentBuilding.button.killButtonManager != null)
            {
                if (PlayerControl.LocalPlayer.Data.IsImpostor && !PlayerControl.LocalPlayer.Data.IsDead)
                    VentBuilding.button.killButtonManager.gameObject.SetActive(true);
                else
                    VentBuilding.button.killButtonManager.gameObject.SetActive(false);
            }
            #endregion VentBuilding
            #region BodyDragging
            if(Peasmod.Settings.bodydragging.GetValue())
            {
                if (BodyDragging.draggers.Contains(__instance.PlayerId))
                {
                    BodyDragging.MoveBody(__instance, BodyDragging.bodys[BodyDragging.draggers.IndexOf(__instance.PlayerId)]);
                }
            }
            #endregion BodyDragging
        }
    }
}
