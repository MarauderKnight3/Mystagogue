using System;
using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class PlayerMaxMinionsCommand : Command
{
	internal static int MaxMinionsOffset;

	public PlayerMaxMinionsCommand() : base("maxminions", "[Amount] Adds the given amount of minion and sentry slots.") { }
	protected internal override void Execute(List<string> args)
	{
		int minionSlotsIncreaseBy = 0;

		// Here we make changes
		if (args.Count > 0 && !int.TryParse(args[0], out minionSlotsIncreaseBy)) {
			Output($"Invalid amount of minion slots [{args[0]}]. It must be a number of an... acceptable size.", true);
			return;
		}

		// Clamp to a reasonable range so we don't halt entropy
		minionSlotsIncreaseBy = Math.Max(0, Math.Min(1000, minionSlotsIncreaseBy));

		// Dewit
		MaxMinionsOffset = minionSlotsIncreaseBy;

		// Let the player know
		Output("Minion slots increased by " + MaxMinionsOffset + ".");
	}

	protected internal override void ResetVariables() => MaxMinionsOffset = 0;

	protected internal override void PostResetEffectsHook()
	{
		Main.LocalPlayer.maxMinions += MaxMinionsOffset;
		Main.LocalPlayer.maxTurrets += MaxMinionsOffset;
	}
}
