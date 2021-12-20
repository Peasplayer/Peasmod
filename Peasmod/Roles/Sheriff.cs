using BepInEx.IL2CPP;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.Roles;
using UnityEngine;

namespace Peasmod.Roles
{
    [RegisterCustomRole]
    public class Sheriff : BaseRole
    {
        public Sheriff(BasePlugin plugin) : base(plugin) { }

        public override string Name => "Sheriff";
        public override string Description => "Execute the impostor";
        public override string TaskText => "Execute the impostor";
        public override Color Color => new Color(255f / 255f, 114f / 255f, 0f / 255f);
        public override Team Team => Team.Crewmate;
        public override Visibility Visibility => Visibility.NoOne;
        public override bool HasToDoTasks => true;
        public override int Limit => (int) Settings.SheriffAmount.Value;
        public override bool CanKill(PlayerControl victim = null) => true;
        public override PlayerControl FindClosestTarget(PlayerControl from, bool protecting)
        {
            PlayerControl result = null;
            float num = GameOptionsData.KillDistances[Mathf.Clamp(PlayerControl.GameOptions.KillDistance, 0, 2)];
            if (!ShipStatus.Instance)
            {
                return null;
            }
            Vector2 truePosition = from.GetTruePosition();
            foreach (var playerInfo in GameData.Instance.AllPlayers)
            {
                if (!playerInfo.Disconnected && playerInfo.PlayerId != from.PlayerId && !playerInfo.IsDead && !playerInfo.Object.inVent)
                {
                    PlayerControl @object = playerInfo.Object;
                    if (@object && @object.Collider.enabled)
                    {
                        Vector2 vector = @object.GetTruePosition() - truePosition;
                        float magnitude = vector.magnitude;
                        if (magnitude <= num && !PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized, magnitude, Constants.ShipAndObjectsMask))
                        {
                            result = @object;
                            num = magnitude;
                        }
                    }
                }
            }
            return result;
        }

        public override void OnKill(PlayerControl victim)
        {
            if (!victim.Data.Role.IsImpostor && !victim.IsLocal())
                PlayerControl.LocalPlayer.RpcMurderPlayer(PlayerControl.LocalPlayer);
        }
    }
}