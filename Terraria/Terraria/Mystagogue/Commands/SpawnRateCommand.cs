using System;
using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class SpawnRateCommand : Command
{
	internal static int SpawnRate;

	public SpawnRateCommand() : base("spawnrate", "[Spawn rate multiplier, 0-50] Sets the multiplier of your character's ambient enemy spawn rate. Run without a specification to undo this change.") { }
	protected internal override void Execute(List<string> args)
	{
		int spawnRateTarget = 1;

		// Here we make changes
		if (args.Count > 0 && !int.TryParse(args[0], out spawnRateTarget)) {
			Output($"Invalid multiplier [{args[0]}]. It must be a number of an... acceptable size.", true);
			return;
		}

		// Clamp to a reasonable range so we don't thin the oxygen in the atmosphere
		spawnRateTarget = Math.Max(0, Math.Min(50, spawnRateTarget));

		// Dewit
		SpawnRate = spawnRateTarget;

		// Let the player know
		if (SpawnRate == 1)
			Output("Spawn rate multiplier inactive.");
		else
			Output("Spawn rate multiplier set to " + SpawnRate + ".");
	}

	protected internal override void ResetVariables() => SpawnRate = 1;
}
