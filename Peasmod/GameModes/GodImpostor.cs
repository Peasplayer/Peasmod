using System;
using BepInEx.IL2CPP;
using PeasAPI.Components;
using PeasAPI.GameModes;
using Peasmod.Roles;

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
            typeof(Roles.GameModes.GodImpostor), typeof(Captain), typeof(Demon), typeof(Doctor), typeof(Inspector),
            typeof(Jester), typeof(Mayor), typeof(Sheriff), typeof(Troll)
        };

        public override bool AllowSabotage(SystemTypes? sabotage) => true;
    }
}