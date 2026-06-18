using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class MapPlayerIconForceEnableCommand : Command
{
	internal static bool PlayerMapIconFeatureFreeForceEnable;

	public MapPlayerIconForceEnableCommand() : base("freemap", "Makes it possible to see all player map icons and teleport to them without using a wormhole potion.") { }
	protected internal override void Execute(List<string> args)
	{
		PlayerMapIconFeatureFreeForceEnable = !PlayerMapIconFeatureFreeForceEnable;
		Output("Free map mode toggled " + (PlayerMapIconFeatureFreeForceEnable ? "on" : "off") + ".");
	}

	protected internal override void ResetVariables() => PlayerMapIconFeatureFreeForceEnable = false;
}
