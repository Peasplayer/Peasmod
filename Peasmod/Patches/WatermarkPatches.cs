using System.IO;
using System.Reflection;
using HarmonyLib;
using Peasmod.Utility;
using Reactor.Extensions;
using TMPro;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace Peasmod.Patches
{
    public class WatermarkPatches
    {
        [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
        public static class PingTrackerPatch
        {
            public static void Postfix(PingTracker __instance)
            {
                __instance.text.text += "\n"+Peasmod.PluginName+" v"+Peasmod.PluginVersion+ "\n by " + StringColor.Green + Peasmod.PluginAuthor;
            }
        }

        [HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
        public static class VersionShowerPatch
        {
            static void Postfix(VersionShower __instance)
            {
                __instance.text.text += "\nReactor-Framework" + "\n" + Peasmod.PluginName + " v" + Peasmod.PluginVersion + " \nby " + StringColor.Green + Peasmod.PluginAuthor + " " + StringColor.Reset + Peasmod.PluginPage;
                __instance.transform.position -= new Vector3(0, 0.5f, 0);
                AccountManager.Instance.accountTab.gameObject.SetActive(false);
                foreach (var _object in GameObject.FindObjectsOfTypeAll(Il2CppType.Of<GameObject>()))
                    if (_object.name.Contains("ReactorVersion"))
                        GameObject.Destroy(_object);
                //if(UnityEngine.Object.FindObjectOfType<MainMenuManager>() != null && UnityEngine.Object.FindObjectOfType<MainMenuManager>().Announcement != null)
                //UnityEngine.Object.FindObjectOfType<MainMenuManager>().Announcement.gameObject.SetActive(true);
            }
        }
        
        [HarmonyPatch(typeof(MainMenuManager), "Start")]
        public static class MainMenuManagerStartPatch
        {
            public static void Postfix()
            {
                Texture2D tex = GUIExtensions.CreateEmptyTexture();
                Assembly assembly = Assembly.GetExecutingAssembly();
                Stream myStream = assembly.GetManifestResourceStream("Peasmod.Resources.Peasmod.png");
                byte[] buttonTexture = Reactor.Extensions.Extensions.ReadFully(myStream);
                ImageConversion.LoadImage(tex, buttonTexture, false);
                GameObject.Find("bannerLogo_AmongUs").GetComponent<SpriteRenderer>().sprite = GUIExtensions.CreateSprite(tex);
                GameObject.Find("AmongUsLogo").GetComponent<SpriteRenderer>().sprite = GUIExtensions.CreateSprite(tex);
                GameObject.Find("AmongUsLogo").transform.position += new Vector3(0.3f, 0, 0);
                ImageConversion.LoadImage(tex, buttonTexture, false);
                Texture2D tex2 = GUIExtensions.CreateEmptyTexture();
                Assembly assembly2 = Assembly.GetExecutingAssembly();
                Stream myStream2 = assembly2.GetManifestResourceStream("Peasmod.Resources.Buttons.Credits.png");
                byte[] buttonTexture2 = Reactor.Extensions.Extensions.ReadFully(myStream2);
                ImageConversion.LoadImage(tex2, buttonTexture2, false);
                if(GameObject.Find("CreditsButton") != null)
                    GameObject.Find("CreditsButton").GetComponent<SpriteRenderer>().sprite = GUIExtensions.CreateSprite(tex2);
            }
        }
        
        [HarmonyPatch(typeof(CreditsScreenPopUp), nameof(CreditsScreenPopUp.OnEnable))]
        public static class CreditScreenPatch
        {
            public static void Postfix(CreditsScreenPopUp __instance)
            {
                Utils.Log("1");
                if(GameObject.Find("AmyText_TMP") != null)
                    GameObject.Find("AmyText_TMP").GetComponent<TextMeshPro>().text = "Peasplayer\nGravity";
                Utils.Log("2");
                if(GameObject.Find("ForestText_TMP") != null)
                    GameObject.Find("ForestText_TMP").GetComponent<TextMeshPro>().text = "Peasplayer";
                Utils.Log("3");
                if(GameObject.Find("CMText") != null)
                    GameObject.Find("CMText").GetComponent<TextMeshPro>().text = "Game-Creators";
                Utils.Log("4");
                if(GameObject.Find("VictoriaText_TMP") != null)
                    GameObject.Find("VictoriaText_TMP").GetComponent<TextMeshPro>().text = "Innersloth";
                Utils.Log("5");
                if(GameObject.Find("PT-BR") != null)
                    GameObject.Find("PT-BR").SetActive(false);
                Utils.Log("6");
                if(GameObject.Find("TwitterIcon") != null)
                    GameObject.Find("TwitterIcon").SetActive(false);
                Utils.Log("7");
                if(GameObject.Find("FacebookIcon") != null)
                    GameObject.Find("FacebookIcon").SetActive(false);
                Utils.Log("8");
                if(GameObject.Find("Discord-Logo-Color") != null)
                    GameObject.Find("Discord-Logo-Color").GetComponent<TwitterLink>().LinkUrl =
                    "https://discord.gg/nQB5EZe";
                Utils.Log("9");
                Texture2D tex = GUIExtensions.CreateEmptyTexture();
                Assembly assembly = Assembly.GetExecutingAssembly();
                Stream myStream = assembly.GetManifestResourceStream("Peasmod.Resources.Peasmod.png");
                byte[] buttonTexture = Reactor.Extensions.Extensions.ReadFully(myStream);
                ImageConversion.LoadImage(tex, buttonTexture, false);
                if(GameObject.Find("logoImage") != null)
                    GameObject.Find("logoImage").GetComponent<SpriteRenderer>().sprite = GUIExtensions.CreateSprite(tex);
            }
        }
    }
}