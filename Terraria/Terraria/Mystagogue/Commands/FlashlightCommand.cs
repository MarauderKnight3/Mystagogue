using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Terraria.Mystagogue.Commands;
internal class FlashlightCommand : Command
{
	internal static bool Flashlight;

	public FlashlightCommand() : base("flashlight", "Enables or disables a tile spotlight.") { }
	protected internal override void Execute(List<string> args)
	{
		Flashlight = !Flashlight;
		Output("Flashlight toggled " + (Flashlight ? "on" : "off") + ".");
	}

	protected internal override void ResetVariables() => Flashlight = false;

	protected internal override void PostResetEffectsHook()
	{
		if (Flashlight) {
			Vector2 vector = default;
			vector.X = Main.mouseX + Main.screenPosition.X;
			if (Main.player[Main.myPlayer].gravDir == 1f) {
				vector.Y = Main.mouseY + Main.screenPosition.Y;
			}
			else {
				vector.Y = Main.screenPosition.Y + Main.screenHeight - Main.mouseY;
			}
			for (int i = -6; i < 7; i++) {
				for (int j = -6; j < 7; j++) {
					Lighting.AddLight(new Vector2(vector.X + 16 * i, vector.Y + 16 * j), 1f, 1f, 1f);
				}
			}
		}
	}
}
