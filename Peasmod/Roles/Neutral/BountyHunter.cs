using System.Collections.Generic;
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
        public override string LongDescription => "";
        public override string TaskText => "Make the other players vote out your target";
        public override Color Color => ModdedPalette.BountyHunterColor;
        public override Visibility Visibility => Visibility.NoOne;
        public override Team Team => Team.Alone;
        public override bool HasToDoTasks => false;
        public override int MaxCount => 1;

        public Dictionary<byte, byte> Targets = new Dictionary<byte, byte>();

        public void ChooseTarget()
        {
            if (Utility.GetAllPlayers().Count(p => !p.Data.Role.IsImpostor && !p.Data.IsDead && !p.IsRole(this) && !Targets.ContainsValue(p.PlayerId)) == 0 && PlayerControl.LocalPlayer.IsRole(this))
            {
                PlayerControl.LocalPlayer.RpcSetRole(null);
                return;
            }

            if (PlayerControl.LocalPlayer.IsRole(this))
            {
                var target = Utility.GetAllPlayers().Where(p => !p.Data.Role.IsImpostor && !p.Data.IsDead && !p.IsRole(this) && !Targets.ContainsValue(p.PlayerId)).Random();
                if (target != null)
                {
                    SetBountyTarget(PlayerControl.LocalPlayer, target.PlayerId);
                    TextMessageManager.ShowMessage("Your Target is: " + target.name, 1f);
                }
                else
                    ChooseTarget();
            }
        }
        
        public override void OnGameStart()
        {
            Targets = new Dictionary<byte, byte>();
            ChooseTarget();
        }

        public override void OnUpdate()
        {
            try
            {
                if (Targets.ContainsKey(PlayerControl.LocalPlayer.PlayerId) && PlayerControl.LocalPlayer.IsRole(this))
                {
                    if (Targets[PlayerControl.LocalPlayer.PlayerId].GetPlayer().Data.Role.IsImpostor)
                    {
                        PlayerControl.LocalPlayer.RpcSetRole(null);
                        Targets.Remove(PlayerControl.LocalPlayer.PlayerId);
                    }
                
                    if (PlayerControl.LocalPlayer.Data.IsDead)
                        Targets.Remove(PlayerControl.LocalPlayer.PlayerId);
                    else
                        Targets[PlayerControl.LocalPlayer.PlayerId].GetPlayer().nameText.text = Color.black.ToTextColor() +
                            Targets[PlayerControl.LocalPlayer.PlayerId].GetPlayer().name + "\nTarget";
                }
            }
            catch
            {
                // ignored
            }
        }

        public override void OnMeetingUpdate(MeetingHud meeting)
        {
            if (PlayerControl.LocalPlayer.IsRole(this) && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                var playerVoteArea = meeting.playerStates.Where(p => p.TargetPlayerId == Targets[PlayerControl.LocalPlayer.PlayerId]).ToList()[0];
                playerVoteArea.NameText.color = Color.black;
                playerVoteArea.NameText.text = $"{Targets[PlayerControl.LocalPlayer.PlayerId].GetPlayer().name}\nTarget";
            }
        }

        public override void OnKill(PlayerControl killer, PlayerControl victim)
        {
            if (PlayerControl.LocalPlayer.IsRole(this) && !PlayerControl.LocalPlayer.Data.IsDead && victim.PlayerId == Targets[PlayerControl.LocalPlayer.PlayerId])
                ChooseTarget();
        }

        public override void OnExiled(PlayerControl victim)
        {
            if (PlayerControl.LocalPlayer.IsRole(this) && !PlayerControl.LocalPlayer.Data.IsDead &&
                victim.PlayerId == Targets[PlayerControl.LocalPlayer.PlayerId])
                new CustomEndReason(PlayerControl.LocalPlayer);
        }

        //[MethodRpc((uint) CustomRpcCalls.SetBountyTarget, LocalHandling = RpcLocalHandling.Before)]
        public void SetBountyTarget(PlayerControl player, byte targetId)
        {
            if (Targets.ContainsKey(player.PlayerId))
                Targets[player.PlayerId] = targetId;
            else
                Targets.Add(player.PlayerId, targetId);
        }
    }
}