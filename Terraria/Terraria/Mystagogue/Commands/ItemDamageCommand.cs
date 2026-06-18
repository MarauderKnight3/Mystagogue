using System;
using System.Collections.Generic;
using Terraria.Mystagogue.Utils;

namespace Terraria.Mystagogue.Commands;
internal class ItemDamageCommand : Command
{
	public ItemDamageCommand() : base("dmg", "[Damage] Changes the damage of the held item.") { }
	protected internal override void Execute(List<string> args)
	{
		var item = ItemHelper.GetItemToChange();

		if (item == null)
			return;

		if (args.Count > 0) {
			// Here we make changes
			if (!int.TryParse(args[0], out int damage)) {
				Output($"Invalid damage [{args[0]}]. It must be a number of an... acceptable size.", true);
				return;
			}

			// Clamp to a reasonable range so we don't break the game with desynchronization
			damage = Math.Max(0, Math.Min(999999, damage));

			// Dewit
			item.damage = damage;
		}
		else {
			// Make a control
			Item referenceItem = new Item();
			referenceItem.SetDefaults(item.type);
			referenceItem.Prefix(item.prefix);
			referenceItem.Refresh(false);

			// Copy from the control
			item.damage = referenceItem.damage;

			referenceItem.TurnToAir(true);
		}

		// Holler
		Output("Item damage set to " + item.damage);
	}
}
