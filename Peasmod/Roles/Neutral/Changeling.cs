using System.Linq;
using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.Managers;
using PeasAPI.Roles;
using UnityEngine;

namespace Peasmod.Roles.Neutral
{
    [RegisterCustomRole]
    public class Changeling : BaseRole
    {
        public Changeling(BasePlugin plugin) : base(plugin)
        {
        }

        public override string Name => "Changeling";
        public override string Description => "Take the role of another player";
        public override string LongDescription => "";
        public override string TaskText => "Take the role of another player";
        public override Color Color => ModdedPalette.ChangelingColor;
        public override Visibility Visibility => Visibility.NoOne;
        public override Team Team => Team.Alone;
        public override bool HasToDoTasks => true;

        public CustomButton Button;

        public override void OnGameStart()
        {
            Button = CustomButton.AddButton(() =>
                {
                    PlayerMenuManager.OpenPlayerMenu(
                        Utility.GetAllPlayers().Where(p => !p.Data.IsDead && !p.IsLocal()).ToList().ConvertAll(p => p.PlayerId),
                        p =>
                        {
                            GameObject.Find(PlayerControl.LocalPlayer.GetCustomRole().Name + "Task")
                                .GetComponent<ImportantTextTask>().Text = p.GetCustomRole() == null
                                ? ""
                                : $"</color>Role: {p.GetCustomRole().Color.GetTextColor()}{p.GetCustomRole().Name}\n{p.GetCustomRole().TaskText}</color>";
                            PlayerControl.LocalPlayer.RpcSetVanillaRole(p.Data.Role.Role);
                            PlayerControl.LocalPlayer.RpcSetRole(p.GetCustomRole());
                            p.RpcSetVanillaRole(RoleTypes.Crewmate);
                            p.RpcSetRole(null);
                        }, () => Button.SetCoolDown(0));
                }, 0f,
                Utility.CreateSprite("Peasmod.Resources.Buttons.Default.png"), p => p.IsCustomRole(this) && !p.Data.IsDead, _ => true, text: "<size=40%>Change\nRole", textOffset: new Vector2(0f, 0.5f));
        }
    }
}