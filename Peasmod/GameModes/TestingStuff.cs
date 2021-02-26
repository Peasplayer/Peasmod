using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Peasmod
{
    class TestingStuff
    {
        public static bool testing = false;

        public static CooldownButton button;

        public static void OnClick()
        {
            var item = Utils.CreateSprite("Peasmod.Resources.Unbenannt.png");
            var box = item.AddComponent<BoxCollider2D>();
            box.isTrigger = true;
            item.transform.localPosition = PlayerControl.LocalPlayer.transform.localPosition;
        }

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
