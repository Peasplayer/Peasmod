using System;
using System.Collections.Generic;
using System.Text;
using UnhollowerBaseLib;
using HarmonyLib;
using Hazel;
using Reactor.Extensions;
using UnityEngine;
using Peasmod.Utility;
using Peasmod.Patches;
using Peasmod.Gamemodes;

namespace Peasmod
{
    enum CustomRpc
    {
        SetRole = 43,
        CreateVent = 44,
        JesterWin = 45,
        DragBody = 46,
        DropBody = 47,
        MoveBody = 48,
        Revive = 49,
        Invisible = 50,
        Visible = 51,
        FreezeTime = 52,
        UnfreezeTime = 53,
        CreateDot = 54,
        SheriffKills = 55,
        SheriffDies = 56,
        PotatoDies = 57,
        PotatoPassed = 58,
        MorphBack = 59,
        BattleRoyaleInit = 60,
        VictoryRoyale = 162
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
    class HandleRpcPatch
    {
        static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] byte packetId, [HarmonyArgument(1)] MessageReader reader)
        {
            switch (packetId)
            {

                case (byte)CustomRpc.SetRole:
                    var player = Utils.GetPlayer(reader.ReadByte());
                    Role role = (Role)reader.ReadByte();
                    player.SetRole(role);
                    break;
                case (byte)CustomRpc.CreateVent:
                    var id = reader.ReadPackedInt32();
                    var position = reader.ReadVector2();
                    var zAxis = reader.ReadSingle();
                    var leftVent = reader.ReadPackedInt32();
                    var centerVent = reader.ReadPackedInt32();
                    var rightVent = reader.ReadPackedInt32();
                    VentBuilding.CreateVent(PlayerControl.LocalPlayer, id, position, zAxis, leftVent, centerVent, rightVent);
                    break;
                case (byte)CustomRpc.JesterWin:
                    byte JesterId = reader.ReadByte();
                    PlayerControl Jester = Utils.GetPlayer(JesterId);
                    JesterMode.Winner = Jester;
                    JesterMode.JesterWon = true;
                    Jester.Data.IsImpostor = true;
                    Jester.infectedSet = true;
                    foreach (PlayerControl _player in PlayerControl.AllPlayerControls)
                    {
                        if(_player.PlayerId != JesterId)
                        {
                            _player.RemoveInfected();
                            _player.Data.IsImpostor = false;
                        }
                    }
                    
                    Patches.PlayerControlWinPatch.HandleWinRpc();
                    break;
                case (byte)CustomRpc.DragBody:
                    var test2 = reader.ReadPackedInt32();
                    player = Utils.GetPlayer((byte)test2);
                    var test = reader.ReadPackedInt32();
                    var body = Utils.GetDeadBody(test);
                    if (body != null)
                    {
                        BodyDragging.draggers.Add(player.PlayerId);
                        BodyDragging.bodys.Add(body.ParentId);
                        BodyDragging.MoveBody(player, body.ParentId);
                    }
                    break;
                case (byte)CustomRpc.DropBody:
                    player = Utils.GetPlayer((byte)reader.ReadPackedInt32());
                    BodyDragging.bodys.Remove(BodyDragging.bodys[BodyDragging.draggers.IndexOf(player.PlayerId)]);
                    BodyDragging.draggers.Remove(player.PlayerId);
                    break;
                case (byte)CustomRpc.Revive:
                    body = Utils.GetDeadBody(reader.ReadByte());
                    Utils.GetPlayer(body.ParentId).Revive();
                    Utils.GetPlayer(body.ParentId).Data.IsDead = false;
                    Utils.GetPlayer(body.ParentId).transform.position = body.transform.position;
                    Utils.GetPlayer(body.ParentId).Collider.enabled = true;
                    if (PlayerControl.LocalPlayer.PlayerId == body.ParentId)
                    {
                        PlayerControl.LocalPlayer.myTasks.RemoveAt(0);
                    }
                    body.gameObject.Destroy();
                    break;
                case (byte)CustomRpc.Invisible:
                    var invisible = Utils.GetPlayer(reader.ReadByte());
                    InvisibilityMode.invisplayers.Add(invisible.PlayerId);
                    if (PlayerControl.LocalPlayer.Data.IsImpostor)
                    {
                        invisible.myRend.color = Palette.DisabledClear;
                        invisible.HatRenderer.color = Palette.DisabledClear;
                        invisible.MyPhysics.Skin.layer.color = Palette.DisabledClear;
                    }
                    else
                    {
                        invisible.Visible = false;
                    }
                    break;
                case (byte)CustomRpc.Visible:
                    var visible = Utils.GetPlayer(reader.ReadByte());
                    visible.Visible = true;
                    visible.myRend.color = Palette.EnabledColor;
                    visible.HatRenderer.color = Palette.EnabledColor;
                    visible.MyPhysics.Skin.layer.color = Palette.EnabledColor;
                    InvisibilityMode.invisplayers.Remove(visible.PlayerId);
                    break;
                case (byte)CustomRpc.FreezeTime:
                    TimeFreezing.timeIsFroozen = true;
                    if (PlayerControl.LocalPlayer.Data.IsImpostor)
                        HudManager.Instance.ShowPopUp("Time has been frozen");
                    break;
                case (byte)CustomRpc.UnfreezeTime:
                    TimeFreezing.timeIsFroozen = false;
                    if(PlayerControl.LocalPlayer != null)
                        PlayerControl.LocalPlayer.moveable = true;
                    break;
                case (byte)CustomRpc.CreateDot:
                    player = Utils.GetPlayer(reader.ReadByte());
                    if(player.PlayerId != PlayerControl.LocalPlayer.PlayerId && !PlayerControl.LocalPlayer.Data.IsDead)
                    {
                        if (PlayerControl.LocalPlayer.IsRole(Role.Inspector))
                        {
                            var dot = new GameObject();
                            var _renderer = dot.AddComponent<SpriteRenderer>();
                            _renderer.sprite = Utils.CreateSprite("Peasmod.Resources.Dot.png");
                            dot.transform.localPosition = new Vector3(player.GetTruePosition().x, player.GetTruePosition().y, player.transform.position.z);
                            dot.GetComponent<SpriteRenderer>().material.color = Utils.ColorIdToColor(player.Data.ColorId);
                            HudManagerPatch.dots.Add(dot, Time.time);
                        }
                    }
                    break;
                case (byte)CustomRpc.SheriffDies:
                    var Sheriff = Utils.GetPlayer(reader.ReadByte());
                    DeadBody deadBody = UnityEngine.Object.Instantiate<DeadBody>(Sheriff.KillAnimations.Random<KillAnimation>().bodyPrefab);
                    deadBody.ParentId = Sheriff.PlayerId;
                    deadBody.transform.position = Sheriff.transform.position;
                    Sheriff.SetPlayerMaterialColors((Renderer)((Component)deadBody).GetComponent<Renderer>());
                    Sheriff.Die(DeathReason.Kill);
                    Sheriff.transform.position = deadBody.transform.position;
                    Sheriff.Collider.enabled = false;
                    break;
                case (byte)CustomRpc.PotatoDies:
                    player = Utils.GetPlayer(reader.ReadByte());
                    player.Die(DeathReason.Kill);
                    player.Data.IsImpostor = false;
                    player.Data.IsDead = true;
                    player.Collider.enabled = false;
                    var potato = new GameObject();
                    var renderer = potato.AddComponent<SpriteRenderer>();
                    renderer.sprite = Utils.CreateSprite("Peasmod.Resources.Potato.png");
                    var scale = new Vector3(potato.transform.localScale.x+2f, potato.transform.localScale.y+2f);
                    potato.transform.localScale = scale;
                    potato.transform.position = player.GetTruePosition();
                    break;
                case (byte)CustomRpc.PotatoPassed:
                    player = Utils.GetPlayer(reader.ReadByte());
                    var target = Utils.GetPlayer(reader.ReadByte());
                    target.Data.IsImpostor = true;
                    player.Data.IsImpostor = false;
                    player.nameText.color = Palette.White;
                    HotPotatoMode.TimeTillDeath -= 0.25f;
                    if (target.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                    {
                        HotPotatoMode.timer.GetComponent<TextRenderer>().Text = "Timer";
                        HotPotatoMode.Timer = HotPotatoMode.TimeTillDeath;
                        HudManager.Instance.KillButton.gameObject.SetActive(true);
                    }
                    break;
                case (byte)CustomRpc.MorphBack:
                    if (PlayerControl.LocalPlayer.IsMorphed())
                        MorphingMode.OnLabelClick(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer, false);
                    break;
                case (byte)CustomRpc.BattleRoyaleInit:
                    foreach (var _player in PlayerControl.AllPlayerControls)
                        _player.Data.IsImpostor = false;
                    Il2CppStructArray<byte> _impostors = new byte[1];
                    PlayerControl.LocalPlayer.SetInfected(_impostors);
                    PlayerControl.LocalPlayer.Data.IsImpostor = true;
                    HudManager.Instance.KillButton.gameObject.SetActive(true);
                    break;
                case (byte)CustomRpc.VictoryRoyale:
                    player = Utils.GetPlayer(reader.ReadByte());
                    BattleRoyaleMode.Winner = player;
                    BattleRoyaleMode.HasWon = true;

                    Patches.PlayerControlWinPatch.HandleWinRpc();
                    break;
            }
        }
    }
}
