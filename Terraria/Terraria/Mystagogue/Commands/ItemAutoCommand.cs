using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class ItemAutoCommand : Command
{
	public ItemAutoCommand() : base("auto", "The selected item in your hotbar will be used again when the input is held to activate it.") { }
	protected internal override void Execute(List<string> args)
	{
		// The player must be holding an item to change it.
		if (Main.player[Main.myPlayer].HeldItem.IsAir) {
			Output("You aren't holding an item to change.", true);
			return;
		}

		// Dewit
		Main.player[Main.myPlayer].HeldItem.autoReuse = !Main.player[Main.myPlayer].HeldItem.autoReuse;

		// Holler
		Output("Automatic reuse turned " + (Main.player[Main.myPlayer].HeldItem.autoReuse ? "on" : "off") + ".");
	}
}
