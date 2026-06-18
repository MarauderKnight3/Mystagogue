using System;
using System.Collections.Generic;
using Terraria.Mystagogue.Utils;

namespace Terraria.Mystagogue.Commands;
internal class ItemCriticalCommand : Command
{
	public ItemCriticalCommand() : base("crit", "[Chance] Changes the critical chance of the held item.") { }
	protected internal override void Execute(List<string> args)
	{
		var item = ItemHelper.GetItemToChange();

		if (item == null)
			return;

		if (args.Count > 0) {
			// Here we make changes
			if (!int.TryParse(args[0], out int crit)) {
				Output($"Invalid critical chance [{args[0]}]. It must be a number of an... acceptable size.", true);
				return;
			}

			// Clamp to a reasonable range so we don't break the game with desynchronization
			crit = Math.Max(0, Math.Min(200, crit));

			// Dewit
			item.crit = crit;
		}
		else {
			// Make a control
			Item referenceItem = new Item();
			referenceItem.SetDefaults(item.type);
			referenceItem.Prefix(item.prefix);
			referenceItem.Refresh(false);

			// Copy from the control
			item.crit = referenceItem.crit;

			referenceItem.TurnToAir(true);
		}

		// Holler
		Output("Item critical chance set to " + item.crit);
	}
}
