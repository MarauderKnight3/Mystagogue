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
		if (!BuildingMode) {
			List<Item[]> inventories = [Main.player[Main.myPlayer].inventory];

			if (Main.player[Main.myPlayer].useVoidBag())
				inventories.Add(Main.player[Main.myPlayer].bank4.item);

			foreach (Item[] contents in inventories) {
				foreach (Item item in contents) {
					if (item.MystagogueBuildingModeModified) {
						item.MystagogueBuildingModeModified = false;
						item.Refresh(false);
					}
				}
			}
		}
	}

	protected internal override void ResetVariables() => BuildingMode = false;

	protected internal override void PostResetEffectsHook()
	{
		if (!BuildingMode)
			return;

		List<Item[]> inventories = [Main.player[Main.myPlayer].inventory];

		if (Main.player[Main.myPlayer].useVoidBag())
			inventories.Add(Main.player[Main.myPlayer].bank4.item);

		foreach (Item[] contents in inventories) {
			foreach (Item item in contents) {
				if (item.IsAir || item.MystagogueBuildingModeModified)
					continue;

				if (item.createTile != -1 || item.createWall != -1 || item.pick != 0 || item.axe != 0 || item.hammer != 0 || item.PaintOrCoating || item.mech || ItemID.Sets.AlsoABuildingItem[item.type]) {
					item.useTime = 0;
					item.tileBoost = 70;

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
}
