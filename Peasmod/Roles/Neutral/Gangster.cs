using System.Linq;
using BepInEx.IL2CPP;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.Managers;
using PeasAPI.Roles;
using UnityEngine;

namespace Peasmod.Roles.Neutral
{
    [RegisterCustomRole]
    public class Gangster : BaseRole
    {
        public Gangster(BasePlugin plugin) : base(plugin)
        {
        }

        public override string Name => "Gangster";
        public override string Description => "Take the role of a dead player";
        public override string TaskText => "Take the role of a dead player";
        public override Color Color => ModdedPalette.ChangelingColor;
        public override Visibility Visibility => Visibility.NoOne;
        public override Team Team => Team.Alone;
        public override bool HasToDoTasks => true;
        public override int Limit => (int) Settings.ChangelingAmount.Value;

        public CustomButton Button;

        public override void OnGameStart()
        {
            Button = CustomButton.AddButton(() =>
                {
                    PlayerMenuManager.OpenPlayerMenu(
                        Utility.GetAllPlayers().Where(p => p.Data.IsDead && !p.IsLocal()).ToList().ConvertAll(p => p.PlayerId),
                        p =>
                        {
                            GameObject.Find(PlayerControl.LocalPlayer.GetRole().Name + "Task")
                                .GetComponent<ImportantTextTask>().Text = p.GetRole() == null
                                ? ""
                                : $"</color>Role: {p.GetRole().Color.GetTextColor()}{p.GetRole().Name}\n{p.GetRole().TaskText}</color>";
                            PlayerControl.LocalPlayer.RpcSetVanillaRole(p.Data.Role.Role);
                            PlayerControl.LocalPlayer.RpcSetRole(p.GetRole());
                            p.RpcSetVanillaRole(RoleTypes.Crewmate);
                            p.RpcSetRole(null);
                        }, () => { Button.SetCoolDown(0); });
                }, 0f,
                Utility.CreateSprite("Peasmod.Resources.Buttons.Default.png"), p => p.IsRole(this) && !p.Data.IsDead, _ => true, text: "<size=40%>Change\nRole", textOffset: new Vector2(0f, 0.5f));
        }
    }
}