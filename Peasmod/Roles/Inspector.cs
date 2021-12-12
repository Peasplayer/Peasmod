using System.Collections.Generic;
using System.Linq;
using BepInEx.IL2CPP;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.Roles;
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

        public override int Limit => (int) Settings.InspectorAmount.Value;

        public override Team Team => Team.Crewmate;

        public override Visibility Visibility => Visibility.NoOne;

        public override bool HasToDoTasks => true;

        public override void OnGameStart()
        {
            Dots.Clear();
            if (PlayerControl.LocalPlayer.IsRole<Inspector>())
            {
                foreach (var player in GameData.Instance.AllPlayers)
                    Dots.Add(player.PlayerId, new Dictionary<GameObject, float>());
            }
        }

        private static float _timer;
        private static readonly float MaxTimer = 0.125f;
        private static readonly Dictionary<byte, Dictionary<GameObject, float>> Dots = new ();

        public override void OnUpdate()
        {
            for (int i = 0; i < Dots.Count; i++)
            {
                var player = Dots.ElementAt(i).Key.GetPlayer();
                if (player == null)
                    continue;
                
                var dots = Dots.ElementAt(i).Value;
                
                if (_timer < 0f)
                {
                    if (!player.Data.IsDead && !player.Data.Disconnected)
                    {
                        if (PlayerControl.LocalPlayer.IsRole<Inspector>())
                        {
                            var dot = new GameObject();
                            var renderer = dot.AddComponent<SpriteRenderer>();
                            renderer.sprite = PeasAPI.Utility.CreateSprite("Peasmod.Resources.Dot.png");
                            dot.transform.localPosition = new Vector3(player.GetTruePosition().x, player.GetTruePosition().y, player.transform.position.z);
                            dot.GetComponent<SpriteRenderer>().material.color = Palette.PlayerColors[player.Data.DefaultOutfit.ColorId];
                            dots.Add(dot, Time.time);
                        }
                    }

                    _timer = MaxTimer;
                }
                else
                    _timer -= Time.deltaTime;

                for (int j = 0; j < dots.Count; j++)
                {
                    var dot = dots.ElementAt(j).Key;
                    var time = dots.ElementAt(j).Value;
                    if (time + 2f <= Time.time)
                    {
                        Color color = dot.GetComponent<SpriteRenderer>().material.color;
                        color.a -= 0.2f;
                        dot.GetComponent<SpriteRenderer>().material.color = color;
                        if (color.a <= 0f)
                            dots.Remove(dot);
                        else
                        {
                            time += 2f;
                            dots.Remove(dot);
                            dots.Add(dot, time);
                        }
                    }
                }

                Dots[player.PlayerId] = dots;
            }
        }
    }
}