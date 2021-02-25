using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Reactor.Extensions;
using UnhollowerBaseLib;

namespace Peasmod.Patches
{
	[HarmonyPatch(typeof(MeetingHud), "CCEPEINGBCN")]
	class VotePatch
    {
        public static bool Prefix(MeetingHud __instance, ref Il2CppStructArray<byte> __result)
        {
            byte[] numArray = new byte[13];
            for (int index1 = 0; index1 < __instance.playerStates.Length; ++index1)
            {
                PlayerVoteArea playerState = __instance.playerStates[index1];
                if (playerState.didVote)
                {
                    int index2 = (int)playerState.votedFor + 1;
                    if (index2 >= 0 && index2 < numArray.Length)
                        ++numArray[index2];
                    if (MayorMode.Mayor1 != null)
                        if (playerState.TargetPlayerId == MayorMode.Mayor1.PlayerId)
                            ++numArray[index2];
                    if (MayorMode.Mayor2 != null)
                        if (playerState.TargetPlayerId == MayorMode.Mayor2.PlayerId)
                            ++numArray[index2];
                }
            }
            __result =  numArray;
            return false;
        }
	}
}
