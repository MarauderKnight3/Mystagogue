using System.Collections.Generic;
using Terraria.ID;
using Terraria.Mystagogue.Utils;

namespace Terraria.Mystagogue.Commands;
internal class ItemShootsCommand : Command
{
	public ItemShootsCommand() : base("shoot", "[Projectile ID] Changes the projectile shot by the held item.") { }
	protected internal override void Execute(List<string> args)
	{
		var item = ItemHelper.GetItemToChange();

		if (item == null)
			return;

		if (args.Count > 0) {
			// Here we make changes
			if (!int.TryParse(args[0], out int projID) || projID > ProjectileID.Count) {
				Output($"Invalid projectile ID [{args[0]}].", true);
				return;
			}

			// Dewit
			item.shoot = projID;
			item.useAmmo = AmmoID.None;
		}
		else {
			// Set the projectile type to the unchanging absolute that is whatever it's supposed to be
			item.shoot = ContentSamples.ItemsByType[item.type].shoot;
			item.useAmmo = ContentSamples.ItemsByType[item.type].useAmmo;
		}

		// Holler
		Output("Item projectile type set to " + Lang.GetProjectileName(item.shoot) + " (" + item.shoot + ")");
	}
}
