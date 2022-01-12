using BepInEx.IL2CPP;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.Roles;
using UnityEngine;

namespace Peasmod.Roles.Crewmate
{
    [RegisterCustomRole]
    public class Sheriff : BaseRole
    {
        public Sheriff(BasePlugin plugin) : base(plugin)
        {
        }

        public override string Name => "Sheriff";
        public override string Description => "Execute the impostor";
        public override string TaskText => "Execute the impostor";
        public override Color Color => new Color(255f / 255f, 114f / 255f, 0f / 255f);
        public override Team Team => Team.Crewmate;
        public override Visibility Visibility => Visibility.NoOne;
        public override bool HasToDoTasks => true;
        public override int Limit => (int)Settings.SheriffAmount.Value;
        public override bool CanKill(PlayerControl victim = null) => true;

        public override void OnKill(PlayerControl killer, PlayerControl victim)
        {
            if (killer.IsRole(this) && killer.IsLocal() && !victim.IsLocal())
                if (!(victim.Data.Role.IsImpostor || victim.GetRole() != null && (victim.GetRole().Team == Team.Role ||
                        victim.GetRole().Team == Team.Alone) &&
                    Settings.SheriffCanKillNeutrals.Value))
                    PlayerControl.LocalPlayer.RpcMurderPlayer(PlayerControl.LocalPlayer);
        }
    }
}