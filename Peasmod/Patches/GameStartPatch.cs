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
            if ((Peasmod.Settings.gamemode.GetValue() == (int)Peasmod.Settings.GameMode.None && Peasmod.Settings.ventbuilding.GetValue()) || Peasmod.Settings.gamemode.GetValue() == (int)Peasmod.Settings.GameMode.HotPotato)
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
            /*if(Peasmod.Settings.engineeramount.GetValue() != 0)
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
            Stream myStream = assembly.GetManifestResourceStream("Peasmod.Resources.Revive.png");
            byte[] buttonTexture = Reactor.Extensions.Extensions.ReadFully(myStream);
            ImageConversion.LoadImage(tex, buttonTexture, false);
            GUIExtensions.CreateSprite(tex);
            if(Peasmod.Settings.gamemode.GetValue() == (int)Peasmod.Settings.GameMode.HotPotato)
            {
                HotPotatoMode.button = new CooldownButton(HotPotatoMode.OnClick, 1f, "Peasmod.Resources.Kill.png", 200f, new Vector2(0f, 0.3f), CooldownButton.Category.OnlyImpostor, HudManager.Instance);
                HotPotatoMode.timer = Utils.CreateText(new Vector3(-5.25f, -2.5f), "Timer");
                HotPotatoMode.TimeTillDeath = Peasmod.Settings.hotpotatotimer.GetValue();
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
            else if(Peasmod.Settings.gamemode.GetValue() == (int)Peasmod.Settings.GameMode.BattleRoyale)
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
                foreach (var player in PlayerControl.AllPlayerControls)
                    player.Data.IsImpostor = true;
                //HudManager.Instance.KillButton.gameObject.SetActive(true);
                //BattleRoyaleMode.button = new CooldownButton(BattleRoyaleMode.OnClick, 5f, "Peasmod.Resources.Kill.png", 200f, Vector2.zero, CooldownButton.Category.Everyone, HudManager.Instance);
            }
            else if (false)//Peasmod.Settings.thanos.GetValue())
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
            }
            else
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
                    BodyDragging.button.PositionOffset = new Vector2(0f, 1.2f * Peasmod.impostorbuttons.Count);
                    Peasmod.impostorbuttons.Add(BodyDragging.button);
                }
                if (Peasmod.Settings.invisibility.GetValue())
                {
                    InvisibilityMode.button = new CooldownButton(InvisibilityMode.OnClicked, Peasmod.Settings.invisibilitycooldown.GetValue(), "Peasmod.Resources.Hide.png", 200f, Vector2.zero, CooldownButton.Category.OnlyImpostor, HudManager.Instance, Peasmod.Settings.invisibilityduration.GetValue(), InvisibilityMode.OnEnded);
                    InvisibilityMode.button.PositionOffset = new Vector2(0f, 1.2f * Peasmod.impostorbuttons.Count);
                    Peasmod.impostorbuttons.Add(InvisibilityMode.button);
                }
                if (Peasmod.Settings.freezetime.GetValue())
                {
                    TimeFreezing.button = new CooldownButton(TimeFreezing.OnClick, Peasmod.Settings.freezetimecooldown.GetValue(), "Peasmod.Resources.TimeFreezing.png", 200f, Vector2.zero, CooldownButton.Category.OnlyImpostor, HudManager.Instance, Peasmod.Settings.freezetimeduration.GetValue(), TimeFreezing.OnEnded);
                    TimeFreezing.button.PositionOffset = new Vector2(0f + (Peasmod.impostorbuttons.Count / 3 * 1.3f), 1.2f * (Peasmod.impostorbuttons.Count - (Peasmod.impostorbuttons.Count / 3 * 3)));
                    Peasmod.impostorbuttons.Add(TimeFreezing.button);
                }
                if (Peasmod.Settings.morphing.GetValue())
                {
                    MorphingMode.button = new CooldownButton(MorphingMode.OnClick, Peasmod.Settings.morphingcooldown.GetValue(), "Peasmod.Resources.Morphing.png", 200f, Vector2.zero, CooldownButton.Category.OnlyImpostor, HudManager.Instance);
                    MorphingMode.button.PositionOffset = new Vector2(0f + (Peasmod.impostorbuttons.Count / 3 * 1.3f), 1.2f * (Peasmod.impostorbuttons.Count - (Peasmod.impostorbuttons.Count / 3 * 3)));
                    Peasmod.impostorbuttons.Add(MorphingMode.button);
                }
            }
            #region JesterMode
            JesterMode.HandleTasks();
            #endregion JesterMode
            #region DoctorMode
            if (Peasmod.Settings.doctoramount.GetValue() >= 1)
                DoctorMode.button = new CooldownButton(DoctorMode.OnClicked, Peasmod.Settings.doctorcooldown.GetValue(), "Peasmod.Resources.Revive.png", 200f, Vector2.zero, CooldownButton.Category.OnlyDoctor, HudManager.Instance);
            #endregion DoctorMode
            #region SheriffMode
            if (Peasmod.Settings.sheriffamount.GetValue() >= 1)
            {
                SheriffMode.CurrentTarget = null;
                if (SheriffMode.button != null)
                    SheriffMode.button.killButtonManager.CurrentTarget = null;
                SheriffMode.button = new CooldownButton(SheriffMode.OnClicked, Peasmod.Settings.sheriffcooldown.GetValue(), "Peasmod.Resources.Kill.png", 200f, Vector2.zero, CooldownButton.Category.OnlySheriff, HudManager.Instance);
            }
            #endregion SheriffMode
            if (TestingStuff.testing)
                TestingStuff.button = new CooldownButton(TestingStuff.OnClick, 1f, "Peasmod.Resources.DragBody.png", 200f, Vector2.zero, CooldownButton.Category.OnlyCrewmate, HudManager.Instance);
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
