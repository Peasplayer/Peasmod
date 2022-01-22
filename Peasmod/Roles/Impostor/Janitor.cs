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
            Instance = this;
        }

        public override string Name => "Janitor";
        public override string Description => "Clear every evidence";
        public override string LongDescription => "";
        public override string TaskText => "Remove dead bodys so you don't get caught";
        public override Color Color => Palette.ImpostorRed;
        public override Visibility Visibility => Visibility.Impostor;
        public override Team Team => Team.Impostor;
        public override bool HasToDoTasks => true;
        public override int MaxCount => 3;
        public override Dictionary<string, CustomOption> AdvancedOptions => new Dictionary<string, CustomOption>()
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

        public static Janitor Instance;
        public CustomButton Button;
        public GameObject TargetBody;
        
        public override void OnGameStart()
        {
            Button = CustomButton.AddButton(() =>
                {
                    if (TargetBody != null)
                        RpcCleanBody(PlayerControl.LocalPlayer, TargetBody.GetComponent<DeadBody>().ParentId);
                    TargetBody = null;
                }, ((CustomNumberOption) AdvancedOptions["CleanBodyCooldown"]).Value,
                PeasAPI.Utility.CreateSprite("Peasmod.Resources.Buttons.Default.png"), p => p.IsRole(this) && !p.Data.IsDead, _ => true, text: "<size=40%>Clear", textOffset: new Vector2(0f, 0.5f));
        }

        public override void OnUpdate()
        {
            if (Button == null)
                return;
            TargetBody = null;
            Button.Usable = false;
            
            var bodys = Object.FindObjectsOfType<DeadBody>().ToList();
            if (bodys.Count != 0)
            {
                TargetBody = bodys[0].gameObject;
                Button.Usable = true;
            }
        }

        [MethodRpc((uint) CustomRpcCalls.CleanBody)]
        public static void RpcCleanBody(PlayerControl sender, byte bodyId)
        {
            Object.FindObjectsOfType<DeadBody>().Where(_body => _body.ParentId == bodyId).ToList()[0].gameObject.Destroy();
        }
    }
}