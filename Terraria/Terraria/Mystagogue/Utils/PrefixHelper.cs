using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Terraria.ID;

namespace Terraria.Mystagogue.Utils;

internal class PrefixHelper
{
	internal static readonly List<string> PrefixNames;

	static PrefixHelper()
	{
		// We use this to collect a list of all prefix names, excluding those with digits in their programmatic names.
		// 'None' will be our name for the lack of a prefix, which is the default value.
		PrefixNames = ["None"];
		for (int i = 1; i < PrefixID.Count; i++) {
			// Initialize with empty strings for all prefix IDs to prevent index errors
			PrefixNames.Add("");
		}
		// Use reflection to get all public static fields in the PrefixID class
		var fields = typeof(PrefixID).GetFields(BindingFlags.Public | BindingFlags.Static);
		foreach (var field in fields) {
			if (field.FieldType == typeof(int) && field.IsLiteral) // Ensure what we are reflecting is an integer constant
			{
				int id = (int)field.GetValue(null);
				if (id < PrefixID.Count && !Regex.IsMatch(field.Name, @"\d")) // Exclude prefixes containing digits
				{
					// Put it where it goes
					PrefixNames[id] = field.Name;
				}
			}
		}
	}

	// We take what is likely one word that is a prefix name and we take the item ID to SwitchSimilar the prefix if needed.
	internal static object NameOrIDToID(string input, int itemID)
	{
		// If it is an integer, check it against the range. We assume the integer is positive.
		// The game already handles invalid negative IDs, luckily.
		if (byte.TryParse(input, out byte prefixID)) {
			if (prefixID >= PrefixID.Count) {
				return $"Prefix ID [{prefixID}] is out of range.";
			}
			return SwitchSimilar(prefixID, itemID);
		}
		else {
			string prefixName = input;
			List<byte> matches = FindPrefixIDsNamesStartWith(prefixName);

			if (matches.Count == 0) {
				return $"Prefix [{prefixName}] not found.";
			}
			else if (matches.Count > 1) {
				// This can be used to compile a list of prefixes if there is more than one match
				// We use this list to accuse the user of being ambiguous
				string matchList = string.Join(", ", matches.Select(i => PrefixNames[i] + " (" + i + ")"));
				return $"Multiple items found matching [{prefixName}]: {matchList}.\nPlease specify an ID or a more specific name.";
			}
			return SwitchSimilar(matches[0], itemID);
		}
	}

	// What it says on the tin.
	internal static List<byte> FindPrefixIDsNamesStartWith(string prefixName)
	{
		List<byte> matches = [];
		for (byte i = 1; i < PrefixID.Count; i++) {
			// Since the user probably doesn't need to 'search' prefix names, we can assume they know the beginning of the name.
			// Use StartsWith instead of Contains because of that
			if (PrefixNames[i].StartsWith(prefixName, System.StringComparison.OrdinalIgnoreCase)) {
				matches.Add(i);
			}
		}
		return matches;
	}

	internal static byte SwitchSimilar(byte prefixID, int itemID)
	{
		// Solves ambiguity using context from the item ID.
		int newID = prefixID;
		if (newID is PrefixID.Hasty or PrefixID.Hasty2)
			newID = !ContentSamples.ItemsByType[itemID].accessory ? PrefixID.Hasty : PrefixID.Hasty2;
		if (newID is PrefixID.Deadly or PrefixID.Deadly2)
			newID = ContentSamples.ItemsByType[itemID].ranged ? PrefixID.Deadly : PrefixID.Deadly2;
		if (newID is PrefixID.Quick or PrefixID.Quick2)
			newID = !ContentSamples.ItemsByType[itemID].accessory ? PrefixID.Quick : PrefixID.Quick2;
		if (newID is PrefixID.Legendary or PrefixID.Legendary2)
			newID = !(itemID == ItemID.Terrarian) ? PrefixID.Legendary : PrefixID.Legendary2;

		return (byte)newID;
	}
}