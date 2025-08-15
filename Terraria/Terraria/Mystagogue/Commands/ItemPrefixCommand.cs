using System.Collections.Generic;
using Terraria.Mystagogue.Utils;

namespace Terraria.Mystagogue.Commands;
internal class ItemPrefixCommand : Command
{
	public ItemPrefixCommand() : base("reforge", "[Item Prefix/ID] The item held in your cursor will have this prefix.") { }
	protected internal override void Execute(List<string> args)
	{
		// The player must be holding an item to change it.
		if (Main.player[Main.myPlayer].HeldItem.IsAir) {
			Output("You aren't holding an item to change.", true);
			return;
		}

		byte prefixID = 0;
		// Interpret the prefix ID or name
		if (args.Count > 0) {
			switch (PrefixHelper.NameOrIDToID(args[0], Main.mouseItem.type)) {
				case string errorReason:
					Output(errorReason, true);
					return;
				case byte result:
					prefixID = result;
					break;
			}
		}

		// Edit the mouse item
		Main.mouseItem.prefix = prefixID;
		Main.mouseItem.Refresh();

		// Let the user know
		Output($"Reforged to [{PrefixHelper.PrefixNames[prefixID]}].");
	}
}
