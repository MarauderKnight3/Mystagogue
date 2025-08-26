using System.Collections.Generic;
using Terraria.Mystagogue.Utils;

namespace Terraria.Mystagogue.Commands;
internal class ItemPrefixCommand : Command
{
	public ItemPrefixCommand() : base("reforge", "[Item Prefix/ID] The item held in the cursor, or in the hotbar if the cursor is empty, will have this prefix.") { }
	protected internal override void Execute(List<string> args)
	{
		// The player must be holding an item to change it.
		if (Main.mouseItem.IsAir && Main.player[Main.myPlayer].HeldItem.IsAir) {
			Output("You aren't holding an item to change.", true);
			return;
		}

		// Switch to the hotbar if the cursor is empty.
		ref var item = ref Main.mouseItem;
		if (Main.mouseItem.IsAir)
			item = Main.player[Main.myPlayer].HeldItem;

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
		item.Refresh();

		// Let the user know
		Output($"Reforged to [{PrefixHelper.PrefixNames[prefixID]}].");
	}
}
