using System;
using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class ItemStackCommand : Command
{
	public ItemStackCommand() : base("stack", "[Desired quantity] The selected item in your cursor will be of this quantity. This cannot exceed the max stack of an item.") { }
	protected internal override void Execute(List<string> args)
	{
		// The player must be holding an item to change it.
		if (Main.mouseItem.IsAir) {
			Output("You aren't holding an item to change.", true);
			return;
		}

		if (args.Count == 0) {
			Output("You must specify an item stack count.", true);
			return;
		}

		// Here we make changes
		if (!int.TryParse(args[0], out int quantity)) {
			Output($"Invalid quantity [{args[0]}]. It must be a number of an... acceptable size.", true);
			return;
		}

		// Clamp to reality
		quantity = Math.Max(1, Math.Min(Main.mouseItem.maxStack, quantity));

		// Dewit
		Main.mouseItem.stack = quantity;

		// Holler
		Output("Stack count set of cursor item set to " + Main.mouseItem.stack);
	}
}
