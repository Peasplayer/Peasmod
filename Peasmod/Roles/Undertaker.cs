﻿using System.Collections.Generic;
using System.Linq;
using BepInEx.IL2CPP;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.Roles;
using Reactor.Networking;
using Reactor.Networking.MethodRpc;
using UnityEngine;

namespace Peasmod.Roles
{
    [RegisterCustomRole]
    public class Undertaker : BaseRole
    {
        public Undertaker(BasePlugin plugin) : base(plugin)
        {
            Instance = this;
        }

        public override string Name => "Undertaker";
        public override string Description => "Take dead bodys away";
        public override string TaskText => "Take dead bodys away";
        public override Color Color => Palette.ImpostorRed;
        public override Visibility Visibility => Visibility.Impostor;
        public override Team Team => Team.Impostor;
        public override bool HasToDoTasks => false;
        public override int Limit => (int)Settings.UndertakerAmount.Value;
        public override bool CanVent => true;
        public override bool CanKill(PlayerControl victim = null) => !victim || victim.Data.Role.IsImpostor;
        public override bool CanSabotage(SystemTypes? sabotage) => true;

        public static Undertaker Instance;
        
        public CustomButton Button;
        public bool CarryingBody;
        public GameObject TargetBody;
        public Dictionary<byte, byte> CarriedBodys;

        public override void OnGameStart()
        {
            CarriedBodys = new Dictionary<byte, byte>();
            Button = CustomButton.AddRoleButton(() =>
            {
                if (CarryingBody)
                {
                    CarryingBody = false;
                    Button.SetImage(PeasAPI.Utility.CreateSprite("Peasmod.Resources.Buttons.DragBody.png", 702f));
                    Button.Text = "<size=40%>Drag";
                    RpcDragBody(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer, false, byte.MaxValue);
                }
                else
                {
                    CarryingBody = true;
                    Button.SetImage(PeasAPI.Utility.CreateSprite("Peasmod.Resources.Buttons.DropBody.png", 803f));
                    Button.Text = "<size=40%>Drop";
                    RpcDragBody(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer, true, TargetBody.GetComponent<DeadBody>().ParentId);
                }
            }, 0f, PeasAPI.Utility.CreateSprite("Peasmod.Resources.Buttons.DragBody.png", 702f), Vector2.zero, false, this, "<size=40%>Drag");
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
                            RpcDragBody(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer, false, byte.MaxValue);
                        }
                        continue;
                    }
                
                    MoveBody(player, body);
                }
            }
            
            if (CarryingBody)
            {
                Button.Usable = true;
            }
            else
            {
                TargetBody = null;
                Button.Usable = false;
            
                var bodys = Physics2D
                    .OverlapCircleAll(PlayerControl.LocalPlayer.GetTruePosition(),
                        PlayerControl.LocalPlayer.MaxReportDistance - 2f, Constants.PlayersOnlyMask)
                    .Where(collider => collider.CompareTag("DeadBody")).ToList();
                if (bodys.Count != 0)
                {
                    TargetBody = bodys[0].gameObject;
                    Button.Usable = true;
                }
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
        public static void RpcDragBody(PlayerControl sender, PlayerControl player, bool enable, byte bodyId)
        {
            if (enable)
            {
                var body = Object.FindObjectsOfType<DeadBody>().First(body => body.ParentId == bodyId);
                if (body == null)
                    return;
                MoveBody(player, bodyId);
                Instance.CarriedBodys.Add(player.PlayerId, body.ParentId);
            }
            else
            {
                if (Instance.CarriedBodys.ContainsKey(player.PlayerId))
                    Instance.CarriedBodys.Remove(player.PlayerId);
            }
        }
    }
}