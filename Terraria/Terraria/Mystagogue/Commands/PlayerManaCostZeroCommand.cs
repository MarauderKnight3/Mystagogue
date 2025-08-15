using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class PlayerManaCostZeroCommand : Command
{
	internal static float ManaCostOffset;

	public PlayerManaCostZeroCommand() : base("nomanacost", "Enables or disables the maximizing of mana cost reduction.") { }
	protected internal override void Execute(List<string> args)
	{
		ManaCostOffset = ManaCostOffset == 0f ? 1f : 0f;
		Output("Mana cost toggled " + (ManaCostOffset == 0f ? "off" : "on") + ".");
	}

	protected internal override void ResetVariables() => ManaCostOffset = 0f;

	protected internal override void PostResetEffectsHook() => Main.player[Main.myPlayer].manaCost -= ManaCostOffset;
}
