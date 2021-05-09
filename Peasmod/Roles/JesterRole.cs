using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Peasmod.Utility;
using UnityEngine;

namespace Peasmod.Roles
{
    public class JesterRole
    {
        public static string name = "Jester";
        public static string description = "Get voted out";
        public static string ejectText = " was the Jester";
        public static Color color = new Color(136f / 256f, 31f / 255f, 136f / 255f);
        public static List<byte> members = new List<byte>();
        public static Roles.Role role = Roles.Role.Jester;
        public static bool hasWon = false;

        public static void Reset()
        {
            members = new List<byte>();
            hasWon = false;
        }
        
        public static void OnGameStart()
        {
            
        }
        
        public static void OnRpcSetInfected()
        {
            members.Clear();
            for (int i = 1; i <= Settings.JesterAmount.GetValue() && Roles.crewmates.Count >= 1; i++)
            {
                var _role = Roles.crewmates[Peasmod.random.Next(0, Roles.crewmates.Count)].GetPlayer();
                _role.RpcSetRole(role);
                Roles.crewmates.Remove(_role.PlayerId);
            }
        }
        
        public static void OnIntroCutScenePrefix(IntroCutscene.Nested_0 scene)
        {
            if (PlayerControl.LocalPlayer.IsRole(role))
            {
                var yourTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
                yourTeam.Add(PlayerControl.LocalPlayer);
                scene.yourTeam = yourTeam;
            }
        }
        
        public static void OnIntroCutScenePostfix(IntroCutscene.Nested_0 scene)
        {
            if (PlayerControl.LocalPlayer.IsRole(role))
            {
                var inst = scene.__this;
                inst.Title.text = name;
                inst.Title.color = color;
                inst.ImpostorText.text = description;
                inst.BackgroundBar.material.color = color;
            }
        }

        public static void OnExiled(ref string __result)
        {
            if (ExileController.Instance.exiled.Object.IsRole(role))
            {
                __result = ExileController.Instance.exiled.PlayerName + ejectText;
            }
        }

        public static void OnUpdate()
        {
            if (PlayerControl.LocalPlayer.IsRole(Roles.Role.Jester))
                PlayerControl.LocalPlayer.nameText.color = color;
            if (MeetingHud.Instance != null)
            {
                foreach (var pstate in MeetingHud.Instance.playerStates)
                    if (pstate.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId &&
                        PlayerControl.LocalPlayer.IsRole(role))
                        pstate.NameText.color = color;
            }
        }

        private class Patches
        {
            [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Exiled))]
            public static class PlayerControlExiledPatch
            {
                public static void Prefix(PlayerControl __instance)
                {
                    if (Settings.IsGameMode(Settings.GameMode.Normal) && __instance.IsRole(Roles.Role.Jester))
                    {
                        
                    }
                }
            }
        }
    }
}