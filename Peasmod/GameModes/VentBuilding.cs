using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using System.Linq;
using UnityEngine;
using Hazel;
using Reactor.Extensions;
using Reactor;

namespace Peasmod
{
    class VentBuilding
    {
        const byte SpawnVentCallId = 62;

        public static CooldownButton button;

        public static Vent lastVent;
        public static bool dragBody=false;
        public static Vector2 VentSize;

        public static void OnClicked()
        {
            var pos = PlayerControl.LocalPlayer.transform.position;
            var ventId = GetAvailableVentId();
            var ventLeft = int.MaxValue;
            var ventCenter = int.MaxValue;
            var ventRight = int.MaxValue;

            if (lastVent != null)
            {
                ventLeft = lastVent.Id;
            }

            HandleCreateVent(ventId, pos, pos.z + .001f, ventLeft, ventCenter, ventRight);
        }

        static int GetAvailableVentId()
        {
            int id = 0;
            while (true)
            {
                if (!ShipStatus.Instance.AllVents.Any(v => v.Id == id))
                    return id;
                id++;
            }
        }

        public static void CreateVent(PlayerControl sender, int id, Vector2 position, float zAxis, int leftVent, int centerVent, int rightVent)
        {
            var realPos = new Vector3(position.x, position.y, zAxis);
            var ventPref = UnityEngine.Object.FindObjectOfType<Vent>();
            var vent = UnityEngine.Object.Instantiate(ventPref, ventPref.transform.parent);
            vent.Id = id;
            vent.transform.position = realPos;
            vent.Left = leftVent == int.MaxValue ? null : ShipStatus.Instance.AllVents.FirstOrDefault(v => v.Id == leftVent);
            vent.Center = centerVent == int.MaxValue ? null : ShipStatus.Instance.AllVents.FirstOrDefault(v => v.Id == centerVent);
            vent.Right = rightVent == int.MaxValue ? null : ShipStatus.Instance.AllVents.FirstOrDefault(v => v.Id == rightVent);
            var allVents = ShipStatus.Instance.AllVents.ToList();
            allVents.Add(vent);
            ShipStatus.Instance.AllVents = allVents.ToArray();
            if (vent.Left != null)
                vent.Left.Right = ShipStatus.Instance.AllVents.FirstOrDefault(v => v.Id == id);
            if (sender.AmOwner)
                lastVent = vent;
        }

        private static void HandleCreateVent(int id, Vector2 position, float zAxis, int leftVent, int centerVent, int rightVent)
        {
            CreateVent(PlayerControl.LocalPlayer, id, position, zAxis, leftVent, centerVent, rightVent);
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.CreateVent, Hazel.SendOption.None, -1);
            writer.WritePacked(id);
            writer.Write(position);
            writer.Write(zAxis);
            writer.WritePacked(leftVent);
            writer.WritePacked(centerVent);
            writer.WritePacked(rightVent);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
    }
}
