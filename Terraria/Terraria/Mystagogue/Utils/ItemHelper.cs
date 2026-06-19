using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Terraria.ID;

namespace Terraria.Mystagogue.Utils;
internal class ItemHelper
{
	internal static Item? GetItemToChange(bool complain = true)
	{
		// The player must be holding an item to change it.
		if (Main.mouseItem.IsAir && Main.LocalPlayer.HeldItem.IsAir) {
			if (complain)
				Anomaly.Output("You aren't holding an item to change.", true);
			return null;
		}

		// Switch to the hotbar if the cursor is empty.
		var item = Main.mouseItem;
		if (Main.mouseItem.IsAir)
			item = Main.LocalPlayer.HeldItem;

		return item;
	}

	internal static object NameOrIDToID(string input)
	{
		// We assume the input is either an ID or a concatenated item name.
		if (int.TryParse(input, out int itemID)) {
			if (itemID >= ItemID.Count)
				return $"Item ID [{itemID}] is out of range.";

			// There could be potential shenanigans with negative IDs, I haven't checked
			return itemID;
		}
		else {
			// Find all matching item names as IDs, only pass if there is exactly one match
			List<int> matches = FindItemIDsNamesStartWith(input);
			if (matches.Count == 0)
				return $"Item [{input}] not found.";

			else if (matches.Count > 1) {
				string matchList = string.Join(", ", matches.Select(i => Lang.GetItemNameValue(i) + " (" + i + ")"));
				return $"Multiple items found matching [{input}]: {matchList}.\nPlease specify an ID or a more specific name.";
			}
			return matches[0];
		}
	}

	internal static List<int> FindItemIDsNamesStartWith(string itemName)
	{
		// Find all matching item names as IDs, only pass if there is exactly one match
		// We assume the input is either an ID or a concatenated item name.
		List<int> matches = [];
		for (int i = 0; i < ItemID.Count; i++) {
			if (Lang.GetItemNameValue(i).Contains(itemName, StringComparison.OrdinalIgnoreCase)) {
				matches.Add(i);
			}
		}
		return matches;
	}

	// Collect all arguments up to the second numeric one as the item name
	// Can be repurposed for similar tasks that take a concatenated name or an ID
	internal static List<object> ConcatFromSecondUntilNumber(List<string> inputPieces)
	{
		int numberIndex = 1;

		for (; numberIndex < inputPieces.Count; numberIndex++)
			if (Regex.IsMatch(inputPieces[numberIndex], @"^\d+$"))
				break;

		List<string> namePartsOrID = inputPieces.GetRange(0, numberIndex);
		List<string> remainder = inputPieces.GetRange(numberIndex, inputPieces.Count - numberIndex);

		return [string.Join(" ", namePartsOrID), remainder];
	}
}
