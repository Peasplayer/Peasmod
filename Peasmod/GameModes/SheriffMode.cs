using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Hazel;
using Reactor.Extensions;
using Peasmod.Utility;

namespace Peasmod.GameModes
{
    class SheriffMode
    {
        public static List<PlayerControl> Sheriffs = new List<PlayerControl>();

        public static PlayerControl CurrentTarget;

        public static CooldownButton button;

        public static Color SheriffColor { get; } = new Color(255f / 255f, 114f / 255f, 0f / 255f);

        public static void OnClicked()
        {
            if (CurrentTarget != null)
            {
                PlayerControl.LocalPlayer.RpcMurderPlayer(CurrentTarget);
                if (!CurrentTarget.Data.IsImpostor)
                {
                    DeadBody deadBody = UnityEngine.Object.Instantiate<DeadBody>(PlayerControl.LocalPlayer.KillAnimations.Random<KillAnimation>().bodyPrefab);
                    deadBody.ParentId = PlayerControl.LocalPlayer.PlayerId;
                    deadBody.transform.position = PlayerControl.LocalPlayer.transform.position;
                    PlayerControl.LocalPlayer.SetPlayerMaterialColors((Renderer)((Component)deadBody).GetComponent<Renderer>());
                    PlayerControl.LocalPlayer.Die(DeathReason.Kill);
                    PlayerControl.LocalPlayer.transform.position = deadBody.transform.position;
                    PlayerControl.LocalPlayer.Collider.enabled = false;
                    HudManager.Instance.ShadowQuad.gameObject.SetActive(false);
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.SheriffDies, Hazel.SendOption.None, -1);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    SpriteRenderer renderer = CurrentTarget.myRend;
                    renderer.material.SetFloat("_Outline", 0f);
                    SheriffMode.button.killButtonManager.isActive = false;
                }
            }
        }
    }
}
