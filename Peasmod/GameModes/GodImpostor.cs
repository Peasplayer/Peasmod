using System;
using BepInEx.IL2CPP;
using PeasAPI.Components;
using PeasAPI.GameModes;
using Peasmod.Roles;
using Peasmod.Roles.Crewmate;
using Peasmod.Roles.Neutral;

namespace Peasmod.GameModes
{
    [RegisterCustomGameMode]
    public class GodImpostor : GameMode
    {
        public GodImpostor(BasePlugin plugin) : base(plugin)
        {
        }

        public override string Name => $"{PeasAPI.Utility.StringColor.Red}GodImpostor";

        public override bool Enabled => GameModeManager.IsGameModeActive(this);

        public override bool HasToDoTasks => true;
        
        public override bool AllowVanillaRoles => false;

        public override Type[] RoleWhitelist { get; } =
        {
            typeof(Roles.GameModes.GodImpostor), typeof(Captain), typeof(Cloak), typeof(Demon), typeof(Doctor), typeof(Foresight), typeof(Inspector), typeof(Mayor), typeof(Officer), typeof(Sheriff),
            typeof(BountyHunter), typeof(Changeling), typeof(Gangster), typeof(Jester), typeof(Jinx), typeof(Traitor), typeof(Troll)
        };

        public override bool AllowSabotage(SystemTypes? sabotage) => true;
    }
}