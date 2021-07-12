using HarmonyLib;
using Hazel;
using PeasAPI;
using Peasmod.Roles;
using Peasmod.Utility;
using Reactor.Extensions;

namespace Peasmod
{
    enum CustomRpc
    {
        DemonAbility = 50,
        DoctorAbility = 51
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
                case (byte)CustomRpc.DoctorAbility:
                    player = reader.ReadByte().GetPlayer();
                    
                    player.Revive();
                    player.transform.position = Utils.GetDeadBody(player.PlayerId).transform.position;
                    Utils.GetDeadBody(player.PlayerId).gameObject.Destroy();
                    break;
            }
        }
    }
}
