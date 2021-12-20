using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Peasmod.Utility;
using Hazel;

namespace Peasmod.Gamemodes
{
    class HotPotatoMode
    {
        public static Color color = new Color(255f / 255f, 114f / 255f, 0f / 255f);
        public static CooldownButton button;
        public static PlayerControl CurrentTarget;
        public static GameObject timer;
        public static float Timer;
        public static float TimeTillDeath;

        public static void OnClick()
        {
            CurrentTarget.Data.IsImpostor = true;
            PlayerControl.LocalPlayer.Data.IsImpostor = false;
            PlayerControl.LocalPlayer.nameText.color = Palette.White;
            HotPotatoMode.timer.GetComponent<TextRenderer>().Text = string.Empty;
            HotPotatoMode.TimeTillDeath -= 0.25f;
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.PotatoPassed, Hazel.SendOption.None, -1);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write(CurrentTarget.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
    }
}
