using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class PlayerAutoPotionCommand : Command
{
	internal static bool AutoPotion;

	public PlayerAutoPotionCommand() : base("ap", "Enables or disables whether your character quick buffs on their own.") { }
	protected internal override void Execute(List<string> args)
	{
		AutoPotion = !AutoPotion;
		Output("God mode toggled " + (AutoPotion ? "on" : "off") + ".");
		Main.player[Main.myPlayer].QuickBuff();
	}

	protected internal override void ResetVariables() => AutoPotion = false;
}
