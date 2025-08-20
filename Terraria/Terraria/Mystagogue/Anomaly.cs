using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Mystagogue.Commands;

namespace Terraria.Mystagogue;
internal static class Anomaly
{
	internal static void Reset()
	{
		foreach (Command command in Command.Library.Values) {
			command.ResetVariables();
		}
	}

	internal static void PreUpdateDeadHook()
	{
		foreach (Command command in Command.Library.Values) {
			command.PreUpdateDeadHook();
		}
	}

	internal static void PreUpdateBuffsHook()
	{
		foreach (Command command in Command.Library.Values) {
			command.PreUpdateBuffsHook();
		}
	}

	internal static void PostResetEffectsHook()
	{
		foreach (Command command in Command.Library.Values) {
			command.PostResetEffectsHook();
		}
	}

	internal static void Output(string message, bool isError = false)
	{
		if (!isError)
			Main.NewText(
				$"Mystagogue: {message}",
				186, 140, 255);
		else
			Main.NewText(
				$"Mystagogue: {message}",
				123, 35, 255);
	}

	internal static bool InterpretInput(string raw, bool clear = true)
	{
		if (!raw.StartsWith(";") || !(raw.Length > 1))
			return false;

		// Turn the input into a list of non-empty arguments
		List<string> args = ParseArguments(raw.Substring(1, raw.Length - 1));
		string commandCalled = args[0].ToLower();
		args.RemoveAt(0);

		// Execute the command if it exists
		if (Command.Library.TryGetValue(commandCalled, out Command command)) {
			try {
				command.Execute(args);
			}
			catch (Exception e) {
				Output($"Exception when running [{command.Name}]: {e.Message}", true);
			}
			if (clear) {
				Main.chatText = "";
			}
			return true;
		}

		return false;
	}

	// This handles arguments in a way that allows for arguments surrounded by quotes to be treated as single arguments.
	private static List<string> ParseArguments(string input)
	{
		List<string> argsSeparatedOnQuotes = [.. input.Split('\"')];
		List<string> argsSeparatedIntermittentlyOnSpaces = [];

		// Every even index is part of the string that was outside the quotes.
		for (int i = 0; i < argsSeparatedOnQuotes.Count; i++) {
			if (i % 2 == 1)
				argsSeparatedIntermittentlyOnSpaces.Add(argsSeparatedOnQuotes[i]);
			else
				argsSeparatedIntermittentlyOnSpaces.AddRange(
					[.. argsSeparatedOnQuotes[i].Split([' '], StringSplitOptions.RemoveEmptyEntries)]
				);
		}

		return [.. from string arg in argsSeparatedIntermittentlyOnSpaces where !string.IsNullOrWhiteSpace(arg) select arg.Trim()];
	}

	internal static void AttemptMapTeleport()
	{
		if (PlayerInput.Triggers.JustPressed.MouseRight) {
			float mapX = Main.mapFullscreenPos.X + (Main.mouseX - Main.screenWidth / 2) / (Main.mapFullscreenScale * (1f / 0.9f));
			float mapY = Main.mapFullscreenPos.Y + (Main.mouseY - Main.screenHeight / 2) / (Main.mapFullscreenScale * (1f / 0.9f));

			float worldX = mapX * 16f - (Main.player[Main.myPlayer].width / 2);
			float worldY = mapY * 16f - (Main.player[Main.myPlayer].height / 2);

			float clampedX = Math.Min(Math.Max(worldX, 0f), Main.maxTilesX * 16f - 64f);
			float clampedY = Math.Min(Math.Max(worldY, 0f), Main.maxTilesY * 16f - 64f);

			Main.player[Main.myPlayer].Teleport(new Vector2(clampedX, clampedY), TeleportationStyleID.TeleporterTile);
		}
	}

	internal static bool TryItemDuplication(Item slot)
	{
		if (!Main.mouseRightRelease)
			return false;

		bool result = false;
		ref Item mouseItem = ref Main.mouseItem;

		if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt) && Main.mouseRight && (mouseItem.IsTheSameAs(slot) | mouseItem.IsAir)) {
			mouseItem = new Item();
			mouseItem = slot.Clone();
			mouseItem.favorited = false;
			mouseItem.stack = mouseItem.maxStack;
			result = true;
		}
		else if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl) && !slot.IsAir) {
			slot.stack = slot.maxStack;
			result = true;
		}
		return result;
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

	internal static string ReplaceEveryXthOccurrence(this string input, string toReplace, string replacement, uint interval = 2)
	{
		if (interval < 2)
			throw new ArgumentOutOfRangeException(nameof(interval), "Interval must be 2 or greater.");
		var regex = new Regex(Regex.Escape(toReplace));
		int count = 0;
		return regex.Replace(input, m => ++count % interval == 0 ? replacement : m.Value);
	}
}
