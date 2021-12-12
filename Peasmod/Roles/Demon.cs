using System.Collections;
using System.Collections.Generic;
using BepInEx.IL2CPP;
using HarmonyLib;
using Hazel;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.Roles;
using Reactor;
using Reactor.Extensions;
using Reactor.Networking;
using UnityEngine;

namespace Peasmod.Roles
{
    [RegisterCustomRole]
    public class Demon : BaseRole
    {
        public Demon(BasePlugin plugin) : base(plugin) { }

        public override string Name => "Demon";

        public override string Description => "Swap into the afterlife";

        public override string TaskText => "Swap into the afterlife";

        public override Color Color => ModdedPalette.DemonColor;

        public override int Limit => (int) Settings.DemonAmount.Value;

        public override Team Team => Team.Crewmate;

        public override Visibility Visibility => Visibility.NoOne;

        public override bool HasToDoTasks => true;

        public static CustomButton Button;

        public override void OnGameStart()
        {
            Button = CustomButton.AddRoleButton(() =>
                {
                    PlayerControl.LocalPlayer.Die(DeathReason.Disconnect);
                    PlayerControl.LocalPlayer.gameObject.layer = LayerMask.NameToLayer("Players");
                    HudManager.Instance.ShadowQuad.gameObject.SetActive(false);
                    Rpc<DemonAbilityRpc>.Instance.Send(new DemonAbilityRpc.Data(PlayerControl.LocalPlayer, true));
                    Coroutines.Start(CoStartDemonAbility(Settings.DemonDuration.Value));
                }, Settings.DemonCooldown.Value, PeasAPI.Utility.CreateSprite("Peasmod.Resources.Buttons.SwapAfterlife.png", 650f), Vector2.zero, false, this, "<size=40%>Swap");
        }

        private IEnumerator CoStartDemonAbility(float cooldown)
        {
            yield return new WaitForSeconds(cooldown);
            
            if (PeasAPI.PeasAPI.GameStarted)
            {
                PlayerControl.LocalPlayer.Revive();
                HudManager.Instance.ShadowQuad.gameObject.SetActive(true);
                Rpc<DemonAbilityRpc>.Instance.Send(new DemonAbilityRpc.Data(PlayerControl.LocalPlayer, false));
            }
        }
        
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
        class PlayerControlMurderPlayerPatch
        {
            public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl victim)
            {
                if (victim.IsRole<Demon>() && victim.IsLocal() && Button != null)
                {
                    Button.KillButtonManager.Destroy();
                    Button = null;
                }
            }
        }
        
        [RegisterCustomRpc((uint) CustomRpcCalls.DemonAbility)]
        public class DemonAbilityRpc : PlayerCustomRpc<PeasmodPlugin, DemonAbilityRpc.Data>
        {
            public DemonAbilityRpc(PeasmodPlugin plugin, uint id) : base(plugin, id)
            {
            }

            public readonly struct Data
            {
                public readonly PlayerControl Player;
                public readonly bool DeathOrRevive;

                public Data(PlayerControl player, bool deathOrRevive)
                {
                    Player = player;
                    DeathOrRevive = deathOrRevive;
                }
            }

            public override RpcLocalHandling LocalHandling => RpcLocalHandling.None;

            public override void Write(MessageWriter writer, Data data)
            {
                writer.Write(data.Player.PlayerId);
                writer.Write(data.DeathOrRevive);
            }

            public override Data Read(MessageReader reader)
            {
                return new Data(reader.ReadByte().GetPlayer(), reader.ReadBoolean());
            }

            public override void Handle(PlayerControl innerNetObject, Data data)
            {
                if (data.DeathOrRevive)
                {
                    data.Player.Die(DeathReason.Kill);
                }
                else
                {
                    data.Player.Revive();
                }
            }
        }
    }
}