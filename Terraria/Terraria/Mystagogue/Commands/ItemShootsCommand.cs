using System.Collections.Generic;
using Terraria.ID;

namespace Terraria.Mystagogue.Commands;
internal class ItemShootsCommand : Command
{
	public ItemShootsCommand() : base("shoot", "[Desired projectile ID] The selected item in your hotbar will fire the given projectile if it can. Run without a specification to undo this change.") { }
	protected internal override void Execute(List<string> args)
	{
		// The player must be holding an item to change it.
		if (Main.player[Main.myPlayer].HeldItem.IsAir) {
			Output("You aren't holding an item to change.", true);
			return;
		}

		if (args.Count > 0) {
			// Here we make changes
			if (!int.TryParse(args[0], out int projID)) {
				Output($"Invalid projectile ID [{args[0]}].", true);
				return;
			}

			// Potential shenanigans with negative IDs, I haven't checked (again)
			if (projID > ProjectileID.Count) {
				Output($"Projectile ID [{projID}] is out of range.", true);
				return;
			}

			// Dewit
			Main.player[Main.myPlayer].HeldItem.shoot = projID;
			Main.player[Main.myPlayer].HeldItem.useAmmo = AmmoID.None;

			// Holler
			Output("Projectile type set to " + Lang.GetProjectileName(Main.player[Main.myPlayer].HeldItem.shoot) + " (" + Main.player[Main.myPlayer].HeldItem.shoot + ")");
		}
		else {
			// Set the projectile type to the unchanging absolute that is whatever it's supposed to be
			Main.player[Main.myPlayer].HeldItem.shoot = ContentSamples.ItemsByType[Main.player[Main.myPlayer].HeldItem.type].shoot;
			Main.player[Main.myPlayer].HeldItem.useAmmo = ContentSamples.ItemsByType[Main.player[Main.myPlayer].HeldItem.type].useAmmo;

			// Holler
			Output("Projectile type set to default: " + Lang.GetProjectileName(Main.player[Main.myPlayer].HeldItem.shoot) + " (" + Main.player[Main.myPlayer].HeldItem.shoot + ")");
		}
	}
}
