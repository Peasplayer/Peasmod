using System.Linq;
using BepInEx.IL2CPP;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace Peasmod.ApiExtension.Gamemodes
{
    public class GameMode
    {
        public GameMode(BasePlugin plugin)
        {
            Id = GameModeManager.GetModeId();
            GameModeManager.RegisterMode(this);
        }
        
        public byte Id { get; } = byte.MaxValue;

        public virtual string Name { get; } = "Super cool mode";

        public virtual bool Enabled { get; } = false;

        public virtual bool HasToDoTasks { get; } = false;
        
        public virtual void OnGameStart() {}
        
        public virtual void OnIntro(IntroCutscene._CoBegin_d__14 scene) {}

        public virtual List<PlayerControl> GetIntroTeam()
        {
            return PlayerControl.AllPlayerControls;
        }
        
        public virtual List<PlayerControl> GetImpostors(List<PlayerControl> originalImpostors)
        {
            return originalImpostors;
        }
        
        public virtual void OnUpdate() {}

        public virtual bool OnKill(PlayerControl killer, PlayerControl victim)
        {
            return true;
        }
        
        public virtual bool OnMeetingCall(PlayerControl caller, GameData.PlayerInfo target)
        {
            return true;
        }

        public virtual string GetObjective(PlayerControl player)
        {
            return null;
        }
        
        public virtual bool AllowSabotage()
        {
            return false;
        }

        public virtual bool ShouldGameStop(GameOverReason reason)
        {
            return true;
        }
    }
}