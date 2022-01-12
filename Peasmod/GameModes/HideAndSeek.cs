using System;
using System.Collections;
using System.Linq;
using BepInEx.IL2CPP;
using HarmonyLib;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomEndReason;
using PeasAPI.GameModes;
using PeasAPI.Managers;
using Peasmod.Roles.GameModes;
using Reactor.Extensions;
using Reactor.Networking;
using Reactor.Networking.MethodRpc;
using TMPro;
using UnityEngine;
using Object = Il2CppSystem.Object;

namespace Peasmod.GameModes
{
    [RegisterCustomGameMode]
    public class HideAndSeek : GameMode
    {
        public HideAndSeek(BasePlugin plugin) : base(plugin)
        {
            Instance = this;
        }

        public override string Name => $"{PeasAPI.Utility.StringColor.Orange}Hide and Seek";

        public override bool Enabled => GameModeManager.IsGameModeActive(this);

        public override bool HasToDoTasks => false;

        public override bool AllowVanillaRoles => false;

        public override Type[] RoleWhitelist { get; } = {
            typeof(Seeker)
        };
        
        public override bool AllowSabotage(SystemTypes? sabotage) => false;

        private static bool IsFroozen = false;
        public static HideAndSeek Instance;

        private TextMeshPro _timeLeftText;
        public float TimeLeft = float.MaxValue;
        public bool SeekingStarted = false;

        public override void OnGameStart()
        {
            SeekingStarted = false;
            TimeLeft = Settings.SeekerDuration.Value;
            Reactor.Coroutines.Start(CoStartGame());
        }

        public override Data.CustomIntroScreen? GetIntroScreen(PlayerControl player)
        {
            if (player.Data.Role.IsImpostor)
                return new Data.CustomIntroScreen(true, "Hide and Seek", "", Palette.Orange);
            return new Data.CustomIntroScreen(true, "Hide and Seek", "", Palette.Orange, null,
                true, "Hider", "Stay hidden", Palette.Orange);
        }

        public override void OnUpdate()
        {
            if (PlayerControl.LocalPlayer != null && PlayerControl.LocalPlayer.Data != null &&
                PlayerControl.LocalPlayer.Data.Role.IsImpostor)
                HudManager.Instance.KillButton.SetCoolDown(0f, 1f);

            if (IsFroozen)
            {
                PlayerControl.LocalPlayer.moveable = false;
                PlayerControl.LocalPlayer.MyPhysics.ResetAnimState();
                PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();
                PlayerControl.LocalPlayer.MyPhysics.body.velocity = Vector2.zero;
            }
            
            if (SeekingStarted)
            {
                TimeLeft -= Time.deltaTime;
                if (TimeLeft > 0)
                {
                    if (_timeLeftText == null)
                    {
                        var pingTracker = GameObject.Find("PingTrackerTMP");
                        var gameObject = UnityEngine.Object.Instantiate(pingTracker, HudManager.Instance.transform);
                        gameObject.GetComponent<PingTracker>().Destroy();
                        gameObject.transform.localPosition = pingTracker.transform.localPosition - new Vector3(1.8f, 0f);
                        _timeLeftText = gameObject.GetComponent<TextMeshPro>();
                    }

                    var seconds = Math.Floor(TimeLeft) - Math.Floor(TimeLeft / 60) * 60;
                    var formatedTime = Math.Floor(TimeLeft / 60) + ":" + (seconds < 10 ? "0" + seconds : seconds);
                    _timeLeftText.text = $"Time Left: {formatedTime}";
                }
                else if (PlayerControl.LocalPlayer.Data.Role.IsImpostor)
                {
                    new CustomEndReason(Palette.CrewmateBlue, "Victory", "Defeat", "crew",
                        PlayerControl.AllPlayerControls.ToArray().Where(player => !player.Data.Role.IsImpostor).ToList()
                            .ConvertAll(player => player.Data));
                }
            }
        }

        public override void OnKill(PlayerControl killer, PlayerControl victim)
        {
            ShipStatus.RpcEndGame(GameOverReason.ImpostorByKill, false);
        }

        public override bool OnMeetingCall(PlayerControl caller, GameData.PlayerInfo target)
        {
            return false;
        }

        public override string GetObjective(PlayerControl player)
        {
            if (player.Data.Role.IsImpostor)
                return "Find all the players";
            return "Hide from the Seekers";
        }

        public override bool ShouldGameStop(GameOverReason reason)
        {
            if (reason == GameOverReason.HumansDisconnect || reason == GameOverReason.ImpostorDisconnect || reason == GameOverReason.HumansByVote || reason == (GameOverReason) 255)
                return true;
            
            var aliveImpostors = 0;
            var aliveCrewmates = 0;

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!player.Data.IsDead && !player.Data.Disconnected)
                {
                    if (player.Data.Role.IsImpostor)
                        aliveImpostors++;
                    else
                        aliveCrewmates++;
                }
            }

            if (aliveCrewmates > 0)
                return false;

            return true;
        }

        private IEnumerator CoStartGame()
        {
            yield return new WaitForSeconds(0.2f);
            if (PlayerControl.LocalPlayer.Data.Role.IsImpostor)
            {
                HudManager.Instance.TaskStuff.SetActive(true);

                PlayerControl.LocalPlayer.MyPhysics.Speed += 0.2f;
                
                IsFroozen = true;
                PlayerControl.LocalPlayer.moveable = false;
                PlayerControl.LocalPlayer.MyPhysics.ResetAnimState();
                PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();
                PlayerControl.LocalPlayer.MyPhysics.body.velocity = Vector2.zero;
                
                yield return new WaitForSeconds(Settings.SeekerCooldown.Value);

                RpcStartSeeking(PlayerControl.LocalPlayer, true);
                
                IsFroozen = false;
                PlayerControl.LocalPlayer.moveable = true;
                TextMessageManager.RpcShowMessage("The seeker can\nseek now!", 1f, PlayerControl.AllPlayerControls.ToArray().ToList());
            }
        }

        [MethodRpc((uint) CustomRpcCalls.StartHideAndSeek, LocalHandling = RpcLocalHandling.After)]
        public static void RpcStartSeeking(PlayerControl sender, bool dummy)
        {
            Instance.SeekingStarted = true;
        }

        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
        class ShipStatusCalculateLightRadiusPatch
        {
            public static bool Prefix(ShipStatus __instance, ref float __result,
                [HarmonyArgument(0)] GameData.PlayerInfo player)
            {
                if (GameModeManager.IsGameModeActive(Instance) && player.Role.IsImpostor)
                {
                    if (player == null || player.IsDead)
                    {
                        __result = __instance.MaxLightRadius;
                    }

                    if (player.Role.IsImpostor)
                    {
                        __result = __instance.MaxLightRadius * 0.5f;
                    }

                    return false;
                }

                return true;
            }
        }
    }
}