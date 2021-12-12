using BepInEx.IL2CPP;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomEndReason;
using PeasAPI.Roles;
using UnityEngine;

namespace Peasmod.Roles
{
    [RegisterCustomRole]
    public class Troll : BaseRole
    {
        public Troll(BasePlugin plugin) : base(plugin) { }

        public override string Name => "Troll";
        public override string Description => "Get killed by an impostor";
        public override string TaskText => "Get killed by an impostor";
        public override Color Color => ModdedPalette.TrollColor;
        public override Visibility Visibility => Visibility.NoOne;
        public override Team Team => Team.Alone;
        public override bool HasToDoTasks => false;
        public override int Limit => (int) Settings.TrollAmount.Value;

        public override void OnKill(PlayerControl victim)
        {
            if (victim.IsRole<Troll>())
                new CustomEndReason(victim);
        }
    }
}