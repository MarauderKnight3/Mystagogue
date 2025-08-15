using System;
using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class ItemAnimationTimeCommand : Command
{
	public ItemAnimationTimeCommand() : base("at", "[Desired animation duration in ticks] The selected item in your hotbar will take this long to complete its relevant animation. This is called \"animation time\". Run without a specification to undo this change. For changes to how long an item takes to execute its logic, see the [ut] command.") { }
	protected internal override void Execute(List<string> args)
	{
		// The player must be holding an item to change it.
		if (Main.player[Main.myPlayer].HeldItem.IsAir) {
			Output("You aren't holding an item to change.", true);
			return;
		}

		if (args.Count > 0) {
			// Here we make changes
			if (!int.TryParse(args[0], out int animationTime)) {
				Output($"Invalid animation time [{args[0]}]. It must be a number of an... acceptable size.", true);
				return;
			}

			// Clamp to a reasonable range so we don't break the game swinging an item for an hour
			animationTime = Math.Max(0, Math.Min(80, animationTime));

			// Dewit
			Main.player[Main.myPlayer].HeldItem.useAnimation = animationTime;

			// Holler
			Output("Animation time set to " + Main.player[Main.myPlayer].HeldItem.useAnimation);
		}
		else {
			// Make a new item that can be the control case
			Item item = new Item();
			item.SetDefaults(Main.player[Main.myPlayer].HeldItem.type);
			item.Prefix(Main.player[Main.myPlayer].HeldItem.prefix);
			item.Refresh();

			// Set the value to what is expected of the control case
			Main.player[Main.myPlayer].HeldItem.useAnimation = item.useAnimation;

			item.TurnToAir(true);

			// Holler
			Output("Animation time set to default: " + Main.player[Main.myPlayer].HeldItem.useAnimation);
		}
	}
}
