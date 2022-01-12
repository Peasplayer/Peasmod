using System.Collections.Generic;
using BepInEx.IL2CPP;
using Il2CppSystem;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
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
        public override string TaskText => "Arrest an kill the impostor";
        public override Color Color => ModdedPalette.OfficerColor;
        public override Visibility Visibility => Visibility.NoOne;
        public override Team Team => Team.Crewmate;
        public override bool HasToDoTasks => true;
        public override int Limit => (int) Settings.OfficerAmount.Value;
        public override bool CanKill(PlayerControl victim = null) => AlreadyKilled < Settings.OfficerKills.Value;

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
                () => { RpcFreeze(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.FindClosestTarget(true), true); },
                Settings.OfficerCooldown.Value, Utility.CreateSprite("Peasmod.Resources.Buttons.Default.png"), p => p.IsRole(this) && !p.Data.IsDead, _ => true,
                text: "<size=40%>Arrest", textOffset: new Vector2(0f, 0.5f));
            if (Settings.OfficerArrestPeriod.Value == 0)
            {
                Button.EffectDuration = Settings.OfficerDuration.Value;
                Button.OnEffectEnd = () => { RpcFreeze(PlayerControl.LocalPlayer, Arrested[PlayerControl.LocalPlayer.PlayerId][0].GetPlayer(), false); };
            }
        }

        public override void OnUpdate()
        {
            if (Button != null)
                Button.Usable = !ArrestedSomeone && PlayerControl.LocalPlayer.FindClosestTarget(true) != null;
            
            foreach (var item in Arrested)
            {
                foreach (var arrested in item.Value)
                {
                    if (!arrested.GetPlayer().Data.IsDead)
                        arrested.GetPlayer().moveable = false;
                }
            }
        }

        public override void OnKill(PlayerControl killer, PlayerControl victim)
        {
            if (killer.IsLocal() && killer.IsRole(this))
                AlreadyKilled++;
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