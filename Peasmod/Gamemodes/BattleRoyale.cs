using System.Linq;
using BepInEx.IL2CPP;
using Il2CppSystem.Collections.Generic;
using PeasAPI;
using PeasAPI.CustomEndReason;
using Peasmod.ApiExtension.Gamemodes;
using Peasmod.Utility;
using Reactor.Extensions;
using UnhollowerBaseLib;
using UnityEngine;

namespace Peasmod.Gamemodes
{
    [RegisterCustomGameMode]
    public class BattleRoyale : GameMode
    {
        public BattleRoyale(BasePlugin plugin) : base(plugin)
        {
        }

        public override string Name { get; } = $"{StringColor.Red}BattleRoyale";

        public override bool Enabled => Settings.IsGameMode(Settings.GameMode.BattleRoyale);

        public override bool HasToDoTasks { get; } = false;

        public override string GetObjective(PlayerControl player)
        {
            return "Be the last survivor!";
        }

        public override void OnGameStart()
        {
            HudManager.Instance.KillButton.enabled = true;
            HudManager.Instance.TaskStuff.SetActive(true);

            var infected = new Il2CppStructArray<byte>(1);
            infected[0] = PlayerControl.LocalPlayer.PlayerId;
            PlayerControl.LocalPlayer.SetRole(RoleTypes.Impostor);
                
            if (!AmongUsClient.Instance.AmHost)
                GameObject.Find("_Player").Destroy();
        }
        
        public override void OnUpdate()
        {
            if (PeasAPI.PeasAPI.GameStarted)
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                    if (player.PlayerId != PlayerControl.LocalPlayer.PlayerId)
                        player.SetRole(RoleTypes.Crewmate);
                
                if (PlayerControl.LocalPlayer.Data.IsDead)
                    HudManager.Instance.KillButton.gameObject.SetActive(false);
            }
        }

        public override bool OnKill(PlayerControl killer, PlayerControl victim)
        {
            if (PlayerControl.AllPlayerControls.ToArray().Count(player => !player.Data.IsDead) - 1 == 1)
            {
                var winners = new System.Collections.Generic.List<GameData.PlayerInfo>();
                winners.Add(killer.Data);
                new CustomEndReason(Palette.ImpostorRed, "Victory Royale", "Defeat", "impostor", winners);
            }
            return true;
        }

        public override List<PlayerControl> GetImpostors(List<PlayerControl> originalImpostors)
        {
            var impostors = new List<PlayerControl>();
            impostors.Add(PlayerControl.LocalPlayer);

            return impostors;
        }

        public override void OnIntro(IntroCutscene._CoBegin_d__18 _scene)
        {
            var scene = _scene.__4__this;
            
            scene.RoleText.text = "BattleRoyale";
            
            scene.ImpostorText.gameObject.SetActive(true);
            scene.ImpostorText.text = "Be the last one to survive";

            scene.BackgroundBar.material.color = Palette.ImpostorRed;
            scene.RoleText.color = Palette.ImpostorRed;
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