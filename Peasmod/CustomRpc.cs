using HarmonyLib;
using Hazel;
using Peasmod.Roles;
using Peasmod.Utility;

namespace Peasmod
{
    enum CustomRpc
    {
        SetRole = 43
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
    class HandleRpcPatch
    {
        static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] byte packetId, [HarmonyArgument(1)] MessageReader reader)
        {
            switch (packetId)
            {
                case (byte)CustomRpc.SetRole:
                    var player = reader.ReadByte().GetPlayer();
                    var role = (Roles.Roles.Role) reader.ReadByte();
                    Utils.Log(player.nameText.text + ": " + role);
                    player.SetRole(role);
                    Utils.Log("2: " + player.nameText.text + ": " + player.GetRole());
                    break;
            }
        }
    }
}
