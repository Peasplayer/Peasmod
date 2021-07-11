using Essentials.Options;
using HarmonyLib;
using Peasmod.Utility;
using UnhollowerBaseLib;
using UnityEngine;

namespace Peasmod
{
    public static class Settings
    {
        public static CustomOptionHeader Header =
            CustomOption.AddHeader(StringColor.Green + "Peasmod" + StringColor.Reset);

        private static readonly CustomOption SectionGeneral = CustomOption.AddButton("˅ General");
        
        public static readonly CustomToggleOption Venting = CustomOption.AddToggle("venting", "• Venting", true, true);

        public static readonly CustomToggleOption ReportBodys =
            CustomOption.AddToggle("reportbodys", "• Body-Reporting", true, true);

        public static readonly CustomToggleOption Sabotaging =
            CustomOption.AddToggle("sabotaging", "• Sabotaging", true, true);

        private static readonly CustomOption SectionSpecial = CustomOption.AddButton("˅ Special");

        public static readonly CustomToggleOption CrewVenting =
            CustomOption.AddToggle("crewventing", "• Crew-Venting", true, false);

        public static readonly CustomToggleOption VentBuilding =
            CustomOption.AddToggle("ventbuilding", "• Vent-Building", true, false);

        public static readonly CustomNumberOption VentBuildingCooldown =
            CustomOption.AddNumber("ventbuildingcooldown", "╚══ Vent-Building-Cooldown", true, 7, 2, 30, 1);

        public static readonly CustomToggleOption BodyDragging =
            CustomOption.AddToggle("bodydragging", "• Body-Dragging", true, false);

        public static readonly CustomToggleOption Invisibility =
            CustomOption.AddToggle("invisibility", "• Invisibility", true, false);

        public static readonly CustomNumberOption InvisibilityCooldown =
            CustomOption.AddNumber("invisibilitycooldown", "╚══ Invisibility-Cooldown", true, 20, 2, 60, 2);

        public static readonly CustomNumberOption InvisibilityDuration =
            CustomOption.AddNumber("invisibilityduration", "╚══ Invisibility-Duration", true, 10, 2, 30, 1);

        public static readonly CustomToggleOption FreezeTime =
            CustomOption.AddToggle("freezetime", "• Time-Freezing", true, false);

        public static readonly CustomNumberOption FreezeTimeCooldown =
            CustomOption.AddNumber("freezetimecooldown", "╚══ Time-Freezing-Cooldown", true, 20, 2, 60, 2);

        public static readonly CustomNumberOption FreezeTimeDuration =
            CustomOption.AddNumber("freezetimeduration", "╚══ Time-Freezing-Duration", true, 10, 2, 30, 1);

        public static readonly CustomToggleOption Morphing =
            CustomOption.AddToggle("morphing", "• Morphing", true, false);

        public static readonly CustomNumberOption MorphingCooldown =
            CustomOption.AddNumber("morphingcooldown", "╚══ Morphing-Cooldown", true, 20, 2, 60, 2);

        private static readonly CustomOptionButton SectionRoles = CustomOption.AddButton("˅ Roles");

        public static readonly CustomNumberOption JesterAmount =
            CustomOption.AddNumber("jesters", "• Jesters", true, 0, 0, 9, 1);

        public static readonly CustomNumberOption DoctorAmount =
            CustomOption.AddNumber("doctors", "• Doctors", true, 0, 0, 9, 1);

        public static readonly CustomNumberOption DoctorCooldown =
            CustomOption.AddNumber("doctorcooldown", "╚══ Revive-Cooldown", true, 10, 2, 60, 2);

        public static readonly CustomNumberOption MayorAmount =
            CustomOption.AddNumber("mayors", "• Mayors", true, 0, 0, 9, 1);

        public static readonly CustomNumberOption InspectorAmount =
            CustomOption.AddNumber("inspectors", "• Inspectors", true, 0, 0, 9, 1);

        public static readonly CustomNumberOption SheriffAmount =
            CustomOption.AddNumber("sheriffs", "• Sheriffs", true, 0, 0, 9, 1);

        public static readonly CustomNumberOption SheriffCooldown =
            CustomOption.AddNumber("sheriffcooldown", "╚══ Shoot-Cooldown", true, 10, 2, 60, 2);

        public static readonly CustomStringOption Gamemode = CustomOption.AddString("gamemode", "Gamemode", "None", "HotPotato", "Battle Royale");

        public static readonly CustomNumberOption HotPotatoTimer =
            CustomOption.AddNumber("hotpotatotimer", "HotPotato-Timer", true, 10, 2, 60, 2);

        public static void Load()
        {
            /*
             * I disabled the credits in game. I provide credit here and on my repository. If there is any problem with the author of this library feel free to contact me via email: peasplayer@peasplayer.tk
             * Essentials: https://github.com/DorCoMaNdO/Reactor-Essentials
             * Author: DorComando (https://github.com/DorCoMaNdO)
             */
            CustomOption.ShamelessPlug = false;
            CustomOption.Debug = false;
            #region OptionsHudVisibility
            SectionGeneralListener(false);
            SectionSpecialListener(false);
            SectionRolesListener(false);
            Venting.HudVisible = false;
            ReportBodys.HudVisible = false;
            Sabotaging.HudVisible = false;
            CrewVenting.HudVisible = false;
            VentBuilding.HudVisible = false;
            VentBuildingCooldown.HudVisible = false;
            BodyDragging.HudVisible = false;
            Invisibility.HudVisible = false;
            InvisibilityCooldown.HudVisible = false;
            InvisibilityDuration.HudVisible = false;
            FreezeTime.HudVisible = false;
            FreezeTimeCooldown.HudVisible = false;
            FreezeTimeDuration.HudVisible = false;
            Morphing.HudVisible = false;
            MorphingCooldown.HudVisible = false;
            JesterAmount.HudVisible = false;
            DoctorAmount.HudVisible = false;
            DoctorCooldown.HudVisible = false;
            MayorAmount.HudVisible = false;
            InspectorAmount.HudVisible = false;
            SheriffAmount.HudVisible = false;
            SheriffCooldown.HudVisible = false;
            Gamemode.HudVisible = false;
            HotPotatoTimer.HudVisible = false;
            #endregion OptionsHudVisibility
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
            DoctorAmount.MenuVisible = value;
            DoctorCooldown.MenuVisible = value;
            MayorAmount.MenuVisible = value;
            InspectorAmount.MenuVisible = value;
            SheriffAmount.MenuVisible = value;
            SheriffCooldown.MenuVisible = value;
        }

        public enum GameMode
        {
            Normal = 0,
            Freeplay = 1,
            HotPotato = 2,
            BattleRoyale = 3
        }

        public static bool IsGameMode(GameMode mode)
        {
            if (mode == GameMode.Freeplay && AmongUsClient.Instance.GameMode == GameModes.FreePlay)
                return true;
            if (AmongUsClient.Instance.GameMode == GameModes.FreePlay)
                return false;
            if (Gamemode.GetValue() == (int) mode)
                return true;
            return false;
        }

        [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Update))]
        class GameOptionsMenuUpdate
        {
            static void Postfix(ref GameOptionsMenu __instance)
            {
                __instance.GetComponentInParent<Scroller>().YBounds.max = 21.5f;
            }
        }

        [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.OnEnable))]
        public static class GameSettingMenuPatch
        {
            static void Prefix(GameSettingMenu __instance)
            {
                __instance.HideForOnline = new Il2CppReferenceArray<Transform>(0);

                SectionGeneral.SetValue(false);
                SectionGeneral.OnValueChanged += (sender, args) => { SectionGeneralListener((bool) args.Value); };

                SectionSpecial.SetValue(false);
                SectionSpecial.OnValueChanged += (sender, args) => { SectionSpecialListener((bool) args.Value); };

                SectionRoles.SetValue(false);
                SectionRoles.OnValueChanged += (sender, args) => { SectionRolesListener((bool) args.Value); };
            }
        }
    }
}