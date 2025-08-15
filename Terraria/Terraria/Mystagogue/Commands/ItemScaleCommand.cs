using System;
using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class ItemScaleCommand : Command
{
	public ItemScaleCommand() : base("scale", "[Desired size in percentage] The selected item in your hotbar will be this percent of its ordinary size when visibly used. Run without a specification to undo this change.") { }
	protected internal override void Execute(List<string> args)
	{
		// The player must be holding an item to change it.
		if (Main.player[Main.myPlayer].HeldItem.IsAir) {
			Output("You aren't holding an item to change.", true);
			return;
		}

		if (args.Count > 0) {
			// Here we make changes
			if (!int.TryParse(args[0], out int scalePercentage)) {
				Output($"Invalid scale [{args[0]}]. It must be a number of an... acceptable size.", true);
				return;
			}

			// Clamp to a barely reasonable range so we don't break the game with something noneuclidean
			scalePercentage = Math.Max(1, Math.Min(10000, scalePercentage));

			// Dewit
			Main.player[Main.myPlayer].HeldItem.scale = scalePercentage / 100;

			// Holler
			Output("Scale set to " + Main.player[Main.myPlayer].HeldItem.scale * 100 + "%");
		}
		else {
			// Make a new item that can be the control case
			Item item = new Item();
			item.SetDefaults(Main.player[Main.myPlayer].HeldItem.type);
			item.Prefix(Main.player[Main.myPlayer].HeldItem.prefix);
			item.Refresh();

			// Set the value to what is expected of the control case
			Main.player[Main.myPlayer].HeldItem.scale = item.scale;

			item.TurnToAir(true);

			// Holler
			Output("Scale set to default: " + Main.player[Main.myPlayer].HeldItem.scale * 100 + "%");
		}
	}
}
