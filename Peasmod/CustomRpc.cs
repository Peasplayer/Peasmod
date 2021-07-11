using HarmonyLib;
using Hazel;
using PeasAPI;
using Peasmod.Roles;
using Peasmod.Utility;

namespace Peasmod
{
    enum CustomRpc
    {
        DemonAbility = 50
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
    class HandleRpcPatch
    {
        static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] byte packetId, [HarmonyArgument(1)] MessageReader reader)
        {
            switch (packetId)
            {
                case (byte)CustomRpc.DemonAbility:
                    var player = reader.ReadByte().GetPlayer();
                    var deathOrRevive = reader.ReadBoolean();

                    if (deathOrRevive)
                    {
                        player.Die(DeathReason.Kill);
                    }
                    else
                    {
                        player.Revive();
                    }
                    break;
            }
        }
    }
}
