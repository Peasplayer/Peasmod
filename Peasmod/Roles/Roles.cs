using System;
using System.Collections.Generic;
using Hazel;

namespace Peasmod.Roles
{
    public static class Roles
    {
        public static List<byte> crewmates = new List<byte>();
        public static List<byte> impostors = new List<byte>();
        
        public enum Role
        {
            Crewmate = 0,
            Impostor = 1,
            Jester = 2
        }
        
        public static bool IsRole(this PlayerControl player, Role role)
        {
            if (role == Role.Impostor)
                return player.Data.IsImpostor;
            if (role == Role.Jester)
                return JesterRole.members.Contains(player.PlayerId);
            if (role == Role.Crewmate)
                return !player.Data.IsImpostor;
            return false;
        }

        public static void SetRole(this PlayerControl player, Nullable<Role> role)
        {
            switch (role)
            {
                case null:
                    JesterRole.members.Remove(player.PlayerId);
                    break;
                case Role.Crewmate:
                    JesterRole.members.Remove(player.PlayerId);
                    break;
                case Role.Jester:
                    JesterRole.members.Add(player.PlayerId);
                    break;
            }
        }

        public static void RpcSetRole(this PlayerControl player, Nullable<Role> role)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(player.NetId, (byte)CustomRpc.SetRole, Hazel.SendOption.None, -1);
            writer.Write(player.PlayerId);
            if (role == null) 
                writer.Write((byte)Role.Crewmate);
            else
                writer.Write((byte)role);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
        
        public static Role GetRole(this PlayerControl player)
        {
            if (player.Data.IsImpostor)
                return Role.Impostor;
            if (JesterRole.members.Contains(player.PlayerId))
                return Role.Jester;
            return Role.Crewmate;
        }
        
        public static void ResetRoles()
        {
            JesterRole.Reset();
        }
    }
}