using System;
using BepInEx.Unity.IL2CPP;
using PeasAPI.Components;
using PeasAPI.Roles;
using Peasmod.GameModes;
using UnityEngine;

namespace Peasmod.Roles.GameModes
{
    [RegisterCustomRole]
    public class HideAndSeeker : BaseRole
    {
        public HideAndSeeker(BasePlugin plugin) : base(plugin)
        {
        }

        public override string Name => "Seeker";
        public override string Description => "Find all the players";
        public override string LongDescription => "";
        public override string TaskText => "Find all the players";
        public override Color Color => Palette.ImpostorRed;
        public override Visibility Visibility => Visibility.Crewmate;
        public override Team Team => Team.Impostor;
        public override bool HasToDoTasks => false;
        public override int Count => 3;
        public override int MaxCount => 3;
        public override bool CreateRoleOption => false;
        public override Type[] GameModeWhitelist { get; } = {
            typeof(HideAndSeek)
        };
        public override bool CanKill(PlayerControl victim = null) => true;
        public override bool CanVent => Settings.HideAndSeekSeekerVenting.Value;
    }
}