using System.Collections.Generic;
using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.Options;
using PeasAPI.Roles;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
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
                "InvisibilityCooldown", new CustomNumberOption(MultiMenu.Impostor, $"Invisibility-Cooldown", 20, 60, 1, 20,
                    CustomOption.Seconds)
            },
            {
                "InvisibilityDuration", new CustomNumberOption(MultiMenu.Impostor, $"Invisibility-Duration", 10, 30, 1, 10,
                    CustomOption.Seconds)
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
                ((CustomNumberOption) AdvancedOptions["InvisibilityCooldown"]).Value, Utility.CreateSprite("Peasmod.Resources.Buttons.Hide.png", 794f), p => p.IsCustomRole(this) && !p.Data.IsDead, _ => true, effectDuration: ((CustomNumberOption) AdvancedOptions["InvisibilityDuration"]).Value,
                onEffectEnd: () => { RpcGoInvisible(PlayerControl.LocalPlayer, false); }, text: "<size=40%>Hide");
        }

        public override void OnUpdate()
        {
            foreach (var _player in _invisiblePlayers)
            {
                var player = _player.GetPlayer();
                if (player.IsLocal())
                {
                    player.cosmetics.currentBodySprite.BodySprite.color =
                        player.cosmetics.currentBodySprite.BodySprite.color.SetAlpha(0.5f);
                    player.SetHatAndVisorAlpha(0.5f);
                    player.MyPhysics.myPlayer.cosmetics.skin.layer.color =
                        player.MyPhysics.myPlayer.cosmetics.skin.layer.color.SetAlpha(0.5f);
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
                sender.cosmetics.currentBodySprite.BodySprite.color =
                    sender.cosmetics.currentBodySprite.BodySprite.color.SetAlpha(enable ? 0.5f : 1f);
                sender.SetHatAndVisorAlpha(enable ? 0.5f : 1f);
                sender.MyPhysics.myPlayer.cosmetics.skin.layer.color =
                    sender.MyPhysics.myPlayer.cosmetics.skin.layer.color.SetAlpha(enable ? 0.5f : 1f);
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
                    __instance.myPlayer.cosmetics.hat.SetIdleAnim(__instance.myPlayer.CurrentOutfit.ColorId);
                    __instance.myPlayer.cosmetics.visor.gameObject.SetActive(true);
                }

                NetworkedPlayerInfo data = __instance.myPlayer.Data;
                if (data != null)
                {
                    __instance.myPlayer.cosmetics.SetHatColor(__instance.myPlayer.CurrentOutfit.ColorId);
                }

                if (data == null || !data.IsDead)
                {
                    __instance.myPlayer.cosmetics.skin.SetIdle(__instance.FlipX);
                    __instance.Animations.Animator.Play(__instance.Animations.group.IdleAnim, 1f);
                    __instance.myPlayer.Visible = true;
                    if (!Instance._invisiblePlayers.Contains(__instance.myPlayer.PlayerId)) // Don't do that
                        __instance.myPlayer.SetHatAndVisorAlpha(1f);
                    return false;
                }

                __instance.myPlayer.cosmetics.skin.SetGhost();
                if (data != null && data.Role != null)
                {
                    __instance.Animations.Animator.Play((data.Role.Role == RoleTypes.GuardianAngel) ? __instance.Animations.group.GhostGuardianAngelAnim : __instance.Animations.group.GhostIdleAnim, 1f);
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
                if (__instance.Animations.Animator.IsPlaying(__instance.Animations.group.SpawnAnim))
                    return false;
                
                if (!GameData.Instance)
                    return false;

                Vector2 velocity = __instance.body.velocity;
                AnimationClip currentAnimation = __instance.Animations.Animator.GetCurrentAnimation();
                if (currentAnimation == __instance.Animations.group.ClimbUpAnim || currentAnimation == __instance.Animations.group.ClimbDownAnim)
                    return false;

                if (!amDead)
                {
                    if (velocity.sqrMagnitude >= 0.05f)
                    {
                        bool flipX = __instance.FlipX;
                        if (velocity.x < -0.01f)
                        {
                            __instance.FlipX = true;
                        }
                        else if (velocity.x > 0.01f)
                        {
                            __instance.FlipX = false;
                        }
                        if (currentAnimation != __instance.Animations.group.RunAnim || flipX != __instance.FlipX)
                        {
                            __instance.Animations.Animator.Play(__instance.Animations.group.RunAnim, 1f);
                            if (!AprilFoolsMode.ShouldHorseAround())
                            {
                                __instance.Animations.Animator.Time = 0.45833334f;
                            }
                            __instance.myPlayer.cosmetics.skin.SetRun(__instance.FlipX);
                        }
                    }
                    else if (currentAnimation == __instance.Animations.group.RunAnim || currentAnimation == __instance.Animations.group.SpawnAnim || !currentAnimation)
                    {
                        __instance.myPlayer.cosmetics.skin.SetIdle(__instance.FlipX);
                        __instance.Animations.Animator.Play(__instance.Animations.group.IdleAnim, 1f);
                        if (!Instance._invisiblePlayers.Contains(__instance.myPlayer.PlayerId)) //Don't do that
                            __instance.myPlayer.SetHatAndVisorAlpha(1f);
                    }
                }
                else
                {
                    __instance.myPlayer.cosmetics.skin.SetGhost();
                    if (__instance.myPlayer.Data.Role.Role == RoleTypes.GuardianAngel)
                    {
                        if (currentAnimation != __instance.Animations.group.GhostGuardianAngelAnim)
                        {
                            __instance.Animations.Animator.Play(__instance.Animations.group.GhostGuardianAngelAnim, 1f);
                            __instance.myPlayer.SetHatAndVisorAlpha(0.5f);
                        }
                    }
                    else if (currentAnimation != __instance.Animations.group.GhostIdleAnim)
                    {
                        __instance.Animations.Animator.Play(__instance.Animations.group.GhostIdleAnim, 1f);
                        __instance.myPlayer.SetHatAndVisorAlpha(0.5f);
                    }

                    if (velocity.x < -0.01f)
                    {
                        __instance.FlipX = true;
                    }
                    else if (velocity.x > 0.01f)
                    {
                        __instance.FlipX = false;
                    }
                }

                __instance.myPlayer.cosmetics.skin.Flipped = __instance.FlipX;
                __instance.myPlayer.cosmetics.visor.SetFlipX(__instance.FlipX);
                return false;
            }
        }
    }
}