using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class PlayerFavoriteAllItemsCommand : Command
{
	public PlayerFavoriteAllItemsCommand() : base("fvt", "All the items in your inventory will be favorited.") { }
	protected internal override void Execute(List<string> args)
	{
		// Favorite all items in the player's inventory
		for (int i = 0; i < Main.player[Main.myPlayer].inventory.Length; i++) {
			Main.player[Main.myPlayer].inventory[i].favorited = true;
		}

		// Let the player know
		Output("All your inventory items have been favorited.");
	}
}
