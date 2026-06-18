using System;
using System.Collections.Generic;
using Terraria.Mystagogue.Utils;

namespace Terraria.Mystagogue.Commands;
internal class ItemAnimationTimeCommand : Command
{
	public ItemAnimationTimeCommand() : base("at", "[Ticks] Changes the duration of the animation of the held item.") { }
	protected internal override void Execute(List<string> args)
	{
		var item = ItemHelper.GetItemToChange();

		if (item == null)
			return;

		if (args.Count > 0) {
			// Here we make changes
			if (!int.TryParse(args[0], out int animationTime)) {
				Output($"Invalid animation time [{args[0]}]. It must be a number of an... acceptable size.", true);
				return;
			}

			// Clamp to a reasonable range so we don't break the game swinging an item for an hour
			animationTime = Math.Max(0, Math.Min(80, animationTime));

			// Dewit
			item.useAnimation = animationTime;
		}
		else {
			// Make a control
			Item referenceItem = new Item();
			referenceItem.SetDefaults(item.type);
			referenceItem.Prefix(item.prefix);
			referenceItem.Refresh(false);

			// Copy from the control
			item.useAnimation = referenceItem.useAnimation;

			referenceItem.TurnToAir(true);
		}

		// Holler
		Output("Item animation time set to " + item.useAnimation);
	}
}
