using System;
using System.Collections.Generic;
using System.Text;
using Hazel;

namespace Peasmod
{
    class TimeFreezing
    {
        public static bool timeIsFroozen = false;

        public static CooldownButton button;

        public static void OnClick()
        {
            timeIsFroozen = true;
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.FreezeTime, Hazel.SendOption.None, -1);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static void OnEnded()
        {
            timeIsFroozen = false;
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.UnfreezeTime, Hazel.SendOption.None, -1);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

    }
}
