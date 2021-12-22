using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.IL2CPP;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.Managers;
using PeasAPI.Roles;
using Reactor.Networking;
using Reactor.Networking.MethodRpc;
using UnityEngine;

namespace Peasmod.Roles
{
	[RegisterCustomRole]
	public class Mentalist : BaseRole
	{
		public Mentalist(BasePlugin plugin) : base(plugin)
		{
			Instance = this;
		}
		public override string Name => "Mentalist";
		public override string Description => "Control the minds of others";
		public override string TaskText => "Control the minds of other players";
		public override Color Color => Palette.ImpostorRed;
		public override Visibility Visibility => Visibility.Impostor;
		public override Team Team => Team.Impostor;
		public override bool HasToDoTasks => true;
		public override int Limit => (int)Settings.MentalistAmount.Value;
		public override bool CanVent => true;
		public override bool CanKill(PlayerControl victim = null) => !victim || victim.Data.Role.IsImpostor;
		public override bool CanSabotage(SystemTypes? sabotage) => true;

		public static Mentalist Instance;

		public CustomButton Button;
		public Dictionary<byte, Vector3> OldPositions;
		public Dictionary<byte, byte> ControlledPlayers;

		public override void OnGameStart()
		{
			OldPositions = new Dictionary<byte, Vector3>();
			ControlledPlayers = new Dictionary<byte, byte>();
			Button = CustomButton.AddRoleButton(delegate()
			{
				PlayerMenuManager.OpenPlayerMenu(PlayerControl.AllPlayerControls.ToArray().Where(player => !player.IsLocal() && !player.Data.IsDead).ToList().ConvertAll(p => p.PlayerId), delegate(PlayerControl player)
				{
					RpcMindControl(PlayerControl.LocalPlayer, player, true);
				});
			}, Settings.ControlCooldown.Value, Utility.CreateSprite("Peasmod.Resources.Buttons.Button1.png"), Vector2.zero, false, this, Settings.ControlDuration.Value, delegate()
			{
				RpcMindControl(PlayerControl.LocalPlayer, ControlledPlayers[PlayerControl.LocalPlayer.PlayerId].GetPlayer(), false);
			}, "<size=40%>Control", new Vector2(0f, 0.5f));
		}

		public override void OnUpdate()
		{
			if (ControlledPlayers != null)
			{
				foreach (KeyValuePair<byte, byte> keyValuePair in ControlledPlayers)
				{
					keyValuePair.Value.GetPlayer().transform.position = keyValuePair.Key.GetPlayer().transform.position;
				}
			}
		}

		[MethodRpc((uint) CustomRpcCalls.MindControl, LocalHandling = RpcLocalHandling.Before)]
		public static void RpcMindControl(PlayerControl sender, PlayerControl target, bool enable)
		{
			if (AmongUsClient.Instance.AmHost)
				sender.RpcShapeshift(enable ? target : sender, false);
			if (enable)
			{
				Instance.OldPositions.Add(sender.PlayerId, sender.transform.position);
				Instance.ControlledPlayers.Add(sender.PlayerId, target.PlayerId);
				target.Visible = false;
				target.moveable = false;
				sender.transform.position = target.transform.position;
			}
			else
			{
				if (Instance.ControlledPlayers.ContainsKey(sender.PlayerId) && Instance.OldPositions.ContainsKey(sender.PlayerId) && Instance.ControlledPlayers.ContainsValue(target.PlayerId))
				{
					Instance.ControlledPlayers.Remove(sender.PlayerId);
					target.Visible = true;
					target.moveable = true;
					//sender.Visible = true;
					sender.transform.position = Instance.OldPositions[sender.PlayerId];
					Instance.OldPositions.Remove(sender.PlayerId);
				}
			}
		}
	}
}
