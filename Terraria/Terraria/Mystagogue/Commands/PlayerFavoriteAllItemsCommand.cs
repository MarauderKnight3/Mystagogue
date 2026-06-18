using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class PlayerFavoriteAllItemsCommand : Command
{
	public PlayerFavoriteAllItemsCommand() : base("fvt", "Favorites all the items in the immediate inventory.") { }
	protected internal override void Execute(List<string> args)
	{
		// Favorite all items in the player's inventory
		foreach (Item item in Main.LocalPlayer.inventory) {
			item.favorited = true;
		}
		
		// Let the player know
		Output("All your inventory items have been favorited.");
	}
}
