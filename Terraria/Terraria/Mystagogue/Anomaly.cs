using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Terraria.GameInput;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Mystagogue.Commands;

namespace Terraria.Mystagogue;
internal static class Anomaly
{
	internal static Dictionary<string, Func<Item, bool>> ItemTags = new() {
		{ "block", (input) => input.createTile >= 0 && !Main.tileFrameImportant[input.createTile] },
		{ "wall", (input) => input.createWall >= 0 },
		{ "object", (input) => input.createTile >= 0 && Main.tileFrameImportant[input.createTile] },
		{ "tool", (input) => input.pick != 0 || input.axe != 0 || input.hammer != 0 },
		{ "armor", (input) => !input.vanity && (input.headSlot > -1 || input.bodySlot > -1 || input.legSlot > -1) },
		{ "accessory", (input) => input.accessory && !input.vanity && input.createTile == -1 },
		{ "vanity", (input) => input.vanity && (input.headSlot > -1 || input.bodySlot > -1 || input.legSlot > -1) },
		{ "vanityaccessory", (input) => input.vanity && input.headSlot == -1 && input.bodySlot == -1 && input.legSlot == -1 },
		{ "dye", (input) => GameShaders.Armor.GetShaderIdFromItemId(input.type) > 0 || GameShaders.Hair.GetShaderIdFromItemId(input.type) >= 0 },
		{ "weapon", (input) => input.damage != -1 && input.pick == 0 && input.axe == 0 && input.hammer == 0 },
		{ "melee", (input) => input.melee && input.pick == 0 && input.axe == 0 && input.hammer == 0 },
		{ "ranged", (input) => input.ranged && input.ammo == AmmoID.None },
		{ "magic", (input) => input.magic },
		{ "summon", (input) => input.summon },
		{ "potion", (input) => (input.buffType != 0 || input.potion || input.healMana > 0) && !Main.lightPet[input.buffType] && !Main.vanityPet[input.buffType] && !input.summon && input.mountType == -1 },
		{ "consumable", (input) => input.consumable && input.createTile == -1 && input.createWall == -1 },
		{ "ammo", (input) => input.ammo != AmmoID.None },
		{ "mount", (input) => input.mountType != -1 && !MountID.Sets.Cart[input.mountType] },
		{ "minecart", (input) => input.cartTrack || (input.mountType != -1 && MountID.Sets.Cart[input.mountType]) },
		{ "material", (input) => input.material && input.createTile == -1 && input.createWall == -1 && !input.accessory && input.damage == -1 },
		{ "quest", (input) => input.questItem },
		{ "fishing", (input) => input.fishingPole >= 1 || input.bait >= 1 },
		{ "expert", (input) => input.expert },
		{ "deprecated", (input) => ItemID.Sets.Deprecated[input.type] }
	};

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
		if (!raw.EndsWith(";") || !(raw.Length > 1))
			return false;

		// Turn the possible command input into a list of non-empty arguments
		List<string> args = ParseArguments(raw.Substring(0, raw.Length - 1));
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
		// Starting set
		List<string> rawArgs = [.. input.Split([' '], StringSplitOptions.RemoveEmptyEntries)];
		// Ending Set
		List<string> results = [];
		int i = 0;

		while (i < rawArgs.Count) {
			string workingArg = rawArgs[i];
			bool startsWithQuote = workingArg.Length > 0 && (workingArg[0] == '"' || workingArg[0] == '\'');

			if (!startsWithQuote) {
				// Regular argument without quotes
				results.Add(workingArg);
				i++;
			}
			else {
				char quoteChar = workingArg[0];
				if (workingArg.Length > 1 && workingArg[workingArg.Length - 1] == quoteChar) {
					// This was just a quoted string!
					results.Add(workingArg.Substring(1, workingArg.Length - 2));
					i++;
				}
				else {
					// Begin concatenating arguments
					StringBuilder combinedArg = new StringBuilder(workingArg.Substring(1));
					i++;
					bool foundClosingQuote = false;

					while (i < rawArgs.Count && !foundClosingQuote) {
						string nextArg = rawArgs[i];
						combinedArg.Append(" ").Append(nextArg);
						if (nextArg.Length > 0 && nextArg[nextArg.Length - 1] == quoteChar) {
							combinedArg.Length--;
							foundClosingQuote = true;
						}
						i++;
					}

					if (!foundClosingQuote)
						// We reached the end, give up on whatever was half closed in quotes
						return results;

					// New argument is ready. If we haven't gone through them all, go back to the start of the loop
					results.Add(combinedArg.ToString());
				}
			}
		}

		return [.. results.Where(arg => !string.IsNullOrWhiteSpace(arg))];
	}

	internal static void CatchWildHelpCommand()
	{
		if (Main.chatText == "/help" || Main.chatText == "/?") {
			Output("Type \"help;\" into chat to get Mystagogue's help message.", false);
		}
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

			Main.player[Main.myPlayer].Teleport(new Vector2(clampedX, clampedY), TeleportationStyleID.DebugTeleport);
		}
	}

	internal static bool TryRightClickDupe(Item slot)
	{
		bool result = false;
		if (Main.cursorOverride == 3) {
			if (Main.mouseRight && Main.mouseRightRelease && (Main.mouseItem.IsTheSameAs(slot) | Main.mouseItem.IsAir)) {
				Main.mouseItem = new Item();
				Main.mouseItem = slot.Clone();
				Main.mouseItem.favorited = false;
				Main.mouseItem.stack = Main.mouseItem.maxStack;
				result = true;
			}
		}
		return result;
	}
}
