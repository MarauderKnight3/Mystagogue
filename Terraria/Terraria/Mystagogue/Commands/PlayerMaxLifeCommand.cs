using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class PlayerMaxLifeCommand : Command
{
	public PlayerMaxLifeCommand() : base("maxlife", "[Amount] Sets this character's maximum life.") { }
	protected internal override void Execute(List<string> args)
	{
		int maxLifeTarget = 500;

		// Here we make changes
		if (args.Count > 0 && !int.TryParse(args[0], out maxLifeTarget)) {
			Output($"Invalid amount [{args[0]}]. It must be a number of an... acceptable size.", true);
			return;
		}

		// Dewit
		Main.player[Main.myPlayer].statLifeMax = maxLifeTarget;

		// Let the player know
		Output("Max Life set to " + Main.player[Main.myPlayer].statLifeMax + ".");
	}
}
