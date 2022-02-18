using System.Collections.Generic;
using BepInEx.IL2CPP;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.Options;
using PeasAPI.Roles;
using UnityEngine;

namespace Peasmod.Roles.Impostor
{
    [RegisterCustomRole]
    public class Mafioso : BaseRole
    {
        public Mafioso(BasePlugin plugin) : base(plugin)
        {
        }

        public override string Name => "Mafioso";
        public override string Description => "Recruit new impostors";
        public override string LongDescription => "";
        public override string TaskText => "Recruit other players to become impostors";
        public override Color Color => Palette.ImpostorRed;
        public override Visibility Visibility => Visibility.Impostor;
        public override Team Team => Team.Impostor;
        public override bool HasToDoTasks => false;

        public override Dictionary<string, CustomOption> AdvancedOptions { get; set; } =
            new Dictionary<string, CustomOption>()
            {
                {
                    "RecruitCooldown", new CustomNumberOption("MafiosoRecruitCooldown", "Recruit Cooldown", 20f, 120f, 1f, 30f, NumberSuffixes.Seconds)
                },
                {
                    "RecruitAmounts", new CustomNumberOption("MafiosoRecruitAmounts", "Recruit-Amounts", 1f, 3f, 1f, 1f, NumberSuffixes.None)
                }
            };
        public override bool CanKill(PlayerControl victim = null) => !victim || !victim.Data.Role.IsImpostor;
        public override bool CanVent => true;
        public override bool CanSabotage(SystemTypes? sabotage) => true;

        public CustomButton Button;
        public int AlreadyRecruited;

        public override void OnGameStart()
        {
            AlreadyRecruited = 0;
            Button = CustomButton.AddButton(() =>
            {
                Button.PlayerTarget.RpcSetVanillaRole(RoleTypes.Impostor);
                Button.PlayerTarget.RpcSetRole(null);
                AlreadyRecruited++;
            }, ((CustomNumberOption) AdvancedOptions["RecruitCooldown"]).Value, Utility.CreateSprite("Peasmod.Resources.Buttons.Default.png"), control => control.IsRole(this) && !control.Data.IsDead, 
                _ => AlreadyRecruited < ((CustomNumberOption) AdvancedOptions["RecruitAmounts"]).Value, text: "<size=40%>Recruit", textOffset: new Vector2(0f, 0.5f), target: CustomButton.TargetType.Player, targetColor: Color, choosePlayerTarget: control => !control.Data.Role.IsImpostor);
        }
    }
}