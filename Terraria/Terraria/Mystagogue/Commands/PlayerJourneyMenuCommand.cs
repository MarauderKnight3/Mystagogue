using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class PlayerJourneyMenuCommand : Command
{
	internal static bool JourneyMenuForceEnable;

	public PlayerJourneyMenuCommand() : base("journey", "Forces the visibility of the journey mode menu.") { }
	protected internal override void Execute(List<string> args)
	{
		JourneyMenuForceEnable = !JourneyMenuForceEnable;
		Output("Journey mode menu toggled " + (JourneyMenuForceEnable ? "on" : "off") + ".");
	}

	protected internal override void ResetVariables() => JourneyMenuForceEnable = false;
}
