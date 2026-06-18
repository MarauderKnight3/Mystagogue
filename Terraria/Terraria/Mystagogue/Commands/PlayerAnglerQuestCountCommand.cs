using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class PlayerAnglerQuestCountCommand : Command
{
	public PlayerAnglerQuestCountCommand() : base("angler", "[Amount] Retroactively changes how many angler quests the player has finished.") { }
	protected internal override void Execute(List<string> args)
	{
		if (args.Count == 0) {
			Output("You must specify an amount.", true);
			return;
		}

		// Here we make changes
		if (!int.TryParse(args[0], out int questTarget)) {
			Output($"Invalid amount [{args[0]}]. It must be a number of an... acceptable size.", true);
			return;
		}

		// Dewit
		Main.LocalPlayer.anglerQuestsFinished = questTarget;

		// Let the player know
		Output("Angler quests finished set to " + Main.LocalPlayer.anglerQuestsFinished + ".");
	}
}
