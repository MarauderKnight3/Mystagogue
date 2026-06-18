using System;
using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class PlayerMobilityBoostCommand : Command
{
	internal static int MobilityBoostCurrentPower;

	public PlayerMobilityBoostCommand() : base("boost", "[Strength, 0-7] Sets the degree that mobility will be boosted by.") { }
	protected internal override void Execute(List<string> args)
	{
		int boostTarget = 0;

		// Here we make changes
		if (args.Count > 0 && !int.TryParse(args[0], out boostTarget)) {
			Output($"Invalid boost [{args[0]}]. It must be a number of an... acceptable size.", true);
			return;
		}

		// Clamp to a reasonable range so we don't twist our ankles
		boostTarget = Math.Max(0, Math.Min(7, boostTarget));

		// Dewit
		MobilityBoostCurrentPower = boostTarget;

		// Let the player know
		if (MobilityBoostCurrentPower == 0)
			Output("Mobility boost inactive.");
		else
			Output("Mobility boost active at power level " + MobilityBoostCurrentPower + ".");
	}

	protected internal override void ResetVariables() => MobilityBoostCurrentPower = 0;

	protected internal override void PostResetEffectsHook()
	{
		if (MobilityBoostCurrentPower > 0) {
			Main.LocalPlayer.moveSpeed = 1f + MobilityBoostCurrentPower * 1.6f;
			Main.LocalPlayer.runSlowdown += MobilityBoostCurrentPower * 0.03f;
			Main.LocalPlayer.gravity += MobilityBoostCurrentPower * 0.075f;
			Main.LocalPlayer.maxFallSpeed += MobilityBoostCurrentPower * 7f;
			Main.LocalPlayer.jumpSpeedBoost = 0f + MobilityBoostCurrentPower * 2.5f;
		}
	}
}
