using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Hazel;
using Reactor.Extensions;
using Peasmod.Utility;

namespace Peasmod.Gamemodes
{
    class DoctorMode
    {
        public static List<PlayerControl> Doctors = new List<PlayerControl>();

        public static CooldownButton button;

        public static Color DoctorColor { get; } = new Color(246f / 255f, 255f / 255f, 0f / 255f);

        public static void OnClicked()
        {
            List<DeadBody> _bodys = new List<DeadBody>();
            foreach (Collider2D collider2D in Physics2D.OverlapCircleAll(PlayerControl.LocalPlayer.GetTruePosition(), PlayerControl.LocalPlayer.MaxReportDistance - 2f, Constants.PlayersOnlyMask))
            {
                if (collider2D.tag == "DeadBody")
                {
                    DeadBody body = (DeadBody)((Component)collider2D).GetComponent<DeadBody>();
                    _bodys.Add(body);
                }
            }
            if (_bodys.Count != 0)
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.Revive, Hazel.SendOption.None, -1);
                writer.WritePacked(_bodys[0].ParentId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.GetPlayer(_bodys[0].ParentId).Revive();
                Utils.GetPlayer(_bodys[0].ParentId).Data.IsDead = false;
                Utils.GetPlayer(_bodys[0].ParentId).transform.position = _bodys[0].transform.position;
                Utils.GetPlayer(_bodys[0].ParentId).Collider.enabled = true;
                _bodys[0].gameObject.Destroy();
            }
        }
    }
}
