using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class PlayerAutoEraseItemsCommand : Command
{
	public PlayerAutoEraseItemsCommand() : base("clr", "All the items in your inventory that are not favorited will be erased.") { }
	protected internal override void Execute(List<string> args)
	{
		// Erase all non-favorited items in the player's inventory
		foreach (Item item in Main.player[Main.myPlayer].inventory) {
			if (!item.favorited) {
				item.TurnToAir(true);
			}
		}

		// Let the player know
		Output("All the items in your inventory that are not favorited have been erased.");
	}
}
