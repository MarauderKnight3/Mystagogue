using System.Collections.Generic;
using Terraria.Mystagogue.Utils;

namespace Terraria.Mystagogue.Commands;
internal class ItemPrefixCommand : Command
{
	public ItemPrefixCommand() : base("prefix", "[Item Prefix/ID] Changes the prefix of the held item.") { }
	protected internal override void Execute(List<string> args)
	{
		var item = ItemHelper.GetItemToChange();

		if (item == null)
			return;

		byte prefixID = 0;
		// Interpret the prefix ID or name
		if (args.Count > 0) {
			switch (PrefixHelper.NameOrIDToID(args[0], item.type)) {
				case string errorReason:
					Output(errorReason, true);
					return;
				case byte result:
					prefixID = result;
					break;
			}
		}

		// Edit the mouse item
		item.prefix = prefixID;
		item.Refresh(false);

		// Let the user know
		Output($"Item reforged to {PrefixHelper.PrefixNames[prefixID]}");
	}
}
