using System;
using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class PlayerDifficultyCommand : Command
{
	public PlayerDifficultyCommand() : base("difficulty", "[Difficulty ID] Retroactively changes the player character's difficulty mode. Options are Classic, Mediumcore, Hardcore, and Journey. IDs are 0, 1, 2, and 3 respectively.") { }
	protected internal override void Execute(List<string> args)
	{
		if (args.Count == 0) {
			Output("You must specify an ID.", true);
			return;
		}

		// Here we make changes
		if (!byte.TryParse(args[0], out byte newDifficulty)) {
			Output($"Invalid difficulty ID [{args[0]}]. It must be a number of an... acceptable size.", true);
			return;
		}

		// Clamp to a reasonable range so we don't explore undefined horizons
		newDifficulty = Math.Max((byte)0, Math.Min((byte)3, newDifficulty));

		// Dewit
		Main.player[Main.myPlayer].difficulty = newDifficulty;

		// Let the player know
		Output("Difficulty set to " + (newDifficulty == 0 ? "Classic" : newDifficulty == 1 ? "Mediumcore" : newDifficulty == 2 ? "Hardcore" : "Journey") + " Mode.");
	}
}
