using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Reflection;
using System.IO;
using Peasmod.Utility;
using HarmonyLib;
using Il2CppSystem;
using Il2CppSystem.Threading;
using Reactor.Extensions;

namespace Peasmod.Gamemodes
{
    class TestingStuff
    {
        public static bool testing = false;

        public static CooldownButton button;
        //public static FollowerCamera cam;
        //public static Camera camera;

        public static void OnClick()
        {
            Texture2D tex = GUIExtensions.CreateEmptyTexture();
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream myStream = assembly.GetManifestResourceStream("Peasmod.Resources.Revive.png");
            byte[] buttonTexture = Reactor.Extensions.Extensions.ReadFully(myStream);
            ImageConversion.LoadImage(tex, buttonTexture, false);
            var test = new Essentials.UI.CooldownButton(GUIExtensions.CreateSprite(tex), Essentials.UI.HudPosition.TopRight, 5);
            /*cam = Object.Instantiate<FollowerCamera>(HudManager.Instance.PlayerCam, HudManager.Instance.transform);//, HudManager.Instance.transform
            cam.enabled = true;
            cam.gameObject.SetActive(true);
            Utils.Log(cam.transform.localScale.x + " | " + cam.transform.localScale.y + " | " + cam.transform.localScale.z);
            cam.transform.localScale = new Vector3(0.2f, 0.2f);
            //cam.transform.position -= new Vector3(1f, 1f);
            Utils.Log(cam.transform.position.x + " | " + cam.transform.position.y);
            //cam.Awake();
            //camera.fieldOfView = 10f;
            var playercam = HudManager.Instance.PlayerCam.GetComponent<Camera>();
            camera = Object.Instantiate<Camera>(playercam);
            playercam.clearFlags = CameraClearFlags.SolidColor;
            camera.transform.SetParent(HudManager.Instance.transform);
            camera.transform.position = cam.transform.position;
            cam.transform.localScale = new Vector3(0.1f, 0.1f);
            camera.transform.localScale = new Vector3(0.1f, 0.1f);
            //cam.CamAspect = 0.2f;
            camera.fieldOfView = 0.2f;
            //Utils.Log(cam.CamAspect + "");
            RenderTexture temporary = RenderTexture.GetTemporary((int)(256.0 * 1), 256, 16, (RenderTextureFormat)0);
            camera.targetTexture = temporary;*/
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        class Update
        {
            public static void Prefix(HudManager __instance)
            {
                //if(cam != null)
                {



                    /*cam.transform.position = new Vector3(1f, 1f);
                    //var camera = cam.GetComponent<Camera>();
                    //camera.fieldOfView = 10f;
                    camera.transform.SetParent(HudManager.Instance.transform);
                    //camera.transform.position = cam.transform.position;
                    camera.transform.localScale = new Vector3(1f, 1f);
                    //cam.CamAspect = 0.2f;
                    RenderTexture temporary = RenderTexture.GetTemporary((int)(256.0 * 1), 256, 16, (RenderTextureFormat)0);

                    camera.targetTexture = temporary;*/
                }
            }
        }
        
        /*[HarmonyPatch(typeof(CreateGameOptions), nameof(CreateGameOptions.CoShow))]
        class SOMERANDOM
        {
            public static void Postfix(CreateGameOptions __instance)
            {
                Utils.Log("test");
                //var mapbutton = GameObject.Instantiate(GameObject.Find("2"));
                if (__instance.Content.active)
                    Utils.Log("null");
                else
                    Utils.Log("not null");
                void test(GameObject obj)
                {
                    
                }
            }
        }*/

        //public static SurvCamera camPref;
        //test.transform.localPosition = new Vector3(-2.5f, -2.5f);
        /*var playerControl = PlayerControl.Instantiate(AmongUsClient.Instance.PlayerPrefab);
        var i = playerControl.PlayerId = 13;
        GameData.Instance.AddPlayer(playerControl);
        AmongUsClient.Instance.Spawn(playerControl, -2, SpawnFlags.None);
        playerControl.transform.position = PlayerControl.LocalPlayer.transform.position;
        playerControl.GetComponent<DummyBehaviour>().enabled = true;
        playerControl.NetTransform.enabled = false;
        playerControl.SetName("Bob");
        playerControl.SetColor(6);
        GameData.Instance.RpcSetTasks(playerControl.PlayerId, new byte[0]);
        /*var test = new PlayerControl();
        test.SetColor(6);
        GUILayout.win
        test.nameText.Text = "Bob";
        test.transform.position = PlayerControl.LocalPlayer.transform.position;
        if (test.myRend == null) Utils.Log("null");
        /*PlayerControl.LocalPlayer.myRend.material.SetColor("_OutlineColor", Palette.ImpostorRed);
        PlayerControl.LocalPlayer.myRend.material.SetFloat("_Outline", 1f);
        /*collider = PlayerControl.LocalPlayer.Collider;
        PlayerControl.LocalPlayer.Collider.enabled = false;
        HudManager.Instance.ShadowQuad.gameObject.SetActive(false);
        Utils.Log(ShipStatus.Instance.MapPrefab.transform.rotation.w + " | " + ShipStatus.Instance.MapPrefab.transform.rotation.x + " | " + ShipStatus.Instance.MapPrefab.transform.rotation.y + " | " + ShipStatus.Instance.MapPrefab.transform.rotation.z);
        /*var cam = UnityEngine.Object.Instantiate(camPref, camPref.transform.parent);
        cam.transform.position = PlayerControl.LocalPlayer.GetTruePosition();
        cam.CamName = Utils.GetRoomName();
        var allCams = ShipStatus.Instance.AllCameras.ToList();
        allCams.Add(cam);
        ShipStatus.Instance.AllCameras = allCams.ToArray();*/
        /*if (SliceButtonFunction.sword != null && SliceButtonFunction.sword.active)
            {
                var vectorToTarget = Camera.main.ScreenToWorldPoint(Input.mousePosition) - SliceButtonFunction.localPlayerObject.transform.position;
                float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * (180 / (float)Math.PI);
                Quaternion q = Quaternion.Euler(0, 0, angle);
                SliceButtonFunction.sword.transform.localRotation = q;
                SliceButtonFunction.sword.GetComponent<SpriteRenderer>().flipY = angle < -90 || angle > 90;
                SliceButtonFunction.sword.transform.position = SliceButtonFunction.localPlayerObject.transform.position;

            }*/
    }
}
