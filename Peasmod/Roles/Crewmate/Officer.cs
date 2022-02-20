using System.Collections.Generic;
using BepInEx.IL2CPP;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.Options;
using PeasAPI.Roles;
using Reactor.Networking.MethodRpc;
using UnityEngine;

namespace Peasmod.Roles.Crewmate
{
    [RegisterCustomRole]
    public class Officer : BaseRole
    {
        public Officer(BasePlugin plugin) : base(plugin)
        {
            Instance = this;
        }

        public override string Name => "Officer";
        public override string Description => "Arrest the impostor";
        public override string LongDescription => "";
        public override string TaskText => "Arrest an kill the impostor";
        public override Color Color => ModdedPalette.OfficerColor;
        public override Visibility Visibility => Visibility.NoOne;
        public override Team Team => Team.Crewmate;
        public override bool HasToDoTasks => true;
        public override Dictionary<string, CustomOption> AdvancedOptions { get; set; } = new Dictionary<string, CustomOption>()
        {
            {
                "ArrestCooldown", new CustomNumberOption("officercooldown", "Arrest-Cooldown", 10, 60, 1, 20, NumberSuffixes.Seconds)
            },
            {
                "ArrestPeriod", new CustomStringOption("officerarrestperiod", "Arrest-Period", "Seconds", "Until Meeting")
            },
            {
                "ArrestDuration", new CustomNumberOption("officerduration", $"Arrest-Duration", 10, 120, 1, 30, NumberSuffixes.Seconds)
            },
            {
                "PossibleKills", new CustomNumberOption("officerkills", $"Number of Kills", 0, 10, 1, 10, NumberSuffixes.None)
            }
        };

        public override bool CanKill(PlayerControl victim = null)
        {
            //AlreadyKilled < ((CustomNumberOption) AdvancedOptions["PossibleKills"]).Value && (victim == null || Arrested.ContainsKey(PlayerControl.LocalPlayer.PlayerId) && Arrested[PlayerControl.LocalPlayer.PlayerId].Contains(victim.PlayerId));
            if (AlreadyKilled >= ((CustomNumberOption)AdvancedOptions["PossibleKills"]).Value)
                return false;
            if (victim == null)
                return true;
            if (!Arrested.ContainsKey(PlayerControl.LocalPlayer.PlayerId))
                return false;
            if (Arrested.ContainsKey(PlayerControl.LocalPlayer.PlayerId) &&
                Arrested[PlayerControl.LocalPlayer.PlayerId].Contains(victim.PlayerId))
                return true;
            return false;
        }

        public static Officer Instance;

        public int AlreadyKilled;
        public bool ArrestedSomeone;
        public Dictionary<byte, List<byte>> Arrested = new Dictionary<byte, List<byte>>();
        public CustomButton Button;

        public override void OnGameStart()
        {
            AlreadyKilled = 0;
            Arrested = new Dictionary<byte, List<byte>>();
            Button = CustomButton.AddButton(
                () => RpcFreeze(PlayerControl.LocalPlayer, Button.PlayerTarget, true),
                ((CustomNumberOption) AdvancedOptions["ArrestCooldown"]).Value, Utility.CreateSprite("Peasmod.Resources.Buttons.Default.png"), p => p.IsRole(this) && !p.Data.IsDead, _ => !ArrestedSomeone,
                text: "<size=40%>Arrest", textOffset: new Vector2(0f, 0.5f), target: CustomButton.TargetType.Player, targetColor: Color);
            if (((CustomStringOption) AdvancedOptions["ArrestPeriod"]).Value == 0)
            {
                Button.EffectDuration = ((CustomNumberOption) AdvancedOptions["ArrestDuration"]).Value;
                Button.OnEffectEnd = () => RpcFreeze(PlayerControl.LocalPlayer, Arrested[PlayerControl.LocalPlayer.PlayerId][0].GetPlayer(), false);
            }
        }

        public override void OnUpdate()
        {
            foreach (var item in Arrested)
            {
                for (int i = 0; i < item.Value.Count; i++ )
                {
                    var arrested = item.Value[i].GetPlayer();
                    if (!arrested.Data.IsDead)
                        arrested.moveable = false;
                    if (item.Key.GetPlayer().IsLocal() && item.Key.GetPlayer().Data.IsDead)
                        Button.OnEffectEnd.Invoke();
                }
            }
        }

        public override void OnKill(PlayerControl killer, PlayerControl victim)
        {
            if (killer.IsLocal() && killer.IsRole(this))
            {
                AlreadyKilled++;
                //ToDo: Remove role when killing innocent
            }
        }

        [MethodRpc((uint)CustomRpcCalls.FreezePlayer)]
        public static void RpcFreeze(PlayerControl sender, PlayerControl target, bool enable)
        {
            if (enable)
            {
                if (Instance.Arrested.ContainsKey(sender.PlayerId))
                    Instance.Arrested[sender.PlayerId].Add(target.PlayerId);
                else
                    Instance.Arrested.Add(sender.PlayerId, new List<byte> { target.PlayerId });
            }
            else
            {
                Instance.Arrested[sender.PlayerId].Remove(target.PlayerId);
                target.moveable = true;
            }
        }
    }
}