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

namespace Peasmod.Roles.Crewmate
{
    [RegisterCustomRole]
    public class Doctor : BaseRole
    {
        public Doctor(BasePlugin plugin) : base(plugin) { }

        public override string Name => "Doctor";
        public override Sprite Icon => Utility.CreateSprite("Peasmod.Resources.Buttons.Revive.png", 803f);
        public override string Description => "Revive dead crewmates";
        public override string LongDescription => "";
        public override string TaskText => "Revive dead crewmates";
        public override Color Color => ModdedPalette.DoctorColor;
        public override Visibility Visibility => Visibility.NoOne;
        public override Team Team => Team.Crewmate;
        public override bool HasToDoTasks => true;
        public override Dictionary<string, CustomOption> AdvancedOptions { get; set; } = new Dictionary<string, CustomOption>()
        {
            {
                "ReviveCooldown", new CustomNumberOption("doctorcooldown", "Revive-Cooldown", 10, 60, 1, 20, NumberSuffixes.Seconds)
            }
        };

        public CustomButton Button;

        public override void OnGameStart()
        {
            Button = CustomButton.AddButton(() =>
                {
                    var body = Button.ObjectTarget.GetComponent<DeadBody>();
                    RpcRevive(body.ParentId.GetPlayer());
                }, ((CustomNumberOption) AdvancedOptions["ReviveCooldown"]).Value,
                Utility.CreateSprite("Peasmod.Resources.Buttons.Revive.png", 803f), p => p.IsRole(this) && !p.Data.IsDead, _ => true, text: "<size=40%>Revive", 
                target: CustomButton.TargetType.Object, targetColor: Color, chooseObjectTarget: o => o.GetComponent<DeadBody>() != null);
        }

        [MethodRpc((uint) CustomRpcCalls.DoctorAbility)]
        public static void RpcRevive(PlayerControl sender)
        {
            if (sender.Data.Disconnected)
                return;
            sender.Revive();
            sender.transform.position = Object.FindObjectsOfType<DeadBody>().Where(body => body.ParentId == sender.PlayerId).ToList()[0].transform.position;
            Object.FindObjectsOfType<DeadBody>().Where(body => body.ParentId == sender.PlayerId).ToList()[0].gameObject.Destroy();
        }
    }
}