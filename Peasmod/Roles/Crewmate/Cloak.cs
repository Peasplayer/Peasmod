using BepInEx.IL2CPP;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.Roles;
using UnityEngine;

namespace Peasmod.Roles.Crewmate
{
    [RegisterCustomRole]
    public class Cloak : BaseRole
    {
        public Cloak(BasePlugin plugin) : base(plugin)
        {
        }

        public override string Name => "Cloak";
        public override string Description => "You can go invisible";
        public override string TaskText => "Go invisible and try to catch the impostor";
        public override Color Color => ModdedPalette.CloakColor;
        public override Visibility Visibility => Visibility.NoOne;
        public override Team Team => Team.Crewmate;
        public override bool HasToDoTasks => true;
        public override int Limit => (int) Settings.CloakAmount.Value;

        public CustomButton Button;

        public override void OnGameStart()
        {
            Button = CustomButton.AddButton(
                () => { PlayerControl.LocalPlayer.RpcGoInvisible(true); },
                Settings.CloakCooldown.Value, PeasAPI.Utility.CreateSprite("Peasmod.Resources.Buttons.Hide.png", 794f), p => p.IsRole(this) && !p.Data.IsDead, _ => true, effectDuration: Settings.CloakDuration.Value,
                onEffectEnd: () => { PlayerControl.LocalPlayer.RpcGoInvisible(false); }, text: "<size=40%>Hide");
        }
    }
}