using System.Collections.Generic;
using Terraria.ID;

namespace Terraria.Mystagogue.Commands;
internal class PlayerBuildingModeCommand : Command
{
	internal static bool BuildingMode;

	public PlayerBuildingModeCommand() : base("building", "Enables or disables building mode, removing restraints from your ability to build or use tools.") { }
	protected internal override void Execute(List<string> args)
	{
		BuildingMode = !BuildingMode;
		Output("Building mode toggled " + (BuildingMode ? "on" : "off") + ".");
		if (!BuildingMode)
			for (int i = 0; i < Main.player[Main.myPlayer].inventory.Length; i++)
				if (Main.player[Main.myPlayer].inventory[i].MystagogueBuildingModeModified) {
					Main.player[Main.myPlayer].inventory[i].MystagogueBuildingModeModified = false;
					Main.player[Main.myPlayer].inventory[i].Refresh();
				}
	}

	protected internal override void ResetVariables() => BuildingMode = false;

	protected internal override void PostResetEffectsHook()
	{
		if (!BuildingMode)
			return;

		for (int i = 0; i < Main.player[Main.myPlayer].inventory.Length; i++) {
			ref Item item = ref Main.player[Main.myPlayer].inventory[i];

			if (item.IsAir || item.MystagogueBuildingModeModified)
				continue;

			if (item.createTile != -1 || item.createWall != -1 || item.pick != 0 || item.axe != 0 || item.hammer != 0 || item.PaintOrCoating || item.mech || ItemID.Sets.AlsoABuildingItem[item.type]) {
				item.useTime = 0;
				item.tileBoost = 10000;

				if (item.pick > 0)
					item.pick = ContentSamples.ItemsByType[ItemID.VortexPickaxe].pick * 10;

				if (item.axe > 0)
					item.axe = ContentSamples.ItemsByType[ItemID.TheAxe].axe * 10;

				if (item.hammer > 0)
					item.hammer = ContentSamples.ItemsByType[ItemID.TheAxe].hammer * 10;

				item.MystagogueBuildingModeModified = true;
			}
		}
	}
}
