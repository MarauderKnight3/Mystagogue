using System;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.Mystagogue.Utils;

namespace Terraria.Mystagogue.Commands;
internal class ItemCommand : Command
{
	public ItemCommand() : base("i", "[Item Name/ID] [Item Count] [Item Prefix/ID] The item held in your cursor will be this item.") { }
	protected internal override void Execute(List<string> args)
	{
		if (args.Count == 0) {
			Output("You must specify an item name or ID.", true);
		}
		// Collect the item name and any remaining arguments
		List<object> itemNameOrIDAndRemainderArgs = ItemHelper.ConcatUntilSecondNumber(args);

		// Take apart the item name and the remainder of the arguments
		string itemName = itemNameOrIDAndRemainderArgs[0] as string;
		List<string> remainingArgs = itemNameOrIDAndRemainderArgs[1] as List<string>;

		// Find the item ID
		int itemID = 0;
		switch (ItemHelper.NameOrIDToID(itemName)) {
			case string errorReason:
				Output(errorReason, true);
				return;
			case int result:
				itemID = result;
				break;
		}

		// Interpret the item stack count
		int itemCount = 1;
		if (remainingArgs.Count > 0) {
			if (int.TryParse(remainingArgs[0], out itemCount)) {
				itemCount = Math.Max(1, Math.Min(itemCount, ContentSamples.ItemsByType[itemID].maxStack));
			}
			else {
				Output($"Invalid item count [{remainingArgs[0]}]. It may be too high.", true);
				itemCount = 1;
			}
		}

		// Interpret the prefix ID or name
		byte prefixID = 0;
		if (remainingArgs.Count > 1) {
			switch (PrefixHelper.NameOrIDToID(remainingArgs[1], itemID)) {
				case string errorReason:
					Output(errorReason, true);
					return;
				case byte id:
					prefixID = id;
					break;
			}
		}

		// Edit the mouse item
		Main.mouseItem.SetDefaults(itemID);
		Main.mouseItem.stack = itemCount;
		Main.mouseItem.prefix = prefixID;
		Main.mouseItem.Refresh();

		// Let the user know
		Output($"Set cursor item to [{Lang.GetItemNameValue(itemID)}] with count [{itemCount}] and prefix [{PrefixHelper.PrefixNames[prefixID]}].");
	}
}