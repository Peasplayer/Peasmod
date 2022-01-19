using BepInEx.IL2CPP;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.Roles;
using UnityEngine;

namespace Peasmod.Roles.Neutral
{
    [RegisterCustomRole]
    public class Traitor : BaseRole
    {
        public Traitor(BasePlugin plugin) : base(plugin)
        {
        }

        public override string Name => "Traitor";
        public override string Description => "Betray the crewmates";
        public override string LongDescription => "";
        public override string TaskText => "Betray the crewmates after you completed your tasks";
        public override Color Color => Palette.ImpostorRed;
        public override Visibility Visibility => HasBetrayed ? Visibility.Impostor : Visibility.NoOne;
        public override Team Team => HasBetrayed ? Team.Impostor : Team.Role;
        public override bool HasToDoTasks => true;
        public override bool CanKill(PlayerControl victim = null) => HasBetrayed;
        public override bool CanVent => HasBetrayed;
        public override bool CanSabotage(SystemTypes? sabotage) => HasBetrayed;

        public bool HasBetrayed;

        public override void OnGameStart()
        {
            HasBetrayed = false;
        }

        public override void OnTaskComplete(PlayerControl player, PlayerTask task)
        {
            if (player.IsLocal() && player.IsRole(this) && player.AllTasksCompleted())
            {
                player.RpcSetVanillaRole(RoleTypes.Impostor);
                HasBetrayed = true;
            }
        }
    }
}