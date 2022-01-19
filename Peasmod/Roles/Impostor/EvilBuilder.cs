using System.Collections.Generic;
using System.Linq;
using BepInEx.IL2CPP;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.Options;
using PeasAPI.Roles;
using Reactor.Networking.MethodRpc;
using UnityEngine;

namespace Peasmod.Roles.Impostor
{
    [RegisterCustomRole]
    public class EvilBuilder : BaseRole
    {
        public EvilBuilder(BasePlugin plugin) : base(plugin)
        {
        }

        public override string Name => "EvilBuilder";
        public override Sprite Icon => Utility.CreateSprite("Peasmod.Resources.Buttons.CreateVent.png");
        public override string Description => "Build new vents";
        public override string LongDescription => "";
        public override string TaskText => "Add new vents to the map";
        public override Color Color => Palette.ImpostorRed;
        public override Visibility Visibility => Visibility.Impostor;
        public override Team Team => Team.Impostor;
        public override bool HasToDoTasks => true;
        public override int MaxCount => 3;
        public override Dictionary<string, CustomOption> AdvancedOptions => new Dictionary<string, CustomOption>()
        {
            {
                "VentBuildingCooldown", new CustomNumberOption("ventbuildingcooldown", $"Vent-Building-Cooldown", 10, 30, 1, 10, NumberSuffixes.Seconds)
            }
        };
        public override bool CanVent => true;
        public override bool CanKill(PlayerControl victim = null) => !victim || victim.Data.Role.IsImpostor;
        public override bool CanSabotage(SystemTypes? sabotage) => false;

        public CustomButton Button;
        public Vector2 VentSize;
        
        public override void OnGameStart()
        {
            Button = CustomButton.AddButton(() =>
            {
                var pos = PlayerControl.LocalPlayer.transform.position;
                RpcCreateVent(PlayerControl.LocalPlayer, pos.x, pos.y, pos.z);
            }, ((CustomNumberOption) AdvancedOptions["VentBuildingCooldown"]).Value, PeasAPI.Utility.CreateSprite("Peasmod.Resources.Buttons.CreateVent.png", 552f), p => p.IsRole(this) && !p.Data.IsDead, _ => true, text: "<size=40%>Build");
        }

        public override void OnUpdate()
        {
            if (PlayerControl.LocalPlayer.IsRole(this))
            {
                if (Object.FindObjectOfType<Vent>() == null)
                    return;
                var vent = Object.FindObjectOfType<Vent>().gameObject;
                var ventSize = Vector2.Scale(vent.GetComponent<BoxCollider2D>().size, vent.transform.localScale) * 0.75f;
                var hits = Physics2D.OverlapBoxAll(PlayerControl.LocalPlayer.transform.position, ventSize, 0).Where(c => (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer != 8 && c.gameObject.layer != 5).ToArray();
                Button.Usable = hits.Length == 0;
            }
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

        [MethodRpc((uint) CustomRpcCalls.CreateVent)]
        public static void RpcCreateVent(PlayerControl sender, float x, float y, float z)
        {
            CreateVent(new Vector3(x, y, z));
        }
    }
}