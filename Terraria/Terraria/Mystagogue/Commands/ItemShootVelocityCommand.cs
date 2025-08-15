using System;
using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class ItemShootVelocityCommand : Command
{
	public ItemShootVelocityCommand() : base("velocity", "[Desired projectile velocity] The selected item in your hotbar will fire its projectile with this velocity, if applicable. Run without a specification to undo this change.") { }
	protected internal override void Execute(List<string> args)
	{
		// The player must be holding an item to change it.
		if (Main.player[Main.myPlayer].HeldItem.IsAir) {
			Output("You aren't holding an item to change.", true);
			return;
		}

		if (args.Count > 0) {
			// Here we make changes
			if (!int.TryParse(args[0], out int velocity)) {
				Output($"Invalid velocity [{args[0]}]. It must be a number of an... acceptable size.", true);
				return;
			}

			// Clamp to a reasonable range so we don't needlessly stress limits
			velocity = Math.Max(0, Math.Min(60, velocity));

			// Dewit
			Main.player[Main.myPlayer].HeldItem.shootSpeed = velocity;

			// Holler
			Output("Velocity chance set to " + Main.player[Main.myPlayer].HeldItem.shootSpeed);
		}
		else {
			// Make a new item that can be the control case
			Item item = new Item();
			item.SetDefaults(Main.player[Main.myPlayer].HeldItem.type);
			item.Prefix(Main.player[Main.myPlayer].HeldItem.prefix);
			item.Refresh();

			// Set the value to what is expected of the control case
			Main.player[Main.myPlayer].HeldItem.shootSpeed = item.shootSpeed;

			item.TurnToAir(true);

			// Holler
			Output("Velocity chance set to default: " + Main.player[Main.myPlayer].HeldItem.shootSpeed);
		}
	}
}
