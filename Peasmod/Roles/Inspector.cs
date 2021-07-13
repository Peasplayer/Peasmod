using System.Collections.Generic;
using System.Linq;
using BepInEx.IL2CPP;
using Hazel;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.Roles;
using Peasmod.Utility;
using Reactor;
using Reactor.Networking;
using UnityEngine;

namespace Peasmod.Roles
{
    [RegisterCustomRole]
    public class Inspector : BaseRole
    {
        public Inspector(BasePlugin plugin) : base(plugin)
        {
        }

        public override string Name => "Inspector";

        public override string Description => "Find the Impostor";

        public override string TaskText => "Find the Impostor by his footprints";

        public override Color Color => ModdedPalette.InspectorColor;

        public override int Limit => (int) Settings.InspectorAmount.GetValue();

        public override Team Team => Team.Crewmate;

        public override Visibility Visibility => Visibility.NoOne;

        public override bool HasToDoTasks => true;

        public override void OnGameStart()
        {
        }

        private static float _timer = 0f;
        private static readonly float _maxTimer = 0.125f;
        public static Dictionary<GameObject, float> Dots = new Dictionary<GameObject, float>();

        public override void OnUpdate()
        {
            if (_timer < 0f)
            {
                if (!PlayerControl.LocalPlayer.Data.IsDead)
                {
                    Rpc<InspectorAbilityRpc>.Instance.Send(new InspectorAbilityRpc.Data(PlayerControl.LocalPlayer));
                }

                _timer = _maxTimer;
            }
            else
                _timer -= Time.deltaTime;

            for (int i = 0; i < Dots.Count; i++)
            {
                var dot = Dots.ElementAt(i).Key;
                var time = Dots.ElementAt(i).Value;
                if (time + 2f <= Time.time)
                {
                    Color color = dot.GetComponent<SpriteRenderer>().material.color;
                    color.a -= 0.2f;
                    dot.GetComponent<SpriteRenderer>().material.color = color;
                    if (color.a <= 0f)
                        Dots.Remove(dot);
                    else
                    {
                        time += 2f;
                        Dots.Remove(dot);
                        Dots.Add(dot, time);
                    }
                }
            }
        }

        public override void OnMeetingUpdate(MeetingHud meeting)
        {
        }
        
        [RegisterCustomRpc((uint) CustomRpcCalls.InspectorAbility)]
        public class InspectorAbilityRpc : PlayerCustomRpc<PeasmodPlugin, InspectorAbilityRpc.Data>
        {
            public InspectorAbilityRpc(PeasmodPlugin plugin, uint id) : base(plugin, id)
            {
            }

            public readonly struct Data
            {
                public readonly PlayerControl Player;

                public Data(PlayerControl player)
                {
                    Player = player;
                }
            }

            public override RpcLocalHandling LocalHandling => RpcLocalHandling.None;

            public override void Write(MessageWriter writer, Data data)
            {
                writer.Write(data.Player.PlayerId);
            }

            public override Data Read(MessageReader reader)
            {
                return new Data(reader.ReadByte().GetPlayer());
            }

            public override void Handle(PlayerControl innerNetObject, Data data)
            {
                if(data.Player.PlayerId != PlayerControl.LocalPlayer.PlayerId && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    if (PlayerControl.LocalPlayer.IsRole<Inspector>())
                    {
                        var dot = new GameObject();
                        var renderer = dot.AddComponent<SpriteRenderer>();
                        renderer.sprite = Utils.CreateSprite("Dot.png");
                        dot.transform.localPosition = new Vector3(data.Player.GetTruePosition().x, data.Player.GetTruePosition().y, data.Player.transform.position.z);
                        dot.GetComponent<SpriteRenderer>().material.color = Utils.ColorIdToColor(data.Player.Data.ColorId);
                        Dots.Add(dot, Time.time);
                    }
                }
            }
        }
    }
}