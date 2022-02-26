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

        public static readonly CustomOptionButton SectionModes =
            new CustomOptionButton("ModeSettings", "˅ GameModes", false);

        public static CustomOptionHeader ModesHeader =
            new CustomOptionHeader($"GameModes");

        public static CustomOptionHeader HideAndSeek =
            new CustomOptionHeader($"Hide and Seek");

        public static readonly CustomNumberOption HideAndSeekSeekerCooldown =
            new CustomNumberOption("hideandseekseekercooldown", "• Seeker-Cooldown", 20, 60, 1, 20, NumberSuffixes.Seconds);

        public static readonly CustomNumberOption HideAndSeekSeekerDuration =
            new CustomNumberOption("hideandseekseekerduration", "• Seeking-Duration", 30, 300, 10, 120, NumberSuffixes.Seconds);

        public static readonly CustomToggleOption HideAndSeekSeekerVenting =
            new CustomToggleOption("hideandseekseekerventing", "• Can Seeker Vent", false);
        
        public static CustomOptionHeader PropHunt =
            new CustomOptionHeader($"PropHunt");

        public static readonly CustomNumberOption PropHuntSeekerCooldown =
            new CustomNumberOption("prophuntseekercooldown", "• Seeker-Cooldown", 20, 60, 1, 20, NumberSuffixes.Seconds);

        public static readonly CustomNumberOption PropHuntSeekerDuration =
            new CustomNumberOption("prophuntseekerduration", "• Seeking-Duration", 30, 300, 10, 120, NumberSuffixes.Seconds);
        
        public static readonly CustomNumberOption PropHuntSeekerClickCooldown =
            new CustomNumberOption("prophuntseekerclickcooldown", "• Seeker-Click-Cooldown", 1, 60, 1, 5, NumberSuffixes.Seconds);

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
            SectionModesListener(false);

            GeneralHeader.MenuVisible = false;
            ModesHeader.MenuVisible = false;
            SectionGeneral.HudVisible = false;
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

        public static void SectionModesListener(bool value)
        {
            HideAndSeek.MenuVisible = value;
            HideAndSeekSeekerCooldown.MenuVisible = value;
            HideAndSeekSeekerDuration.MenuVisible = value;
            HideAndSeekSeekerVenting.MenuVisible = value;
            PropHunt.MenuVisible = value;
            PropHuntSeekerCooldown.MenuVisible = value;
            PropHuntSeekerDuration.MenuVisible = value;
            PropHuntSeekerClickCooldown.MenuVisible = value;
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

                SectionModes.OnValueChanged += args => { SectionModesListener(args.NewValue); };
            }
        }
    }
}