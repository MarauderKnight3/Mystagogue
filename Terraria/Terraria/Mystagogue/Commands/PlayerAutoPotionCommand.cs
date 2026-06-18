using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class PlayerAutoPotionCommand : Command
{
	internal static bool AutoPotion;

	public PlayerAutoPotionCommand() : base("ap", "Toggles automatic quick buffing.") { }
	protected internal override void Execute(List<string> args)
	{
		AutoPotion = !AutoPotion;
		Output("Auto potion toggled " + (AutoPotion ? "on" : "off") + ".");
	}

	protected internal override void ResetVariables() => AutoPotion = false;
}
