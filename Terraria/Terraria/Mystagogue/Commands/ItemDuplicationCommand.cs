using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class ItemDuplicationCommand : Command
{
	internal static bool ItemDuplicationEnabled;

	public ItemDuplicationCommand() : base("dupe", "Enables mouse-controlled item duplication. Ctrl+Alt to fill a stack, Alt+Right to copy a stack, and Ctrl+Alt+Right to copy a stack inversing the consumable item duping rule.") { }
	protected internal override void Execute(List<string> args)
	{
		ItemDuplicationEnabled = !ItemDuplicationEnabled;
		Output("Item duplication toggled " + (ItemDuplicationEnabled ? "on" : "off") + ".");
	}

	protected internal override void ResetVariables() => ItemDuplicationEnabled = false;

	internal static bool TryItemDuplication(Item slot)
	{
		if (!ItemDuplicationCommand.ItemDuplicationEnabled)
			return false;

		bool leftAlt = Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt);
		bool leftControl = Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl);
		bool mouseRight = Main.mouseRight && Main.mouseRightRelease;

		if (leftAlt && mouseRight && Main.mouseItem.IsAir) {
			Main.mouseItem = slot.Clone();
			Main.mouseItem.favorited = false;
			if (Main.mouseItem.consumable != leftControl)
				Main.mouseItem.stack = Main.mouseItem.maxStack;
			else {
				Main.mouseItem.maxStack = 1;
				Main.mouseItem.stack = 1;
			}
			return true;
		}
		if (leftControl && leftAlt && !slot.IsAir && slot.consumable && !mouseRight) {
			slot.stack = slot.maxStack;
			return true;
		}
		return false;
	}
}