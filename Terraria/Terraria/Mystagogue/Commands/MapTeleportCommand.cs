using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace Terraria.Mystagogue.Commands;
internal class MapTeleportCommand : Command
{
	internal static bool MapTeleportationEnabled;

	public MapTeleportCommand() : base("mtp", "Enables teleporting by Ctrl+Alt+Right-clicking on the world map.") { }
	protected internal override void Execute(List<string> args)
	{
		MapTeleportationEnabled = !MapTeleportationEnabled;
		Output("Map teleportation toggled " + (MapTeleportationEnabled ? "on" : "off") + ".");
	}

	protected internal override void ResetVariables() => MapTeleportationEnabled = false;

	internal static void AttemptMapTeleport()
	{
		if (!MapTeleportCommand.MapTeleportationEnabled)
			return;

		bool leftAlt = Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt);
		bool leftControl = Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl);
		bool mouseRight = Main.mouseRight && Main.mouseRightRelease;

		if (mouseRight && leftControl && leftAlt) {
			float mapX = Main.mapFullscreenPos.X + (Main.mouseX - Main.screenWidth / 2) / Main.mapFullscreenScale;
			float mapY = Main.mapFullscreenPos.Y + (Main.mouseY - Main.screenHeight / 2) / Main.mapFullscreenScale;

			float worldX = mapX * 16f - (Main.LocalPlayer.width / 2);
			float worldY = mapY * 16f - (Main.LocalPlayer.height / 2);

			float clampedX = Math.Min(Math.Max(worldX, 0f), Main.maxTilesX * 16f - 64f);
			float clampedY = Math.Min(Math.Max(worldY, 0f), Main.maxTilesY * 16f - 64f);

			Main.LocalPlayer.Teleport(new Vector2(clampedX, clampedY), TeleportationStyleID.DebugTeleport);
			Main.mapFullscreen = false;
		}
	}
}