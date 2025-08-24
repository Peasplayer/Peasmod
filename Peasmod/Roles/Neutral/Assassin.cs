using System.Linq;
using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP;
using PeasAPI;
using PeasAPI.CustomEndReason;
using PeasAPI.Roles;
using UnityEngine;

namespace Peasmod.Roles.Neutral
{
    //[RegisterCustomRole]
    public class Assassin : BaseRole
    {
        public Assassin(BasePlugin plugin) : base(plugin)
        {
            Instance = this;
        }

        public override string Name => "Assasin";
        public override string Description => "Kill every player";
        public override string LongDescription => "";
        public override string TaskText => "Kill every impostor and crewmate";
        public override Color Color => Color.magenta;
        public override Visibility Visibility => Visibility.NoOne;
        public override Team Team => Team.Alone;
        public override bool HasToDoTasks => false;
        //public override int Limit => (int)Settings.AssassinAmount.Value;
        public override int MaxCount => 0;
        public override float KillDistance => LegacyGameOptions.KillDistances[Mathf.Clamp(GameOptionsManager.Instance.currentNormalGameOptions.KillDistance, 0, 2)] + 0.35f;
        public override bool CanKill(PlayerControl victim = null) => true;
        public override bool CanSabotage(SystemTypes? sabotage) => sabotage == null || sabotage == SystemTypes.Comms;
        public override bool ShouldGameEnd(GameOverReason reason)
        {
            PeasAPI.PeasAPI.Logger.LogInfo(reason.ToString());
            if (reason == EndReasonManager.CustomGameOverReason || reason == GameOverReason.ImpostorsBySabotage)
                return true;
            if (Utility.GetAllPlayers().Count(p => !p.Data.IsDead && p.GetCustomRole() != null && p.IsCustomRole(this)) == 0)
                return true;
            return false;
        }

        public static Assassin Instance;

        public override void OnKill(PlayerControl killer, PlayerControl victim)
        {
            if (Utility.GetAllPlayers().Count(p => !p.Data.IsDead && p.GetCustomRole() != null && p.IsCustomRole(this)) != 0)
                GameManager.Instance.RpcEndGame(GameOverReason.ImpostorsByKill, false);
            if (PlayerControl.LocalPlayer.IsCustomRole(this) && killer.IsLocal() &&
                Utility.GetAllPlayers().Count(p => !p.Data.IsDead && !p.IsLocal()) == 0)
            {
                new CustomEndReason(PlayerControl.LocalPlayer);
            }
            if (PlayerControl.LocalPlayer.IsCustomRole(this) && !PlayerControl.LocalPlayer.Data.IsDead && killer.IsLocal() &&
                Utility.GetAllPlayers().Count(p => !p.Data.IsDead && !p.IsLocal() && !p.Data.Role.IsImpostor) == 1)
            {
                new CustomEndReason(PlayerControl.LocalPlayer);
            }
        }
        
        public override void OnExiled(PlayerControl target)
        {
            if (PlayerControl.LocalPlayer.IsCustomRole(this) && target.IsLocal())
                GameManager.Instance.RpcEndGame(GameOverReason.ImpostorsByVote, false);
            if (PlayerControl.LocalPlayer.IsCustomRole(this) && !PlayerControl.LocalPlayer.Data.IsDead && !target.IsLocal() &&
                Utility.GetAllPlayers().Count(p => !p.Data.IsDead && !p.IsLocal()) == 0)
            {
                new CustomEndReason(PlayerControl.LocalPlayer);
            }
            if (PlayerControl.LocalPlayer.IsCustomRole(this) && !PlayerControl.LocalPlayer.Data.IsDead && !target.IsLocal() &&
                Utility.GetAllPlayers().Count(p => !p.Data.IsDead && !p.IsLocal() && !p.Data.Role.IsImpostor) == 1)
            {
                new CustomEndReason(PlayerControl.LocalPlayer);
            }
        }
    }
}