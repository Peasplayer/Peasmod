using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnhollowerBaseLib;
using Reactor.Extensions;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using TMPro;
using Reactor.Unstrip;

namespace Peasmod.Utility
{
    public class StringColor
    {
        public const string Reset = "<color=#ffffffff>";
        public const string White = "<color=#ffffffff>";
        public const string Black = "<color=#000000ff>";
        public const string Red = "<color=#ff0000ff>";
        public const string Green = "<color=#169116ff>";
        public const string Blue = "<color=#0400ffff>";
        public const string Yellow = "<color=#f5e90cff>";
        public const string Purple = "<color=#a600ffff>";
        public const string Cyan = "<color=#00fff2ff>";
        public const string Pink = "<color=#e34dd4ff>";
        public const string Orange = "<color=#ff8c00ff>";
        public const string Brown = "<color=#8c5108ff>";
        public const string Lime = "<color=#1eff00ff>";
    }

    public static class Utils
    {

        public static void Log(String message)
        {
            PeasmodPlugin.Logger.Log(BepInEx.Logging.LogLevel.Info, message);
        }

        public static PlayerControl GetPlayer(this WinningPlayerData data)
        {
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (new WinningPlayerData(player.Data) == data)
                {
                    return player;
                }
            }
            return null;
        }

        public static string GetRoomName()
        {

            if (HudManager.Instance.roomTracker.transform.localPosition.y > -3f)
                return HudManager.Instance.roomTracker.text.text;
            return "Outside/Hallway";
        }

        public static DeadBody GetDeadBody(int id)
        {
            foreach (Collider2D collider2D in Physics2D.OverlapCircleAll(PlayerControl.LocalPlayer.GetTruePosition(), PlayerControl.LocalPlayer.MaxReportDistance*1000000f, Constants.PlayersOnlyMask))
            {
                if (collider2D.tag == "DeadBody")
                {
                    DeadBody body = (DeadBody)((Component)collider2D).GetComponent<DeadBody>();
                    if(id == body.ParentId)
                        return body;
                }
            }
            return null;
        }

        public static Sprite CreateSprite(string image)
        {
            var imagePath = "Peasmod.Resources." + image;
            Texture2D tex = GUIExtensions.CreateEmptyTexture();
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream myStream = assembly.GetManifestResourceStream(imagePath);
            byte[] buttonTexture = Reactor.Extensions.Extensions.ReadFully(myStream);
            ImageConversion.LoadImage(tex, buttonTexture, false);
            var sprite = GUIExtensions.CreateSprite(tex);
            return sprite;
        }

        public static TextMeshPro CreateText(Position pos, string text)
        {
            GameObject go = new GameObject();
            go.transform.SetParent(HudManager.Instance.gameObject.transform);
            switch(pos) {
                case Position.BottomRight:
                    go.transform.localPosition = new Vector3(-5.25f, -2.5f);
                    break;
            }
            TextMeshPro _text = UnityEngine.Object.Instantiate(HudManager.Instance.TaskText, go.transform);
            _text.text = text;
            return _text;
        }

        public static Color ColorIdToColor(int id)
        {
            Color color = new Color();
            switch(id)
            {
                case 0:
                    color = Palette.ImpostorRed;
                    break;
                case 1:
                    color = Palette.Blue;
                    break;
                case 2:
                    color = new Color(17 / 255f, 128 / 255f, 45 / 255f);
                    break;
                case 3:
                    color = new Color(238 / 255f, 84 / 255f, 187 / 255f);
                    break;
                case 4:
                    color = Palette.Orange;
                    break;
                case 5:
                    color = new Color(246 / 255f, 246 / 255f, 87 / 255f);
                    break;
                case 6:
                    color = Palette.DisabledGrey;
                    break;
                case 7:
                    color = Palette.White;
                    break;
                case 8:
                    color = Palette.Purple;
                    break;
                case 9:
                    color = Palette.Brown;
                    break;
                case 10:
                    color = Palette.CrewmateBlue;
                    break;
                case 11:
                    color = new Color(80 / 255f, 240 / 255f, 57 / 255f);
                    break;
            }
            return color;
        }

        public static PlayerControl GetClosestPlayer(PlayerControl refplayer)
        {
            double mindist = double.MaxValue;
            PlayerControl closestplayer = null;
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (player.Data.IsDead) continue;
                if (player != refplayer)
                {

                    double dist = GetDistBetweenPlayers(player, refplayer);
                    if (dist < mindist)
                    {
                        mindist = dist;
                        closestplayer = player;
                    }

                }

            }
            return closestplayer;
        }

        public static double GetDistBetweenPlayers(PlayerControl player, PlayerControl refplayer)
        {
            var refpos = refplayer.GetTruePosition();
            var playerpos = player.GetTruePosition();

            return Math.Sqrt((refpos[0] - playerpos[0]) * (refpos[0] - playerpos[0]) + (refpos[1] - playerpos[1]) * (refpos[1] - playerpos[1]));
        }

    }

    public enum Position
    {
        BottomRight = 1,
        BottomLeft = 2,
        Center = 3
    }

}
