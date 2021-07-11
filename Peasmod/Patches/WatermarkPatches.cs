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
                if(GameObject.Find("AmyText_TMP") != null)
                    GameObject.Find("AmyText_TMP").GetComponent<TextMeshPro>().text = "Peasplayer\nGravity";
                if(GameObject.Find("ForestText_TMP") != null)
                    GameObject.Find("ForestText_TMP").GetComponent<TextMeshPro>().text = "Innersloth\nPeasplayer";
                if(GameObject.Find("CMText") != null)
                    GameObject.Find("CMText").GetComponent<TextMeshPro>().text = "Game-Creators";
                if (Object.FindObjectOfType<AutoScroll>() != null)
                {
                    Object.FindObjectOfType<AutoScroll>().transform.localPosition = new Vector3(0, 0, 0);
                    Object.FindObjectOfType<AutoScroll>().Destroy();
                }
                if (Object.FindObjectOfType<HideObjectIfMinor>() != null)
                {
                    Object.FindObjectOfType<HideObjectIfMinor>().Destroy();
                    GameObject.Find("FollowUs").SetActive(true);
                }

                if (GameObject.Find("TwitterIcon") != null)
                {
                    GameObject.Find("TwitterIcon").SetActive(false);
                    //GameObject.Find("TwitterIcon").GetComponent<SpriteRenderer>().sprite =
                    //    Utils.CreateSprite("GitHub.png");
                    //GameObject.Find("TwitterIcon").GetComponent<SpriteRenderer>().size /= 45;
                    //GameObject.Find("TwitterIcon").GetComponent<TwitterLink>().LinkUrl =
                    //    "https://github.com/Peasplayer";
                }
                if(GameObject.Find("FacebookIcon") != null)
                    GameObject.Find("FacebookIcon").SetActive(false);
                if(GameObject.Find("Discord-Logo-Color") != null)
                    GameObject.Find("Discord-Logo-Color").GetComponent<TwitterLink>().LinkUrl =
                    "https://discord.gg/nQB5EZe";
                if(GameObject.Find("logoImage") != null)
                    GameObject.Find("logoImage").GetComponent<SpriteRenderer>().sprite = Utils.CreateSprite("Peasmod.png");
            }
        }
    }
}