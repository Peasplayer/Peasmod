using System;
using System.Collections.Generic;
using System.Text;
using Peasmod.Utility;

namespace Peasmod.Gamemodes
{
    class BattleRoyaleMode
    {
        public static PlayerControl Winner;
        public static bool HasWon = false;
        public static bool HasKilled = false;
        public static PlayerControl CurrentTarget;

        public static void OnClick()
        {
            if(CurrentTarget != null)
                PlayerControl.LocalPlayer.RpcMurderPlayer(CurrentTarget);
        }
    }
}
