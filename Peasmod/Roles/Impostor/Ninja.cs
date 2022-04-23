using System.Collections.Generic;
using BepInEx.IL2CPP;
using HarmonyLib;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.Options;
using PeasAPI.Roles;
using Reactor.Networking;
using Reactor.Networking.MethodRpc;
using UnityEngine;

namespace Peasmod.Roles.Impostor
{
    [RegisterCustomRole]
    public class Ninja : BaseRole
    {
        public Ninja(BasePlugin plugin) : base(plugin)
        {
            Instance = this;
        }

        public override string Name => "Ninja";
        public override Sprite Icon => Utility.CreateSprite("Peasmod.Resources.Buttons.Hide.png", 794f);
        public override string Description => "Kill players without being seen";
        public override string LongDescription => "";
        public override string TaskText => "Kill players without being seen by using your ability";
        public override Color Color => Palette.ImpostorRed;
        public override Visibility Visibility => Visibility.Impostor;
        public override Team Team => Team.Impostor;
        public override bool HasToDoTasks => false;
        public override int MaxCount => 3;

        public override Dictionary<string, CustomOption> AdvancedOptions { get; set; } = new Dictionary<string, CustomOption>()
        {
            {
                "InvisibilityCooldown", new CustomNumberOption("invisibilitycooldown", $"Invisibility-Cooldown", 20, 60, 1, 20,
                    NumberSuffixes.Seconds)
            },
            {
                "InvisibilityDuration", new CustomNumberOption("invisibilityduration", $"Invisibility-Duration", 10, 30, 1, 10,
                    NumberSuffixes.Seconds)
            }
        };
        public override bool CanVent => true;
        public override bool CanKill(PlayerControl victim = null) => !victim || !victim.Data.Role.IsImpostor;
        public override bool CanSabotage(SystemTypes? sabotage) => true;

        public static Ninja Instance;
        
        public CustomButton Button;
        private List<byte> _invisiblePlayers = new List<byte>();

        public override void OnGameStart()
        {
            _invisiblePlayers.Clear();
            Button = CustomButton.AddButton(
                () => { RpcGoInvisible(PlayerControl.LocalPlayer, true); },
                ((CustomNumberOption) AdvancedOptions["InvisibilityCooldown"]).Value, Utility.CreateSprite("Peasmod.Resources.Buttons.Hide.png", 794f), p => p.IsRole(this) && !p.Data.IsDead, _ => true, effectDuration: ((CustomNumberOption) AdvancedOptions["InvisibilityDuration"]).Value,
                onEffectEnd: () => { RpcGoInvisible(PlayerControl.LocalPlayer, false); }, text: "<size=40%>Hide");
        }

        public override void OnUpdate()
        {
            foreach (var _player in _invisiblePlayers)
            {
                var player = _player.GetPlayer();
                if (player.IsLocal())
                {
                    player.MyRend.color =
                        player.MyRend.color.SetAlpha(0.5f);
                    player.SetHatAndVisorAlpha(0.5f);
                    player.MyPhysics.Skin.layer.color =
                        player.MyPhysics.Skin.layer.color.SetAlpha(0.5f);
                }
                else
                {
                    player.Visible = false;
                }
            }
        }

        [MethodRpc((uint)CustomRpcCalls.GoInvisible, LocalHandling = RpcLocalHandling.Before)]
        public static void RpcGoInvisible(PlayerControl sender, bool enable)
        {
            if (enable)
                Instance._invisiblePlayers.Add(sender.PlayerId);
            else 
                Instance._invisiblePlayers.Remove(sender.PlayerId);
            
            if (sender.IsLocal())
            {
                sender.MyRend.color =
                    sender.MyRend.color.SetAlpha(enable ? 0.5f : 1f);
                sender.SetHatAndVisorAlpha(enable ? 0.5f : 1f);
                sender.MyPhysics.Skin.layer.color =
                    sender.MyPhysics.Skin.layer.color.SetAlpha(enable ? 0.5f : 1f);
            }
            else
            {
                sender.Visible = !enable;
            }
        }

        [HarmonyPatch]
        private static class Patches
        {
            [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Revive))]
            [HarmonyPrefix]
            private static void PlayerControlRevivePatch(PlayerControl __instance)
            {
                __instance.SetHatAndVisorAlpha(1f);
            }
            
            [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ResetAnimState))]
            [HarmonyPrefix]
            private static bool PlayerPhysicsResetAnimStatePatch(PlayerPhysics __instance)
            {
                if (__instance.myPlayer)
                {
                    __instance.myPlayer.FootSteps.Stop();
                    __instance.myPlayer.FootSteps.loop = false;
                    __instance.myPlayer.HatRenderer.SetIdleAnim();
                    __instance.myPlayer.VisorSlot.gameObject.SetActive(true);
                }

                GameData.PlayerInfo data = __instance.myPlayer.Data;
                if (data != null)
                {
                    __instance.myPlayer.HatRenderer.SetColor(__instance.myPlayer.CurrentOutfit.ColorId);
                }

                if (data == null || !data.IsDead)
                {
                    __instance.Skin.SetIdle(__instance.rend.flipX);
                    __instance.Animator.Play(__instance.CurrentAnimationGroup.IdleAnim, 1f);
                    __instance.myPlayer.Visible = true;
                    if (!Instance._invisiblePlayers.Contains(__instance.myPlayer.PlayerId)) // Don't do that
                        __instance.myPlayer.SetHatAndVisorAlpha(1f);
                    return false;
                }

                __instance.Skin.SetGhost();
                if (data != null && data.Role != null)
                {
                    __instance.Animator.Play((data.Role.Role == RoleTypes.GuardianAngel) ? __instance.CurrentAnimationGroup.GhostGuardianAngelAnim : __instance.CurrentAnimationGroup.GhostIdleAnim, 1f);
                }

                PlayerControl playerControl = __instance.myPlayer;
                if (playerControl == null)
                {
                    return false;
                }

                playerControl.SetHatAndVisorAlpha(0.5f);
                return false;
            }

            [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.HandleAnimation))]
            [HarmonyPrefix]
            private static bool PlayerPhysicsHandleAnimationPatch(PlayerPhysics __instance,
                [HarmonyArgument(0)] bool amDead)
            {
                if (__instance.Animator.IsPlaying(__instance.CurrentAnimationGroup.SpawnAnim))
                    return false;
                
                if (!GameData.Instance)
                    return false;

                Vector2 velocity = __instance.body.velocity;
                AnimationClip currentAnimation = __instance.Animator.GetCurrentAnimation();
                if (currentAnimation == __instance.CurrentAnimationGroup.ClimbAnim || currentAnimation == __instance.CurrentAnimationGroup.ClimbDownAnim)
                    return false;

                if (!amDead)
                {
                    if (velocity.sqrMagnitude >= 0.05f)
                    {
                        bool flipX = __instance.rend.flipX;
                        if (velocity.x < -0.01f)
                        {
                            __instance.rend.flipX = true;
                        }
                        else if (velocity.x > 0.01f)
                        {
                            __instance.rend.flipX = false;
                        }
                        if (currentAnimation != __instance.CurrentAnimationGroup.RunAnim || flipX != __instance.rend.flipX)
                        {
                            __instance.Animator.Play(__instance.CurrentAnimationGroup.RunAnim, 1f);
                            if (!Constants.ShouldHorseAround())
                            {
                                __instance.Animator.Time = 0.45833334f;
                            }
                            __instance.Skin.SetRun(__instance.rend.flipX);
                        }
                    }
                    else if (currentAnimation == __instance.CurrentAnimationGroup.RunAnim || currentAnimation == __instance.CurrentAnimationGroup.SpawnAnim || !currentAnimation)
                    {
                        __instance.Skin.SetIdle(__instance.rend.flipX);
                        __instance.Animator.Play(__instance.CurrentAnimationGroup.IdleAnim, 1f);
                        if (!Instance._invisiblePlayers.Contains(__instance.myPlayer.PlayerId)) //Don't do that
                            __instance.myPlayer.SetHatAndVisorAlpha(1f);
                    }
                }
                else
                {
                    __instance.Skin.SetGhost();
                    if (__instance.myPlayer.Data.Role.Role == RoleTypes.GuardianAngel)
                    {
                        if (currentAnimation != __instance.CurrentAnimationGroup.GhostGuardianAngelAnim)
                        {
                            __instance.Animator.Play(__instance.CurrentAnimationGroup.GhostGuardianAngelAnim, 1f);
                            __instance.myPlayer.SetHatAndVisorAlpha(0.5f);
                        }
                    }
                    else if (currentAnimation != __instance.CurrentAnimationGroup.GhostIdleAnim)
                    {
                        __instance.Animator.Play(__instance.CurrentAnimationGroup.GhostIdleAnim, 1f);
                        __instance.myPlayer.SetHatAndVisorAlpha(0.5f);
                    }

                    if (velocity.x < -0.01f)
                    {
                        __instance.rend.flipX = true;
                    }
                    else if (velocity.x > 0.01f)
                    {
                        __instance.rend.flipX = false;
                    }
                }

                __instance.Skin.Flipped = __instance.rend.flipX;
                __instance.myPlayer.VisorSlot.SetFlipX(__instance.rend.flipX);
                return false;
            }
        }
    }
}