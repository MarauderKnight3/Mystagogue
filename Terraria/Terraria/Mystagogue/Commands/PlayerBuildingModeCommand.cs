using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class PlayerBuildingModeCommand : Command
{
	internal static bool BuildingMode;

	public PlayerBuildingModeCommand() : base("building", "Enables or disables building mode, removing restraints from your ability to build or use tools.") { }
	protected internal override void Execute(List<string> args)
	{
		BuildingMode = !BuildingMode;
		Output("Building mode toggled " + (BuildingMode ? "on" : "off") + ".");
	}

	protected internal override void ResetVariables() => BuildingMode = false;

	protected internal override void PostResetEffectsHook()
	{
		// Add a method to be run in this hook that will make any held building item, such as blocks, walls, tools, painting, wire related stuff, etc.
		// have infinite reach, zero use time, and maximum power.
		// It can just check the mouse item and the hotbar item.
		// The method ought return if the hack is disabled.
		// disabling it should refresh items the hack would affect.
	}
}

