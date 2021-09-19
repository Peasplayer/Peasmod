using Essentials.Options;
using HarmonyLib;
using PeasAPI;
using Peasmod.Roles;
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
            CustomOption.AddHeader(StringColor.Green + "\nPeasmod" + StringColor.Reset);

        private static readonly CustomOption SectionGeneral = CustomOption.AddButton("˅ General");
        
        public static CustomOptionHeader GeneralHeader =
            CustomOption.AddHeader("General");
        
        public static readonly CustomToggleOption Venting = CustomOption.AddToggle("venting", $"• {Palette.CrewmateBlue.GetTextColor()}Venting{StringColor.Reset}", true, true);

        public static readonly CustomToggleOption ReportBodys =
            CustomOption.AddToggle("reportbodys", $"• {Palette.CrewmateBlue.GetTextColor()}Body-Reporting{StringColor.Reset}", true, true);

        public static readonly CustomToggleOption Sabotaging =
            CustomOption.AddToggle("sabotaging", $"• {Palette.CrewmateBlue.GetTextColor()}Sabotaging{StringColor.Reset}", true, true);

        private static readonly CustomOption SectionSpecial = CustomOption.AddButton("˅ Special");
        
        public static CustomOptionHeader SpecialHeader =
            CustomOption.AddHeader($"{StringColor.Reset}Special");

        public static readonly CustomToggleOption CrewVenting =
            CustomOption.AddToggle("crewventing", $"• {Palette.ImpostorRed.GetTextColor()}Crew-Venting{StringColor.Reset}", true, false);

        public static readonly CustomToggleOption VentBuilding =
            CustomOption.AddToggle("ventbuilding", $"• {Palette.ImpostorRed.GetTextColor()}Vent-Building{StringColor.Reset}", true, false);

        public static readonly CustomNumberOption VentBuildingCooldown =
            CustomOption.AddNumber("ventbuildingcooldown", $"└ {Palette.ImpostorRed.GetTextColor()}Vent-Building-Cooldown{StringColor.Reset}", true, 7, 2, 30, 1);

        public static readonly CustomToggleOption BodyDragging =
            CustomOption.AddToggle("bodydragging", $"• {Palette.ImpostorRed.GetTextColor()}Body-Dragging{StringColor.Reset}", true, false);

        public static readonly CustomToggleOption Invisibility =
            CustomOption.AddToggle("invisibility", $"• {Palette.ImpostorRed.GetTextColor()}Invisibility{StringColor.Reset}", true, false);

        public static readonly CustomNumberOption InvisibilityCooldown =
            CustomOption.AddNumber("invisibilitycooldown", $"└ {Palette.ImpostorRed.GetTextColor()}Invisibility-Cooldown{StringColor.Reset}", true, 20, 2, 60, 2);

        public static readonly CustomNumberOption InvisibilityDuration =
            CustomOption.AddNumber("invisibilityduration", $"└ {Palette.ImpostorRed.GetTextColor()}Invisibility-Duration{StringColor.Reset}", true, 10, 2, 30, 1);

        public static readonly CustomToggleOption FreezeTime =
            CustomOption.AddToggle("freezetime", $"• {Palette.ImpostorRed.GetTextColor()}Time-Freezing{StringColor.Reset}", true, false);

        public static readonly CustomNumberOption FreezeTimeCooldown =
            CustomOption.AddNumber("freezetimecooldown", $"└ {Palette.ImpostorRed.GetTextColor()}Time-Freezing-Cooldown{StringColor.Reset}", true, 20, 2, 60, 2);

        public static readonly CustomNumberOption FreezeTimeDuration =
            CustomOption.AddNumber("freezetimeduration", $"└ {Palette.ImpostorRed.GetTextColor()}Time-Freezing-Duration{StringColor.Reset}", true, 10, 2, 30, 1);

        public static readonly CustomToggleOption Morphing =
            CustomOption.AddToggle("morphing", $"• {Palette.ImpostorRed.GetTextColor()}Morphing{StringColor.Reset}", true, false);

        public static readonly CustomNumberOption MorphingCooldown =
            CustomOption.AddNumber("morphingcooldown", $"└ {Palette.ImpostorRed.GetTextColor()}Morphing-Cooldown{StringColor.Reset}", true, 20, 2, 60, 2);

        private static readonly CustomOptionButton SectionRoles = CustomOption.AddButton("˅ Roles");
        
        public static CustomOptionHeader RolesHeader =
            CustomOption.AddHeader($"{StringColor.Reset}Roles");

        public static readonly CustomNumberOption JesterAmount =
            CustomOption.AddNumber("jesters", $"• {ModdedPalette.JesterColor.GetTextColor()}Jesters{StringColor.Reset}", true, 0, 0, 14, 1);
        
        public static readonly CustomNumberOption TrollAmount =
            CustomOption.AddNumber("trolls", $"• {ModdedPalette.TrollColor.GetTextColor()}Trolls{StringColor.Reset}", true, 0, 0, 14, 1);
        
        public static readonly CustomNumberOption CaptainAmount =
            CustomOption.AddNumber("captains", $"• {ModdedPalette.CaptainColor.GetTextColor()}Captains{StringColor.Reset}", true, 0, 0, 14, 1);
        
        public static readonly CustomNumberOption DemonAmount =
            CustomOption.AddNumber("demons", $"• {ModdedPalette.DemonColor.GetTextColor()}Demons{StringColor.Reset}", true, 0, 0, 14, 1);
        
        public static readonly CustomNumberOption DemonCooldown =
            CustomOption.AddNumber("demoncooldown", "└ Demon-Ability-Cooldown", true, 10, 2, 60, 2);
        
        public static readonly CustomNumberOption DemonDuration =
            CustomOption.AddNumber("demonduration", "└ Demon-Ability-Duration", true, 10, 2, 60, 2);

        public static readonly CustomNumberOption DoctorAmount =
            CustomOption.AddNumber("doctors", $"• {ModdedPalette.DoctorColor.GetTextColor()}Doctors{StringColor.Reset}", true, 0, 0, 14, 1);

        public static readonly CustomNumberOption DoctorCooldown =
            CustomOption.AddNumber("doctorcooldown", "└ Revive-Cooldown", true, 10, 2, 60, 2);

        public static readonly CustomNumberOption InspectorAmount =
            CustomOption.AddNumber("inspectors", $"• {ModdedPalette.InspectorColor.GetTextColor()}Inspectors{StringColor.Reset}", true, 0, 0, 14, 1);

        public static readonly CustomNumberOption MayorAmount =
            CustomOption.AddNumber("mayors", $"• {ModdedPalette.MayorColor.GetTextColor()}Mayors{StringColor.Reset}", true, 0, 0, 14, 1);

        public static readonly CustomNumberOption SheriffAmount =
            CustomOption.AddNumber("sheriffs", "• Sheriffs", true, 0, 0, 14, 1);

        public static readonly CustomNumberOption SheriffCooldown =
            CustomOption.AddNumber("sheriffcooldown", "└ Shoot-Cooldown", true, 10, 2, 60, 2);

        public static readonly CustomStringOption GameModeOption = CustomOption.AddString("gamemode", "GameMode", "None", "Hide and Seek" ,"BattleRoyale");

        public static readonly CustomNumberOption SeekerCooldown =
            CustomOption.AddNumber("seekercooldown", "Seeker-Cooldown", true, 10, 2, 60, 2);

        public static void Load()
        {
            /*
             * I disabled the credits in game. I provide credit here and on my repository. If there is any problem with the author of this library feel free to contact me via email: peasplayer@peasplayer.tk
             * Essentials: https://github.com/DorCoMaNdO/Reactor-Essentials
             * Author: DorComando (https://github.com/DorCoMaNdO)
             */
            CustomOption.ShamelessPlug = false;
            CustomOption.Debug = false;
            
            SectionGeneralListener(false);
            SectionSpecialListener(false);
            SectionRolesListener(false);
            
            GeneralHeader.MenuVisible = false;
            SpecialHeader.MenuVisible = false;
            RolesHeader.MenuVisible = false;
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
            if (GameModeOption.GetValue() == (int) mode)
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
                
                GameModeOption.OnValueChanged += (sender, args) =>
                {
                    PeasApi.EnableRoles = (int)args.Value == (int)GameMode.Roles;
                };
            }
        }
        
        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameJoined))]
        class AmongUsClientOnGameJoinedPatch
        {
            static void Postfix(AmongUsClient __instance)
            {
                PeasApi.EnableRoles = GameModeOption.GetValue() == (int)GameMode.Roles;
            }
        }
    }
}