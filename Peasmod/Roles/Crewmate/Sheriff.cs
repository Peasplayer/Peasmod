using System.Collections.Generic;
using BepInEx.Unity.IL2CPP;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.Options;
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
        public override string LongDescription => "";
        public override string TaskText => "Execute the impostor";
        public override Color Color => ModdedPalette.SheriffColor;
        public override Team Team => Team.Crewmate;
        public override Visibility Visibility => Visibility.NoOne;
        public override bool HasToDoTasks => true;
        public override Dictionary<string, CustomOption> AdvancedOptions { get; set; } = new Dictionary<string, CustomOption>()
        {
            {
                "CanKillNeutrals", new CustomToggleOption(MultiMenu.Crewmate, "Can Kill Neutrals", false)
            }
        };
        public override bool CanKill(PlayerControl victim = null) => true;

        public override void OnKill(PlayerControl killer, PlayerControl victim)
        {
            if (killer.IsCustomRole(this) && killer.IsLocal() && !victim.IsLocal())
                if (!(victim.Data.Role.IsImpostor || victim.GetCustomRole() != null && (victim.GetCustomRole().Team == Team.Role ||
                        victim.GetCustomRole().Team == Team.Alone) &&
                    ((CustomToggleOption) AdvancedOptions["CanKillNeutrals"]).Value))
                    PlayerControl.LocalPlayer.RpcMurderPlayer(PlayerControl.LocalPlayer, true);
        }
    }
}