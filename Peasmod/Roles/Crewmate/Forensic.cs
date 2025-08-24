using System;
using System.Collections.Generic;
using BepInEx.Unity.IL2CPP;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.Managers;
using PeasAPI.Options;
using PeasAPI.Roles;
using UnityEngine;

namespace Peasmod.Roles.Crewmate;

[RegisterCustomRole]
public class Forensic : BaseRole
{
    public Forensic(BasePlugin plugin) : base(plugin) { }

    public override string Name => "Forensic";
    public override string Description => "Analyse the blood of the other crewmates";
    public override string LongDescription => "Take blood samples of the other crewmates and analyse them to find out the team of them";
    public override string TaskText => "Analyse the blood of the other crewmates";
    public override Color Color => ModdedPalette.ForensicColor;
    public override Visibility Visibility => Visibility.NoOne;
    public override Team Team => Team.Crewmate;
    public override bool HasToDoTasks => true;
    public override Dictionary<string, CustomOption> AdvancedOptions { get; set; } = new Dictionary<string, CustomOption>()
    {
        {
            "AnalyseCooldown", new CustomNumberOption(MultiMenu.Crewmate, "Analyse-Cooldown", 30, 180, 1, 30, CustomOption.Seconds)
        },
        {
            "AnalyseDuration", new CustomNumberOption(MultiMenu.Crewmate, "Analyse-Duration", 30, 180, 1, 30, CustomOption.Seconds)
        }/*,
        {
            "AnalyseCount", new CustomNumberOption("analysecount", "Analyses", 1, 15, 1, 2)
        }*/
    };

    public CustomButton Button;
    public byte BloodSample;

    public override void OnGameStart()
    {
        BloodSample = Byte.MaxValue;
        Button = CustomButton.AddButton(() =>
            {
                BloodSample = PlayerControl.LocalPlayer.Data.Role.FindClosestTarget().PlayerId;
            }, ((CustomNumberOption) AdvancedOptions["AnalyseCooldown"]).Value, Utility.CreateSprite("Peasmod.Resources.Buttons.Default.png"),
            p => p.IsCustomRole(this) && !p.Data.IsDead, p => PlayerControl.LocalPlayer.Data.Role.FindClosestTarget() != null, text: "<size=40%>Analyse\nBlood", textOffset: new Vector2(0f, 0.5f),
            onEffectEnd: () =>
            {
                var target = BloodSample.GetPlayer();
                BloodSample = Byte.MaxValue;
                var team = target.GetCustomRole() == null ? target.Data.Role.IsImpostor ? "evil" : "good" :
                    target.GetCustomRole().Team == Team.Crewmate ? "good" :
                    target.GetCustomRole().Team == Team.Impostor ? "evil" : "neutral";
                TextMessageManager.ShowMessage($"The analyse showed that {target.Data.PlayerName} is {team}", 3f);
            }, effectDuration: ((CustomNumberOption) AdvancedOptions["AnalyseDuration"]).Value, target: CustomButton.TargetType.Player, targetColor: Color);
    }
}