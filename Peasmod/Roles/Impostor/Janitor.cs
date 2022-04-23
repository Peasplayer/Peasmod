using System.Collections.Generic;
using System.Linq;
using BepInEx.IL2CPP;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.Options;
using PeasAPI.Roles;
using Reactor.Extensions;
using Reactor.Networking.MethodRpc;
using UnityEngine;

namespace Peasmod.Roles.Impostor
{
    [RegisterCustomRole]
    public class Janitor : BaseRole
    {
        public Janitor(BasePlugin plugin) : base(plugin)
        {
        }

        public override string Name => "Janitor";
        public override string Description => "Clear every evidence";
        public override string LongDescription => "";
        public override string TaskText => "Remove dead bodys so you don't get caught";
        public override Color Color => Palette.ImpostorRed;
        public override Visibility Visibility => Visibility.Impostor;
        public override Team Team => Team.Impostor;
        public override bool HasToDoTasks => false;
        public override int MaxCount => 3;
        public override Dictionary<string, CustomOption> AdvancedOptions { get; set; } = new Dictionary<string, CustomOption>()
        {
            {
                "CleanBodyCooldown", new CustomNumberOption("janitorcooldown", "Clean-Body-Cooldown", 10, 120, 1, 40, NumberSuffixes.Seconds)
            },
            {
                "CanKill", new CustomToggleOption("janitorcankill", "Can Kill", true)
            }
        };
        public override bool CanVent => true;
        public override bool CanKill(PlayerControl victim = null) => (!victim || !victim.Data.Role.IsImpostor) && ((CustomToggleOption)AdvancedOptions["CanKill"]).Value;
        public override bool CanSabotage(SystemTypes? sabotage) => true;
        
        public CustomButton Button;
        
        public override void OnGameStart()
        {
            Button = CustomButton.AddButton(() => RpcCleanBody(PlayerControl.LocalPlayer, Button.ObjectTarget.GetComponent<DeadBody>().ParentId), ((CustomNumberOption) AdvancedOptions["CleanBodyCooldown"]).Value,
                PeasAPI.Utility.CreateSprite("Peasmod.Resources.Buttons.Default.png"), p => p.IsRole(this) && !p.Data.IsDead, _ => true, text: "<size=40%>Clear", textOffset: new Vector2(0f, 0.5f),
                target: CustomButton.TargetType.Object, targetColor: Color);
        }

        [MethodRpc((uint) CustomRpcCalls.CleanBody)]
        public static void RpcCleanBody(PlayerControl sender, byte bodyId)
        {
            Object.FindObjectsOfType<DeadBody>().Where(_body => _body.ParentId == bodyId).ToList()[0].gameObject.Destroy();
        }
    }
}