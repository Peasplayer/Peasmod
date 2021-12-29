using System.Collections.Generic;
using System.Linq;
using BepInEx.IL2CPP;
using HarmonyLib;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.CustomEndReason;
using PeasAPI.Roles;
using Reactor.Networking;
using Reactor.Networking.MethodRpc;
using UnityEngine;

namespace Peasmod.Roles
{
    [RegisterCustomRole]
    public class Assassin : BaseRole
    {
        public Assassin(BasePlugin plugin) : base(plugin)
        {
            Instance = this;
        }

        public override string Name => "Assasin";
        public override string Description => "Kill every player";
        public override string TaskText => "Kill every impostor and crewmate";
        public override Color Color => Color.magenta;
        public override Visibility Visibility => Visibility.NoOne;
        public override Team Team => Team.Alone;
        public override bool HasToDoTasks => false;
        public override int Limit => (int)Settings.AssassinAmount.Value;
        public override float KillDistance => GameOptionsData.KillDistances[Mathf.Clamp(PlayerControl.GameOptions.KillDistance, 0, 2)] + 0.35f;
        public override bool CanKill(PlayerControl victim = null) => true;
        public override bool CanSabotage(SystemTypes? sabotage) => sabotage == null || sabotage == SystemTypes.Comms;
        public override bool ShouldGameEnd(GameOverReason reason)
        {
            PeasAPI.PeasAPI.Logger.LogInfo(reason.ToString());
            if (reason == EndReasonManager.CustomGameOverReason || reason == GameOverReason.ImpostorBySabotage)
                return true;
            if (Utility.GetAllPlayers().Count(p => !p.Data.IsDead && p.GetRole() != null && p.IsRole(this)) == 0)
                return true;
            return false;
        }

        public static Assassin Instance;

        public override void OnKill(PlayerControl killer, PlayerControl victim)
        {
            if (Members.Count != 0)
                ShipStatus.RpcEndGame(GameOverReason.ImpostorByKill, false);
            if (PlayerControl.LocalPlayer.IsRole(this) && killer.IsLocal() &&
                Utility.GetAllPlayers().Count(p => !p.Data.IsDead && !p.IsLocal()) == 0)
            {
                new CustomEndReason(PlayerControl.LocalPlayer);
            }
        }
    }
}