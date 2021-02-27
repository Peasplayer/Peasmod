using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using UnityEngine;
using Peasmod.GameModes;
using Peasmod.Utility;

namespace Peasmod.Patches
{
    [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
    class KeybindPatch
    {
        public static void Prefix(KeyboardJoystick __instance)
        {
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            #region VentBuilding
            if(Peasmod.Settings.ventbuilding.GetValue())
            {
                if (VentBuilding.button != null && PlayerControl.LocalPlayer.Data.IsImpostor && Input.GetKey(KeyCode.R))
                    if (!VentBuilding.button.killButtonManager.isCoolingDown && VentBuilding.button.CanUse() && VentBuilding.button.enabled && VentBuilding.button.killButtonManager.isActive)
                    {
                        VentBuilding.OnClicked();
                        VentBuilding.button.killButtonManager.isCoolingDown = true;
                        VentBuilding.button.Timer = VentBuilding.button.MaxTimer;
                    }
            }
            #endregion VentBuilding
            #region BodyDragging
            if (Peasmod.Settings.bodydragging.GetValue())
            {
                if (BodyDragging.button != null && PlayerControl.LocalPlayer.Data.IsImpostor && Input.GetKey(KeyCode.T))
                    if (!BodyDragging.button.killButtonManager.isCoolingDown && BodyDragging.button.CanUse() && BodyDragging.button.enabled && BodyDragging.button.killButtonManager.isActive)
                    {
                        BodyDragging.OnClicked();
                        BodyDragging.button.killButtonManager.isCoolingDown = true;
                        BodyDragging.button.Timer = BodyDragging.button.MaxTimer;
                    }
            }
            #endregion BodyDragging
            #region InvisibilityMode
            if (Peasmod.Settings.invisibility.GetValue())
            {
                if (InvisibilityMode.button != null && PlayerControl.LocalPlayer.Data.IsImpostor && Input.GetKey(KeyCode.G))
                    if (InvisibilityMode.button.Timer <= 0f && !InvisibilityMode.button.isEffectActive && InvisibilityMode.button.CanUse() && InvisibilityMode.button.enabled && InvisibilityMode.button.killButtonManager.gameObject.active)
                    {
                        InvisibilityMode.button.killButtonManager.renderer.color = new Color(1f, 1f, 1f, 0.3f);
                        InvisibilityMode.button.OnClick();
                        InvisibilityMode.button.Timer = InvisibilityMode.button.MaxTimer;
                        if (InvisibilityMode.button.hasEffectDuration)
                        {
                            InvisibilityMode.button.isEffectActive = true;
                            InvisibilityMode.button.Timer = InvisibilityMode.button.EffectDuration;
                            InvisibilityMode.button.killButtonManager.TimerText.Color = new Color(0, 255, 0);
                        }
                    }
            }
            #endregion InvisibilityMode
            #region DoctorMode
            if (DoctorMode.Doctors.Count != 0)
            {
                if (DoctorMode.button != null && Input.GetKey(KeyCode.F))
                    if (DoctorMode.button.Timer <= 0f && DoctorMode.button.CanUse() && DoctorMode.button.enabled && DoctorMode.button.killButtonManager.gameObject.active)
                    {
                        DoctorMode.button.killButtonManager.renderer.color = new Color(1f, 1f, 1f, 0.3f);
                        DoctorMode.button.OnClick();
                        DoctorMode.button.Timer = DoctorMode.button.MaxTimer;
                    }
            }
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            if (Input.GetMouseButtonDown(0))
            {
                #region MorphingMode
                if(MorphingMode.menuOpenedAt != -1 && MorphingMode.menuOpenedAt+1f < Time.time)
                {
                    RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
                    if (hit.collider != null)
                        if (hit.collider.gameObject.name.Contains("|"))
                        {
                            string[] data = hit.collider.gameObject.name.Split("|");
                            if (data[0] == "PlayerLabel")
                                MorphingMode.OnLabelClick(PlayerControl.LocalPlayer, Utils.GetPlayer(byte.Parse(data[1])), true);
                        }
                }
                #endregion MorphingMode
            }
            #endregion DoctorMode
        }
    }
}
