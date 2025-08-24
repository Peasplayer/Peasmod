using HarmonyLib;
using PeasAPI;
using UnityEngine;

namespace Peasmod.Patches
{
    [HarmonyPatch]
    public static class WatermarkPatches
    {
        [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
        [HarmonyPostfix]
        public static void PeasmodLogoPatch()
        {
            var peasmodLogoSprite = Utility.CreateSprite("Peasmod.Resources.Peasmod.png");

            var peasmodLogo = new GameObject("bannerLogo_Peasmod");
            peasmodLogo.transform.SetParent(GameObject.Find("RightPanel").transform, false);
            peasmodLogo.transform.localPosition = new Vector3(-0.4f, 1f, 5f);
            peasmodLogo.AddComponent<SpriteRenderer>().sprite = peasmodLogoSprite;
        }
    }
}
