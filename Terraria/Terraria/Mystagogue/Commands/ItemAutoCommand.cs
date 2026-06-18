using System.Collections.Generic;
using Terraria.Mystagogue.Utils;

namespace Terraria.Mystagogue.Commands;
internal class ItemAutoCommand : Command
{
	public ItemAutoCommand() : base("auto", "Toggles auto reuse for the held item.") { }
	protected internal override void Execute(List<string> args)
	{
		var item = ItemHelper.GetItemToChange();

		if (item == null)
			return;

		// Dewit
		item.autoReuse = !item.autoReuse;

		// Holler
		Output("Item auto reuse turned " + (item.autoReuse ? "on" : "off") + ".");
	}
}
