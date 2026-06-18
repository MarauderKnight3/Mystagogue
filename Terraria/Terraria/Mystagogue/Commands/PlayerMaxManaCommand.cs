using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class PlayerMaxManaCommand : Command
{
	public PlayerMaxManaCommand() : base("maxmana", "[Amount] Sets the maximum of the player's mana.") { }
	protected internal override void Execute(List<string> args)
	{
		int maxManaTarget = 200;

		// Here we make changes
		if (args.Count > 0 && !int.TryParse(args[0], out maxManaTarget)) {
			Output($"Invalid amount [{args[0]}]. It must be a number of an... acceptable size.", true);
			return;
		}

		// Dewit
		Main.LocalPlayer.statManaMax = maxManaTarget;

		// Let the player know
		Output("Max Mana set to " + Main.LocalPlayer.statManaMax + ".");
	}
}
