using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class PlayerRefreshAllItemsCommand : Command
{
	public PlayerRefreshAllItemsCommand() : base("ris", "All the items in your inventory and banks (such as the Piggy Bank, Safe, Defender's Forge, and Void Bag) will be normalized. Effective at making items typical.") { }
	protected internal override void Execute(List<string> args)
	{
		// Refresh all items in the player's inventory and banks.
		Main.LocalPlayer.RefreshItems(false);

		// Let the user know
		Output($"Normalized all items.");
	}
}
