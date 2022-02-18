using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx.IL2CPP;
using HarmonyLib;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.CustomEndReason;
using PeasAPI.GameModes;
using PeasAPI.Managers;
using Peasmod.Roles.GameModes;
using Reactor;
using Reactor.Extensions;
using Reactor.Networking;
using Reactor.Networking.MethodRpc;
using TMPro;
using UnityEngine;

namespace Peasmod.GameModes
{
    [HarmonyPatch]
    [RegisterCustomGameMode]
    public class PropHunt : GameMode
    {
        public PropHunt(BasePlugin plugin) : base(plugin)
        {
            Instance = this;
        }
        
        public override string Name => $"{Color.yellow.ToTextColor()}PropHunt";

        public override bool Enabled => GameModeManager.IsGameModeActive(this);

        public override bool HasToDoTasks => false;

        public override bool AllowVanillaRoles => false;

        public override Type[] RoleWhitelist { get; } = {
            typeof(PropHunter)
        };
        
        public override bool AllowSabotage(SystemTypes? sabotage) => false;

        private static PropHunt Instance;
        private static bool IsFroozen = false;

        private TextMeshPro _timeLeftText;
        public float TimeLeft = float.MaxValue;
        public float ClickCooldown;
        public bool SeekingStarted = false;
        public List<int> Objects = new List<int>();
        public CustomButton UnmorphButton;
        
        public override void OnGameStart()
        {
            UnmorphButton = CustomButton.AddButton(() => RpcRemoveProp(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer), 0f, Utility.CreateSprite("Peasmod.Resources.Buttons.Default.png"), 
                _ => PlayerControl.LocalPlayer.transform.FindChild("PropInUse") != null, _ => PlayerControl.LocalPlayer.transform.FindChild("PropInUse") != null, text: "<size=40%>Unmorph", textOffset: new Vector2(0f, 0.5f));
            Objects.Clear();
            foreach (var gameObject in UnityEngine.Object.FindObjectsOfType<GameObject>())
            {
                if (gameObject.layer != LayerMask.NameToLayer("ShortObjects") && !gameObject.name.ToLower().Contains("door"))
                    continue;
                if (gameObject.GetComponent<SpriteRenderer>() == null)
                    continue;
                if (gameObject.GetComponent<Collider2D>() == null)
                    continue;
                Objects.Add(gameObject.GetInstanceID());
            }
            
            SeekingStarted = false;
            TimeLeft = Settings.PropHuntSeekerDuration.Value;
            Reactor.Coroutines.Start(CoStartGame());
        }

        public override Data.CustomIntroScreen? GetIntroScreen(PlayerControl player)
        {
            if (player.Data.Role.IsImpostor)
                return new Data.CustomIntroScreen(true, "PropHunt", "", Color.yellow);
            return new Data.CustomIntroScreen(true, "PropHunt", "", Color.yellow, null,
                true, "Hider", "Stay hidden", Color.yellow);
        }

        public override void OnUpdate()
        {
            if (PlayerControl.LocalPlayer.Data.Role.IsImpostor && ClickCooldown > 0)
                ClickCooldown -= Time.deltaTime;
            
            HudManager.Instance.ReportButton.Hide();
            if (HudManager.Instance.KillButton.currentTarget != null && HudManager.Instance.KillButton.currentTarget.transform.FindChild("PropInUse") != null)
                HudManager.Instance.KillButton.SetDisabled();
            
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
            if (victim.IsLocal())
                RpcRemoveProp(PlayerControl.LocalPlayer, victim);
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
                
                yield return new WaitForSeconds(Settings.PropHuntSeekerCooldown.Value);

                RpcStartSeeking(PlayerControl.LocalPlayer);
                
                IsFroozen = false;
                PlayerControl.LocalPlayer.moveable = true;
                TextMessageManager.RpcShowMessage("The seekers can\nseek now!", 1f, PlayerControl.AllPlayerControls.ToArray().ToList());
            }
        }
        
        public static GameObject GetObject(int id)
        {
            return UnityEngine.Object.FindObjectsOfType<GameObject>().First(obj => obj.GetInstanceID() == id);
        }

        [MethodRpc((uint) CustomRpcCalls.StartPropHunt, LocalHandling = RpcLocalHandling.After)]
        public static void RpcStartSeeking(PlayerControl sender)
        {
            Instance.SeekingStarted = true;
        }
        
        [MethodRpc((uint) CustomRpcCalls.MoveProp)]
        public static void RpcMoveProp(PlayerControl sender, string direction, float value)
        {
            var prop = sender.transform.FindChild("PropInUse");
            if (direction == "x")
                prop.position += new Vector3(value, 0f);
            else
                prop.position += new Vector3(0f, value);
        }

        [MethodRpc((uint) CustomRpcCalls.RemoveProp)]
        public static void RpcRemoveProp(PlayerControl sender, PlayerControl target)
        {
            var oldProp = target.transform.FindChild("PropInUse");
            if (oldProp != null)
            {
                oldProp.name = "Prop";
                oldProp.transform.SetParent(ShipStatus.Instance.transform);
            }
            sender.Visible = true;
        }

        [MethodRpc((uint) CustomRpcCalls.ChangeProp)]
        public static void RpcChangeProp(PlayerControl sender, int objId)
        {
            var gameObject = GetObject(Instance.Objects[objId]);
            var oldProp = sender.transform.FindChild("PropInUse");
            if (oldProp != null)
            {
                oldProp.name = "Prop";
                oldProp.transform.SetParent(ShipStatus.Instance.transform);
            }

            sender.Visible = false;
            gameObject.name = "PropInUse";
            gameObject.transform.SetParent(sender.transform);
            gameObject.transform.position = sender.transform.position;
            gameObject.GetComponent<Collider2D>().isTrigger = true;
        }
        
        [HarmonyPatch]
        public static class ObjectsCanUsePatch
        {
            public static IEnumerable<MethodBase> TargetMethods()
            {
                return AccessTools.GetTypesFromAssembly(typeof(IUsable).Assembly).SelectMany(type => type.GetMethods())
                    .Where(method =>
                        method.DeclaringType != typeof(IUsable) && method.Name == "CanUse" && method.DeclaringType
                            .GetProperties().Count(prop => prop.Name == "UsableDistance") !=
                        0);
            }

            public static bool Prefix(IUsable __instance, ref float __result, [HarmonyArgument(0)] GameData.PlayerInfo pc, [HarmonyArgument(1)] ref bool canUse, [HarmonyArgument(2)] ref bool couldUse)
            {
                if (PeasAPI.PeasAPI.GameStarted && GameModeManager.IsGameModeActive(Instance))
                {
                    __result = 5;
                    return couldUse = canUse = false;
                }

                return true;
            }
        }
        
        [HarmonyPatch]
        public static class ObjectsUsePatch
        {
            public static IEnumerable<MethodBase> TargetMethods()
            {
                return AccessTools.GetTypesFromAssembly(typeof(IUsable).Assembly).SelectMany(type => type.GetMethods())
                    .Where(method =>
                        method.DeclaringType != typeof(IUsable) && method.Name == "Use" && method.DeclaringType
                            .GetProperties().Count(prop => prop.Name == "UsableDistance") !=
                        0);
            }

            public static bool Prefix(IUsable __instance)
            {
                if (PeasAPI.PeasAPI.GameStarted && GameModeManager.IsGameModeActive(Instance))
                    return false;
                return true;
            }
        }
        
        [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
        [HarmonyPrefix]
        public static void ButtonPressedPatch(KeyboardJoystick __instance)
        {
            if (!GameModeManager.IsGameModeActive(Instance))
                return;
            if (PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null)
                return;
            if (PlayerControl.LocalPlayer.Data.IsDead)
                return;
            if (PlayerControl.LocalPlayer.Data.Role != null && !PlayerControl.LocalPlayer.Data.Role.IsImpostor)
            {
                if (Input.GetKeyDown(KeyCode.I))
                {
                    var prop = PlayerControl.LocalPlayer.transform.FindChild("PropInUse");
                    if (prop != null && prop.transform.position.y <=
                        (PlayerControl.LocalPlayer.transform.position + new Vector3(0f, 0.5f)).y)
                    {
                        RpcMoveProp(PlayerControl.LocalPlayer, "y", 0.05f);
                    }
                }

                if (Input.GetKeyDown(KeyCode.K))
                {
                    var prop = PlayerControl.LocalPlayer.transform.FindChild("PropInUse");
                    if (prop != null && prop.transform.position.y >=
                        (PlayerControl.LocalPlayer.transform.position - new Vector3(0f, 0.5f)).y)
                    {
                        RpcMoveProp(PlayerControl.LocalPlayer, "y", -0.05f);
                    }
                }

                if (Input.GetKeyDown(KeyCode.J))
                {
                    var prop = PlayerControl.LocalPlayer.transform.FindChild("PropInUse");
                    if (prop != null && prop.transform.position.x >=
                        (PlayerControl.LocalPlayer.transform.position - new Vector3(0.5f, 0f)).x)
                    {
                        RpcMoveProp(PlayerControl.LocalPlayer, "x", -0.05f);
                    }
                }

                if (Input.GetKeyDown(KeyCode.L))
                {
                    var prop = PlayerControl.LocalPlayer.transform.FindChild("PropInUse");
                    if (prop != null && prop.transform.position.x <=
                        (PlayerControl.LocalPlayer.transform.position + new Vector3(0.5f, 0f)).x)
                    {
                        prop.transform.position += new Vector3(0.05f, 0f);
                        RpcMoveProp(PlayerControl.LocalPlayer, "x", 0.05f);
                    }
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                foreach (var gameObject in UnityEngine.Object.FindObjectsOfType<GameObject>())
                {
                    if (gameObject.layer != LayerMask.NameToLayer("ShortObjects") && !gameObject.name.ToLower().Contains("prop") && !gameObject.name.ToLower().Contains("door"))
                        continue;
                    if (gameObject.GetComponent<SpriteRenderer>() == null)
                        continue;
                    if (gameObject.GetComponent<Collider2D>() == null)
                        continue;
                    if (Vector2.Distance(gameObject.transform.position, PlayerControl.LocalPlayer.transform.position) <=
                        2.5f && gameObject.GetComponent<Collider2D>().OverlapPoint(mousePos))
                    {
                        if (PlayerControl.LocalPlayer.Data.Role.IsImpostor)
                        {
                            if (Instance.ClickCooldown > 0)
                                return;
                            Instance.ClickCooldown = Settings.PropHuntSeekerClickCooldown.Value;
                            
                            if (gameObject.name == "PropInUse")
                            {
                                var target = gameObject.transform.parent.gameObject.GetComponent<PlayerControl>();
                                RpcRemoveProp(PlayerControl.LocalPlayer, target);
                                PlayerControl.LocalPlayer.RpcMurderPlayer(target);
                            }
                        }
                        else
                        {
                            if (gameObject.name == "PropInUse")
                                continue;
                            
                            RpcChangeProp(PlayerControl.LocalPlayer, Instance.Objects.FindIndex(obj => obj == gameObject.GetInstanceID()));
                        }
                    }
                }
            }
        }
        
        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
        [HarmonyPrefix]
        public static bool BlindSeekerPatch(ShipStatus __instance, ref float __result,
            [HarmonyArgument(0)] GameData.PlayerInfo player)
        {
            if (GameModeManager.IsGameModeActive(Instance) && player.Role.IsImpostor && IsFroozen)
            {
                if (player == null || player.IsDead)
                {
                    __result = __instance.MaxLightRadius;
                }

                if (player.Role.IsImpostor)
                {
                    __result = 0.1f;
                }

                return false;
            }

            return true;
        }
    }
}