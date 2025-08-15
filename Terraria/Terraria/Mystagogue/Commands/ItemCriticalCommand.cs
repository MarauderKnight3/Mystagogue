using System;
using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class ItemCriticalCommand : Command
{
	public ItemCriticalCommand() : base("crit", "[Desired critical chance] The selected item in your hotbar will have this critical chance value to work with. Run without a specification to undo this change.") { }
	protected internal override void Execute(List<string> args)
	{
		// The player must be holding an item to change it.
		if (Main.player[Main.myPlayer].HeldItem.IsAir) {
			Output("You aren't holding an item to change.", true);
			return;
		}

		if (args.Count > 0) {
			// Here we make changes
			if (!int.TryParse(args[0], out int crit)) {
				Output($"Invalid critical chance [{args[0]}]. It must be a number of an... acceptable size.", true);
				return;
			}

			// Clamp to a reasonable range so we don't break the game with desynchronization
			crit = Math.Max(0, Math.Min(999999, crit));

			// Dewit
			Main.player[Main.myPlayer].HeldItem.crit = crit;

			// Holler
			Output("Critical chance set to " + Main.player[Main.myPlayer].HeldItem.crit);
		}
		else {
			// Make a new item that can be the control case
			Item item = new Item();
			item.SetDefaults(Main.player[Main.myPlayer].HeldItem.type);
			item.Prefix(Main.player[Main.myPlayer].HeldItem.prefix);
			item.Refresh();

			// Set the value to what is expected of the control case
			Main.player[Main.myPlayer].HeldItem.crit = item.crit;

			item.TurnToAir(true);

			// Holler
			Output("Critical chance set to default: " + Main.player[Main.myPlayer].HeldItem.crit);
		}
	}
}
