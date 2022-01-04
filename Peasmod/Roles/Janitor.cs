using System.Linq;
using BepInEx.IL2CPP;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.Roles;
using Reactor.Extensions;
using Reactor.Networking.MethodRpc;
using UnityEngine;

namespace Peasmod.Roles
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
        public override string TaskText => "Remove dead bodys so you don't get caught";
        public override Color Color => Palette.ImpostorRed;
        public override Visibility Visibility => Visibility.Impostor;
        public override Team Team => Team.Impostor;
        public override bool HasToDoTasks => true;
        public override int Limit => (int)Settings.JanitorAmount.Value;
        public override bool CanVent => true;
        public override bool CanKill(PlayerControl victim = null) => !victim || victim.Data.Role.IsImpostor;
        public override bool CanSabotage(SystemTypes? sabotage) => true;

        public static Janitor Instance;
        public CustomButton Button;
        public GameObject TargetBody;
        
        public override void OnGameStart()
        {
            Button = CustomButton.AddRoleButton(() =>
                {
                    if (TargetBody != null)
                        RpcCleanBody(PlayerControl.LocalPlayer, TargetBody.GetComponent<DeadBody>().ParentId);
                    TargetBody = null;
                }, Settings.JanitorCooldown.Value,
                PeasAPI.Utility.CreateSprite("Peasmod.Resources.Buttons.Button1.png"), this, text: "<size=40%>Clear", textOffset: new Vector2(0f, 0.5f));
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