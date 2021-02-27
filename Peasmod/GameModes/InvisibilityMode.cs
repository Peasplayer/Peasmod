using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Hazel;
using HarmonyLib;
using Reactor.Extensions;
using Reactor;
using System.Linq;
using UnhollowerBaseLib;
using Peasmod.Utility;

namespace Peasmod.GameModes
{
    class InvisibilityMode
    {
        public static CooldownButton button;

        public static List<byte> invisplayers = new List<byte>();

        public static void OnClicked()
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.Invisible, Hazel.SendOption.None, -1);
            writer.WritePacked(PlayerControl.LocalPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            invisplayers.Add(PlayerControl.LocalPlayer.PlayerId);
            PlayerControl.LocalPlayer.myRend.color = Palette.DisabledColor;
            PlayerControl.LocalPlayer.HatRenderer.color = Palette.DisabledColor;
            PlayerControl.LocalPlayer.MyPhysics.Skin.layer.color = Palette.DisabledColor;
            //HudManager.Instance.ShowMap((Action<MapBehaviour>)(m => m.ShowCountOverlay()));
            //PlayerControl.LocalPlayer.moveable = true;
            //MapBehaviour.Instance.ShowCountOverlay();
        }

        public static void OnEnded()
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.Visible, Hazel.SendOption.None, -1);
            writer.WritePacked(PlayerControl.LocalPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            int index = invisplayers.IndexOf(PlayerControl.LocalPlayer.PlayerId);
            invisplayers.Remove(PlayerControl.LocalPlayer.PlayerId);
            PlayerControl.LocalPlayer.myRend.color = Palette.EnabledColor;
            PlayerControl.LocalPlayer.HatRenderer.color = Palette.EnabledColor;
            PlayerControl.LocalPlayer.MyPhysics.Skin.layer.color = Palette.EnabledColor;
        }
    }
}
