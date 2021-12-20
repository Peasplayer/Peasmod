using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using UnityEngine;
using Peasmod.Gamemodes;

namespace Peasmod.Utility
{
    public static class PlayerControlExtensions
    {
        public static bool IsRole(this PlayerControl player, Role role)
        {
            switch(role)
            {
                case Role.Jester:
                    foreach(var jester in JesterMode.Jesters)
                        if (jester.PlayerId == player.PlayerId)
                            return true;
                    return false;
                case Role.Mayor:
                    foreach (var mayor in MayorMode.Mayors)
                        if (mayor.PlayerId == player.PlayerId)
                            return true;
                    return false;
                case Role.Inspector:
                    foreach (var inspector in InspectorMode.Inspectors)
                        if (inspector.PlayerId == player.PlayerId)
                            return true;
                    return false;
                case Role.Doctor:
                    foreach (var doctor in DoctorMode.Doctors)
                        if (doctor.PlayerId == player.PlayerId)
                            return true;
                    return false;
                case Role.Sheriff:
                    foreach (var sheriff in SheriffMode.Sheriffs)
                        if (sheriff.PlayerId == player.PlayerId)
                            return true;
                    return false;
                case Role.Thanos:
                    foreach (var thanos in ThanosMode.Thanos)
                        if (thanos.PlayerId == player.PlayerId)
                            return true;
                    return false;
            }
            return false;
        }

        public static void SetRole(this PlayerControl player, Nullable<Role> role)
        {
            JesterMode.Jesters.Remove(player);
            MayorMode.Mayors.Remove(player);
            InspectorMode.Inspectors.Remove(player);
            DoctorMode.Doctors.Remove(player);
            SheriffMode.Sheriffs.Remove(player);
            if(role != null)
                switch (role)
                {
                    case Role.Jester:
                        JesterMode.Jesters.Add(player);
                        break;
                    case Role.Mayor:
                        MayorMode.Mayors.Add(player);
                        break;
                    case Role.Inspector:
                        InspectorMode.Inspectors.Add(player);
                        break;
                    case Role.Doctor:
                        DoctorMode.Doctors.Add(player);
                        break;
                    case Role.Sheriff:
                        SheriffMode.Sheriffs.Add(player);
                        break;
                    case Role.Thanos:
                        ThanosMode.Thanos.Add(player);
                        break;
                }
        }

        public static void RpcSetRole(this PlayerControl player, Nullable<Role> role)
        {
            if (role == null) role = 0;
            player.SetRole(role);
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(player.NetId, (byte)CustomRpc.SetRole, Hazel.SendOption.None, -1);
            writer.Write(player.PlayerId);
            writer.Write((byte)role);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static Role GetRole(this PlayerControl player)
        {
            foreach (var jester in JesterMode.Jesters)
                if (jester.PlayerId == player.PlayerId)
                    return Role.Jester;
            foreach (var mayor in MayorMode.Mayors)
                if (mayor.PlayerId == player.PlayerId)
                    return Role.Mayor;
            foreach (var inspector in InspectorMode.Inspectors)
                if (inspector.PlayerId == player.PlayerId)
                    return Role.Inspector;
            foreach (var doctor in DoctorMode.Doctors)
                if (doctor.PlayerId == player.PlayerId)
                    return Role.Doctor;
            foreach (var sheriff in SheriffMode.Sheriffs)
                if (sheriff.PlayerId == player.PlayerId)
                    return Role.Sheriff;
            foreach (var thanos in ThanosMode.Thanos)
                if (thanos.PlayerId == player.PlayerId)
                    return Role.Thanos;
            if (player.Data.IsImpostor)
                return Role.Impostor;
            else
                return Role.Crewmate;
        }

        private static System.Collections.Generic.Dictionary<byte, bool> isMorphed = new System.Collections.Generic.Dictionary<byte, bool>();

        public static bool IsMorphed(this PlayerControl player)
        {
            if (isMorphed.TryGetValue(player.PlayerId, out bool _isMorphed) == false)
                _isMorphed = false;
            return _isMorphed;
        }

        public static void SetMorphed(this PlayerControl player, bool morphed)
        {
            if (isMorphed.TryGetValue(player.PlayerId, out bool _isMorphed) == false)
                isMorphed.Add(player.PlayerId, morphed);
            else
                isMorphed[player.PlayerId] = morphed;
        }
        
        public static PlayerControl FindClosestTarget(this PlayerControl player, float distance)
                {
                    PlayerControl playerControl1 = (PlayerControl)null;
                    if (!ShipStatus.Instance)
                        return (PlayerControl)null;
                    Vector2 truePosition = player.GetTruePosition();
                    System.Collections.Generic.List<GameData.PlayerInfo> allPlayers = GameData.Instance.AllPlayers.ToArray().ToList();
                    for (int index = 0; index < allPlayers.Count; ++index)
                    {
                        GameData.PlayerInfo playerInfo = allPlayers[index];
                        if (!playerInfo.Disconnected && playerInfo.PlayerId != player.PlayerId && !playerInfo.IsDead)
                        {
                            PlayerControl playerControl2 = playerInfo.Object;
                            if (playerControl2)
                            {
                                Vector2 vector2 = playerControl2.GetTruePosition() - truePosition;
                                float magnitude = vector2.magnitude;
                                if (magnitude <= distance && !PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector2.normalized, magnitude, Constants.ShipAndObjectsMask))
                                {
                                    playerControl1 = playerControl2;
                                    distance = magnitude;
                                }
                            }
                        }
                    }
                    return playerControl1;
                }
        
                public static PlayerControl GetPlayer(this byte id)
                {
                    foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                    {
                        if (player.PlayerId == id)
                        {
                            return player;
                        }
                    }
                    return null;
                }

        public static void ResetRoles()
        {
            JesterMode.Jesters = new List<PlayerControl>();
            JesterMode.JesterWon = false;
            JesterMode.Winner = null;
            MayorMode.Mayors = new List<PlayerControl>();
            InspectorMode.Inspectors = new List<PlayerControl>();
            DoctorMode.Doctors = new List<PlayerControl>();
            SheriffMode.Sheriffs = new List<PlayerControl>();
            ThanosMode.Thanos = new List<PlayerControl>();
        }
    }

    public enum Role
    {
        Crewmate = 0,
        Impostor = 1,
        Jester = 2,
        Mayor = 3,
        Inspector = 4,
        Doctor = 5,
        Sheriff = 6,
        Thanos = 7
    }
}
