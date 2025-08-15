using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class MapPlayerIconForceEnableCommand : Command
{
	internal static bool PlayerMapIconFeatureFreeForceEnable;

	public MapPlayerIconForceEnableCommand() : base("freemap", "Allows you to see and teleport to ALL players on the map without a wormhole potion.") { }
	protected internal override void Execute(List<string> args)
	{
		PlayerMapIconFeatureFreeForceEnable = !PlayerMapIconFeatureFreeForceEnable;
		Output("Free map mode toggled " + (PlayerMapIconFeatureFreeForceEnable ? "on" : "off") + ".");
	}

	protected internal override void ResetVariables() => PlayerMapIconFeatureFreeForceEnable = false;
}
