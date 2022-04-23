using System.Collections.Generic;
using System.Linq;
using BepInEx.IL2CPP;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.Roles;
using Reactor.Networking;
using Reactor.Networking.MethodRpc;
using UnityEngine;

namespace Peasmod.Roles.Impostor
{
    [RegisterCustomRole]
    public class Undertaker : BaseRole
    {
        public Undertaker(BasePlugin plugin) : base(plugin)
        {
            Instance = this;
        }

        public override string Name => "Undertaker";
        public override Sprite Icon => Utility.CreateSprite("Peasmod.Resources.Buttons.DragBody.png", 702f);
        public override string Description => "Take dead bodies away";
        public override string LongDescription => "";
        public override string TaskText => "Take dead bodies away";
        public override Color Color => Palette.ImpostorRed;
        public override Visibility Visibility => Visibility.Impostor;
        public override Team Team => Team.Impostor;
        public override bool HasToDoTasks => false;
        public override int MaxCount => 3;
        public override bool CanVent => true;
        public override bool CanKill(PlayerControl victim = null) => !victim || !victim.Data.Role.IsImpostor;
        public override bool CanSabotage(SystemTypes? sabotage) => true;

        public static Undertaker Instance;
        
        public CustomButton Button;
        public bool CarryingBody;
        public Dictionary<byte, byte> CarriedBodys = new ();

        public override void OnGameStart()
        {
            CarriedBodys = new Dictionary<byte, byte>();
            Button = CustomButton.AddButton(() =>
            {
                if (CarryingBody)
                {
                    CarryingBody = false;
                    Button.SetImage(Utility.CreateSprite("Peasmod.Resources.Buttons.DragBody.png", 702f));
                    Button.Text = "<size=40%>Drag";
                    RpcDragBody(PlayerControl.LocalPlayer, false, byte.MaxValue);
                }
                else
                {
                    CarryingBody = true;
                    Button.SetImage(Utility.CreateSprite("Peasmod.Resources.Buttons.DropBody.png", 803f));
                    Button.Text = "<size=40%>Drop";
                    RpcDragBody(PlayerControl.LocalPlayer, true, Button.ObjectTarget.GetComponent<DeadBody>().ParentId);
                }
            }, 0f, Utility.CreateSprite("Peasmod.Resources.Buttons.DragBody.png", 702f), p => p.IsRole(this) && !p.Data.IsDead, _ => true, text: "<size=40%>Drag",
                target: CustomButton.TargetType.Object, targetColor: Color, chooseObjectTarget: o => o.GetComponent<DeadBody>() != null);
        }
        
        public override void OnUpdate()
        {
            if (Button == null)
                return;
            
            if (CarriedBodys != null)
            {
                for (int i = 0; i < CarriedBodys.Count; i++)
                {
                    var player = CarriedBodys.Keys.ToArray()[i].GetPlayer();
                    var body = CarriedBodys.Values.ToArray()[i];
                
                    var bodyObject = Object.FindObjectsOfType<DeadBody>().Where(_body => _body.ParentId == body).ToList();
                    if (bodyObject.Count == 0)
                    {
                        if (player.IsLocal())
                        {
                            CarryingBody = false;
                            Button.SetImage(PeasAPI.Utility.CreateSprite("Peasmod.Resources.Buttons.DragBody.png"));
                            RpcDragBody(PlayerControl.LocalPlayer, false, byte.MaxValue);
                        }
                        continue;
                    }
                
                    MoveBody(player, body);
                }
            }
        }

        public override void OnMeetingStart(MeetingHud meeting)
        {
            if (PlayerControl.LocalPlayer.IsRole(this) && CarryingBody)
            {
                CarryingBody = false;
                Button.SetImage(Utility.CreateSprite("Peasmod.Resources.Buttons.DragBody.png"));
                RpcDragBody(PlayerControl.LocalPlayer, false, byte.MaxValue);
            }
        }

        public static void MoveBody(PlayerControl player, int bodyId)
        {
            if (player.transform == null) 
                return;
            var body = Object.FindObjectsOfType<DeadBody>().First(body => body.ParentId == bodyId);
            if (body == null) 
                return;
            if (!player.inVent)
            {
                var pos = player.transform.position;
                pos.Set(pos.x, pos.y, pos.z + .001f);
                body.transform.position = Vector3.Lerp(body.transform.position, pos, Time.deltaTime + 0.05f);
            }
            else
                body.transform.position = player.transform.position;
        }

        [MethodRpc((uint) CustomRpcCalls.DragBody, LocalHandling = RpcLocalHandling.Before)]
        public static void RpcDragBody(PlayerControl sender, bool enable, byte bodyId)
        {
            if (enable)
            {
                var body = Object.FindObjectsOfType<DeadBody>().First(body => body.ParentId == bodyId);
                if (body == null)
                    return;
                MoveBody(sender, bodyId);
                Instance.CarriedBodys.Add(sender.PlayerId, body.ParentId);
            }
            else
            {
                if (Instance.CarriedBodys.ContainsKey(sender.PlayerId))
                    Instance.CarriedBodys.Remove(sender.PlayerId);
            }
        }
    }
}