using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.GameInput;
using Terraria.Graphics.Capture;
using Terraria.ID;

namespace Terraria.Mystagogue.Commands;
internal class RightClickTeleportCommand : Command
{
	static int RightClickTeleportSetting;

	public RightClickTeleportCommand() : base("tps", "[Setting] Enables right clicking to teleport (as long as no right click functions are in your hands). Run without a specification to toggle. 0 = Off, 1 = On, 2 = Spam when mouse down.") { }
	protected internal override void Execute(List<string> args)
	{
		int newSetting = RightClickTeleportSetting == 0 && args.Count == 0 ? 1 : 0;

		// Here we make changes
		if (args.Count > 0 && !int.TryParse(args[0], out newSetting)) {
			Output($"Invalid setting [{args[0]}]. It must be a number of an... acceptable size.", true);
			return;
		}

		// Clamp to a reasonable range so we don't travel back in time
		newSetting = Math.Max(0, Math.Min(2, newSetting));

		// Dewit
		RightClickTeleportSetting = newSetting;

		// Let the player know
		switch (RightClickTeleportSetting) {
			default:
				Output("Teleport on right click inactive.");
				break;
			case 1:
				Output("Teleport on right click active.");
				break;
			case 2:
				Output("Teleport while right click held active.");
				break;
		}
	}

	protected internal override void ResetVariables() => RightClickTeleportSetting = 0;

	protected internal static void TryRightClickTeleport()
	{
		var player = Main.player[Main.myPlayer];

		// Check if teleport is enabled and conditions are met
		if (player.tileInteractionHappened || player.mouseInterface ||
			CaptureManager.Instance.Active || Main.HoveringOverAnNPC || Main.SmartInteractShowingGenuine ||
			player.talkNPC != -1 || player.chest != -1 || player.HeldItem.summon || player.scope ||
			player.inventory[player.selectedItem].type == ItemID.DD2SquireDemonSword || player.inventory[player.selectedItem].type == ItemID.BouncingShield) {
			return;
		}

		// Check mouse input based on teleport setting
		bool isMouseRightTriggered = RightClickTeleportSetting switch {
			1 => PlayerInput.Triggers.JustPressed.MouseRight,
			2 => PlayerInput.Triggers.Current.MouseRight,
			_ => false
		};

		if (!isMouseRightTriggered) {
			return;
		}

		Vector2 pointPosition = default;
		pointPosition.X = Main.mouseX + Main.screenPosition.X - player.width / 2;
		if (player.gravDir == 1f)
			pointPosition.Y = Main.mouseY + Main.screenPosition.Y - player.height;
		else
			pointPosition.Y = Main.screenPosition.Y + Main.screenHeight - Main.mouseY;

		player.Teleport(pointPosition, TeleportationStyleID.DebugTeleport);
		NetMessage.SendData(13, number: Main.myPlayer);
	}
}
