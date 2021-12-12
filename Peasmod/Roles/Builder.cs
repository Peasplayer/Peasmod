using System.Linq;
using BepInEx.IL2CPP;
using Hazel;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.Roles;
using Reactor;
using Reactor.Networking;
using Reactor.Networking.MethodRpc;
using UnityEngine;

namespace Peasmod.Roles
{
    [RegisterCustomRole]
    public class Builder : BaseRole
    {
        public Builder(BasePlugin plugin) : base(plugin)
        {
        }

        public override string Name => "Builder";
        public override string Description => "Build new vents";
        public override string TaskText => "Add new vents to the map";
        public override Color Color => Palette.ImpostorRed;
        public override Visibility Visibility => Visibility.Impostor;
        public override Team Team => Team.Impostor;
        public override bool HasToDoTasks => false;
        public override int Limit => (int) Settings.BuilderAmount.Value;
        public override bool CanVent => true;
        public override bool CanKill(PlayerControl victim = null) => !victim || victim.Data.Role.IsImpostor;
        public override bool CanSabotage(SystemTypes? sabotage) => true;

        public CustomButton Button;
        
        public override void OnGameStart()
        {
            Button = CustomButton.AddRoleButton(() =>
            {
                Rpc<RpcCreateVent>.Instance.Send(PlayerControl.LocalPlayer.transform.position);
            }, Settings.VentBuildingCooldown.Value, PeasAPI.Utility.CreateSprite("Peasmod.Resources.Buttons.CreateVent.png", 552f), Vector2.zero, false, this, "<size=40%>Build");
        }

        private static int _lastVent = int.MaxValue;
        private static int GetVentId() => ShipStatus.Instance.AllVents.Count;
        
        public static void CreateVent(Vector3 position)
        {
            var realPos = new Vector3(position.x, position.y, position.z + .001f);
            var ventPref = Object.FindObjectOfType<Vent>();
            var vent = Object.Instantiate(ventPref, ventPref.transform.parent);
            vent.Id = GetVentId();
            vent.transform.position = realPos;
            var leftVent = int.MaxValue;
            if (_lastVent != int.MaxValue)
            {
                leftVent = _lastVent;
            }
            
            vent.Left = leftVent == int.MaxValue ? null : ShipStatus.Instance.AllVents.FirstOrDefault(v => v.Id == leftVent);
            vent.Center = null;
            vent.Right = null;
            
            var allVents = ShipStatus.Instance.AllVents.ToList();
            allVents.Add(vent);
            ShipStatus.Instance.AllVents = allVents.ToArray();
            if (vent.Left != null)
                vent.Left.Right = ShipStatus.Instance.AllVents.FirstOrDefault(v => v.Id == vent.Id);
            _lastVent = vent.Id;
        }

        [RegisterCustomRpc((uint) CustomRpcCalls.CreateVent)]
        public class RpcCreateVent : PlayerCustomRpc<PeasmodPlugin, Vector3>
        {
            public RpcCreateVent(PeasmodPlugin plugin, uint id) : base(plugin, id)
            {
            }

            public override RpcLocalHandling LocalHandling => RpcLocalHandling.Before;
            
            public override void Write(MessageWriter writer, Vector3 data)
            {
                writer.Write(data.x);
                writer.Write(data.y);
                writer.Write(data.z);
            }

            public override Vector3 Read(MessageReader reader)
            {
                return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            }

            public override void Handle(PlayerControl innerNetObject, Vector3 data)
            {
                CreateVent(data);
            }
        }
    }
}