using System.Collections.Generic;
using System.Linq;
using BepInEx.IL2CPP;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.Managers;
using PeasAPI.Options;
using PeasAPI.Roles;
using Reactor.Extensions;
using UnityEngine;

namespace Peasmod.Roles.Crewmate
{
    [RegisterCustomRole]
    public class Foresight : BaseRole
    {
        public Foresight(BasePlugin plugin) : base(plugin)
        {
        }

        public override string Name => "Foresight";
        public override string Description => ((CustomStringOption) AdvancedOptions["RevealVariant"]).Value == 0 ? "Reveal someone as a crewmate" : "Reveal the team of someone";
        public override string LongDescription => "";
        public override string TaskText => ((CustomStringOption) AdvancedOptions["RevealVariant"]).Value == 0 ? "Reveal someone as a crewmate" : "Reveal the team of someone";
        public override Color Color => ModdedPalette.ForesightColor;

        public override Visibility Visibility =>
            ((CustomStringOption) AdvancedOptions["RevealVariant"]).Value == 0 ? Visibility.NoOne : UsedAbility == 0 ? Visibility.NoOne : Visibility.Impostor;

        public override Team Team => Team.Crewmate;
        public override bool HasToDoTasks => true;
        public override Dictionary<string, CustomOption> AdvancedOptions { get; set; } = new Dictionary<string, CustomOption>()
        {
            {
                "RevealCooldown", new CustomNumberOption("foresightcooldown", "Reveal-Cooldown", 10, 120, 1, 20, NumberSuffixes.Seconds) {AdvancedRoleOption = true}
            },
            {
                "RevealCount", new CustomNumberOption("foresightreveals", "Reveals", 1, 15, 1, 2, NumberSuffixes.None) {AdvancedRoleOption = true}
            },
            {
                "RevealVariant", new CustomStringOption("foresightvariants", "Variant", "Safe", "Strong") {AdvancedRoleOption = true}
            },
            {
                "RevealTarget", new CustomStringOption("foresighttarget", "Choose player (Variant B)", "Random", "In Menu", "In Range") {AdvancedRoleOption = true}
            }
        };

        public CustomButton Button;
        public int UsedAbility;
        public List<byte> AlreadyRevealed;

        public override void OnGameStart()
        {
            UsedAbility = 0;
            AlreadyRevealed = new List<byte>();
            Button = CustomButton.AddButton(() =>
                {
                    if (((CustomStringOption) AdvancedOptions["RevealVariant"]).Value == 0)
                    {
                        var player = Utility.GetAllPlayers().Where(p =>
                            !p.Data.Role.IsImpostor &&
                            (p.Data.GetRole() == null ||
                             p.Data.GetRole() != null && p.GetRole().Team == Team.Crewmate) &&
                            !p.Data.IsDead && !p.IsLocal() && !AlreadyRevealed.Contains(p.PlayerId)).Random();
                        if (player != null)
                        {
                            TextMessageManager.ShowMessage($"You see that {player.Data.PlayerName} is a crewmate", 3f);
                            AlreadyRevealed.Add(player.PlayerId);
                        }
                        else
                            TextMessageManager.ShowMessage("You are not able to see any crewmates", 3f);

                        UsedAbility++;
                    }
                    else
                    {
                        if (((CustomStringOption) AdvancedOptions["RevealTarget"]).Value == 0)
                        {
                            var player = Utility.GetAllPlayers().Where(p =>
                                !p.Data.IsDead && !p.IsLocal() && !AlreadyRevealed.Contains(p.PlayerId)).Random();
                            if (player != null)
                            {
                                var team = player.GetRole() == null ? player.Data.Role.IsImpostor ? "evil" : "good" :
                                    player.GetRole().Team == Team.Crewmate ? "good" :
                                    player.GetRole().Team == Team.Impostor ? "evil" : "neutral";
                                TextMessageManager.ShowMessage($"You see that {player.Data.PlayerName} is {team}", 3f);
                                AlreadyRevealed.Add(player.PlayerId);
                            }
                            else
                                TextMessageManager.ShowMessage("You are not able to see anyone new", 3f);

                            UsedAbility++;
                        }
                        else if (((CustomStringOption) AdvancedOptions["RevealTarget"]).Value == 1)
                        {
                            PlayerMenuManager.OpenPlayerMenu(Utility.GetAllPlayers()
                                .Where(p => !p.Data.IsDead && !p.IsLocal())
                                .ToList().ConvertAll(p => p.PlayerId), p =>
                            {
                                var team = p.GetRole() == null ? p.Data.Role.IsImpostor ? "evil" : "good" :
                                    p.GetRole().Team == Team.Crewmate ? "good" :
                                    p.GetRole().Team == Team.Impostor ? "evil" : "neutral";
                                TextMessageManager.ShowMessage($"You see that {p.Data.PlayerName} is {team}", 3f);

                                UsedAbility++;
                            }, () => Button.SetCoolDown(0));
                        }
                        else if (((CustomStringOption) AdvancedOptions["RevealTarget"]).Value == 2)
                        {
                            var p = PlayerControl.LocalPlayer.FindClosestTarget(true);
                            var team = p.GetRole() == null ? p.Data.Role.IsImpostor ? "evil" : "good" :
                                p.GetRole().Team == Team.Crewmate ? "good" :
                                p.GetRole().Team == Team.Impostor ? "evil" : "neutral";
                            TextMessageManager.ShowMessage($"You see that {p.Data.PlayerName} is {team}", 3f);

                            UsedAbility++;
                        }
                    }
                }, ((CustomNumberOption) AdvancedOptions["RevealCooldown"]).Value, Utility.CreateSprite("Peasmod.Resources.Buttons.Default.png"),
                p => p.IsRole(this) && !p.Data.IsDead,
                p => UsedAbility < ((CustomNumberOption) AdvancedOptions["RevealCount"]).Value && (((CustomStringOption) AdvancedOptions["RevealTarget"]).Value != 2 && ((CustomStringOption) AdvancedOptions["RevealVariant"]).Value == 1 ||
                                                                       PlayerControl.LocalPlayer
                                                                           .FindClosestTarget(true) != null),
                text: "<size=40%>Reveal", textOffset: new Vector2(0f, 0.5f));
        }
    }
}