using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using UnhollowerBaseLib;
using UnityEngine;
using BepInEx;
using BepInEx.IL2CPP;
using Reactor.Extensions;
using Reactor.Unstrip;

namespace Peasmod.Utility
{
    class CustomHatManager
    {
        public static string[] hats = { "PeasMask" };

        public static HatBehaviour CreateHat(string hat)
        {
            HatBehaviour newHat = new HatBehaviour();
            Texture2D tex = GUIExtensions.CreateEmptyTexture(2, 2);
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream myStream = assembly.GetManifestResourceStream("Peasmod.Resources.Hats."+ hat + ".png");
            byte[] buttonTexture = Reactor.Extensions.Extensions.ReadFully(myStream);
            ImageConversion.LoadImage(tex, buttonTexture, false);
            newHat.MainImage = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.8f),
                225f
            );
            newHat.ProductId = $"+{hat}";
            newHat.InFront = true;
            newHat.NoBounce = true;
            return newHat;
        }
    }

    [HarmonyPatch(typeof(HatManager), nameof(HatManager.GetHatById))]
    public static class HatManagerPatch
    {
        private static bool modded = false;
        public static void Prefix(HatManager __instance)
        {
            if(!modded)
            {
                modded = true;
                foreach (string hat in CustomHatManager.hats)
                {
                    __instance.AllHats.Add(CustomHatManager.CreateHat(hat));
                }
                __instance.AllHats.Sort((Il2CppSystem.Comparison<HatBehaviour>)((h1, h2) => h2.ProductId.CompareTo(h1.ProductId)));
            }
        }
    }
}
