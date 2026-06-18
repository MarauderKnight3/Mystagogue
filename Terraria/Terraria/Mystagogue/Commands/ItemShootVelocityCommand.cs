using System;
using System.Collections.Generic;
using Terraria.Mystagogue.Utils;

namespace Terraria.Mystagogue.Commands;
internal class ItemShootVelocityCommand : Command
{
	public ItemShootVelocityCommand() : base("vel", "[Velocity] Changes the velocity of the projectiles shot by the held item.") { }
	protected internal override void Execute(List<string> args)
	{
		var item = ItemHelper.GetItemToChange();

		if (item == null)
			return;

		if (args.Count > 0) {
			// Here we make changes
			if (!int.TryParse(args[0], out int velocity)) {
				Output($"Invalid velocity [{args[0]}]. It must be a number of an... acceptable size.", true);
				return;
			}

			// Clamp to a reasonable range so we don't needlessly stress limits
			velocity = Math.Max(0, Math.Min(60, velocity));

			// Dewit
			item.shootSpeed = velocity;
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
		Output("Item projectile velocity set to " + item.shootSpeed);
	}
}
