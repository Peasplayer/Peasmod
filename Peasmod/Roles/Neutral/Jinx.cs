using System.Linq;
using BepInEx.IL2CPP;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomEndReason;
using PeasAPI.Roles;
using UnityEngine;

namespace Peasmod.Roles.Neutral
{
    [RegisterCustomRole]
    public class Jinx : BaseRole
    {
        public Jinx(BasePlugin plugin) : base(plugin)
        {
        }

        public override string Name => "Jinx";
        public override string Description => "Be one of the last survivors";
        public override string LongDescription => "";
        public override string TaskText => "Be one of the last three survivors to win";
        public override Color Color => ModdedPalette.JinxColor;
        public override Visibility Visibility => Visibility.NoOne;
        public override Team Team => Team.Alone;
        public override bool HasToDoTasks => false;

        public override void OnUpdate()
        {
            if (Utility.GetAllPlayers().Count(p => !p.Data.IsDead && !p.Data.Disconnected) == 3 && 
                Utility.GetAllPlayers().Count(p => !p.Data.IsDead && !p.Data.Disconnected && p.Data.Role.IsImpostor) >= 1 &&
                PlayerControl.LocalPlayer.IsRole(this) && !PlayerControl.LocalPlayer.Data.IsDead)
                new CustomEndReason(PlayerControl.LocalPlayer);
        }
    }
}