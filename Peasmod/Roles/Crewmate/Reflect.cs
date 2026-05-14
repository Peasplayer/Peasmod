using System.Collections;
using System.Collections.Generic;
using BepInEx.IL2CPP;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.Options;
using PeasAPI.Roles;
using Reactor;
using Reactor.Networking.MethodRpc;
using UnityEngine;

namespace Peasmod.Roles.Crewmate;

[RegisterCustomRole]
public class Reflect : BaseRole
{
    public Reflect(BasePlugin plugin) : base(plugin) { }

    public override string Name => "Reflect";
    public override string Description => "Shield yourself from the impostors";
    public override string LongDescription => "";
    public override string TaskText => "Shield yourself from the impostors";
    public override Color Color => ModdedPalette.ZombieColor;
    public override Visibility Visibility => Visibility.NoOne;
    public override Team Team => Team.Crewmate;
    public override bool HasToDoTasks => true;
    public override Dictionary<string, CustomOption> AdvancedOptions { get; set; } = new Dictionary<string, CustomOption>()
    {
        {
            "ShieldCooldown", new CustomNumberOption("reflectshieldcooldown", "Shield-Cooldown", 20, 120, 1, 20, NumberSuffixes.Seconds) {AdvancedRoleOption = true}
        },
        {
            "ShieldDuration", new CustomNumberOption("reflectshielduration", "Shield-Duration", 20, 60, 1, 20, NumberSuffixes.Seconds) {AdvancedRoleOption = true}
        }
    };

    public CustomButton Button;
    public Dictionary<byte, bool> IsProtected = new Dictionary<byte, bool>();

    public override void OnGameStart()
    {
        IsProtected = new Dictionary<byte, bool>();
        Button = CustomButton.AddButton(() =>
            {
                RpcProtect(PlayerControl.LocalPlayer, (int) ((CustomNumberOption) AdvancedOptions["ShieldDuration"]).Value);
            }, ((CustomNumberOption) AdvancedOptions["ShieldCooldown"]).Value, Utility.CreateSprite("Peasmod.Resources.Buttons.Default.png"), p => p.IsRole(this) && !p.Data.IsDead,
            p => true, text: "Shield", textOffset: new Vector2(0f, 0.5f), onEffectEnd: () => { }, effectDuration: ((CustomNumberOption) AdvancedOptions["ShieldDuration"]).Value);
    }

    public override void OnUpdate()
    {
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (IsProtected.ContainsKey(player.PlayerId) && IsProtected[player.PlayerId] && !player.protectedByGuardian)
                player.protectedByGuardian = true;
        }
    }

    [MethodRpc((uint) CustomRpcCalls.Protect)]
    public static void RpcProtect(PlayerControl player, int duration)
    {
        Coroutines.Start(CoProtect(player, duration));
    }

    private static IEnumerator CoProtect(PlayerControl player, int duration)
    {
        player.protectedByGuardian = true;
        PeasAPI.Roles.RoleManager.GetRole<Reflect>().IsProtected[player.PlayerId] = true;
        yield return new WaitForSeconds(duration);
        PeasAPI.Roles.RoleManager.GetRole<Reflect>().IsProtected[player.PlayerId] = false;
        player.protectedByGuardian = false;
    }
}