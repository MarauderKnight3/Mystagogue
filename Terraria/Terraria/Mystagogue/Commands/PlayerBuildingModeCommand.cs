using System.Collections.Generic;
using Terraria.ID;
using Terraria.Mystagogue.Utils;

namespace Terraria.Mystagogue.Commands;
internal class PlayerBuildingModeCommand : Command
{
	internal static bool BuildingMode;

	public PlayerBuildingModeCommand() : base("building", "Toggles building mode, which buffs tools and placeables while active.") { }
	protected internal override void Execute(List<string> args)
	{
		BuildingMode = !BuildingMode;
		Output("Building mode toggled " + (BuildingMode ? "on" : "off") + ".");
	}

	protected internal override void ResetVariables() => BuildingMode = false;

	protected internal override void PostResetEffectsHook()
	{
		if (!BuildingMode)
			return;

		var item = ItemHelper.GetItemToChange(false);

		if (item == null)
			return;

		if (item.createTile == -1 && item.createWall == -1
			&& item.pick < 1 && item.axe < 1 && item.hammer < 1
			&& !item.PaintOrCoating && !item.mech
			&& !ItemID.Sets.AlsoABuildingItem[item.type])
			return;

		item.useTime = 1;
		item.consumable = false;
		item.tileBoost = 30;

		if (item.pick > 0)
			item.pick = ContentSamples.ItemsByType[ItemID.VortexPickaxe].pick * 20;

		if (item.axe > 0)
			item.axe = ContentSamples.ItemsByType[ItemID.TheAxe].axe * 20;

		if (item.hammer > 0)
			item.hammer = ContentSamples.ItemsByType[ItemID.TheAxe].hammer * 20;
	}
}
