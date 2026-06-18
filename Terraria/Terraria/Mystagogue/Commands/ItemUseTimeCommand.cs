using System;
using System.Collections.Generic;
using Terraria.Mystagogue.Utils;

namespace Terraria.Mystagogue.Commands;
internal class ItemUseTimeCommand : Command
{
	public ItemUseTimeCommand() : base("ut", "[Ticks] Changes the delay between logic activations of the held item.") { }
	protected internal override void Execute(List<string> args)
	{
		var item = ItemHelper.GetItemToChange();

		if (item == null)
			return;

		if (args.Count > 0) {
			// Here we make changes
			if (!int.TryParse(args[0], out int useTime)) {
				Output($"Invalid use time [{args[0]}]. It must be a number of an... acceptable size.", true);
				return;
			}

			// Clamp to a reasonable range so we don't break the game swinging an item for an hour
			useTime = Math.Max(0, Math.Min(80, useTime));

			// Dewit
			item.useTime = useTime;
		}
		else {
			// Make a control
			Item referenceItem = new Item();
			referenceItem.SetDefaults(item.type);
			referenceItem.Prefix(item.prefix);
			referenceItem.Refresh(false);

			// Copy from the control
			item.useTime = referenceItem.useTime;

			referenceItem.TurnToAir(true);
		}

		// Holler
		Output("Use time set to " + item.useTime);
	}
}
