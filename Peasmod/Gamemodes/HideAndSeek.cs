using System.Collections;
using System.Collections.Generic;
using BepInEx.IL2CPP;
using HarmonyLib;
using PeasAPI;
using PeasAPI.Roles;
using Peasmod.ApiExtension.Gamemodes;
using Peasmod.Utility;
using UnityEngine;

namespace Peasmod.Gamemodes
{
    [RegisterCustomGameMode]
    public class HideAndSeek : GameMode
    {
        public HideAndSeek(BasePlugin plugin) : base(plugin)
        {
        }
        
        public override string Name { get; } = $"{StringColor.Orange}Hide and Seek";

        public override bool Enabled => Settings.IsGameMode(Settings.GameMode.HideAndSeek);

        public override bool HasToDoTasks { get; } = false;

        public override void OnGameStart()
        {
            
        }

        public override void OnIntro(IntroCutscene._CoBegin_d__14 _scene)
        {
            var scene = _scene.__4__this;
            
            scene.Title.text = "Hider";
            if (PlayerControl.LocalPlayer.Data.IsImpostor)
                scene.Title.text = "Seeker";
            
            scene.ImpostorText.gameObject.SetActive(true);
            scene.ImpostorText.text = "This is the Seeker:";
            if (PlayerControl.LocalPlayer.Data.IsImpostor)
                scene.ImpostorText.text = "Find everyone";
            
            if (PlayerControl.LocalPlayer.Data.IsImpostor)
            {
                HudManager.Instance.TaskStuff.SetActive(true);
                
                IsFroozen = true;
                PlayerControl.LocalPlayer.moveable = false;
                PlayerControl.LocalPlayer.MyPhysics.ResetAnimState();
                PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();
                PlayerControl.LocalPlayer.MyPhysics.body.velocity = Vector2.zero;
                if (!CoRoutineStarted)
                    Reactor.Coroutines.Start(SeekerCooldown(Settings.SeekerCooldown.GetValue()));
            }
        }

        public override Il2CppSystem.Collections.Generic.List<PlayerControl> GetIntroTeam()
        {
            var team = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            
            foreach (var hider in RoleManager.Impostors)
            {
                team.Add(hider.GetPlayer());
            }

            return team;
        }

        public override void OnUpdate()
        {
            if (PlayerControl.LocalPlayer != null && PlayerControl.LocalPlayer.Data != null && PlayerControl.LocalPlayer.Data.IsImpostor)
                HudManager.Instance.KillButton.SetCoolDown(0f, 1f);
            
            if (IsFroozen)
            {
                PlayerControl.LocalPlayer.moveable = false;
                PlayerControl.LocalPlayer.MyPhysics.ResetAnimState();
                PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();
                PlayerControl.LocalPlayer.MyPhysics.body.velocity = Vector2.zero;
            }
        }

        public override bool OnKill(PlayerControl killer, PlayerControl victim)
        {
            ShipStatus.RpcEndGame(GameOverReason.ImpostorByKill, false);
            return true;
        }
        
        public override bool OnMeetingCall(PlayerControl caller, GameData.PlayerInfo target)
        {
            return false;
        }

        public override string GetObjective(PlayerControl player)
        {
            if (player.Data.IsImpostor)
                return "Find every Crewmate";
            return "Hide from the Seekers";
        }
        
        public override bool ShouldGameStop(GameOverReason reason)
        {
            var aliveImpostors = 0;
            var aliveCrewmates = 0;

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!player.Data.IsDead && !player.Data.Disconnected)
                {
                    if (player.Data.IsImpostor)
                        aliveImpostors++;
                    else
                        aliveCrewmates++;
                }
            }
            Utils.Log(aliveCrewmates + " - " + aliveImpostors);
            if (!(aliveCrewmates == 0 && aliveImpostors > 0))
                return false;
            
            return reason != GameOverReason.HumansByTask;
        }

        private static bool IsFroozen = false;
        private static bool CoRoutineStarted = false;

        private IEnumerator SeekerCooldown(float seconds)
        {
            CoRoutineStarted = true;
            
            yield return new WaitForSeconds(seconds);
            
            IsFroozen = false;
            PlayerControl.LocalPlayer.moveable = true;
            CoRoutineStarted = false;
        }
        
        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
        class ShipStatusCalculateLightRadiusPatch
        {
            public static bool Prefix(ShipStatus __instance, ref float __result, [HarmonyArgument(0)] GameData.PlayerInfo player)
            {
                foreach (var mode in GameModeManager.Modes)
                {
                    if (mode.Enabled && mode.Name == $"{StringColor.Orange}Hide and Seek" && player.IsImpostor)
                    {
                        if (player == null || player.IsDead)
                        {
                            __result = __instance.MaxLightRadius;
                        }
                        
                        if (player.IsImpostor)
                        {
                            __result = __instance.MaxLightRadius * 0.5f;
                        }

                        return false;
                    }
                }

                return true;
            }
        }
    }
}