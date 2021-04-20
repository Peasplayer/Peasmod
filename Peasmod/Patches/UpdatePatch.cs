using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Timers;
using Hazel;
using Reactor.Extensions;
using Reactor;
using System.Reflection;
using System.IO;
using Reactor.Unstrip;
using Peasmod.Utility;
using Peasmod.Gamemodes;

namespace Peasmod.Patches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerPatch
    {

        private static float Timer = 0f;
        private static float Timer2 = 0f;
        private static float MaxTimer = 0.125f;
        private static Vector3 lastPosition;
        public static Dictionary<GameObject, float> dots = new Dictionary<GameObject, float>();

        public static void Postfix(HudManager __instance)
        {
            #region GameSettingsText
            if (__instance.GameSettings != null && !__instance.GameSettings.text.Contains("Peasplayer") && __instance.GameSettings.text.Contains("Peasmod"))
            {
                string[] settings = __instance.GameSettings.text.Split("\nSection");
                __instance.GameSettings.text = StringColor.Green + "Peasplayer" + StringColor.Reset + "\n" + settings[0];
                string mode = "\nGamemode: ";
                if (Peasmod.Settings.IsGameMode(Peasmod.Settings.GameMode.HotPotato))
                {
                    mode += "HotPotato";
                    __instance.GameSettings.text += mode;
                }
                else if (Peasmod.Settings.IsGameMode(Peasmod.Settings.GameMode.BattleRoyale))
                {
                    mode += "Battle Royale";
                    __instance.GameSettings.text += mode;
                }
                else
                {
                    string roles = "\nRoles: ";
                    if (Peasmod.Settings.JesterAmount.GetValue() > 0)
                    {
                        if (roles == "\nRoles: ")
                            roles += Peasmod.Settings.JesterAmount.GetValue() + " Jester";
                        else
                            roles += ", " + Peasmod.Settings.JesterAmount.GetValue() + " Jester"; ;
                    }
                    if (Peasmod.Settings.DoctorAmount.GetValue() > 0)
                    {
                        if (roles == "\nRoles: ")
                            roles += Peasmod.Settings.DoctorAmount.GetValue() + " Doctor";
                        else
                            roles += ", " + Peasmod.Settings.DoctorAmount.GetValue() + " Doctor"; ;
                    }
                    if (Peasmod.Settings.MayorAmount.GetValue() > 0)
                    {
                        if (roles == "\nRoles: ")
                            roles += Peasmod.Settings.MayorAmount.GetValue() + " Mayor";
                        else
                            roles += ", " + Peasmod.Settings.MayorAmount.GetValue() + " Mayor"; ;
                    }
                    if (Peasmod.Settings.InspectorAmount.GetValue() > 0)
                    {
                        if (roles == "\nRoles: ")
                            roles += Peasmod.Settings.InspectorAmount.GetValue() + " Inspector";
                        else
                            roles += ", " + Peasmod.Settings.InspectorAmount.GetValue() + " Inspector"; ;
                    }
                    if (Peasmod.Settings.SheriffAmount.GetValue() > 0)
                    {
                        if (roles == "\nRoles: ")
                            roles += Peasmod.Settings.SheriffAmount.GetValue() + " Sheriff";
                        else
                            roles += ", " + Peasmod.Settings.SheriffAmount.GetValue() + " Sheriff"; ;
                    }
                    __instance.GameSettings.text += roles;
                    string special = "\nSpecial: ";
                    var specials = 0;
                    if (Peasmod.Settings.CrewVenting.GetValue())
                    {
                        if (special == "\nSpecial: ")
                            special += "Crew-Venting";
                        else
                            special += ", Crew-Venting";
                        ++specials;
                    }
                    if (Peasmod.Settings.VentBuilding.GetValue())
                    {
                        if (special == "\nSpecial: ")
                            special += "Vent-Building";
                        else
                            special += ", Vent-Building";
                        ++specials;
                    }
                    if (Peasmod.Settings.BodyDragging.GetValue())
                    {
                        if (special == "\nSpecial: ")
                            special += "Body-Dragging";
                        else
                            special += ", Body-Dragging";
                        ++specials;
                    }
                    if (Peasmod.Settings.Invisibility.GetValue())
                    {
                        if (special == "\nSpecial: ")
                            special += "Invisibility";
                        else if (specials / 3 > 0)
                        {
                            special += "\n Invisibility";
                            specials -= specials / 3 * 3;
                        }
                        else
                            special += ", Invisibility";
                        ++specials;
                    }
                    if (Peasmod.Settings.FreezeTime.GetValue())
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
                    if (Peasmod.Settings.Morphing.GetValue())
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
                    __instance.GameSettings.text += special;
                }
            }
            #endregion GameSettingsText
            if (!PlayerControl.LocalPlayer) return;
            #region HotPotatoMode
            if (Peasmod.Settings.IsGameMode(Peasmod.Settings.GameMode.HotPotato))
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Data.IsImpostor)
                    {
                        player.nameText.color = HotPotatoMode.color;
                    } 
                    else
                    {
                        player.nameText.color = Palette.White;
                    }
                }
                HudManager.Instance.KillButton.gameObject.SetActive(false);
                if (HotPotatoMode.timer != null)
                {
                    if (!PlayerControl.LocalPlayer.Data.IsImpostor)
                        HotPotatoMode.timer.GetComponent<TextRenderer>().Text = string.Empty;
                    else
                    {
                        if (Timer2 < 0f)
                        {
                            HotPotatoMode.Timer -= 0.1f;
                            HotPotatoMode.timer.GetComponent<TextRenderer>().Text = "Time left till you die: " + StringColor.Red + $"{Math.Round(HotPotatoMode.Timer, 2)}";
                            Timer2 = 0.1f;
                        }
                        else
                            Timer2 -= Time.deltaTime;
                        if (HotPotatoMode.Timer <= 0f)
                        {
                            List<PlayerControl> players = new List<PlayerControl>();
                            foreach(var _player in PlayerControl.AllPlayerControls)
                            {
                                if (!_player.Data.IsDead && !_player.Data.IsImpostor)
                                    players.Add(_player);
                            }
                            var player = players[Peasmod.random.Next(0, players.Count)];
                            HotPotatoMode.TimeTillDeath -= 0.25f;
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.PotatoPassed, Hazel.SendOption.None, -1);
                            writer.Write(PlayerControl.LocalPlayer.PlayerId);
                            writer.Write(player.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            player.Data.IsImpostor = true;
                            PlayerControl.LocalPlayer.Die(DeathReason.Kill);
                            PlayerControl.LocalPlayer.Data.IsImpostor = false;
                            PlayerControl.LocalPlayer.Collider.enabled = false;
                            HudManager.Instance.ShadowQuad.gameObject.SetActive(false);
                            var potato = new GameObject("potato");
                            var render = potato.AddComponent<SpriteRenderer>();
                            render.sprite = Utils.CreateSprite("Peasmod.Resources.Potato.png");
                            var scale = new Vector3(potato.transform.localScale.x + 2f, potato.transform.localScale.y + 2f);
                            potato.transform.localScale = scale;
                            potato.transform.position = PlayerControl.LocalPlayer.GetTruePosition();
                            writer = AmongUsClient.Instance.StartRpcImmediately(player.NetId, (byte)CustomRpc.PotatoDies, Hazel.SendOption.None, -1);
                            writer.Write(PlayerControl.LocalPlayer.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                        }
                        if(Utils.GetClosestPlayer(PlayerControl.LocalPlayer) != null && Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, Utils.GetClosestPlayer(PlayerControl.LocalPlayer)) <= GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance])
                        {
                            HotPotatoMode.CurrentTarget = Utils.GetClosestPlayer(PlayerControl.LocalPlayer);
                            HotPotatoMode.button.enabled = true;
                            HotPotatoMode.button.killButtonManager.renderer.sprite = Utils.CreateSprite("Peasmod.Resources.Kill.png");
                        }
                        else
                        {
                            HotPotatoMode.CurrentTarget = null;
                            HotPotatoMode.button.enabled = false;
                            HotPotatoMode.button.killButtonManager.renderer.color = Palette.DisabledClear;
                        }
                    }
                }
            }
            #endregion HotPotatoMode
            #region BattleRoyaleMode
            else if (Peasmod.Settings.IsGameMode(Peasmod.Settings.GameMode.BattleRoyale))
            {
                if(Peasmod.GameStarted)
                {
                    //if (BattleRoyaleMode.button != null)
                    //    BattleRoyaleMode.button.killButtonManager.transform.position = HudManager.Instance.KillButton.transform.position;
                    if (PlayerControl.LocalPlayer.Data.IsDead)
                        HudManager.Instance.KillButton.gameObject.SetActive(false);
                    else
                    {
                        if (PlayerControl.LocalPlayer.FindClosestTarget(GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance]) != null)
                        {
                            //BattleRoyaleMode.CurrentTarget = Utils.GetClosestPlayer(PlayerControl.LocalPlayer);
                            var target = PlayerControl.LocalPlayer.FindClosestTarget(
                                GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance]);
                            HudManager.Instance.KillButton.enabled = true;
                            HudManager.Instance.KillButton.renderer.color = Palette.EnabledColor;
                            HudManager.Instance.KillButton.renderer.material.SetFloat("_Desat", 0.0f);
                            HudManager.Instance.KillButton.SetTarget(target);
                        }
                        else
                        {
                            //BattleRoyaleMode.CurrentTarget = null;
                            HudManager.Instance.KillButton.enabled = false;
                            HudManager.Instance.KillButton.renderer.color = Palette.DisabledClear;
                            HudManager.Instance.KillButton.SetTarget(null);
                        }
                    }
                    /*PlayerControl target = Utils.GetClosestPlayer(PlayerControl.LocalPlayer);
                    if (Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, target) > GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance])
                        target = null;
                    if (target == null)
                    {
                        BattleRoyaleMode.button.killButtonManager.renderer.color = Palette.DisabledColor;
                        if (BattleRoyaleMode.CurrentTarget != null)
                        {
                            SpriteRenderer renderer = BattleRoyaleMode.CurrentTarget.myRend;
                            renderer.material.SetFloat("_Outline", 0f);
                        }
                        BattleRoyaleMode.CurrentTarget = target;
                        BattleRoyaleMode.button.enabled = false;
                    }
                    else
                    {
                        SpriteRenderer renderer = target.myRend;
                        renderer.material.SetFloat("_Outline", 1f);
                        renderer.material.SetColor("_OutlineColor", Color.red);
                        if (BattleRoyaleMode.CurrentTarget != null)
                        {
                            renderer = BattleRoyaleMode.CurrentTarget.myRend;
                            renderer.material.SetFloat("_Outline", 1f);
                            renderer.material.SetColor("_OutlineColor", Color.red);
                        }
                        BattleRoyaleMode.button.killButtonManager.renderer.color = Palette.EnabledColor;
                        BattleRoyaleMode.CurrentTarget = target;
                        BattleRoyaleMode.button.enabled = true;
                    }
                    BattleRoyaleMode.button.killButtonManager.renderer.color = Palette.DisabledGrey;*/
                    if (BattleRoyaleMode.HasKilled)
                        HudManager.Instance.KillButton.SetCoolDown(0, 1);
                    foreach(var player in PlayerControl.AllPlayerControls)
                    {
                        if (player.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                            player.nameText.color = Palette.ImpostorRed;
                        else
                            player.nameText.color = Palette.White;
                    }
                }
            }
            #endregion BattleRoyaleMode
            #region JesterMode
            if (PlayerControl.LocalPlayer.IsRole(Role.Jester))
                PlayerControl.LocalPlayer.nameText.color = JesterMode.JesterColor;
            #endregion JesterMode
            #region DoctorMode
            if (PlayerControl.LocalPlayer.IsRole(Role.Doctor))
                PlayerControl.LocalPlayer.nameText.color = DoctorMode.DoctorColor;
            #endregion JesterMode
            #region MayorMode
            if (PlayerControl.LocalPlayer.IsRole(Role.Mayor))
                PlayerControl.LocalPlayer.nameText.color = MayorMode.MayorColor;
            #endregion MayorMode
            #region InspectorMode
            if (PlayerControl.LocalPlayer.IsRole(Role.Inspector))
                PlayerControl.LocalPlayer.nameText.color = InspectorMode.InspectorColor;
            #endregion InspectorMode
            #region SheriffMode
            if (PlayerControl.LocalPlayer.IsRole(Role.Sheriff))
                PlayerControl.LocalPlayer.nameText.color = SheriffMode.SheriffColor;
            #endregion SheriffMode
            #region VentBuilding
            CooldownButton.HudUpdate();
            if(Peasmod.Settings.VentBuilding.GetValue())
            {
                if (VentBuilding.button != null)
                {
                    if (VentBuilding.button.killButtonManager.renderer != null)
                    {
                        var hits = Physics2D.OverlapBoxAll(PlayerControl.LocalPlayer.transform.position, VentBuilding.VentSize, 0);
                        hits = hits.ToArray().Where((c) => (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer != 8 && c.gameObject.layer != 5).ToArray();
                        if (hits.Count() != 0)
                        {
                            VentBuilding.button.killButtonManager.renderer.color = Palette.DisabledClear;
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
                            BodyDragging.button.killButtonManager.renderer.color = Palette.DisabledClear;
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
                        DoctorMode.button.killButtonManager.renderer.color = Palette.DisabledClear;
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
                if (SheriffMode.button.killButtonManager.renderer != null && PlayerControl.LocalPlayer.IsRole(Role.Sheriff))
                {
                    if (PlayerControl.LocalPlayer.Data.IsDead)
                    {
                        SheriffMode.button.enabled = false;
                    }
                    else
                    {
                        PlayerControl target = Utils.GetClosestPlayer(PlayerControl.LocalPlayer);
                        if (Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, target) > GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance])
                            target = null;
                        if (target == null)
                        {
                            SheriffMode.button.killButtonManager.renderer.color = Palette.DisabledClear;
                            if (SheriffMode.CurrentTarget != null)
                            {
                                SpriteRenderer renderer = SheriffMode.CurrentTarget.myRend;
                                renderer.material.SetFloat("_Outline", 0f);
                            }
                            SheriffMode.CurrentTarget = target;
                            SheriffMode.button.enabled = false;
                        }
                        else
                        {
                            SpriteRenderer renderer = target.myRend;
                            renderer.material.SetFloat("_Outline", 1f);
                            renderer.material.SetColor("_OutlineColor", Color.red);
                            if (SheriffMode.CurrentTarget != null)
                            {
                                renderer = SheriffMode.CurrentTarget.myRend;
                                renderer.material.SetFloat("_Outline", 1f);
                                renderer.material.SetColor("_OutlineColor", Color.red);
                            }
                            SheriffMode.button.killButtonManager.renderer.color = Palette.EnabledColor;
                            SheriffMode.CurrentTarget = target;
                            SheriffMode.button.enabled = true;
                        }
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
                        player.myRend.color = Palette.DisabledClear;
                        player.HatRenderer.color = Palette.DisabledClear;
                        player.MyPhysics.Skin.layer.color = Palette.DisabledClear;
                    }
                    else
                    {
                        player.Visible = false;
                    }
                }
            }
            #endregion InvisibilityMode
            #region ThanosMode
            if (PlayerControl.LocalPlayer.IsRole(Role.Thanos))
                PlayerControl.LocalPlayer.nameText.color = ThanosMode.ThanosColor;
            #endregion ThanosMode
            #region TimeFreezing
            if (TimeFreezing.timeIsFroozen && !PlayerControl.LocalPlayer.Data.IsImpostor)
            {
                if(!PlayerControl.LocalPlayer.Data.IsDead)
                {
                    PlayerControl.LocalPlayer.moveable = false;
                    if (Minigame.Instance != null)
                        Minigame.Instance.ForceClose();
                    PlayerControl.LocalPlayer.MyPhysics.ResetAnimState();
                    PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();
                    PlayerControl.LocalPlayer.MyPhysics.body.velocity = Vector2.zero;
                }
            }
            #endregion TimeFreezing
            if (Peasmod.GameStarted)
            {
                #region InspectorMode
                if (Timer < 0f)
                {
                    if(lastPosition == null && !PlayerControl.LocalPlayer.Data.IsDead && !PlayerControl.LocalPlayer.inVent)
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
                if(!Peasmod.Settings.ReportBodys.GetValue() || Peasmod.Settings.IsGameMode(Peasmod.Settings.GameMode.BattleRoyale))
                {
                    HudManager.Instance.ReportButton.gameObject.SetActive(false);
                }
            }
            else
            {
                TimeFreezing.timeIsFroozen = false;
                //if (!PlayerControl.LocalPlayer.moveable)
                //    PlayerControl.LocalPlayer.moveable = true;
                //MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.UnfreezeTime, Hazel.SendOption.None, -1);
                //AmongUsClient.Instance.FinishRpcImmediately(writer);
                var notinvisplayers = new List<Byte>();
                foreach (var player in InvisibilityMode.invisplayers)
                {
                    if(Utils.GetPlayer(player) != null)
                    {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.Visible, Hazel.SendOption.None, -1);
                        writer.WritePacked(player);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        var visible = Utils.GetPlayer(player);
                        visible.Visible = true;
                        notinvisplayers.Add(player);
                    }
                }
                foreach(var id in notinvisplayers)
                {
                    InvisibilityMode.invisplayers.Remove(id);
                }
                PlayerControl.LocalPlayer.myRend.color = Palette.EnabledColor;
                PlayerControl.LocalPlayer.HatRenderer.color = Palette.EnabledColor;
                PlayerControl.LocalPlayer.MyPhysics.Skin.layer.color = Palette.EnabledColor;
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    class PlayerUpdate
    {
        static void Prefix(PlayerControl __instance)
        {
            #region BattleRoyle
            if (Peasmod.Settings.IsGameMode(Peasmod.Settings.GameMode.BattleRoyale) && Peasmod.GameStarted)
            {
                __instance.Data.IsImpostor = !__instance.Data.IsDead;
            } 
            #endregion BattleRoyale
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
            if(Peasmod.Settings.BodyDragging.GetValue())
            {
                if (BodyDragging.draggers.Contains(__instance.PlayerId))
                {
                    BodyDragging.MoveBody(__instance, BodyDragging.bodys[BodyDragging.draggers.IndexOf(__instance.PlayerId)]);
                }
            }
            #endregion BodyDragging
        }
    }

    [HarmonyPatch(typeof(UseButtonManager), nameof(UseButtonManager.SetTarget))]
    class SabotagePatch
    {
        static void Postfix(UseButtonManager __instance)
        {
            if ((!Peasmod.Settings.Sabotaging.GetValue() || Peasmod.Settings.IsGameMode(Peasmod.Settings.GameMode.BattleRoyale)) && Peasmod.GameStarted)
            {
                if (__instance.currentTarget == null)
                {
                    //__instance.UseButton.sprite = 
                    __instance.UseButton.color = UseButtonManager.DisabledColor;
                    __instance.enabled = false;
                }
                else
                {
                    __instance.UseButton.color = UseButtonManager.EnabledColor;
                    __instance.enabled = true;
                }
            }
        }
    }
}
