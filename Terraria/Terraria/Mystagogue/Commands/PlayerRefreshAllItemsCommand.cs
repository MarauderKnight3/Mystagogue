using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class PlayerRefreshAllItemsCommand : Command
{
	public PlayerRefreshAllItemsCommand() : base("ris", "Refreshes ALL of the items in ALL of the player's inventories.") { }
	protected internal override void Execute(List<string> args)
	{
		// Refresh all items in the player's inventory and banks.
		Main.LocalPlayer.RefreshItems(false);

		// Let the user know
		Output("Refreshed all items.");
	}
}
