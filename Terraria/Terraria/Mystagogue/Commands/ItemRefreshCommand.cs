using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class ItemRefreshCommand : Command
{
	public ItemRefreshCommand() : base("ri", "The item held in the cursor, or in the hotbar if the cursor is empty, will be normalized. Effective at making items typical.") { }
	protected internal override void Execute(List<string> args)
	{
		// The player must be holding an item to change it.
		if (Main.mouseItem.IsAir && Main.player[Main.myPlayer].HeldItem.IsAir) {
			Output("You aren't holding an item to change.", true);
			return;
		}

		// Switch to the hotbar if the cursor is empty.
		var item = Main.mouseItem;
		if (Main.mouseItem.IsAir)
			item = Main.player[Main.myPlayer].HeldItem;

		// Refresh the item, whether it is in the cursor or in the hotbar.
		item.Refresh(false);

		// Let the user know
		Output($"Normalized the item [{item.Name}].");
	}
}
