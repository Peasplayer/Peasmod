using System;
using Il2CppSystem.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Reactor.Extensions;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Peasmod.Patches
{
    [HarmonyPatch]
    public class FloatingBodySyncPatch
    {
        private static ObjectPoolBehavior _globalPool;
        private static StarGen _starGen;
        private static readonly string[] Scenes = {"MainMenu", "MMOnline", "MatchMaking"};

        public static void OnSceneChanged(Scene scene, LoadSceneMode _) {
            if (_starGen != null && _starGen.gameObject != null)
                _starGen.gameObject.SetActive(Scenes.Contains(scene.name));
            if (_globalPool != null && _globalPool.gameObject != null)
                _globalPool.gameObject.SetActive(Scenes.Contains(scene.name));
        }

        [HarmonyPatch(typeof(PlayerParticles), nameof(PlayerParticles.Start))]
        [HarmonyPrefix]
        public static void PlayerSyncStartPatch(PlayerParticles __instance) {
            if (_globalPool == null) {
                _globalPool = Object.Instantiate(__instance.pool).DontDestroy();
                Object.Destroy(_globalPool.GetComponent<PlayerParticles>());
                _globalPool.poolSize = 18; //Using the current amount of vanilla colors and not Palette.ColorNames.Length because the screen would be to filled with dead bodies;
                foreach (PoolableBehavior pb in _globalPool.inactiveChildren) Object.DestroyImmediate(pb);
                foreach (PoolableBehavior pb in _globalPool.activeChildren) Object.DestroyImmediate(pb);
                _globalPool.activeChildren = new List<PoolableBehavior>();
                _globalPool.inactiveChildren = new List<PoolableBehavior>();
                _globalPool.InitPool(_globalPool.Prefab);
                SceneManager.add_sceneLoaded(new Action<Scene, LoadSceneMode>(OnSceneChanged));
            }

            Object.Destroy(__instance.pool);
            __instance.pool = _globalPool;
        }

        [HarmonyPatch(typeof(PlayerParticle), nameof(PlayerParticle.Update))]
        [HarmonyPrefix]
        public static void PlayerSyncUpdatePatch(PlayerParticle __instance) {
            __instance.OwnerPool = _globalPool;
        }

        [HarmonyPatch(typeof(StarGen), nameof(StarGen.Start))]
        [HarmonyPrefix]
        public static bool StarSyncStartPatch(StarGen __instance) {
            if (_starGen) {
                if (!Scenes.ToArray().Contains(SceneManager.GetActiveScene().name)) return true;
                Object.Destroy(__instance);
                return false;
            }

            _starGen = __instance.DontDestroy();
            return true;
        }
    }
}