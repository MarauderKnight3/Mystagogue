using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class PlayerGodModeCommand : Command
{
	internal static bool GodMode;

	public PlayerGodModeCommand() : base("god", "Enables or disables the equivalent of journey god mode.") { }
	protected internal override void Execute(List<string> args)
	{
		GodMode = !GodMode;
		Output("God mode toggled " + (GodMode ? "on" : "off") + ".");
	}

	protected internal override void ResetVariables() => GodMode = false;

	protected internal override void PostResetEffectsHook() => Main.player[Main.myPlayer].creativeGodMode = GodMode;
}
