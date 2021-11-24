using HarmonyLib;
using PeasAPI;
using PeasAPI.Options;
using Peasmod.Utility;
using UnhollowerBaseLib;
using UnityEngine;

namespace Peasmod
{
    public static class Settings
    {
        /*
         * This are the unicode symboles I used to have. I keep them here if I will need them again.
         * •; └; └──
         */
        
        public static CustomOptionHeader Header =
            new CustomOptionHeader(StringColor.Green + "\nPeasmod" + StringColor.Reset);

        public static readonly CustomToggleOption SectionGeneral = new CustomToggleOption("general", "˅ General", false);
        
        public static CustomOptionHeader GeneralHeader = new CustomOptionHeader("General");
        
        public static readonly CustomToggleOption Venting = new CustomToggleOption("venting", $"• {Palette.CrewmateBlue.GetTextColor()}Venting{StringColor.Reset}", true);

        public static readonly CustomToggleOption ReportBodys =
            new CustomToggleOption("reporting", $"• {Palette.CrewmateBlue.GetTextColor()}Body-Reporting{StringColor.Reset}", true);

        public static readonly CustomToggleOption Sabotaging =
            new CustomToggleOption("sabotaging", $"• {Palette.CrewmateBlue.GetTextColor()}Sabotaging{StringColor.Reset}", true);

        public static readonly CustomToggleOption SectionSpecial = new CustomToggleOption("special", "˅ Special", false);
        
        public static CustomOptionHeader SpecialHeader =
            new CustomOptionHeader($"{StringColor.Reset}Special");

        public static readonly CustomToggleOption CrewVenting =
            new CustomToggleOption("crewventing", $"• {Palette.ImpostorRed.GetTextColor()}Crew-Venting{StringColor.Reset}", false);

        public static readonly CustomToggleOption VentBuilding =
            new CustomToggleOption("ventbuilding", $"• {Palette.ImpostorRed.GetTextColor()}Vent-Building{StringColor.Reset}",  false);

        public static readonly CustomNumberOption VentBuildingCooldown =
            new CustomNumberOption("ventbuildingcooldown", $"└ {Palette.ImpostorRed.GetTextColor()}Vent-Building-Cooldown{StringColor.Reset}", 7, 30, 2, 1, NumberSuffixes.Seconds);

        public static readonly CustomToggleOption BodyDragging =
            new CustomToggleOption("bodydragging", $"• {Palette.ImpostorRed.GetTextColor()}Body-Dragging{StringColor.Reset}", false);

        public static readonly CustomToggleOption Invisibility =
            new CustomToggleOption("invisibility", $"• {Palette.ImpostorRed.GetTextColor()}Invisibility{StringColor.Reset}", false);

        public static readonly CustomNumberOption InvisibilityCooldown =
            new CustomNumberOption("invisibilitycooldown", $"└ {Palette.ImpostorRed.GetTextColor()}Invisibility-Cooldown{StringColor.Reset}", 20, 60, 2, 2, NumberSuffixes.Seconds);

        public static readonly CustomNumberOption InvisibilityDuration =
            new CustomNumberOption("invisibilityduration", $"└ {Palette.ImpostorRed.GetTextColor()}Invisibility-Duration{StringColor.Reset}", 10, 30, 2, 1, NumberSuffixes.Seconds);

        public static readonly CustomToggleOption FreezeTime =
            new CustomToggleOption("freeze", $"• {Palette.ImpostorRed.GetTextColor()}Time-Freezing{StringColor.Reset}", false);

        public static readonly CustomNumberOption FreezeTimeCooldown =
            new CustomNumberOption("freezecooldown", $"└ {Palette.ImpostorRed.GetTextColor()}Time-Freezing-Cooldown{StringColor.Reset}", 20, 60, 2, 2, NumberSuffixes.Seconds);

        public static readonly CustomNumberOption FreezeTimeDuration =
            new CustomNumberOption("freezeduration", $"└ {Palette.ImpostorRed.GetTextColor()}Time-Freezing-Duration{StringColor.Reset}", 10, 30, 2, 1, NumberSuffixes.Seconds);

        public static readonly CustomToggleOption Morphing =
            new CustomToggleOption("morphing", $"• {Palette.ImpostorRed.GetTextColor()}Morphing{StringColor.Reset}", false);

        public static readonly CustomNumberOption MorphingCooldown =
            new CustomNumberOption("morphingcooldown", $"└ {Palette.ImpostorRed.GetTextColor()}Morphing-Cooldown{StringColor.Reset}", 20, 60, 2, 2, NumberSuffixes.Seconds);

        public static readonly CustomToggleOption SectionRoles = new CustomToggleOption("Roles", "˅ Roles", false);
        
        public static CustomOptionHeader RolesHeader =
            new CustomOptionHeader($"{StringColor.Reset}Roles");

        public static readonly CustomNumberOption JesterAmount =
            new CustomNumberOption("jesters", $"• {ModdedPalette.JesterColor.GetTextColor()}Jesters{StringColor.Reset}", 0, 14, 1, 0, NumberSuffixes.None);
        
        public static readonly CustomNumberOption TrollAmount =
            new CustomNumberOption("trolls", $"• {ModdedPalette.TrollColor.GetTextColor()}Trolls{StringColor.Reset}", 0, 14, 1, 0, NumberSuffixes.None);
        
        public static readonly CustomNumberOption CaptainAmount =
            new CustomNumberOption("captains", $"• {ModdedPalette.CaptainColor.GetTextColor()}Captains{StringColor.Reset}", 0, 14, 1, 0, NumberSuffixes.None);
        
        public static readonly CustomNumberOption DemonAmount =
            new CustomNumberOption("demons", $"• {ModdedPalette.DemonColor.GetTextColor()}Demons{StringColor.Reset}", 0, 14, 1, 0, NumberSuffixes.None);
        
        public static readonly CustomNumberOption DemonCooldown =
            new CustomNumberOption("demoncooldown", "└ Demon-Ability-Cooldown", 10, 60, 2, 2, NumberSuffixes.Seconds);
        
        public static readonly CustomNumberOption DemonDuration =
            new CustomNumberOption("demonduration", "└ Demon-Ability-Duration", 10, 60, 2, 2, NumberSuffixes.Seconds);

        public static readonly CustomNumberOption DoctorAmount =
            new CustomNumberOption("doctors", $"• {ModdedPalette.DoctorColor.GetTextColor()}Doctors{StringColor.Reset}", 0, 14, 1, 0, NumberSuffixes.None);

        public static readonly CustomNumberOption DoctorCooldown =
            new CustomNumberOption("doctorcooldown", "└ Revive-Cooldown", 10, 60, 2, 2, NumberSuffixes.Seconds);

        public static readonly CustomNumberOption InspectorAmount =
            new CustomNumberOption("inspectors", $"• {ModdedPalette.InspectorColor.GetTextColor()}Inspectors{StringColor.Reset}", 0, 0, 14, 1, NumberSuffixes.None);

        public static readonly CustomNumberOption MayorAmount =
            new CustomNumberOption("mayors", $"• {ModdedPalette.MayorColor.GetTextColor()}Mayors{StringColor.Reset}", 0, 14, 1, 0, NumberSuffixes.None);

        public static readonly CustomNumberOption SheriffAmount =
            new CustomNumberOption("sheriffs", "• Sheriffs", 0, 14, 1, 0, NumberSuffixes.None);

        public static readonly CustomNumberOption SheriffCooldown =
            new CustomNumberOption("sheriffcooldown", "└ Shoot-Cooldown", 10, 60, 2, 2, NumberSuffixes.Seconds);

        public static readonly CustomStringOption GameModeOption = new CustomStringOption("gamemode", "GameMode", "None", "Hide and Seek", "BattleRoyale");

        public static readonly CustomNumberOption SeekerCooldown =
            new CustomNumberOption("seekercooldown", "Seeker-Cooldown", 10, 60, 2, 2, NumberSuffixes.Seconds);

        public static void Load()
        {
            SectionGeneralListener(false);
            SectionSpecialListener(false);
            SectionRolesListener(false);
            
            GeneralHeader.MenuVisible = false;
            SpecialHeader.MenuVisible = false;
            RolesHeader.MenuVisible = false;
            SectionGeneral.HudVisible = false;
            SectionSpecial.HudVisible = false;
            SectionRoles.HudVisible = false;
        }
        
        public static void SectionGeneralListener(bool value)
        {
            Venting.MenuVisible = value;
            ReportBodys.MenuVisible = value;
            Sabotaging.MenuVisible = value;
        }

        public static void SectionSpecialListener(bool value)
        {
            CrewVenting.MenuVisible = value;
            VentBuilding.MenuVisible = value;
            VentBuildingCooldown.MenuVisible = value;
            BodyDragging.MenuVisible = value;
            Invisibility.MenuVisible = value;
            InvisibilityCooldown.MenuVisible = value;
            InvisibilityDuration.MenuVisible = value;
            FreezeTime.MenuVisible = value;
            FreezeTimeCooldown.MenuVisible = value;
            FreezeTimeDuration.MenuVisible = value;
            Morphing.MenuVisible = value;
            MorphingCooldown.MenuVisible = value;
        }

        public static void SectionRolesListener(bool value)
        {
            JesterAmount.MenuVisible = value;
            TrollAmount.MenuVisible = value;
            CaptainAmount.MenuVisible = value;
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

        public enum GameMode : int
        {
            Roles = 0,
            HideAndSeek = 1,
            BattleRoyale = 2,
            HotPotato = 3
        }

        public static bool IsGameMode(GameMode mode)
        {
            if (GameModeOption.Value == (int) mode)
                return true;
            return false;
        }

        [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Update))]
        class GameOptionsMenuUpdate
        {
            static void Postfix(ref GameOptionsMenu __instance)
            {
                //__instance.GetComponentInParent<Scroller>().YBounds.max = 21.5f;
            }
        }

        [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Start))]
        public static class GameSettingMenuPatch
        {
            static void Prefix(GameSettingMenu __instance)
            {
                __instance.HideForOnline = new Il2CppReferenceArray<Transform>(0);

                SectionGeneral.OnValueChanged += (args) => { SectionGeneralListener(args.NewValue); };
                SectionGeneral.SetValue(false);

                SectionSpecial.OnValueChanged += (args) => { SectionSpecialListener(args.NewValue); };
                SectionSpecial.SetValue(false);

                SectionRoles.OnValueChanged += (args) => { SectionRolesListener(args.NewValue); };
                SectionRoles.SetValue(false);
                
                GameModeOption.OnValueChanged += (args) =>
                {
                    PeasAPI.PeasAPI.EnableRoles = args.NewValue == (int)GameMode.Roles;
                };
            }
        }
        
        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameJoined))]
        class AmongUsClientOnGameJoinedPatch
        {
            static void Postfix(AmongUsClient __instance)
            {
                PeasAPI.PeasAPI.EnableRoles = GameModeOption.Value == (int)GameMode.Roles;
            }
        }
    }
}