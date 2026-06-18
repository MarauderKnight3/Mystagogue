using System.Collections.Generic;
using Terraria.Mystagogue.Utils;

namespace Terraria.Mystagogue.Commands;
internal class ItemRefreshCommand : Command
{
	public ItemRefreshCommand() : base("ri", "Refreshes the held item.") { }
	protected internal override void Execute(List<string> args)
	{
		var item = ItemHelper.GetItemToChange();

		if (item == null)
			return;

		// Dewit
		item.Refresh(false);

		// Holler
		Output($"Refreshed item [{item.Name}].");
	}
}
