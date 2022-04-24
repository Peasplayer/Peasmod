using System.Collections.Generic;
using BepInEx.IL2CPP;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.Options;
using PeasAPI.Roles;
using UnityEngine;

namespace Peasmod.Roles.Neutral;

[RegisterCustomRole]
public class Zombie : BaseRole
{
    public Zombie(BasePlugin plugin) : base(plugin) { }
    
    public override string Name => "Zombie";
    public override string Description => "Infect every crewmate";
    public override string LongDescription => "Spread the zombie disease to every other crewmate and turn them into zombies too but watch out! You can't infect impostors because they already have a different disease";
    public override string TaskText => "Infect every crewmate";
    public override Color Color => ModdedPalette.ZombieColor;
    public override Visibility Visibility => Visibility.Role;
    public override Team Team => Team.Role;
    public override bool HasToDoTasks => false;
    public override int MaxCount => 1;
    public override Dictionary<string, CustomOption> AdvancedOptions { get; set; } = new Dictionary<string, CustomOption>()
    {
        {
            "InfectCooldown", new CustomNumberOption("infectcooldown", "Infect-Cooldown", 40, 180, 5, 40, NumberSuffixes.Seconds) {AdvancedRoleOption = true}
        }
    };

    public CustomButton Button;
    
    public override void OnGameStart()
    {
        Button = CustomButton.AddButton(() =>
            {
                var victim = Button.PlayerTarget;
                victim.RpcSetRole(this);
            }, ((CustomNumberOption) AdvancedOptions["InfectCooldown"]).Value, Utility.CreateSprite("Peasmod.Resources.Buttons.Default.png"), p => p.IsRole(this) && !p.Data.IsDead,
            p => true, text: "<size=40%>Infect", textOffset: new Vector2(0f, 0.5f), target: CustomButton.TargetType.Player,
            targetColor: Color, choosePlayerTarget: p => !p.IsRole(this) && !p.Data.Role.IsImpostor);
    }
}