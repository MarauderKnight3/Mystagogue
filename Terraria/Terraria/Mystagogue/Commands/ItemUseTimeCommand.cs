using System;
using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class ItemUseTimeCommand : Command
{
	public ItemUseTimeCommand() : base("ut", "[Desired use duration in ticks] The selected item in your hotbar will take this long to execute its logic. This is called \"use time\". Run without a specification to undo this change. For changes to how long an item takes to swing or hold in hands when used, see the [at] command.") { }
	protected internal override void Execute(List<string> args)
	{
		// The player must be holding an item to change it.
		if (Main.player[Main.myPlayer].HeldItem.IsAir) {
			Output("You aren't holding an item to change.", true);
			return;
		}

		if (args.Count > 0) {
			// Here we make changes
			if (!int.TryParse(args[0], out int useTime)) {
				Output($"Invalid use time [{args[0]}]. It must be a number of an... acceptable size.", true);
				return;
			}

			// Clamp to a reasonable range so we don't break the game swinging an item for an hour
			useTime = Math.Max(0, Math.Min(80, useTime));

			// Dewit
			Main.player[Main.myPlayer].HeldItem.useTime = useTime;

			// Holler
			Output("Use time set to " + Main.player[Main.myPlayer].HeldItem.useTime);
		}
		else {
			// Make a new item that can be the control case
			Item item = new Item();
			item.SetDefaults(Main.player[Main.myPlayer].HeldItem.type);
			item.Prefix(Main.player[Main.myPlayer].HeldItem.prefix);
			item.Refresh();

			// Set the value to what is expected of the control case
			Main.player[Main.myPlayer].HeldItem.useTime = item.useTime;

			item.TurnToAir(true);

			// Holler
			Output("Use time set to default: " + Main.player[Main.myPlayer].HeldItem.useTime);
		}
	}
}
