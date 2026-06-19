using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;

namespace Terraria.Mystagogue.Utils;

public static class CommandSettings
{
	// The assumption is made here that all player names are unique.
	// If two saved players share a name, they'll get the same settings.
	// Key = player name, Value = (command name.field name, value)

	private static string SavePath => Path.Combine(Main.SavePath, $"anomalies.json");

	private static readonly DataContractJsonSerializer Serializer =
		new(typeof(SortedDictionary<string, SortedDictionary<string, object>>));

	private static List<FieldInfo> SettingFields()
	{
		List<FieldInfo> allFields = [];
		foreach (Commands.Command c in Commands.Command.Library.Values) {
			allFields.AddRange([.. c.GetType()
				.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
				.Where(f => !f.GetCustomAttributes(typeof(DoNotSaveAttribute), false).Any() &&
					(f.FieldType == typeof(bool) ||
					f.FieldType == typeof(int) ||
					f.FieldType == typeof(string)))]);
		}
		return allFields;
	}

	internal static void Save(string player)
	{
		var allPlayerSettings = Load();
		if (allPlayerSettings.ContainsKey(player)) {
			allPlayerSettings.Remove(player);
		}
		allPlayerSettings.Add(player, []);

		// Apply all current command states to player name.
		foreach (var field in SettingFields()) {
			allPlayerSettings[player].Add(
				field.DeclaringType.Name + "." + field.Name,
				field.GetValue(null));
		}

		try {
			using var stream = new MemoryStream();
			Serializer.WriteObject(stream, allPlayerSettings);
			File.WriteAllBytes(SavePath, stream.ToArray());
		}
		catch (Exception e) {
			Debug.WriteLine($"Failed to save Mystagogue anomalies: {e.Message}");
		}
	}

	// In case the file was edited by hand, we load the file every time.
	private static SortedDictionary<string, SortedDictionary<string, object>> Load()
	{
		if (!File.Exists(SavePath))
			return [];

		try {
			using var stream = new FileStream(SavePath, FileMode.Open, FileAccess.Read);
			var allPlayerSettings =
				(SortedDictionary<string, SortedDictionary<string, object>>)
				Serializer.ReadObject(stream);
			return allPlayerSettings;
		}
		catch (Exception e) {
			Debug.WriteLine($"Failed to load Mystagogue anomalies: {e.Message}");
			Anomaly.Reset();
			return [];
		}
	}

	private static void Apply(SortedDictionary<string, object> settings)
	{
		foreach (var field in SettingFields()) {
			string key = field.DeclaringType.Name + "." + field.Name;
			if (settings.TryGetValue(key, out object value)) {
				try {
					// Handle type conversion safely
					if (field.FieldType == typeof(bool) && bool.TryParse(value.ToString(), out bool b))
						field.SetValue(null, b);
					else if (field.FieldType == typeof(int) && int.TryParse(value.ToString(), out int i))
						field.SetValue(null, i);
					else if (field.FieldType == typeof(string) && value.ToString() is string s)
						field.SetValue(null, s);
				}
				catch (Exception e) {
					Debug.WriteLine($"Failed to load Mystagogue anomaly: {key}: {e.Message}");
				}
			}
		}
	}

	internal static void LoadAndApply(string player)
	{
		Anomaly.Reset();
		var allPlayerSettings = Load();
		if (allPlayerSettings.TryGetValue(player, out var settings)) {
			Debug.WriteLine($"Attempting to load anomalies for player {player}");
			Apply(settings);
		}
		else {
			Debug.WriteLine($"No saved anomalies found for player {player}");
		}
	}
}