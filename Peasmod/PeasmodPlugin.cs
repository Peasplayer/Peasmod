using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using HarmonyLib;
using Reactor;
using BepInEx.Logging;
using PeasAPI;
using PeasAPI.Managers;
using UnityEngine;

namespace Peasmod
{
    [BepInPlugin(Id, "Peasmod", PluginVersion)]
    [BepInProcess("Among Us.exe")]
    [BepInDependency(ReactorPlugin.Id)]
    [BepInDependency(PeasAPI.PeasAPI.Id)]
    public class PeasmodPlugin : BasePlugin
    {
        public const string Id = "tk.peasplayer.peasmod";

        public const string PluginName = "Peasmod";
        public const string PluginAuthor = "Peasplayer#2541";
        public const string PluginVersion = "3.0.0-pre1.1";

        public Harmony Harmony { get; } = new Harmony(Id);
        
        public static ManualLogSource Logger { get; private set; }
        
        public static ConfigFile ConfigFile { get; private set; }

        public override void Load()
        {
            Logger = Log;
            ConfigFile = Config;

            WatermarkManager.AddWatermark($" | {PluginName} v{PluginVersion} {PeasAPI.Utility.StringColor.Green} by {PluginAuthor}", $" | {PluginName} v{PluginVersion}\n{PeasAPI.Utility.StringColor.Green} by {PluginAuthor}", 
                new Vector3(0f, -0.3f),  new Vector3(-0.9f, 0f));
            PeasAPI.PeasAPI.AccountTabOffset = new Vector3(0f, -0.3f);
            PeasAPI.PeasAPI.AccountTabOnlyChangesName = false;
            
            CustomServerManager.RegisterServer("Peaspowered", "au.peasplayer.tk", 22023);
            CustomServerManager.RegisterServer("matux.fr", "152.228.160.91", 22023);
            
            UpdateManager.RegisterGitHubUpdateListener("Peasplayer", "Peasmod");
            
            CustomHatManager.RegisterNewVisor("DreamMask", "Peasmod.Resources.Hats.DreamMask.png", new Vector2(0f, 0.2f));
            CustomHatManager.RegisterNewVisor("PeasMask", "Peasmod.Resources.Hats.PeasMask.png", new Vector2(0f, 0.2f));
            CustomHatManager.RegisterNewHat("Sitting Tux", "Peasmod.Resources.Hats.Tux.png", new Vector2(0f, 0.2f), true, false, PeasAPI.Utility.CreateSprite("Peasmod.Resources.Hats.Tuxb.png"));
            CustomHatManager.RegisterNewHat("Laying Tux", "Peasmod.Resources.Hats.Tux2.png", new Vector2(0f, 0.2f), true, true, PeasAPI.Utility.CreateSprite("Peasmod.Resources.Hats.Tux2b.png"));
            CustomHatManager.RegisterNewHat("KristalCrown", "Peasmod.Resources.Hats.KristalCrown.png");
            CustomHatManager.RegisterNewHat("Elf Hat", "Peasmod.Resources.Hats.Elf.png", new Vector2(0f, 0.2f), true, false, PeasAPI.Utility.CreateSprite("Peasmod.Resources.Hats.Elfb.png"));
            CustomHatManager.RegisterNewHat("Santa", "Peasmod.Resources.Hats.Santa.png", new Vector2(0f, 0.3f), true, false, PeasAPI.Utility.CreateSprite("Peasmod.Resources.Hats.Santab.png"));
            CustomHatManager.RegisterNewHat("Christmas Tree", "Peasmod.Resources.Hats.XmasTree.png", new Vector2(0f, 0.2f), true, false, PeasAPI.Utility.CreateSprite("Peasmod.Resources.Hats.XmasTreeb.png"));
            CustomHatManager.RegisterNewHat("Christmas Sock", "Peasmod.Resources.Hats.Sock.png", new Vector2(0f, 0.2f), true, false, PeasAPI.Utility.CreateSprite("Peasmod.Resources.Hats.Sockb.png"));
            
            CustomColorManager.RegisterCustomColor(Palette.AcceptedGreen, "The Peas");
            
            Settings.Load();
            
            Harmony.PatchAll();
        }
    }
}