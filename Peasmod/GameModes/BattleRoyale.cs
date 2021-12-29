using System.Linq;
using BepInEx.IL2CPP;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomEndReason;
using PeasAPI.GameModes;
using Reactor.Extensions;
using UnityEngine;

namespace Peasmod.GameModes
{
    [RegisterCustomGameMode]
    public class BattleRoyale : GameMode
    {
        public BattleRoyale(BasePlugin plugin) : base(plugin)
        {
            Instance = this;
        }

        public override string Name => $"{Utility.StringColor.Red}BattleRoyale";

        public override bool HasToDoTasks => false;

        public static BattleRoyale Instance;

        public override string GetObjective(PlayerControl player)
        {
            return "Be the last survivor!";
        }

        public override void OnGameStart()
        {
            HudManager.Instance.KillButton.enabled = true;
            HudManager.Instance.TaskStuff.SetActive(true);

            if (!AmongUsClient.Instance.AmHost)
                GameObject.Find("_Player").Destroy();
        }

        public override void OnUpdate()
        {
            if (PeasAPI.PeasAPI.GameStarted)
            {
                HudManager.Instance.KillButton.ToggleVisible(!PlayerControl.LocalPlayer.Data.IsDead);
            }
        }

        public override void OnKill(PlayerControl killer, PlayerControl victim)
        {
            if (PlayerControl.AllPlayerControls.ToArray().Count(player => !player.Data.IsDead && player.PlayerId != victim.PlayerId) == 1 && killer.IsLocal())
            {
                var winners = new System.Collections.Generic.List<GameData.PlayerInfo>();
                winners.Add(killer.Data);
                new CustomEndReason(Palette.ImpostorRed, "Victory Royale", "Defeat", "impostor", winners);
            }
        }

        public override Data.CustomIntroScreen? GetIntroScreen(PlayerControl player)
        {
            return new Data.CustomIntroScreen(true, "BattleRoyale", "", Palette.ImpostorRed,
                new System.Collections.Generic.List<byte> { PlayerControl.LocalPlayer.PlayerId },
                player.GetRole() == null, "Survivor", "Be the last one to survive", Palette.ImpostorRed);
        }

        public override RoleTypes? AssignLocalRole(PlayerControl player)
        {
            if (player.IsLocal())
                return RoleTypes.Impostor;
            return RoleTypes.Crewmate;
        }

        public override void AssignRoles()
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!player.IsLocal())
                    player.SetRole(RoleTypes.Crewmate);
            }
        }

        public override bool OnMeetingCall(PlayerControl caller, GameData.PlayerInfo target)
        {
            return false;
        }

        public override bool ShouldGameStop(GameOverReason reason)
        {
            return reason == EndReasonManager.CustomGameOverReason;
        }
    }
}