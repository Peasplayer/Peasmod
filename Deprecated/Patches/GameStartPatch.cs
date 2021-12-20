using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using UnityEngine;
using Reactor.Extensions;
using System.Reflection;
using System.IO;
using Reactor.Unstrip;
using Peasmod.Gamemodes;
using Peasmod.Utility;

namespace Peasmod.Patches
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
    public static class ShipstatusOnEnablePatch
    {
        static void Prefix(ShipStatus __instance)
        {
            #region VentBuilding
            if ((Settings.IsGameMode(Settings.GameMode.None) && Settings.VentBuilding.GetValue()) || Settings.IsGameMode(Settings.GameMode.HotPotato))
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
            /*if(Settings.engineeramount.GetValue() != 0)
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
            }*/
            #endregion EngineerMode
        }

        static void Postfix(ShipStatus __instance)
        {
            Texture2D tex = GUIExtensions.CreateEmptyTexture();
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream myStream = assembly.GetManifestResourceStream("Peasmod.Resources.Buttons.Peasplayer.Revive.png");
            byte[] buttonTexture = Reactor.Extensions.Extensions.ReadFully(myStream);
            ImageConversion.LoadImage(tex, buttonTexture, false);
            GUIExtensions.CreateSprite(tex);
            
            var buttonTextures = Peasmod.Instance.Config.Bind("Settings", "ButtonTextures", true, "If set to true the textures that Peasplayer made will be used. Else the textures of Gravity will be used.");
            var resource = buttonTextures.Value ? "Peasmod.Resources.Buttons.Peasplayer" : "Peasmod.Resources.Buttons.Gravity";
            
            if(Settings.IsGameMode(Settings.GameMode.HotPotato))
            {
                HotPotatoMode.button = new CooldownButton(HotPotatoMode.OnClick, 1f, "Peasmod.Resources.Kill.png", 200f, new Vector2(0f, 0.3f), CooldownButton.Category.OnlyImpostor, HudManager.Instance);
                HotPotatoMode.timer = Utils.CreateText(new Vector3(-5.25f, -2.5f), "Timer");
                HotPotatoMode.TimeTillDeath = Settings.HotPotatoTimer.GetValue();
                HotPotatoMode.Timer = HotPotatoMode.TimeTillDeath;
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    var removeTask = new List<PlayerTask>();
                    foreach (PlayerTask task in player.myTasks)
                    {

                        if (task.TaskType != TaskTypes.FixComms && task.TaskType != TaskTypes.FixLights && task.TaskType != TaskTypes.ResetReactor && task.TaskType != TaskTypes.ResetSeismic && task.TaskType != TaskTypes.RestoreOxy)
                            removeTask.Add(task);
                    }
                    foreach (PlayerTask task in removeTask)
                        player.RemoveTask(task);
                }
            }
            else if(Settings.IsGameMode(Settings.GameMode.BattleRoyale))
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    var removeTask = new List<PlayerTask>();
                    foreach (PlayerTask task in player.myTasks)
                    {

                        if (task.TaskType != TaskTypes.FixComms && task.TaskType != TaskTypes.FixLights && task.TaskType != TaskTypes.ResetReactor && task.TaskType != TaskTypes.ResetSeismic && task.TaskType != TaskTypes.RestoreOxy)
                            removeTask.Add(task);
                    }
                    foreach (PlayerTask task in removeTask)
                        player.RemoveTask(task);
                }
                //foreach (var player in PlayerControl.AllPlayerControls)
                //   player.Data.IsImpostor = true;
                //HudManager.Instance.KillButton.gameObject.SetActive(true);
                //HudManager.Instance.UseButton.gameObject.SetActive(false);
                //BattleRoyaleMode.button = new CooldownButton(BattleRoyaleMode.OnClick, 5f, "Peasmod.Resources.Kill.png", 200f, Vector2.zero, CooldownButton.Category.Everyone, HudManager.Instance);
            }
            /*else if (false)//Settings.thanos.GetValue())
            {
                foreach (var thanos in ThanosMode.Thanos)
                {
                    var removeTask = new List<PlayerTask>();
                    foreach (PlayerTask task in thanos.myTasks)
                    {

                        if (task.TaskType != TaskTypes.FixComms && task.TaskType != TaskTypes.FixLights && task.TaskType != TaskTypes.ResetReactor && task.TaskType != TaskTypes.ResetSeismic && task.TaskType != TaskTypes.RestoreOxy)
                            removeTask.Add(task);
                    }
                    foreach (PlayerTask task in removeTask)
                        thanos.RemoveTask(task);
                }
            }*/
            else
            {
                Peasmod.impostorbuttons.Clear();
                InvisibilityMode.invisplayers.Clear();
                Utils.Log(resource);
                if (Settings.VentBuilding.GetValue())
                {
                    VentBuilding.button = new CooldownButton(VentBuilding.OnClicked, Settings.VentBuildingCooldown.GetValue(), resource + ".BuildVent.png", 200f, Vector2.zero, CooldownButton.Category.OnlyImpostor, HudManager.Instance);
                    Peasmod.impostorbuttons.Add(VentBuilding.button);
                }
                if (Settings.BodyDragging.GetValue())
                {
                    BodyDragging.button = new CooldownButton(BodyDragging.OnClicked, 0.5f, resource + ".DragBody.png", 200f, Vector2.zero, CooldownButton.Category.OnlyImpostor, HudManager.Instance);
                    BodyDragging.button.PositionOffset = new Vector2(0f, 1.2f * Peasmod.impostorbuttons.Count);
                    Peasmod.impostorbuttons.Add(BodyDragging.button);
                }
                if (Settings.Invisibility.GetValue())
                {
                    InvisibilityMode.button = new CooldownButton(InvisibilityMode.OnClicked, Settings.InvisibilityCooldown.GetValue(), resource + ".Hide.png", 200f, Vector2.zero, CooldownButton.Category.OnlyImpostor, HudManager.Instance, Settings.InvisibilityDuration.GetValue(), InvisibilityMode.OnEnded);
                    InvisibilityMode.button.PositionOffset = new Vector2(0f, 1.2f * Peasmod.impostorbuttons.Count);
                    Peasmod.impostorbuttons.Add(InvisibilityMode.button);
                }
                if (Settings.FreezeTime.GetValue())
                {
                    TimeFreezing.button = new CooldownButton(TimeFreezing.OnClick, Settings.FreezeTimeCooldown.GetValue(), resource + ".TimeFreezing.png", 200f, Vector2.zero, CooldownButton.Category.OnlyImpostor, HudManager.Instance, Settings.FreezeTimeDuration.GetValue(), TimeFreezing.OnEnded);
                    TimeFreezing.button.PositionOffset = new Vector2(0f + (Peasmod.impostorbuttons.Count / 3 * 1.3f), 1.2f * (Peasmod.impostorbuttons.Count - (Peasmod.impostorbuttons.Count / 3 * 3)));
                    Peasmod.impostorbuttons.Add(TimeFreezing.button);
                }
                if (Settings.Morphing.GetValue())
                {
                    MorphingMode.button = new CooldownButton(MorphingMode.OnClick, Settings.MorphingCooldown.GetValue(), resource + ".Morphing.png", 200f, Vector2.zero, CooldownButton.Category.OnlyImpostor, HudManager.Instance);
                    MorphingMode.button.PositionOffset = new Vector2(0f + (Peasmod.impostorbuttons.Count / 3 * 1.3f), 1.2f * (Peasmod.impostorbuttons.Count - (Peasmod.impostorbuttons.Count / 3 * 3)));
                    Peasmod.impostorbuttons.Add(MorphingMode.button);
                }
            }
            #region JesterMode
            JesterMode.HandleTasks();
            #endregion JesterMode
            #region DoctorMode
            if (Settings.DoctorAmount.GetValue() >= 1)
                DoctorMode.button = new CooldownButton(DoctorMode.OnClicked, Settings.DoctorCooldown.GetValue(), resource + ".Revive.png", 200f, Vector2.zero, CooldownButton.Category.OnlyDoctor, HudManager.Instance);
            #endregion DoctorMode
            #region SheriffMode
            if (Settings.SheriffAmount.GetValue() >= 1)
            {
                SheriffMode.CurrentTarget = null;
                if (SheriffMode.button != null)
                    SheriffMode.button.killButtonManager.CurrentTarget = null;
                SheriffMode.button = new CooldownButton(SheriffMode.OnClicked, Settings.SheriffCooldown.GetValue(), "Peasmod.Resources.Kill.png", 200f, Vector2.zero, CooldownButton.Category.OnlySheriff, HudManager.Instance);
            }
            #endregion SheriffMode
            if (TestingStuff.testing)
                TestingStuff.button = new CooldownButton(TestingStuff.OnClick, 1f, resource + ".DragBody.png", 200f, Vector2.zero, CooldownButton.Category.OnlyCrewmate, HudManager.Instance);
            foreach (var player in PlayerControl.AllPlayerControls)
                PlayerData.GetPlayerData(player);
            HudManagerPatch.dots.Clear();
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
