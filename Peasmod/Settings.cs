using PeasAPI;
using PeasAPI.Options;

namespace Peasmod
{
    public class Settings
    {
        /*
         * This are the unicode symboles I used to have. I keep them here in case I need them again.
         * •; └; └──
         */
        public static CustomHeaderOption GeneralHeader = new CustomHeaderOption(MultiMenu.Main, "General");

        public static readonly CustomToggleOption Venting = new CustomToggleOption(MultiMenu.Main,
            $"• {Palette.CrewmateBlue.GetTextColor()}Venting{Utility.StringColor.Reset}", true);

        public static readonly CustomToggleOption ReportBodys =
            new CustomToggleOption(MultiMenu.Main,
                $"• {Palette.CrewmateBlue.GetTextColor()}Body-Reporting{Utility.StringColor.Reset}", true);

        public static readonly CustomToggleOption Sabotaging =
            new CustomToggleOption(MultiMenu.Main,
                $"• {Palette.CrewmateBlue.GetTextColor()}Sabotaging{Utility.StringColor.Reset}", true);

        public static readonly CustomToggleOption CrewVenting =
            new CustomToggleOption(MultiMenu.Main,
                $"• {Palette.CrewmateBlue.GetTextColor()}Crew-Venting{Utility.StringColor.Reset}", false);

        public static CustomHeaderOption HideAndSeek =
            new CustomHeaderOption(MultiMenu.Main, $"Hide and Seek");

        public static readonly CustomNumberOption HideAndSeekSeekerCooldown =
            new CustomNumberOption(MultiMenu.Main, "• Seeker-Cooldown", 20, 60, 1, 20, CustomOption.Seconds);

        public static readonly CustomNumberOption HideAndSeekSeekerDuration =
            new CustomNumberOption(MultiMenu.Main, "• Seeking-Duration", 30, 300, 10, 120, CustomOption.Seconds);

        public static readonly CustomToggleOption HideAndSeekSeekerVenting =
            new CustomToggleOption(MultiMenu.Main, "• Can Seeker Vent", false);

        public static CustomHeaderOption PropHunt =
            new CustomHeaderOption(MultiMenu.Main, $"Prop Hunt");

        public static readonly CustomNumberOption PropHuntSeekerCooldown =
            new CustomNumberOption(MultiMenu.Main, "• Seeker-Cooldown", 20, 60, 1, 20, CustomOption.Seconds);

        public static readonly CustomNumberOption PropHuntSeekerDuration =
            new CustomNumberOption(MultiMenu.Main, "• Seeking-Duration", 30, 300, 10, 120, CustomOption.Seconds);

        public static readonly CustomNumberOption PropHuntSeekerClickCooldown =
            new CustomNumberOption(MultiMenu.Main, "• Seeker-Click-Cooldown", 1, 60, 1, 5, CustomOption.Seconds);

        public static CustomHeaderOption GodImpostor =
            new CustomHeaderOption(MultiMenu.Main, $"God Impostor");

        public static readonly CustomToggleOption VentBuilding =
            new CustomToggleOption(MultiMenu.Main, $"• Vent-Building", false);

        public static readonly CustomToggleOption BodyDragging =
            new CustomToggleOption(MultiMenu.Main, $"• Body-Dragging", false);

        public static readonly CustomToggleOption Invisibility =
            new CustomToggleOption(MultiMenu.Main, $"• Invisibility", false);

        public static readonly CustomToggleOption Freeze =
            new CustomToggleOption(MultiMenu.Main, $"• Freezing", false);

        public static readonly CustomToggleOption Morphing =
            new CustomToggleOption(MultiMenu.Main, $"• Morphing", false);

        public static readonly CustomNumberOption MorphingCooldown =
            new CustomNumberOption(MultiMenu.Main, $"└ Morphing-Cooldown", 20, 60, 1, 20, CustomOption.Seconds);
    }
}