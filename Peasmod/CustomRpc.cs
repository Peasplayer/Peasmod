using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Hazel;
using Reactor.Extensions;
using UnityEngine;

namespace Peasmod
{
    enum CustomRpc
    {
        CreateVent = 43,
        SetJester1 = 44,
        SetJester2 = 45,
        JesterWin = 46,
        DragBody = 47,
        DropBody = 48,
        MoveBody = 49,
        SetDoctor1 = 50,
        SetDoctor2 = 51,
        Revive = 52,
        SetMayor1 = 53,
        SetMayor2 = 54,
        Invisible = 55,
        Visible = 56,
        FreezeTime = 57,
        UnfreezeTime = 58,
        SetInspector1 = 59,
        SetInspector2 = 60,
        CreateDot = 61,
        SetSheriff1 = 62,
        SetSheriff2 = 63,
        SheriffDies = 64
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
    class HandleRpcPatch
    {
        static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] byte packetId, [HarmonyArgument(1)] MessageReader reader)
        {
            switch (packetId)
            {
                case (byte)CustomRpc.CreateVent:
                    var id = reader.ReadPackedInt32();
                    var position = reader.ReadVector2();
                    var zAxis = reader.ReadSingle();
                    var leftVent = reader.ReadPackedInt32();
                    var centerVent = reader.ReadPackedInt32();
                    var rightVent = reader.ReadPackedInt32();
                    VentBuilding.CreateVent(PlayerControl.LocalPlayer, id, position, zAxis, leftVent, centerVent, rightVent);
                    break;
                case (byte)CustomRpc.SetJester1:
                    byte Jester1Id = reader.ReadByte();
                    foreach (PlayerControl _player in PlayerControl.AllPlayerControls)
                        if (_player.PlayerId == Jester1Id)
                            JesterMode.Jester1 = _player;
                    break;
                case (byte)CustomRpc.SetJester2:
                    byte Jester2Id = reader.ReadByte();
                    foreach (PlayerControl _player in PlayerControl.AllPlayerControls)
                        if (_player.PlayerId == Jester2Id)
                            JesterMode.Jester2 = _player;
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
                    PlayerControl player = Utils.GetPlayer((byte)test2);
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
                case (byte)CustomRpc.SetDoctor1:
                    byte Doctor1Id = reader.ReadByte();
                    foreach (PlayerControl _player in PlayerControl.AllPlayerControls)
                        if (_player.PlayerId == Doctor1Id)
                            DoctorMode.Doctor1 = _player;
                    break;
                case (byte)CustomRpc.SetDoctor2:
                    byte Doctor2Id = reader.ReadByte();
                    foreach (PlayerControl _player in PlayerControl.AllPlayerControls)
                        if (_player.PlayerId == Doctor2Id)
                            DoctorMode.Doctor2 = _player;
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
                case (byte)CustomRpc.SetMayor1:
                    byte Mayor1Id = reader.ReadByte();
                    foreach (PlayerControl _player in PlayerControl.AllPlayerControls)
                        if (_player.PlayerId == Mayor1Id)
                            MayorMode.Mayor1 = _player;
                    break;
                case (byte)CustomRpc.SetMayor2:
                    byte Mayor2Id = reader.ReadByte();
                    foreach (PlayerControl _player in PlayerControl.AllPlayerControls)
                        if (_player.PlayerId == Mayor2Id)
                            MayorMode.Mayor2 = _player;
                    break;
                case (byte)CustomRpc.Invisible:
                    var invisible = Utils.GetPlayer(reader.ReadByte());
                    InvisibilityMode.invisplayers.Add(invisible.PlayerId);
                    if (PlayerControl.LocalPlayer.Data.IsImpostor)
                    {
                        invisible.myRend.color = Palette.DisabledColor;
                        invisible.HatRenderer.color = Palette.DisabledColor;
                        invisible.MyPhysics.Skin.layer.color = Palette.DisabledColor;
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
                    PlayerControl.LocalPlayer.moveable = true;
                    break;
                case (byte)CustomRpc.SetInspector1:
                    byte Detective1Id = reader.ReadByte();
                    foreach (PlayerControl _player in PlayerControl.AllPlayerControls)
                        if (_player.PlayerId == Detective1Id)
                            InspectorMode.Inspector1 = _player;
                    break;
                case (byte)CustomRpc.SetInspector2:
                    byte Detective2Id = reader.ReadByte();
                    foreach (PlayerControl _player in PlayerControl.AllPlayerControls)
                        if (_player.PlayerId == Detective2Id)
                            InspectorMode.Inspector2 = _player;
                    break;
                case (byte)CustomRpc.CreateDot:
                    player = Utils.GetPlayer(reader.ReadByte());
                    if(player.PlayerId != PlayerControl.LocalPlayer.PlayerId && !PlayerControl.LocalPlayer.Data.IsDead)
                    {
                        if (PlayerControl.LocalPlayer == InspectorMode.Inspector1 || PlayerControl.LocalPlayer == InspectorMode.Inspector2)
                        {
                            var dot = Utils.CreateSprite("Peasmod.Resources.Dot.png");
                            dot.transform.localPosition = new Vector3(player.GetTruePosition().x, player.GetTruePosition().y, player.transform.position.z);
                            dot.GetComponent<SpriteRenderer>().material.color = Utils.ColorIdToColor(player.Data.ColorId);
                            HudManagerPatch.dots.Add(dot, Time.time);
                        }
                    }
                    break;
                case (byte)CustomRpc.SetSheriff1:
                    byte Sheriff1Id = reader.ReadByte();
                    foreach (PlayerControl _player in PlayerControl.AllPlayerControls)
                        if (_player.PlayerId == Sheriff1Id)
                            SheriffMode.Sheriff1 = _player;
                    break;
                case (byte)CustomRpc.SetSheriff2:
                    byte Sheriff2Id = reader.ReadByte();
                    foreach (PlayerControl _player in PlayerControl.AllPlayerControls)
                        if (_player.PlayerId == Sheriff2Id)
                            SheriffMode.Sheriff2 = _player;
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
            }
        }
    }
}
