using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Terraria.ID;

namespace Terraria.Mystagogue.Utils;
internal class ItemHelper
{
	internal static object NameOrIDToID(string input)
	{
		// We assume the input is either an ID or a concatenated item name.
		if (int.TryParse(input, out int itemID)) {
			if (itemID >= ItemID.Count) {
				return $"Item ID [{itemID}] is out of range.";
			}
			// There could be potential shenanigans with negative IDs, I haven't checked
			return itemID;
		}
		else {
			// Find all matching item names as IDs, only pass if there is exactly one match
			List<int> matches = FindItemIDsNamesStartWith(input);
			if (matches.Count == 0) {
				return $"Item [{input}] not found.";
			}
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
			if (Lang.GetItemNameValue(i).StartsWith(itemName, StringComparison.OrdinalIgnoreCase)) {
				matches.Add(i);
			}
		}
		return matches;
	}

	// Collect all arguments up to the second numeric one as the item name
	// Can be repurposed for similar tasks that take a concatenated name or an ID
	internal static List<object> ConcatUntilSecondNumber(List<string> inputPieces)
	{
		List<string> namePartsOrID = [];
		List<string> remainder = inputPieces;

		// Always collect the first piece in case it is a number! It could be an ID.
		namePartsOrID.Add(inputPieces[0]);
		remainder.RemoveAt(0);

		for (int i = 0; i < inputPieces.Count && !Regex.IsMatch(inputPieces[i], @"^\d+$"); i++) {
			namePartsOrID.Add(inputPieces[i]);
			remainder.RemoveAt(0);
			// `remainder` will be left with just the item count and prefix name or ID, if any, in the ordinary use case of this method
			// exits immediately if the next piece is a number already.
		}
		return [string.Join(" ", namePartsOrID), remainder];
	}
}

// We can use this to check if a string contains another string with a specific comparison type, such as ignoring case.
// Currently otherwise not available in `string.Contains()` without this.
internal static class StringExtensions
{
	internal static bool Contains(this string source, string toCheck, StringComparison comp)
	{
		return source?.IndexOf(toCheck, comp) >= 0;
	}
}