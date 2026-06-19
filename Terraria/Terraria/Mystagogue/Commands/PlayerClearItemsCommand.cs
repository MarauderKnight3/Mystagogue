using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class PlayerClearItemsCommand : Command
{
	public PlayerClearItemsCommand() : base("clr", "Voids all items in the inventory that are not favorited.") { }
	protected internal override void Execute(List<string> args)
	{
		// Erase all non-favorited items in the player's inventory
		foreach (Item item in Main.LocalPlayer.inventory) {
			if (!item.favorited) {
				item.TurnToAir(true);
			}
		}

		// Let the player know
		Output("Inventory voiding complete.");
	}
}
