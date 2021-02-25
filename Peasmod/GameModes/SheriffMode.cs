using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Hazel;
using Reactor.Extensions;

namespace Peasmod
{
    class SheriffMode
    {
        public static PlayerControl Sheriff1;
        public static PlayerControl Sheriff2;

        public static PlayerControl CurrentTarget;

        public static CooldownButton button;

        public static Color SheriffColor { get; } = new Color(255f / 255f, 114f / 255f, 0f / 255f);

        public static void OnClicked()
        {
            if (CurrentTarget != null)
            {
                PlayerControl.LocalPlayer.RpcMurderPlayer(CurrentTarget);
                Utils.Log("13: " + CurrentTarget.nameText.Text + " | " + CurrentTarget.Data.IsImpostor);
                if (!CurrentTarget.Data.IsImpostor)
                {
                    Utils.Log("14: "+CurrentTarget.nameText.Text+" | "+ CurrentTarget.Data.IsImpostor);
                    DeadBody deadBody = UnityEngine.Object.Instantiate<DeadBody>(PlayerControl.LocalPlayer.KillAnimations.Random<KillAnimation>().bodyPrefab);
                    deadBody.ParentId = PlayerControl.LocalPlayer.PlayerId;
                    deadBody.transform.position = PlayerControl.LocalPlayer.transform.position;
                    PlayerControl.LocalPlayer.SetPlayerMaterialColors((Renderer)((Component)deadBody).GetComponent<Renderer>());
                    PlayerControl.LocalPlayer.RpcMurderPlayer(PlayerControl.LocalPlayer);
                    PlayerControl.LocalPlayer.transform.position = deadBody.transform.position;
                    PlayerControl.LocalPlayer.Collider.enabled = false;
                    HudManager.Instance.ShadowQuad.gameObject.SetActive(false);
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.SheriffDies, Hazel.SendOption.None, -1);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
        }
    }
}
