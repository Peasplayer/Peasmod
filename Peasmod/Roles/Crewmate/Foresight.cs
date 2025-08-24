using System.Collections.Generic;
using System.Linq;
using BepInEx.Unity.IL2CPP;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.Managers;
using PeasAPI.Options;
using PeasAPI.Roles;
using Reactor.Utilities.Extensions;
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
        public override string Description => ((CustomStringOption)AdvancedOptions["RevealVariant"]).Value == 0 ? "Reveal someone as a crewmate" : "Reveal the team of someone";
        public override string LongDescription => "";
        public override string TaskText => ((CustomStringOption)AdvancedOptions["RevealVariant"]).Value == 0 ? "Reveal someone as a crewmate" : "Reveal the team of someone";
        public override Color Color => ModdedPalette.ForesightColor;

        public override Visibility Visibility =>
            ((CustomStringOption)AdvancedOptions["RevealVariant"]).Value == 0 ? Visibility.NoOne : UsedAbility == 0 ? Visibility.NoOne : Visibility.Impostor;

        public override Team Team => Team.Crewmate;
        public override bool HasToDoTasks => true;
        public override Dictionary<string, CustomOption> AdvancedOptions { get; set; } = new Dictionary<string, CustomOption>()
        {
            {
                "RevealCooldown", new CustomNumberOption(MultiMenu.Crewmate, "Reveal-Cooldown", 10, 120, 1, 20, CustomOption.Seconds)
            },
            {
                "RevealCount", new CustomNumberOption(MultiMenu.Crewmate, "Reveals", 1, 15, 1, 2)
            },
            {
                "RevealVariant", new CustomStringOption(MultiMenu.Crewmate, "Variant", new string[] { "Safe", "Strong" })
            },
            {
                "RevealTarget", new CustomStringOption(MultiMenu.Crewmate, "Choose player (Variant B)", new string[] { "Random", "In Menu", "In Range" })
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
                if (((CustomStringOption)AdvancedOptions["RevealVariant"]).Value == 0)
                {
                    var player = Utility.GetAllPlayers().Where(p =>
                        !p.Data.Role.IsImpostor &&
                        (p.Data.GetCustomRole() == null ||
                         p.Data.GetCustomRole() != null && p.GetCustomRole().Team == Team.Crewmate) &&
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
                    if (((CustomStringOption)AdvancedOptions["RevealTarget"]).Value == 0)
                    {
                        var player = Utility.GetAllPlayers().Where(p =>
                            !p.Data.IsDead && !p.IsLocal() && !AlreadyRevealed.Contains(p.PlayerId)).Random();
                        if (player != null)
                        {
                            var team = player.GetCustomRole() == null ? player.Data.Role.IsImpostor ? "evil" : "good" :
                                player.GetCustomRole().Team == Team.Crewmate ? "good" :
                                player.GetCustomRole().Team == Team.Impostor ? "evil" : "neutral";
                            TextMessageManager.ShowMessage($"You see that {player.Data.PlayerName} is {team}", 3f);
                            AlreadyRevealed.Add(player.PlayerId);
                        }
                        else
                            TextMessageManager.ShowMessage("You are not able to see anyone new", 3f);

                        UsedAbility++;
                    }
                    else if (((CustomStringOption)AdvancedOptions["RevealTarget"]).Value == 1)
                    {
                        PlayerMenuManager.OpenPlayerMenu(Utility.GetAllPlayers()
                            .Where(p => !p.Data.IsDead && !p.IsLocal())
                            .ToList().ConvertAll(p => p.PlayerId), p =>
                            {
                                var team = p.GetCustomRole() == null ? p.Data.Role.IsImpostor ? "evil" : "good" :
                                        p.GetCustomRole().Team == Team.Crewmate ? "good" :
                                        p.GetCustomRole().Team == Team.Impostor ? "evil" : "neutral";
                                TextMessageManager.ShowMessage($"You see that {p.Data.PlayerName} is {team}", 3f);

                                UsedAbility++;
                            }, () => Button.SetCoolDown(0));
                    }
                    else if (((CustomStringOption)AdvancedOptions["RevealTarget"]).Value == 2)
                    {
                        var p = PlayerControl.LocalPlayer.Data.Role.FindClosestTarget();
                        var team = p.GetCustomRole() == null ? p.Data.Role.IsImpostor ? "evil" : "good" :
                            p.GetCustomRole().Team == Team.Crewmate ? "good" :
                            p.GetCustomRole().Team == Team.Impostor ? "evil" : "neutral";
                        TextMessageManager.ShowMessage($"You see that {p.Data.PlayerName} is {team}", 3f);

                        UsedAbility++;
                    }
                }
            }, ((CustomNumberOption)AdvancedOptions["RevealCooldown"]).Value, Utility.CreateSprite("Peasmod.Resources.Buttons.Default.png"),
                p => p.IsCustomRole(this) && !p.Data.IsDead,
                p => UsedAbility < ((CustomNumberOption)AdvancedOptions["RevealCount"]).Value && (((CustomStringOption)AdvancedOptions["RevealTarget"]).Value != 2 && ((CustomStringOption)AdvancedOptions["RevealVariant"]).Value == 1 ||
                                                                       PlayerControl.LocalPlayer
                                                                           .Data.Role
                                                                           .FindClosestTarget() != null),
                text: "<size=40%>Reveal", textOffset: new Vector2(0f, 0.5f));
        }
    }
}