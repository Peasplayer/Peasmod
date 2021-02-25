using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using UnityEngine;
using Reactor.Extensions;
using System.Reflection;
using System.IO;
using Reactor.Unstrip;

namespace Peasmod
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
    public static class ShipstatusOnEnablePatch
    {
        public static bool gameStarted = false;
        static void Prefix(ShipStatus __instance)
        {
            #region VentBuilding
            if (Peasmod.Settings.ventbuilding.GetValue())
            {
                var vents = GameObject.FindObjectsOfType<Vent>();
                bool first = true;
                foreach (var vent in vents)
                {
                    if (first)
                    {
                        first = false;
                        vent.transform.position = new Vector2(50, 50);
                        VentBuilding.VentSize = Vector2.Scale(vent.GetComponent<BoxCollider2D>().size, vent.transform.localScale) * 0.75f;
                        continue;
                    }
                    GameObject.Destroy(vent.gameObject);
                }
                __instance.AllVents = new Vent[0];
            }
            #endregion VentBuilding
            #region BodyDragging
            BodyDragging.draggers.Clear();
            BodyDragging.bodys.Clear();
            #endregion BodyDragging
            #region EngineerMode
            if(Peasmod.Settings.engineeramount.GetValue() != 0)
            {
                var cams = GameObject.FindObjectsOfType<SurvCamera>();
                bool first = true;
                foreach (var cam in cams)
                {
                    if (first)
                    {
                        first = false;
                        //MorphingMode.camPref = cam;
                        cam.transform.position = new Vector2(50, 50);
                        continue;
                    }
                    GameObject.Destroy(cam.gameObject);
                }
                __instance.AllCameras = new SurvCamera[0];
            }
            #endregion EngineerMode
        }

        static void Postfix(ShipStatus __instance)
        {
            Peasmod.impostorbuttons.Clear();
            InvisibilityMode.invisplayers.Clear();
            if (Peasmod.Settings.ventbuilding.GetValue())
            {
                VentBuilding.button = new CooldownButton(VentBuilding.OnClicked, Peasmod.Settings.ventbuildingcooldown.GetValue(), "Peasmod.Resources.BuildVent.png", 200f, Vector2.zero, CooldownButton.Category.OnlyImpostor, HudManager.Instance);
                Peasmod.impostorbuttons.Add(VentBuilding.button);
            }  
            if (Peasmod.Settings.bodydragging.GetValue())
            {
                BodyDragging.button = new CooldownButton(BodyDragging.OnClicked, 0.5f, "Peasmod.Resources.DragBody.png", 200f, Vector2.zero, CooldownButton.Category.OnlyImpostor, HudManager.Instance);
                BodyDragging.button.PositionOffset = new Vector2(0f, 1.5f*Peasmod.impostorbuttons.Count);
                Peasmod.impostorbuttons.Add(BodyDragging.button);
            }
            if (Peasmod.Settings.invisibility.GetValue())
            {
                InvisibilityMode.button = new CooldownButton(InvisibilityMode.OnClicked, Peasmod.Settings.invisibilitycooldown.GetValue(), "Peasmod.Resources.Hide.png", 200f, Vector2.zero, CooldownButton.Category.OnlyImpostor, HudManager.Instance, Peasmod.Settings.invisibilityduration.GetValue(), InvisibilityMode.OnEnded);
                InvisibilityMode.button.PositionOffset = new Vector2(0f, 1.5f * Peasmod.impostorbuttons.Count);
                Peasmod.impostorbuttons.Add(InvisibilityMode.button);
            }
            if (Peasmod.Settings.freezetime.GetValue())
            {
                TimeFreezing.button = new CooldownButton(TimeFreezing.OnClick, Peasmod.Settings.freezetimecooldown.GetValue(), "Peasmod.Resources.TimeFreezing.png", 200f, Vector2.zero, CooldownButton.Category.OnlyImpostor, HudManager.Instance, Peasmod.Settings.freezetimeduration.GetValue(), TimeFreezing.OnEnded);
                TimeFreezing.button.PositionOffset = new Vector2(0f+(Peasmod.impostorbuttons.Count/3*1.5f), 1.5f * (Peasmod.impostorbuttons.Count-(Peasmod.impostorbuttons.Count/3*3)));
                Peasmod.impostorbuttons.Add(TimeFreezing.button);
            }
            if (Peasmod.Settings.doctoramount.GetValue() >= 1)
                DoctorMode.button = new CooldownButton(DoctorMode.OnClicked, Peasmod.Settings.doctorcooldown.GetValue(), "Peasmod.Resources.Revive.png", 200f, Vector2.zero, CooldownButton.Category.Doctor, HudManager.Instance);
            if (Peasmod.Settings.sheriffamount.GetValue() >= 1)
                SheriffMode.button = new CooldownButton(SheriffMode.OnClicked, Peasmod.Settings.sheriffcooldown.GetValue(), "Peasmod.Resources.Revive.png", 200f, Vector2.zero, CooldownButton.Category.Sheriff, HudManager.Instance);
            if (Peasmod.Settings.morphing.GetValue())
            {
                MorphingMode.button = new CooldownButton(MorphingMode.OnClick, Peasmod.Settings.morphingcooldown.GetValue(), "Peasmod.Resources.DragBody.png", 200f, Vector2.zero, CooldownButton.Category.OnlyImpostor, HudManager.Instance);
                MorphingMode.button.PositionOffset = new Vector2(0f + (Peasmod.impostorbuttons.Count / 3 * 1.5f), 1.5f * (Peasmod.impostorbuttons.Count - (Peasmod.impostorbuttons.Count / 3 * 3)));
                Peasmod.impostorbuttons.Add(MorphingMode.button);
            }
            gameStarted = true;
            HudManagerPatch.dots.Clear();
            SheriffMode.CurrentTarget = null;
            SheriffMode.button.killButtonManager.CurrentTarget = null;
        }
    }

    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Start))]
    public static class GameManagerStartPatch
    {
        static void Prefix(GameStartManager __instance)
        {
            
        }

        static void Postfix(GameStartManager __instance)
        {
            
        }
    }
}
