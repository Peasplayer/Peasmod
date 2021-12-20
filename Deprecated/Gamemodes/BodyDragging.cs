using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using System.Linq;
using UnityEngine;
using Hazel;
using Reactor.Extensions;
using Reactor;
using Peasmod.Utility;

namespace Peasmod.Gamemodes
{
    class BodyDragging
    { 

        public static CooldownButton button;

        public static List<byte> draggers = new List<byte>();
        public static List<int> bodys = new List<int>();

        public static void OnClicked()
        {
            var buttonTextures = Peasmod.Instance.Config.Bind("Settings", "ButtonTextures", true, "If set to true the textures that Peasplayer made will be used. Else the textures of Gravity will be used.");
            var resource = buttonTextures.Value ? "Peasmod.Resources.Buttons.Peasplayer" : "Peasmod.Resources.Buttons.Gravity";
            if(draggers.Contains(PlayerControl.LocalPlayer.PlayerId))
            {
                bodys.RemoveAt(draggers.IndexOf(PlayerControl.LocalPlayer.PlayerId));
                draggers.Remove(PlayerControl.LocalPlayer.PlayerId);
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.DropBody, Hazel.SendOption.None, -1);
                writer.WritePacked(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                button.SetTexture(resource + ".DragBody.png");
            }
            else
            {
                button.SetTexture(resource + ".DropBody.png");
                List<DeadBody> _bodys = new List<DeadBody>();
                foreach (Collider2D collider2D in Physics2D.OverlapCircleAll(PlayerControl.LocalPlayer.GetTruePosition(), PlayerControl.LocalPlayer.MaxReportDistance-2f, Constants.PlayersOnlyMask))
                {
                    if (collider2D.tag == "DeadBody")
                    {
                        DeadBody body = (DeadBody)((Component)collider2D).GetComponent<DeadBody>();
                        _bodys.Add(body);
                    }
                }
                if(_bodys.Count != 0)
                {
                    draggers.Add(PlayerControl.LocalPlayer.PlayerId);
                    bodys.Add(_bodys[0].ParentId);
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.DragBody, Hazel.SendOption.None, -1);
                    writer.WritePacked(PlayerControl.LocalPlayer.PlayerId);
                    writer.WritePacked(_bodys[0].ParentId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    MoveBody(PlayerControl.LocalPlayer, _bodys[0].ParentId);
                }
            }
        }

        public static void MoveBody(PlayerControl player, int bodyid)
        {
            if (player.transform == null) return;
            if (draggers.Contains(player.PlayerId))
            {
                var body = Utils.GetDeadBody(bodyid);
                if (body == null) return;
                if (!player.inVent)
                {
                    var pos = player.transform.position;
                    pos.Set(pos.x, pos.y, pos.z + .001f);
                    body.transform.position = (Vector3.Lerp(Utils.GetDeadBody(bodys[(draggers.IndexOf(player.PlayerId))]).transform.position, pos, Time.deltaTime+0.05f));
                }
                else
                    body.transform.position = (player.transform.position);
                return;
            }
        }
    }
}