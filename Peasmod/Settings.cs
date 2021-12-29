using HarmonyLib;
using PeasAPI;
using PeasAPI.Options;
using UnhollowerBaseLib;
using UnityEngine;

namespace Peasmod
{
    public static class Settings
    {
        /*
         * This are the unicode symboles I used to have. I keep them here in case I need them again.
         * •; └; └──
         */

        public static CustomOptionHeader Header =
            new CustomOptionHeader(Utility.StringColor.Green + "\nPeasmod" + Utility.StringColor.Reset);

        public static readonly CustomOptionButton
            SectionGeneral = new CustomOptionButton("general", "˅ General", false);

        public static CustomOptionHeader GeneralHeader = new CustomOptionHeader("General");

        public static readonly CustomToggleOption Venting = new CustomToggleOption("venting",
            $"• {Palette.CrewmateBlue.GetTextColor()}Venting{Utility.StringColor.Reset}", true);

        public static readonly CustomToggleOption ReportBodys =
            new CustomToggleOption("reporting",
                $"• {Palette.CrewmateBlue.GetTextColor()}Body-Reporting{Utility.StringColor.Reset}", true);

        public static readonly CustomToggleOption Sabotaging =
            new CustomToggleOption("sabotaging",
                $"• {Palette.CrewmateBlue.GetTextColor()}Sabotaging{Utility.StringColor.Reset}", true);

        public static readonly CustomToggleOption CrewVenting =
            new CustomToggleOption("crewventing",
                $"• {Palette.CrewmateBlue.GetTextColor()}Crew-Venting{Utility.StringColor.Reset}", false);

        public static readonly CustomOptionButton SectionNeutralRoles =
            new CustomOptionButton("NeutralRoles", "˅ Neutral Roles", false);

        public static CustomOptionHeader NeutralRolesHeader =
            new CustomOptionHeader($"{Utility.StringColor.Reset}Neutral Roles");

        public static readonly CustomNumberOption AssassinAmount =
            new CustomNumberOption("assassins",
                $"• {Color.magenta.GetTextColor()}Assassins{Utility.StringColor.Reset}", 0, 14, 1, 0,
                NumberSuffixes.None);
        
        public static readonly CustomNumberOption ChangelingAmount =
            new CustomNumberOption("changelings",
                $"• {ModdedPalette.ChangelingColor.GetTextColor()}Changelings{Utility.StringColor.Reset}", 0, 14, 1, 0,
                NumberSuffixes.None);
        
        public static readonly CustomNumberOption JesterAmount =
            new CustomNumberOption("jesters",
                $"• {ModdedPalette.JesterColor.GetTextColor()}Jesters{Utility.StringColor.Reset}", 0, 14, 1, 0,
                NumberSuffixes.None);

        public static readonly CustomNumberOption TrollAmount =
            new CustomNumberOption("trolls",
                $"• {ModdedPalette.TrollColor.GetTextColor()}Trolls{Utility.StringColor.Reset}", 0, 14, 1, 0,
                NumberSuffixes.None);

        public static readonly CustomOptionButton SectionCrewmateRoles =
            new CustomOptionButton("CrewmateRoles", "˅ Crewmate Roles", false);

        public static CustomOptionHeader CrewmateRolesHeader =
            new CustomOptionHeader($"{Utility.StringColor.Reset}Crewmate Roles");

        public static readonly CustomNumberOption CaptainAmount =
            new CustomNumberOption("captains",
                $"• {ModdedPalette.CaptainColor.GetTextColor()}Captains{Utility.StringColor.Reset}", 0, 14, 1, 0,
                NumberSuffixes.None);

        public static readonly CustomNumberOption CaptainCooldown =
            new CustomNumberOption("captaincooldown", "└ Call-Cooldown", 10, 60, 1, 20, NumberSuffixes.Seconds);

        public static readonly CustomNumberOption DemonAmount =
            new CustomNumberOption("demons",
                $"• {ModdedPalette.DemonColor.GetTextColor()}Demons{Utility.StringColor.Reset}", 0, 14, 1, 0,
                NumberSuffixes.None);

        public static readonly CustomNumberOption DemonCooldown =
            new CustomNumberOption("demoncooldown", "└ Demon-Ability-Cooldown", 10, 60, 1, 20, NumberSuffixes.Seconds);

        public static readonly CustomNumberOption DemonDuration =
            new CustomNumberOption("demonduration", "└ Demon-Ability-Duration", 10, 60, 1, 10, NumberSuffixes.Seconds);

        public static readonly CustomNumberOption DoctorAmount =
            new CustomNumberOption("doctors",
                $"• {ModdedPalette.DoctorColor.GetTextColor()}Doctors{Utility.StringColor.Reset}", 0, 14, 1, 0,
                NumberSuffixes.None);

        public static readonly CustomNumberOption DoctorCooldown =
            new CustomNumberOption("doctorcooldown", "└ Revive-Cooldown", 10, 60, 1, 20, NumberSuffixes.Seconds);

        public static readonly CustomNumberOption InspectorAmount =
            new CustomNumberOption("inspectors",
                $"• {ModdedPalette.InspectorColor.GetTextColor()}Inspectors{Utility.StringColor.Reset}", 0, 14, 1, 0,
                NumberSuffixes.None);

        public static readonly CustomNumberOption MayorAmount =
            new CustomNumberOption("mayors",
                $"• {ModdedPalette.MayorColor.GetTextColor()}Mayors{Utility.StringColor.Reset}", 0, 14, 1, 0,
                NumberSuffixes.None);

        public static readonly CustomNumberOption SheriffAmount =
            new CustomNumberOption("sheriffs",
                $"• {ModdedPalette.SheriffColor.GetTextColor()}Sheriffs{Utility.StringColor.Reset}", 0, 14, 1, 0,
                NumberSuffixes.None);

        public static readonly CustomNumberOption SheriffCooldown =
            new CustomNumberOption("sheriffcooldown", "└ Shoot-Cooldown", 10, 60, 1, 20, NumberSuffixes.Seconds);

        public static readonly CustomOptionButton SectionImpostorRoles =
            new CustomOptionButton("ImpostorRoles", "˅ Impostor Roles", false);

        public static CustomOptionHeader ImpostorRolesHeader =
            new CustomOptionHeader($"Impostor Roles");

        public static readonly CustomNumberOption JanitorAmount = new CustomNumberOption("janitors",
            $"• {Palette.ImpostorRed.GetTextColor()}Janitors{Utility.StringColor.Reset}", 0, 3, 1, 0,
            NumberSuffixes.None);

        public static readonly CustomNumberOption JanitorCooldown =
            new CustomNumberOption("janitorcooldown", "└ Clean-Body-Cooldown", 10, 120, 1, 40, NumberSuffixes.Seconds);

        public static readonly CustomToggleOption JanitorCanKill =
            new CustomToggleOption("janitorcankill", "└ Can Kill", true);

        public static readonly CustomNumberOption BuilderAmount = new CustomNumberOption("builders",
            $"• {Palette.ImpostorRed.GetTextColor()}Builders{Utility.StringColor.Reset}", 0, 3, 1, 0,
            NumberSuffixes.None);

        public static readonly CustomNumberOption VentBuildingCooldown =
            new CustomNumberOption("ventbuildingcooldown", $"└ Vent-Building-Cooldown", 10, 30, 1, 10,
                NumberSuffixes.Seconds);

        public static readonly CustomNumberOption NinjaAmount = new CustomNumberOption("ninjas",
            $"• {Palette.ImpostorRed.GetTextColor()}Ninjas{Utility.StringColor.Reset}", 0, 3, 1, 0,
            NumberSuffixes.None);

        public static readonly CustomNumberOption InvisibilityCooldown =
            new CustomNumberOption("invisibilitycooldown", $"└ Invisibility-Cooldown", 20, 60, 1, 20,
                NumberSuffixes.Seconds);

        public static readonly CustomNumberOption InvisibilityDuration =
            new CustomNumberOption("invisibilityduration", $"└ Invisibility-Duration", 10, 30, 1, 10,
                NumberSuffixes.Seconds);

        public static readonly CustomNumberOption UndertakerAmount = new CustomNumberOption("undertakers",
            $"• {Palette.ImpostorRed.GetTextColor()}Undertakers{Utility.StringColor.Reset}", 0, 3, 1, 0,
            NumberSuffixes.None);

        public static readonly CustomNumberOption GlaciaterAmount = new CustomNumberOption("Glaciaters",
            $"• {Palette.ImpostorRed.GetTextColor()}Glaciaters{Utility.StringColor.Reset}", 0, 3, 1, 0,
            NumberSuffixes.None);

        public static readonly CustomNumberOption FreezeCooldown =
            new CustomNumberOption("freezecooldown",
                $"└ Freezing-Cooldown", 20, 60, 1, 20,
                NumberSuffixes.Seconds);

        public static readonly CustomNumberOption FreezeDuration =
            new CustomNumberOption("freezeduration",
                $"└ Freezing-Duration", 10, 30, 1, 10,
                NumberSuffixes.Seconds);
        
        public static readonly CustomNumberOption MentalistAmount = new CustomNumberOption("mentalists", "• " + Palette.ImpostorRed.GetTextColor() + "Mentalists<color=#ffffffff>", 0f, 3f, 1f, 0f, NumberSuffixes.None);

        public static readonly CustomNumberOption ControlCooldown = new CustomNumberOption("controlcooldown", "└ Controlling-Cooldown", 20f, 60f, 1f, 20f, NumberSuffixes.Seconds);

        public static readonly CustomNumberOption ControlDuration = new CustomNumberOption("controlduration", "└ Controlling-Duration", 10f, 30f, 1f, 10f, NumberSuffixes.Seconds);

        public static readonly CustomOptionButton SectionModes =
            new CustomOptionButton("ModeSettings", "˅ GameModes", false);

        public static CustomOptionHeader ModesHeader =
            new CustomOptionHeader($"GameModes");

        public static CustomOptionHeader HideAndSeek =
            new CustomOptionHeader($"Hide and Seek");

        public static readonly CustomNumberOption SeekerCooldown =
            new CustomNumberOption("seekercooldown", "• Seeker-Cooldown", 10, 60, 1, 10, NumberSuffixes.Seconds);

        public static readonly CustomNumberOption SeekerDuration =
            new CustomNumberOption("seekerduration", "└ Seeking-Duration", 30, 300, 1, 120, NumberSuffixes.Seconds);

        public static readonly CustomToggleOption SeekerVenting =
            new CustomToggleOption("seekerventing", "└ Can Seeker Vent", false);

        public static CustomOptionHeader GodImpostor =
            new CustomOptionHeader($"God Impostor");

        public static readonly CustomToggleOption VentBuilding =
            new CustomToggleOption("ventbuilding", $"• Vent-Building", false);

        public static readonly CustomToggleOption BodyDragging =
            new CustomToggleOption("bodydragging", $"• Body-Dragging", false);

        public static readonly CustomToggleOption Invisibility =
            new CustomToggleOption("invisibility", $"• Invisibility", false);

        public static readonly CustomToggleOption Freeze =
            new CustomToggleOption("freeze", $"• Freezing", false);

        public static readonly CustomToggleOption Morphing =
            new CustomToggleOption("morphing", $"• Morphing", false);

        public static readonly CustomNumberOption MorphingCooldown =
            new CustomNumberOption("morphingcooldown", $"└ Morphing-Cooldown", 20, 60, 1, 20, NumberSuffixes.Seconds);

        public static void Load()
        {
            SectionGeneralListener(false);
            SectionNeutralRolesListener(false);
            SectionCrewmateRolesListener(false);
            SectionImpostorRolesListener(false);
            SectionModesListener(false);

            GeneralHeader.MenuVisible = false;
            NeutralRolesHeader.MenuVisible = false;
            CrewmateRolesHeader.MenuVisible = false;
            ImpostorRolesHeader.MenuVisible = false;
            ModesHeader.MenuVisible = false;
            SectionGeneral.HudVisible = false;
            SectionNeutralRoles.HudVisible = false;
            SectionCrewmateRoles.HudVisible = false;
            SectionImpostorRoles.HudVisible = false;
            SectionModes.HudVisible = false;
            HideAndSeek.HudVisible = false;
            GodImpostor.HudVisible = false;
        }

        public static void SectionGeneralListener(bool value)
        {
            Venting.MenuVisible = value;
            ReportBodys.MenuVisible = value;
            Sabotaging.MenuVisible = value;
            CrewVenting.MenuVisible = value;
        }

        public static void SectionNeutralRolesListener(bool value)
        {
            AssassinAmount.MenuVisible = value;
            ChangelingAmount.MenuVisible = value;
            JesterAmount.MenuVisible = value;
            TrollAmount.MenuVisible = value;
        }

        public static void SectionCrewmateRolesListener(bool value)
        {
            CaptainAmount.MenuVisible = value;
            CaptainCooldown.MenuVisible = value;
            DemonAmount.MenuVisible = value;
            DemonCooldown.MenuVisible = value;
            DemonDuration.MenuVisible = value;
            DoctorAmount.MenuVisible = value;
            DoctorCooldown.MenuVisible = value;
            MayorAmount.MenuVisible = value;
            InspectorAmount.MenuVisible = value;
            SheriffAmount.MenuVisible = value;
            SheriffCooldown.MenuVisible = value;
        }

        public static void SectionImpostorRolesListener(bool value)
        {
            JanitorAmount.MenuVisible = value;
            JanitorCooldown.MenuVisible = value;
            JanitorCanKill.MenuVisible = value;
            BuilderAmount.MenuVisible = value;
            VentBuildingCooldown.MenuVisible = value;
            NinjaAmount.MenuVisible = value;
            InvisibilityCooldown.MenuVisible = value;
            InvisibilityDuration.MenuVisible = value;
            UndertakerAmount.MenuVisible = value;
            GlaciaterAmount.MenuVisible = value;
            FreezeCooldown.MenuVisible = value;
            FreezeDuration.MenuVisible = value;
            MentalistAmount.MenuVisible = value;
            ControlCooldown.MenuVisible = value;
            ControlDuration.MenuVisible = value;
        }

        public static void SectionModesListener(bool value)
        {
            HideAndSeek.MenuVisible = value;
            SeekerCooldown.MenuVisible = value;
            SeekerDuration.MenuVisible = value;
            SeekerVenting.MenuVisible = value;
            GodImpostor.MenuVisible = value;
            VentBuilding.MenuVisible = value;
            BodyDragging.MenuVisible = value;
            Invisibility.MenuVisible = value;
            Freeze.MenuVisible = value;
            Morphing.MenuVisible = value;
            MorphingCooldown.MenuVisible = value;
        }

        [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Start))]
        public static class GameSettingMenuPatch
        {
            static void Prefix(GameSettingMenu __instance)
            {
                __instance.HideForOnline = new Il2CppReferenceArray<Transform>(0);

                SectionGeneral.OnValueChanged += args => { SectionGeneralListener(args.NewValue); };

                SectionNeutralRoles.OnValueChanged += args => { SectionNeutralRolesListener(args.NewValue); };

                SectionCrewmateRoles.OnValueChanged += args => { SectionCrewmateRolesListener(args.NewValue); };

                SectionImpostorRoles.OnValueChanged += args => { SectionImpostorRolesListener(args.NewValue); };

                SectionModes.OnValueChanged += args => { SectionModesListener(args.NewValue); };
            }
        }
    }
}