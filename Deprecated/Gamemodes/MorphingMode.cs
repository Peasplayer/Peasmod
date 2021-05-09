using System;
using System.Collections.Generic;
using System.Text;
using UnhollowerBaseLib;
using Hazel;
using Reactor;
using Reactor.Extensions;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Peasmod.Utility;

namespace Peasmod.Gamemodes
{
    class MorphingMode
    {
        public static CooldownButton button;
        
        private static GameObject labelprefab;
        public static List<GameObject> labels = new List<GameObject>();
        public static bool menuOpen = false;
        public static float menuOpenedAt = -1f;

        public static void OnLabelClick(PlayerControl who, PlayerControl into, bool fromlabel)
        {
            if(!who.IsMorphed())
                PlayerData.GetPlayerData(who);
            var data = PlayerData.GetPlayerData(into);
            who.RpcSetName(data.Name);
            who.RpcSetColor((byte)data.Color);
            who.RpcSetSkin(data.Skin);
            who.RpcSetHat(data.Hat);
            who.RpcSetPet(data.Pet);
            if(who.PlayerId == into.PlayerId)
                who.SetMorphed(false);
            else
                who.SetMorphed(true);
            if (fromlabel)
            {
                foreach (var button in labels)
                    button.gameObject.Destroy();
                labels.Clear();
                button.Visibile = true;
                menuOpen = false;
            }
        }
        public static void OnClick()
        {
            if (menuOpen) return;
            menuOpen = true;
            menuOpenedAt = Time.time;
            button.Visibile = false;
            if (PlayerData.GetPlayerData(PlayerControl.LocalPlayer) == null)
                new PlayerData(PlayerControl.LocalPlayer);
            if (labelprefab == null)
            {
                var _object = new GameObject();
                var _objectRenderer = _object.AddComponent<SpriteRenderer>();
                _objectRenderer.sprite = Utils.CreateSprite("Peasmod.Resources.Label.png");
                labelprefab = _object;
                var collider = labelprefab.AddComponent<BoxCollider2D>();
                collider.isTrigger = true;
                collider.size = labelprefab.GetComponent<SpriteRenderer>().size;
                labelprefab.transform.position += new Vector3(10000f, 10000f);
            }
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                var label = UnityEngine.Object.Instantiate(labelprefab, HudManager.Instance.transform);
                label.name = "PlayerLabel|" + player.PlayerId;
                var collider = label.GetComponent<BoxCollider2D>();
                collider.transform.localPosition = new Vector3(collider.transform.localPosition.x, collider.transform.localPosition.y, PlayerControl.LocalPlayer.transform.localPosition.z - 1f);
                var pos = button.killButtonManager.transform.localPosition + new Vector3(labels.Count / 4 * 1f - 0.2f, (-0.3f * (labels.Count - (labels.Count / 4 * 4))) + 0.45f, 1f);
                label.transform.localPosition = pos;
                label.transform.localScale = new Vector2(label.transform.localScale.x - 0.2f, label.transform.localScale.y - 0.2f);
                TextMeshPro text = UnityEngine.Object.Instantiate(HudManager.Instance.TaskText, label.transform);
                var data = PlayerData.GetPlayerData(player);
                if(data == null)
                    data = new PlayerData(player);
                text.text = data.Name;
                text.color = Color.black;
                text.transform.localPosition += new Vector3(-0.6f, 0.2f);
                text.transform.localScale -= new Vector3(0.2f, 0.2f);
                label.GetComponent<SpriteRenderer>().material.color = Utils.ColorIdToColor(data.Color);
                labels.Add(label);
            }
        }
    }

    class PlayerData
    {
        private static Dictionary<byte, PlayerData> PlayerDatas = new Dictionary<byte, PlayerData>(); 

        public string Name;
        public int Color;
        public uint Hat, Skin, Pet;

        public PlayerData(PlayerControl player)
        {
            Name = player.nameText.text;
            Color = player.Data.ColorId;
            Hat = player.Data.HatId;
            Skin = player.Data.SkinId;
            Pet = player.Data.PetId;
            PlayerDatas.Add(player.PlayerId, this);
        }

        public static PlayerData GetPlayerData(PlayerControl player)
        {
            if (PlayerDatas.ContainsKey(player.PlayerId))
                return PlayerDatas[player.PlayerId];
            else
                return null;
        }
    }
}