using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class PlayerManaCostZeroCommand : Command
{
	internal static int ManaCostOffset;

	public PlayerManaCostZeroCommand() : base("nomanacost", "Disables the player's mana consumption.") { }
	protected internal override void Execute(List<string> args)
	{
		ManaCostOffset = ManaCostOffset == 0 ? -1 : 0;
		Output("Mana cost toggled " + (ManaCostOffset == 0 ? "off" : "on") + ".");
	}

	protected internal override void ResetVariables() => ManaCostOffset = 0;

	protected internal override void PostResetEffectsHook() => Main.LocalPlayer.manaCost += ManaCostOffset;
}
