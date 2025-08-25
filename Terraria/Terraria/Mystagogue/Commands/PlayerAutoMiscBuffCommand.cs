using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class PlayerAutoMiscBuffCommand : Command
{
	internal static bool AutoMiscBuff;

	public PlayerAutoMiscBuffCommand() : base("misc", "Enables or disables whether your character gains a collection of environmental and table buffs automatically.") { }
	protected internal override void Execute(List<string> args)
	{
		AutoMiscBuff = !AutoMiscBuff;
		Output("Auto misc buffs toggled " + (AutoMiscBuff ? "on" : "off") + ".");
	}

	protected internal override void ResetVariables() => AutoMiscBuff = false;
}
