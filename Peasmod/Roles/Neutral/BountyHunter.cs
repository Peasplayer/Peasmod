using System.Linq;
using BepInEx.IL2CPP;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomEndReason;
using PeasAPI.Managers;
using PeasAPI.Roles;
using Reactor.Extensions;
using UnityEngine;

namespace Peasmod.Roles.Neutral
{
    [RegisterCustomRole]
    public class BountyHunter : BaseRole
    {
        public BountyHunter(BasePlugin plugin) : base(plugin)
        {
        }

        public override string Name => "Bounty Hunter";
        public override string Description => "Get your target voted out";
        public override string TaskText => "Make the other players vote out your target";
        public override Color Color => ModdedPalette.BountyHunterColor;
        public override Visibility Visibility => Visibility.NoOne;
        public override Team Team => Team.Alone;
        public override bool HasToDoTasks => false;
        public override int Limit => (int) Settings.BountyHunterAmount.Value;

        public byte Target;

        public override void OnGameStart()
        {
            if (Utility.GetAllPlayers().Count(p => !p.Data.Role.IsImpostor && !p.IsRole(this)) == 0 && PlayerControl.LocalPlayer.IsRole(this))
            {
                PlayerControl.LocalPlayer.RpcSetRole(null);
                return;
            }

            if (PlayerControl.LocalPlayer.IsRole(this))
            {
                var target = Utility.GetAllPlayers().Where(p => !p.Data.Role.IsImpostor && !p.IsRole(this)).Random();
                if (target != null)
                {
                    Target = target.PlayerId;
                    TextMessageManager.ShowMessage("Your Target is: " + target.name, 1f);
                }
                else
                    OnGameStart();
            }
        }

        public override void OnUpdate()
        {
            if (Target != default && PlayerControl.LocalPlayer.IsRole(this))
                Target.GetPlayer().nameText.text = Color.black.ToTextColor() + Target.GetPlayer().name + "\nTarget";
        }

        public override void OnKill(PlayerControl killer, PlayerControl victim)
        {
            if (PlayerControl.LocalPlayer.IsRole(this) && !PlayerControl.LocalPlayer.Data.IsDead && victim.PlayerId == Target)
                OnGameStart();
        }

        public override void OnExiled(PlayerControl victim)
        {
            if (PlayerControl.LocalPlayer.IsRole(this) && !PlayerControl.LocalPlayer.Data.IsDead &&
                victim.PlayerId == Target)
                new CustomEndReason(PlayerControl.LocalPlayer);
        }
    }
}