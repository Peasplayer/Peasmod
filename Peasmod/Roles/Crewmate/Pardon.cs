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
    public class Pardon : BaseRole
    {
        public Pardon(BasePlugin plugin) : base(plugin)
        {
        }

        public override string Name => "Pardon";
        public override string Description => "Pardon the last exiled player";
        public override string LongDescription => "";
        public override string TaskText => "Pardon the last player that got exiled even though they were innocent";
        public override Color Color => ModdedPalette.PardonColor;
        public override Visibility Visibility => Visibility.NoOne;
        public override Team Team => Team.Crewmate;
        public override bool HasToDoTasks => true;
        public override Dictionary<string, CustomOption> AdvancedOptions { get; set; } = new Dictionary<string, CustomOption>()
        {
            {
                "PardonCooldown", new CustomNumberOption("PardonCooldown", "Pardon-Cooldown", 20, 60, 1, 20, NumberSuffixes.Seconds)
            },
            {
                "PardonMaxUses", new CustomNumberOption("PardonMaxUses", "Maximum of Pardons", 1, 10, 1, 2, NumberSuffixes.None)
            }
        };

        public CustomButton Button;
        public byte LastExiled;
        public int TimesExiled;

        public override void OnGameStart()
        {
            LastExiled = byte.MaxValue;
            TimesExiled = 0;
            Button = CustomButton.AddButton(
                () =>
                {
                    var player = LastExiled.GetPlayer();
                    RpcRevive(PlayerControl.LocalPlayer, player);
                },
                ((CustomNumberOption) AdvancedOptions["PardonCooldown"]).Value, Utility.CreateSprite("Peasmod.Resources.Buttons.Default.png"), p => p.IsRole(this) && !p.Data.IsDead, 
                _ => LastExiled != byte.MaxValue && !LastExiled.GetPlayer().Data.Disconnected && TimesExiled < ((CustomNumberOption) AdvancedOptions["PardonMaxUses"]).Value, text: "<size=40%>Pardon");
        }

        public override void OnExiled(PlayerControl victim)
        {
            LastExiled = victim.PlayerId;
        }

        [MethodRpc((uint) CustomRpcCalls.PardonAbility)]
        public static void RpcRevive(PlayerControl sender, PlayerControl target)
        {
            if (target.Data.Disconnected)
                return;
            target.Revive();
            target.NetTransform.SnapTo(sender.transform.position);
        }
    }
}