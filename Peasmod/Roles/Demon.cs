using System.Collections;
using BepInEx.IL2CPP;
using HarmonyLib;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.Roles;
using Reactor;
using Reactor.Extensions;
using Reactor.Networking.MethodRpc;
using UnityEngine;

namespace Peasmod.Roles
{
    [RegisterCustomRole]
    public class Demon : BaseRole
    {
        public Demon(BasePlugin plugin) : base(plugin)
        {
            Instance = this;
        }

        public override string Name => "Demon";
        public override string Description => "Swap into the afterlife";
        public override string TaskText => "Swap into the afterlife";
        public override Color Color => ModdedPalette.DemonColor;
        public override Visibility Visibility => Visibility.NoOne;
        public override Team Team => Team.Crewmate;
        public override bool HasToDoTasks => true;
        public override int Limit => (int)Settings.DemonAmount.Value;

        public static Demon Instance;
        
        public CustomButton Button;
        public bool IsSwaped;

        public override void OnGameStart()
        {
            Button = CustomButton.AddRoleButton(() =>
                {
                    IsSwaped = true;
                    PlayerControl.LocalPlayer.Die(DeathReason.Disconnect);
                    PlayerControl.LocalPlayer.gameObject.layer = LayerMask.NameToLayer("Players");
                    HudManager.Instance.ShadowQuad.gameObject.SetActive(false);
                    RpcDemonAbility(PlayerControl.LocalPlayer, true);
                    Coroutines.Start(CoStartDemonAbility(Settings.DemonDuration.Value));
                }, Settings.DemonCooldown.Value,
                Utility.CreateSprite("Peasmod.Resources.Buttons.SwapAfterlife.png", 650f), Vector2.zero, false,
                this, "<size=40%>Swap");
        }

        private IEnumerator CoStartDemonAbility(float cooldown)
        {
            yield return new WaitForSeconds(cooldown);

            if (PeasAPI.PeasAPI.GameStarted)
            {
                PlayerControl.LocalPlayer.Revive();
                HudManager.Instance.ShadowQuad.gameObject.SetActive(true);
                RpcDemonAbility(PlayerControl.LocalPlayer, false);
                IsSwaped = false;
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
        class PlayerControlMurderPlayerPatch
        {
            public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl victim)
            {
                if (victim.IsRole<Demon>() && victim.IsLocal() && Instance.Button != null)
                {
                    Instance.Button.KillButtonManager.Destroy();
                    Instance.Button = null;
                }
            }
        }

        [MethodRpc((uint)CustomRpcCalls.DemonAbility)]
        public static void RpcDemonAbility(PlayerControl sender, bool deathOrRevive)
        {
            if (deathOrRevive)
            {
                sender.Die(DeathReason.Kill);
            }
            else
            {
                sender.Revive();
            }
        }

        [HarmonyPatch]
        private static class Patches
        {
            [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
            [HarmonyPrefix]
            public static void RemoveButtonOnActualDeath(PlayerControl __instance,
                [HarmonyArgument(0)] PlayerControl victim)
            {
                if (victim.IsRole<Demon>() && victim.IsLocal() && Instance.Button != null)
                {
                    Instance.Button.KillButtonManager.Destroy();
                    Instance.Button = null;
                }
            }

            [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Awake))]
            [HarmonyPrefix]
            public static void ReviveDemonBeforeMeeting(MeetingHud __instance)
            {
                if (PlayerControl.LocalPlayer.IsRole<Demon>() && Instance.IsSwaped)
                {
                    Coroutines.Stop(Instance.CoStartDemonAbility(Settings.DemonDuration.Value));
                    PlayerControl.LocalPlayer.Revive();
                    HudManager.Instance.ShadowQuad.gameObject.SetActive(true);
                    RpcDemonAbility(PlayerControl.LocalPlayer, false);
                }
            }
        }
    }
}