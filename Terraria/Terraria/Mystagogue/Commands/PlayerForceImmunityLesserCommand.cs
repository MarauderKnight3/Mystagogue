using System.Collections.Generic;
using Terraria.ID;

namespace Terraria.Mystagogue.Commands;
internal class PlayerForceImmunityLesserCommand : Command
{
	internal static bool ForceIncapacitationDebuffImmunity;

	public PlayerForceImmunityLesserCommand() : base("haltimmune", "Makes you immune to Frozen, Webbed, and Stoned.") { }
	protected internal override void Execute(List<string> args)
	{
		ForceIncapacitationDebuffImmunity = !ForceIncapacitationDebuffImmunity;
		Output("Immunity toggled " + (ForceIncapacitationDebuffImmunity ? "on" : "off") + ".");
	}

	protected internal override void ResetVariables() => ForceIncapacitationDebuffImmunity = false;

	protected internal override void PreUpdateBuffsHook()
	{
		if (ForceIncapacitationDebuffImmunity) {
			foreach (int num2 in new List<int>
			{
				BuffID.Frozen,
				BuffID.Webbed,
				BuffID.Stoned
			}) {
				Main.LocalPlayer.buffImmune[num2] = true;
			}
		}
	}
}
