using System;
using System.Collections.Generic;
using Terraria.Mystagogue.Utils;

namespace Terraria.Mystagogue.Commands;
internal class ItemScaleCommand : Command
{
	public ItemScaleCommand() : base("scale", "[Percent multiplier] Changes the size of the held item.") { }
	protected internal override void Execute(List<string> args)
	{
		var item = ItemHelper.GetItemToChange();

		if (item == null)
			return;

		if (args.Count > 0) {
			// Here we make changes
			if (!int.TryParse(args[0], out int scalePercentage)) {
				Output($"Invalid scale [{args[0]}]. It must be a number of an... acceptable size.", true);
				return;
			}

			// Clamp to a barely reasonable range so we don't break the game with something noneuclidean
			scalePercentage = Math.Max(1, Math.Min(100000, scalePercentage));

			// Dewit
			item.scale = scalePercentage / 100;
		}
		else {
			// Make a control
			Item referenceItem = new Item();
			referenceItem.SetDefaults(item.type);
			referenceItem.Prefix(item.prefix);
			referenceItem.Refresh(false);

			// Copy from the control
			item.scale = referenceItem.scale;

			referenceItem.TurnToAir(true);
		}

		// Holler
		Output("Item scale set to " + (int)(item.scale * 100) + "%");
	}
}
