using HarmonyLib;
using Hazel;
using Peasmod.Roles;
using Peasmod.Utility;

namespace Peasmod
{
    enum CustomRpc
    {
        
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
    class HandleRpcPatch
    {
        static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] byte packetId, [HarmonyArgument(1)] MessageReader reader)
        {
            switch (packetId)
            {
                
            }
        }
    }
}
