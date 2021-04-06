using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.IO;

namespace Terraria
{
	public class MystagogueCMD
	{
		public MystagogueCMD(string Command_Name, string Command_Description, Action Delegate_Function)
		{
			this.name = Command_Name;
			this.desc = Command_Description;
			this.func = Delegate_Function;
			MystagogueCMD.library.Add(this.name, this);
		}

		static MystagogueCMD()
		{
			MystagogueCMD.tags = new Dictionary<string, Func<Item, bool>>();
			MystagogueCMD.tags.Add("block", (Item input) => input.createTile >= 0 && !Main.tileFrameImportant[input.createTile]);
			MystagogueCMD.tags.Add("wall", (Item input) => input.createWall >= 0);
			MystagogueCMD.tags.Add("object", (Item input) => input.createTile >= 0 && Main.tileFrameImportant[input.createTile]);
			MystagogueCMD.tags.Add("tool", (Item input) => input.pick != 0 || input.axe != 0 || input.hammer != 0);
			MystagogueCMD.tags.Add("armor", (Item input) => !input.vanity && (input.headSlot > -1 || input.bodySlot > -1 || input.legSlot > -1));
			MystagogueCMD.tags.Add("acc", (Item input) => input.accessory && !input.vanity && input.createTile == -1);
			MystagogueCMD.tags.Add("vanityarmor", (Item input) => input.vanity && (input.headSlot > -1 || input.bodySlot > -1 || input.legSlot > -1));
			MystagogueCMD.tags.Add("vanityacc", (Item input) => input.vanity && input.headSlot == -1 && input.bodySlot == -1 && input.legSlot == -1);
			MystagogueCMD.tags.Add("dye", (Item input) => GameShaders.Armor.GetShaderIdFromItemId(input.type) > 0 || GameShaders.Hair.GetShaderIdFromItemId(input.type) >= 0);
			MystagogueCMD.tags.Add("damage", (Item input) => input.damage != -1 && input.pick == 0 && input.axe == 0 && input.hammer == 0);
			MystagogueCMD.tags.Add("melee", (Item input) => input.melee && input.pick == 0 && input.axe == 0 && input.hammer == 0);
			MystagogueCMD.tags.Add("ranged", (Item input) => input.ranged && input.ammo == AmmoID.None);
			MystagogueCMD.tags.Add("magic", (Item input) => input.magic);
			MystagogueCMD.tags.Add("summon", (Item input) => input.summon);
			MystagogueCMD.tags.Add("potion", (Item input) => (input.buffType != 0 || input.potion || input.healMana > 0) && !Main.lightPet[input.buffType] && !Main.vanityPet[input.buffType] && !input.summon && input.mountType == -1);
			MystagogueCMD.tags.Add("consumable", (Item input) => input.consumable && input.createTile == -1 && input.createWall == -1);
			MystagogueCMD.tags.Add("ammo", (Item input) => input.ammo != AmmoID.None);
			MystagogueCMD.tags.Add("mount", (Item input) => input.mountType != -1 && !MountID.Sets.Cart[input.mountType]);
			MystagogueCMD.tags.Add("minecart", (Item input) => input.cartTrack || (input.mountType != -1 && MountID.Sets.Cart[input.mountType]));
			MystagogueCMD.tags.Add("material", (Item input) => input.material && input.createTile == -1 && input.createWall == -1 && !input.accessory && input.damage == -1);
			MystagogueCMD.tags.Add("quest", (Item input) => input.questItem);
			MystagogueCMD.tags.Add("fishing", (Item input) => input.fishingPole >= 1 || input.bait >= 1);
			MystagogueCMD.tags.Add("expert", (Item input) => input.expert);
			MystagogueCMD.tags.Add("depreciated", (Item input) => ItemID.Sets.Deprecated[input.type]);
			new MystagogueCMD("help", "(Name of command when used to read about a command), Gives helpful information, a list of commands, and more. Put in a command as the first argument to read about that command.", delegate()
			{
				if (Mystagogue.CommandArgs.Count > 1 && MystagogueCMD.library.ContainsKey(Mystagogue.CommandArgs[1]))
				{
					Mystagogue.Output("You chose to read about [" + Mystagogue.CommandArgs[1] + "].\n>> " + MystagogueCMD.library[Mystagogue.CommandArgs[1]].desc, false);
					return;
				}
				Mystagogue.Output(string.Concat(new object[]
				{
					"Thank you for using Mystagogue by MarauderKnight3!\nCommands are specified by two trailing semicolons (;;) at the end of an input while a valid command is already typed out. When this is detected, the command will be run and the chat input will be cleared, but not closed. You do not need to press enter. Read about a command's function by executing [help] <query>.\nThere are ",
					MystagogueCMD.library.Count,
					" commands loaded.\nList of commands: [",
					string.Join("], [", new List<string>(MystagogueCMD.library.Keys)),
					"]"
				}), false);
			});
			new MystagogueCMD("?", MystagogueCMD.library["help"].desc, MystagogueCMD.library["help"].func);
			new MystagogueCMD("i", "(Name-Concatenated/ID, Stack, Prefix) Spawns an item by converting your currently held cursor item (or thin air) to it.", delegate()
			{
				if (Mystagogue.CommandArgs.Count == 1)
				{
					Mystagogue.Output("That command requires arguments", false);
					return;
				}
				int i = 0;
				List<string> list = new List<string>();
				if (!new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
				{
					string text = Mystagogue.CommandArgs[1];
					while (text.StartsWith("0"))
					{
						text = text.Remove(0, 1);
					}
					while (text != i.ToString())
					{
						if (i == 5087)
						{
							Mystagogue.Output("Given item ID does not correspond to an item", false);
							return;
						}
						i++;
					}
					for (int j = 0; j < Mystagogue.CommandArgs.Count - 2; j++)
					{
						list.Add(Mystagogue.CommandArgs[2 + j]);
					}
				}
				else
				{
					i = 1;
					List<int> list2 = new List<int>();
					while (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[i]))
					{
						i++;
						if (i == Mystagogue.CommandArgs.Count)
						{
							break;
						}
					}
					string text2 = string.Join(" ", Mystagogue.CommandArgs.GetRange(1, i - 1)).ToUpper();
					int num = i;
					while (Mystagogue.CommandArgs.Count - num != 0)
					{
						list.Add(Mystagogue.CommandArgs[num]);
						num++;
					}
					for (i = 0; i < 5088; i++)
					{
						if (Lang.GetItemNameValue(i).ToUpper().StartsWith(text2))
						{
							list2.Add(i);
						}
					}
					if (list2.Count == 0)
					{
						Mystagogue.Output("No item names match", false);
						return;
					}
					if (list2.Count > 1)
					{
						List<string> list3 = new List<string>();
						foreach (int id in list2)
						{
							list3.Add(Lang.GetItemNameValue(id).ToUpper());
						}
						List<string> list4 = new List<string>();
						foreach (int num2 in list2)
						{
							list4.Add(string.Concat(new object[]
							{
								Lang.GetItemNameValue(num2),
								" (",
								num2,
								")"
							}));
						}
						bool flag = false;
						for (int k = 0; k < list3.Count; k++)
						{
							if (list3[k] == text2)
							{
								i = list2[k];
								list4.RemoveAt(k);
								Mystagogue.Output("Other matches include " + string.Join(", ", list4), false);
								break;
							}
							if (k + 1 == list2.Count && !flag)
							{
								Mystagogue.Output("Found " + string.Join(", ", list4), false);
								return;
							}
						}
					}
					else
					{
						i = list2[0];
					}
				}
				int num3 = i;
				int stack = 1;
				int num4 = 0;
				if (list.Count >= 1)
				{
					if (new Regex("\\D").IsMatch(list[0]))
					{
						Mystagogue.Output("Stack must be a positive integer", false);
						return;
					}
					string text3 = list[0];
					while (text3.StartsWith("0"))
					{
						text3 = text3.Remove(0, 1);
					}
					if (text3.Length > 10)
					{
						stack = int.MaxValue;
					}
					else if (Convert.ToInt64(text3) > 2147483647L)
					{
						stack = int.MaxValue;
					}
					else if (text3.Length > 0)
					{
						stack = int.Parse(text3);
					}
				}
				if (list.Count >= 2)
				{
					if (!new Regex("\\D").IsMatch(list[1]))
					{
						string text4 = list[1];
						while (text4.StartsWith("0"))
						{
							text4 = text4.Remove(0, 1);
						}
						i = 0;
						while (text4 != i.ToString())
						{
							if (i == 83)
							{
								Mystagogue.Output("Given prefix ID does not correspond to a prefix", false);
								return;
							}
							i++;
						}
					}
					else
					{
						string value = list[1].ToUpper();
						List<int> list5 = new List<int>();
						for (i = 0; i < MystagogueCMD.Prefixes.Length; i++)
						{
							if (MystagogueCMD.Prefixes[i].ToUpper().StartsWith(value) && i != 75 && i != 43 && i != 76)
							{
								list5.Add(i);
							}
						}
						if (list5.Count == 0)
						{
							Mystagogue.Output("No prefix names match", false);
							return;
						}
						if (list5.Count > 1)
						{
							List<string> list6 = new List<string>();
							foreach (int num5 in list5)
							{
								list6.Add(string.Concat(new object[]
								{
									MystagogueCMD.Prefixes[num5],
									" (",
									num5,
									")"
								}));
							}
							Mystagogue.Output("Found " + string.Join(", ", list6), false);
							return;
						}
						i = list5[0];
					}
					if (i == 18 || i == 75)
					{
						if (!ContentSamples.ItemsByType[num3].accessory)
						{
							i = 18;
						}
						else
						{
							i = 75;
						}
					}
					if (i == 20 || i == 43)
					{
						if (ContentSamples.ItemsByType[num3].ranged)
						{
							i = 20;
						}
						else
						{
							i = 43;
						}
					}
					if (i == 42 || i == 76)
					{
						if (!ContentSamples.ItemsByType[num3].accessory)
						{
							i = 42;
						}
						else
						{
							i = 76;
						}
					}
					num4 = i;
				}
				Main.mouseItem.SetDefaults(num3);
				Main.mouseItem.stack = stack;
				Main.mouseItem.prefix = (byte)num4;
				Main.mouseItem.Refresh();
				string text5 = "";
				if (Main.mouseItem.prefix > 0)
				{
					text5 = " " + MystagogueCMD.Prefixes[(int)Main.mouseItem.prefix];
				}
				Mystagogue.Output(string.Concat(new object[]
				{
					"Set cursor item to ",
					Main.mouseItem.stack,
					text5,
					" ",
					Lang.GetItemNameValue(Main.mouseItem.type),
					" (",
					Main.mouseItem.type,
					")"
				}), false);
			});
			new MystagogueCMD("search", "(Name-Concatenated) Returns all items with a name containing the concatenated arguments.", delegate()
			{
				if (Mystagogue.CommandArgs.Count == 1)
				{
					Mystagogue.Output("That command requires arguments", false);
					return;
				}
				List<int> list = new List<int>();
				string value = string.Join(" ", Mystagogue.CommandArgs.GetRange(1, Mystagogue.CommandArgs.Count - 1)).ToUpper();
				for (int i = 0; i < 5088; i++)
				{
					if (Lang.GetItemNameValue(i).ToUpper().Contains(value))
					{
						list.Add(i);
					}
				}
				if (list.Count == 0)
				{
					Mystagogue.Output("No item names match", false);
					return;
				}
				List<string> list2 = new List<string>();
				foreach (int num in list)
				{
					list2.Add(string.Concat(new object[]
					{
						Lang.GetItemNameValue(num),
						" (",
						num,
						")"
					}));
				}
				Mystagogue.Output("Found " + string.Join(", ", list2), false);
			});
			new MystagogueCMD("sl", "(Query or '!', followed with a comma ',' then tags and page number specification) Hosts a spawnlist of items in the recipe section, all crafted out of thin air. Seperate more specifications via a comma e.g. \"sl fractal,depreciated\" or \"sl chloro , melee\" or \"sl !, damage\". Using the \"!\" search term is name unspecific and will not sort items by name. The first comma can be placed anywhere. Unrecognized tags will be ignored. \"page\" will unconventionally scan the next tag as a page number, do not call it without one, as it will incorrectly interpret another unrelated tag or cause no effect at all (if it is the last tag). While the spawnlist command is in effect, other recipes will not appear. Items crafted with spawnlist recipes will not get prefixes. Execute the command with no specifications to turn the spawnlist off.", delegate()
			{
				if (Mystagogue.FirstFreedRecipeSlot == 0)
				{
					int num = Recipe.maxRecipes - 1;
					while (Main.recipe[num].createItem.type == 0)
					{
						Mystagogue.FirstFreedRecipeSlot = num;
						num--;
					}
				}
				if (Mystagogue.CommandArgs.Count == 1)
				{
					Mystagogue.Output("Resetting the spawnlist", false);
					for (int i = Mystagogue.FirstFreedRecipeSlot; i < Recipe.maxRecipes; i++)
					{
						Main.recipe[i] = new Recipe();
					}
					Recipe.numRecipes = Mystagogue.FirstFreedRecipeSlot;
					Recipe.FindRecipes(false);
					return;
				}
				int num2 = Recipe.maxRecipes - Mystagogue.FirstFreedRecipeSlot;
				List<int> list = new List<int>();
				List<string> list10 = Mystagogue.CommandArgs.GetRange(1, Mystagogue.CommandArgs.Count - 1);
				int num3 = 0;
				string text = string.Join(" ", list10).Trim();
				if (text.Contains(","))
				{
					list10 = (from arg in text.Split(new char[]
					{
						','
					}, 2)[1].Replace(',', ' ').Split(new char[]
					{
						' '
					})
					where !string.IsNullOrWhiteSpace(arg)
					select arg).ToList<string>();
				}
				else
				{
					list10 = new List<string>();
				}
				text = text.Substring(0, text.Contains(",") ? text.IndexOf(",") : text.Length).Trim();
				for (int j = 1; j < 5088; j++)
				{
					if (Lang.GetItemNameValue(j).ToUpper().Contains(text.ToUpper()) || text == "!")
					{
						list.Add(j);
					}
				}
				if (list10.Count > 0)
				{
					bool flag = false;
					for (int k = 0; k < list10.Count; k++)
					{
						if (flag)
						{
							flag = false;
							if (new Regex("\\D").IsMatch(list10[k]))
							{
								Mystagogue.Output("Page must be a positive integer", false);
							}
							else
							{
								string text2 = list10[k];
								while (text2.StartsWith("0"))
								{
									text2 = text2.Remove(0, 1);
								}
								if (text2.Length > 2)
								{
									num3 = 99;
								}
								else if (text2.Length > 0)
								{
									num3 = int.Parse(text2);
								}
								if (num3 > 0)
								{
									num3--;
								}
							}
							list10.RemoveAt(k);
							k--;
						}
						else if (list10[k] == "page")
						{
							flag = true;
							list10.RemoveAt(k);
							k--;
						}
					}
					bool usedCat = false;
					Func<int, List<string>, bool> Chosen = delegate(int input, List<string> args)
					{
						Item item = new Item();
						item.SetDefaults(input);
						foreach (string key in args)
						{
							if (MystagogueCMD.tags.ContainsKey(key))
							{
								usedCat = true;
							}
							if (MystagogueCMD.tags.ContainsKey(key) && MystagogueCMD.tags[key](item))
							{
								return true;
							}
						}
						return false;
					};
					if ((from id in list
					where Chosen(id, list10)
					select id).ToList<int>().Count > 0)
					{
						list = (from id in list
						where Chosen(id, list10)
						select id).ToList<int>();
					}
					if (!usedCat)
					{
						Mystagogue.Output("Categories: " + string.Join(", ", new List<string>(MystagogueCMD.tags.Keys)), false);
					}
				}
				int count = list.Count;
				num3 = ((num3 > (int)Math.Floor((double)list.Count / (double)num2)) ? ((int)Math.Floor((double)list.Count / (double)num2)) : num3);
				list = list.GetRange(num3 * num2, (list.Count - num3 * num2 < num2) ? (list.Count - num3 * num2) : num2);
				if (list.Count == 0)
				{
					Mystagogue.Output("There was no matching items", false);
					return;
				}
				for (int l = Mystagogue.FirstFreedRecipeSlot; l < Recipe.maxRecipes; l++)
				{
					Main.recipe[l] = new Recipe();
				}
				Recipe.numRecipes = Mystagogue.FirstFreedRecipeSlot;
				foreach (int defaults in list)
				{
					Recipe recipe = new Recipe();
					recipe.createItem.SetDefaults(defaults);
					recipe.createItem.stack = 1;
					Main.recipe[Recipe.numRecipes] = recipe;
					Recipe.numRecipes++;
				}
				Recipe.FindRecipes(false);
				string text3 = "";
				string text4 = "";
				if (text == "!")
				{
					text3 = "items ";
				}
				else
				{
					text4 = " results for '" + text + "'";
				}
				Mystagogue.Output(string.Concat(new object[]
				{
					"Listing ",
					text3,
					num3 * num2 + 1,
					"-",
					num3 * num2 + ((count - num3 * num2 < num2) ? (count - num3 * num2) : num2),
					" of ",
					count,
					text4,
					(count > num2) ? string.Concat(new string[]
					{
						" (Page ",
						(num3 + 1).ToString(),
						" of ",
						((int)Math.Floor((double)count / (double)num2) + 1).ToString(),
						")"
					}) : ""
				}), false);
			});
			new MystagogueCMD("reforge", "(Prefix Name or ID) Will reforge the item held with the cursor. \"Basic\" or no parameter will unforge it.", delegate()
			{
				if (Main.mouseItem.IsAir)
				{
					Mystagogue.Output("Must be holding an item with the cursor", false);
					return;
				}
				if (Mystagogue.CommandArgs.Count == 1)
				{
					Main.mouseItem.prefix = 0;
					Main.mouseItem.Refresh();
					Mystagogue.Output(string.Concat(new object[]
					{
						"Reforged item ",
						Main.mouseItem.stack,
						" ",
						Lang.GetItemNameValue(Main.mouseItem.type),
						" (",
						Main.mouseItem.type,
						")"
					}), false);
					return;
				}
				int i = 0;
				if (!new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
				{
					string text = Mystagogue.CommandArgs[1];
					while (text.StartsWith("0"))
					{
						text = text.Remove(0, 1);
					}
					while (text != i.ToString())
					{
						if (i == 83)
						{
							Mystagogue.Output("Given prefix ID does not correspond to a prefix", false);
							return;
						}
						i++;
					}
				}
				else
				{
					string value = Mystagogue.CommandArgs[1].ToUpper();
					List<int> list = new List<int>();
					for (i = 0; i < MystagogueCMD.Prefixes.Length; i++)
					{
						if (MystagogueCMD.Prefixes[i].ToUpper().StartsWith(value) && i != 75 && i != 43 && i != 76)
						{
							list.Add(i);
						}
					}
					if (list.Count == 0)
					{
						Mystagogue.Output("No prefix names match", false);
						return;
					}
					if (list.Count > 1)
					{
						List<string> list2 = new List<string>();
						foreach (int num in list)
						{
							list2.Add(string.Concat(new object[]
							{
								MystagogueCMD.Prefixes[num],
								" (",
								num,
								")"
							}));
						}
						Mystagogue.Output("Found " + string.Join(", ", list2), false);
						return;
					}
					i = list[0];
				}
				if (i == 18 || i == 75)
				{
					if (!ContentSamples.ItemsByType[Main.mouseItem.type].accessory)
					{
						i = 18;
					}
					else
					{
						i = 75;
					}
				}
				if (i == 20 || i == 43)
				{
					if (ContentSamples.ItemsByType[Main.mouseItem.type].ranged)
					{
						i = 20;
					}
					else
					{
						i = 43;
					}
				}
				if (i == 42 || i == 76)
				{
					if (!ContentSamples.ItemsByType[Main.mouseItem.type].accessory)
					{
						i = 42;
					}
					else
					{
						i = 76;
					}
				}
				Main.mouseItem.prefix = (byte)i;
				Main.mouseItem.Refresh();
				string text2 = "";
				if (Main.mouseItem.prefix > 0)
				{
					text2 = " " + MystagogueCMD.Prefixes[(int)Main.mouseItem.prefix];
				}
				Mystagogue.Output(string.Concat(new object[]
				{
					"Reforged item ",
					Main.mouseItem.stack,
					text2,
					" ",
					Lang.GetItemNameValue(Main.mouseItem.type),
					" (",
					Main.mouseItem.type,
					")"
				}), false);
			});
			new MystagogueCMD("rename", "(New Name) Renames the item held with the cursor. This does not change kill messages, it only changes the display name.", delegate()
			{
				if (Main.mouseItem.IsAir)
				{
					Mystagogue.Output("Must be holding an item with the cursor", false);
					return;
				}
				if (Mystagogue.CommandArgs.Count == 1)
				{
					Main.mouseItem.ClearNameOverride();
					Mystagogue.Output("Item name reset", false);
					return;
				}
				Main.mouseItem.SetNameOverride(string.Join(" ", Mystagogue.CommandArgs.GetRange(1, Mystagogue.CommandArgs.Count - 1)));
				Mystagogue.Output("Item renamed", false);
			});
			new MystagogueCMD("ut", "(Use Time) Set the use time (or the rate of special function execution, such as the speed that projectiles are created when shooting a bow or swinging the Meowmere) of the item selected in the hotbar. Some items may not work at certain speeds. Lower values are faster.", delegate()
			{
				if (Mystagogue.CommandArgs.Count != 1 && !Main.player[Main.myPlayer].HeldItem.IsAir)
				{
					if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
					{
						Mystagogue.Output("Must be a positive integer", false);
						return;
					}
					string text = Mystagogue.CommandArgs[1];
					while (text.StartsWith("0"))
					{
						text = text.Remove(0, 1);
					}
					int useTime;
					if (text.Length != 0)
					{
						if (text.Length > 2)
						{
							useTime = 99;
						}
						else
						{
							useTime = int.Parse(text);
						}
					}
					else
					{
						useTime = 0;
					}
					Main.player[Main.myPlayer].HeldItem.useTime = useTime;
					Mystagogue.Output("Use time " + Main.player[Main.myPlayer].HeldItem.useTime, false);
					return;
				}
				else
				{
					if (!Main.player[Main.myPlayer].HeldItem.IsAir)
					{
						Item item = new Item();
						item.SetDefaults(Main.player[Main.myPlayer].HeldItem.type);
						item.Prefix((int)Main.player[Main.myPlayer].HeldItem.prefix);
						item.Refresh();
						if (item.useTime != Main.player[Main.myPlayer].HeldItem.useTime)
						{
							Main.player[Main.myPlayer].HeldItem.useTime = item.useTime;
						}
						else
						{
							Main.player[Main.myPlayer].HeldItem.useTime = 0;
							if (Main.player[Main.myPlayer].HeldItem.type == 4956)
							{
								Main.player[Main.myPlayer].HeldItem.useTime = 1;
							}
						}
						Mystagogue.Output("Use time " + Main.player[Main.myPlayer].HeldItem.useTime, false);
						return;
					}
					Mystagogue.Output("Must be holding an item in the hotbar", false);
					return;
				}
			});
			new MystagogueCMD("at", "(Animation Time) Set the animation time (or the rate at which item graphics visually do a full cycle on screen, like the rate of sword swings) of the item selected in the hotbar. Some items may not work at certain speeds. Lower values are faster.", delegate()
			{
				if (Mystagogue.CommandArgs.Count != 1 && !Main.player[Main.myPlayer].HeldItem.IsAir)
				{
					if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
					{
						Mystagogue.Output("Must be a positive integer", false);
						return;
					}
					string text = Mystagogue.CommandArgs[1];
					while (text.StartsWith("0"))
					{
						text = text.Remove(0, 1);
					}
					int num;
					if (text.Length != 0)
					{
						if (text.Length > 2)
						{
							num = 99;
						}
						else
						{
							num = int.Parse(text);
							if (num < 3)
							{
								num = 3;
							}
						}
					}
					else
					{
						num = 3;
					}
					Main.player[Main.myPlayer].HeldItem.useAnimation = num;
					Mystagogue.Output("Animation time " + Main.player[Main.myPlayer].HeldItem.useAnimation, false);
					return;
				}
				else
				{
					if (!Main.player[Main.myPlayer].HeldItem.IsAir)
					{
						Item item = new Item();
						item.SetDefaults(Main.player[Main.myPlayer].HeldItem.type);
						item.Prefix((int)Main.player[Main.myPlayer].HeldItem.prefix);
						item.Refresh();
						if (item.useTime != Main.player[Main.myPlayer].HeldItem.useTime)
						{
							Main.player[Main.myPlayer].HeldItem.useTime = item.useTime;
						}
						else
						{
							Main.player[Main.myPlayer].HeldItem.useAnimation = 3;
						}
						Mystagogue.Output("Animation time " + Main.player[Main.myPlayer].HeldItem.useAnimation, false);
						return;
					}
					Mystagogue.Output("Must be holding an item in the hotbar", false);
					return;
				}
			});
			new MystagogueCMD("shoot", "(Projectile Type) Set the type of projectile (of a weapon that should shoot a projectile) that is shot by item selected in the hotbar. Some items may not work with certain projectile types, or work with projectiles at all. Higher values generally come from later updates of the game.", delegate()
			{
				if (Mystagogue.CommandArgs.Count == 1)
				{
					Main.player[Main.myPlayer].HeldItem.shoot = ContentSamples.ItemsByType[Main.player[Main.myPlayer].HeldItem.type].shoot;
					Main.player[Main.myPlayer].HeldItem.useAmmo = ContentSamples.ItemsByType[Main.player[Main.myPlayer].HeldItem.type].useAmmo;
					Mystagogue.Output("Item now shoots its original projectile", false);
					return;
				}
				if (Main.player[Main.myPlayer].HeldItem.IsAir)
				{
					Mystagogue.Output("Must be holding an item in the hotbar", false);
					return;
				}
				if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
				{
					Mystagogue.Output("Must be a positive integer", false);
					return;
				}
				string text = Mystagogue.CommandArgs[1];
				while (text.StartsWith("0"))
				{
					text = text.Remove(0, 1);
				}
				int num = 0;
				while (text != num.ToString())
				{
					if (num == 949)
					{
						Mystagogue.Output("Given projectile ID does not correspond to a projectile", false);
						return;
					}
					num++;
				}
				Main.player[Main.myPlayer].HeldItem.shoot = num;
				Main.player[Main.myPlayer].HeldItem.useAmmo = AmmoID.None;
				Mystagogue.Output(string.Concat(new object[]
				{
					"Item now shoots ",
					Lang.GetProjectileName(Main.player[Main.myPlayer].HeldItem.shoot),
					" (",
					Main.player[Main.myPlayer].HeldItem.shoot,
					") if applicable"
				}), false);
			});
			new MystagogueCMD("auto", "(No arguments) Set whether the item selected in the hotbar will use itself again when the mouse is held.", delegate()
			{
				if (!Main.player[Main.myPlayer].HeldItem.IsAir)
				{
					if (Mystagogue.CommandArgs.Count > 1)
					{
						if (Mystagogue.CommandArgs[1] == "off")
						{
							Main.player[Main.myPlayer].HeldItem.autoReuse = false;
							Mystagogue.Output("Item won't autoswing", false);
						}
						if (Mystagogue.CommandArgs[1] == "on")
						{
							Main.player[Main.myPlayer].HeldItem.autoReuse = true;
							Mystagogue.Output("Item will autoswing", false);
							return;
						}
					}
					else
					{
						Main.player[Main.myPlayer].HeldItem.autoReuse = !Main.player[Main.myPlayer].HeldItem.autoReuse;
						if (Main.player[Main.myPlayer].HeldItem.autoReuse)
						{
							Mystagogue.Output("Item will autoswing", false);
							return;
						}
						Mystagogue.Output("Item won't autoswing", false);
						return;
					}
				}
				else
				{
					Mystagogue.Output("Must be holding an item in the hotbar", false);
				}
			});
			new MystagogueCMD("scale", "(No arguments) Set whether the item selected in the hotbar will be used perpetually should the mouse be held, by toggle.", delegate()
			{
				if (Mystagogue.CommandArgs.Count != 1 && !Main.player[Main.myPlayer].HeldItem.IsAir)
				{
					if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
					{
						Mystagogue.Output("Must be a positive integer", false);
						return;
					}
					string text = Mystagogue.CommandArgs[1];
					while (text.StartsWith("0"))
					{
						text = text.Remove(0, 1);
					}
					int num;
					if (text.Length != 0)
					{
						if (text.Length > 5)
						{
							num = 10000;
						}
						else
						{
							num = int.Parse(text);
							if (num > 10000)
							{
								num = 10000;
							}
						}
					}
					else
					{
						num = 1;
					}
					Main.player[Main.myPlayer].HeldItem.scale = (float)(num / 100);
					Mystagogue.Output("Scale " + Main.player[Main.myPlayer].HeldItem.scale * 100f + "%", false);
					return;
				}
				else
				{
					if (!Main.player[Main.myPlayer].HeldItem.IsAir)
					{
						Item item = new Item();
						item.SetDefaults(Main.player[Main.myPlayer].HeldItem.type);
						item.prefix = Main.player[Main.myPlayer].HeldItem.prefix;
						item.Refresh();
						if (Main.player[Main.myPlayer].HeldItem.scale == item.scale)
						{
							Main.player[Main.myPlayer].HeldItem.scale = 100f;
						}
						else
						{
							Main.player[Main.myPlayer].HeldItem.scale = item.scale;
						}
						Mystagogue.Output("Scale " + Main.player[Main.myPlayer].HeldItem.scale * 100f + "%", false);
						return;
					}
					Mystagogue.Output("Must be holding an item in the hotbar", false);
					return;
				}
			});
			new MystagogueCMD("stack", "(Amount) Set the quantity of the item held by the cursor. Even swords in a stack.", delegate()
			{
				if (Mystagogue.CommandArgs.Count != 1 && !Main.mouseItem.IsAir)
				{
					if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
					{
						Mystagogue.Output("Stack must be a positive integer", false);
						return;
					}
					string text = Mystagogue.CommandArgs[1];
					int stack = Main.mouseItem.maxStack;
					while (text.StartsWith("0"))
					{
						text = text.Remove(0, 1);
					}
					if (text.Length > 10)
					{
						stack = int.MaxValue;
					}
					else if (Convert.ToInt64(text) > 2147483647L)
					{
						stack = int.MaxValue;
					}
					else if (text.Length > 0)
					{
						stack = int.Parse(text);
					}
					Main.mouseItem.stack = stack;
					Mystagogue.Output("Stack set to " + Main.mouseItem.stack, false);
					return;
				}
				else
				{
					if (!Main.mouseItem.IsAir)
					{
						Main.mouseItem.stack = Main.mouseItem.maxStack;
						Mystagogue.Output("Stack set to " + Main.mouseItem.stack, false);
						return;
					}
					Mystagogue.Output("Must be holding an item with the cursor", false);
					return;
				}
			});
			new MystagogueCMD("maxstack", "(Max Amount) Set the max amount of similar items that the stack of items the cursor is holding can accept without additional hacks to force it. Even with swords.", delegate()
			{
				if (Mystagogue.CommandArgs.Count != 1 && !Main.mouseItem.IsAir)
				{
					if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
					{
						Mystagogue.Output("Must be a positive integer", false);
						return;
					}
					string text = Mystagogue.CommandArgs[1];
					int maxStack = ContentSamples.ItemsByType[Main.mouseItem.type].maxStack;
					while (text.StartsWith("0"))
					{
						text = text.Remove(0, 1);
					}
					if (text.Length > 10)
					{
						maxStack = int.MaxValue;
					}
					else if (Convert.ToInt64(text) > 2147483647L)
					{
						maxStack = int.MaxValue;
					}
					else if (text.Length > 0)
					{
						maxStack = int.Parse(text);
					}
					Main.mouseItem.maxStack = maxStack;
					Mystagogue.Output("Max stack set to " + Main.mouseItem.maxStack, false);
					return;
				}
				else
				{
					if (!Main.mouseItem.IsAir)
					{
						Main.mouseItem.maxStack = ContentSamples.ItemsByType[Main.mouseItem.type].maxStack;
						Mystagogue.Output("Max stack set to " + Main.mouseItem.maxStack, false);
						return;
					}
					Mystagogue.Output("Must be holding an item with the cursor", false);
					return;
				}
			});
			new MystagogueCMD("dmg", "(New Damage) Set the amount of damage that any damage-dealing item selected in the hotbar will inflict on enemies.", delegate()
			{
				if (Mystagogue.CommandArgs.Count != 1 && !Main.player[Main.myPlayer].HeldItem.IsAir)
				{
					if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
					{
						Mystagogue.Output("Must be a positive integer", false);
						return;
					}
					string text = Mystagogue.CommandArgs[1];
					while (text.StartsWith("0"))
					{
						text = text.Remove(0, 1);
					}
					int damage;
					if (text.Length != 0)
					{
						if (text.Length > 7)
						{
							damage = 999999;
						}
						else
						{
							damage = int.Parse(text);
						}
					}
					else
					{
						damage = 0;
					}
					Main.player[Main.myPlayer].HeldItem.damage = damage;
					Mystagogue.Output("Item now deals " + Main.player[Main.myPlayer].HeldItem.damage + " damage", false);
					return;
				}
				else
				{
					if (!Main.player[Main.myPlayer].HeldItem.IsAir)
					{
						Item item = new Item();
						item.SetDefaults(Main.player[Main.myPlayer].HeldItem.type);
						item.prefix = Main.player[Main.myPlayer].HeldItem.prefix;
						item.Refresh();
						if (Main.player[Main.myPlayer].HeldItem.damage == item.damage)
						{
							Main.player[Main.myPlayer].HeldItem.damage = 9999999;
						}
						else
						{
							Main.player[Main.myPlayer].HeldItem.damage = item.damage;
						}
						Mystagogue.Output("Item now deals " + Main.player[Main.myPlayer].HeldItem.damage + " damage", false);
						return;
					}
					Mystagogue.Output("Must be holding an item in the hotbar", false);
					return;
				}
			});
			new MystagogueCMD("crit", "(New Critical Chance) Set the amount of critical chance that any damage-dealing item selected in the hotbar will consider when inflicting damage on enemies.", delegate()
			{
				if (Mystagogue.CommandArgs.Count != 1 && !Main.player[Main.myPlayer].HeldItem.IsAir)
				{
					if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
					{
						Mystagogue.Output("Must be a positive integer", false);
						return;
					}
					string text = Mystagogue.CommandArgs[1];
					while (text.StartsWith("0"))
					{
						text = text.Remove(0, 1);
					}
					int num;
					if (text.Length != 0)
					{
						if (text.Length > 3)
						{
							num = 100;
						}
						else
						{
							num = int.Parse(text);
							if (num > 100)
							{
								num = 100;
							}
						}
					}
					else
					{
						num = 0;
					}
					Main.player[Main.myPlayer].HeldItem.crit = num;
					Mystagogue.Output("Item now has " + Main.player[Main.myPlayer].HeldItem.crit + " critical chance", false);
					return;
				}
				else
				{
					if (!Main.player[Main.myPlayer].HeldItem.IsAir)
					{
						Item item = new Item();
						item.SetDefaults(Main.player[Main.myPlayer].HeldItem.type);
						item.prefix = Main.player[Main.myPlayer].HeldItem.prefix;
						item.Refresh();
						if (Main.player[Main.myPlayer].HeldItem.crit == item.crit)
						{
							Main.player[Main.myPlayer].HeldItem.crit = 100;
						}
						else
						{
							Main.player[Main.myPlayer].HeldItem.crit = item.crit;
						}
						Mystagogue.Output("Item now has " + Main.player[Main.myPlayer].HeldItem.crit + " critical chance", false);
						return;
					}
					Mystagogue.Output("Must be holding an item in the hotbar", false);
					return;
				}
			});
			new MystagogueCMD("veloc", "(New Projectile Velocity) Set the amount of speed that projectiles issue from the item you select in the hotbar.", delegate()
			{
				if (Mystagogue.CommandArgs.Count != 1 && !Main.player[Main.myPlayer].HeldItem.IsAir)
				{
					if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
					{
						Mystagogue.Output("Must be a positive integer", false);
						return;
					}
					string text = Mystagogue.CommandArgs[1];
					while (text.StartsWith("0"))
					{
						text = text.Remove(0, 1);
					}
					int num;
					if (text.Length != 0)
					{
						if (text.Length > 2)
						{
							num = 60;
						}
						else
						{
							num = int.Parse(text);
							if (num > 60)
							{
								num = 60;
							}
						}
					}
					else
					{
						num = 0;
					}
					Main.player[Main.myPlayer].HeldItem.shootSpeed = (float)num;
					Mystagogue.Output("Item now has a shoot velocity of " + Main.player[Main.myPlayer].HeldItem.shootSpeed, false);
					return;
				}
				else
				{
					if (!Main.player[Main.myPlayer].HeldItem.IsAir)
					{
						Main.player[Main.myPlayer].HeldItem.shootSpeed = ContentSamples.ItemsByType[Main.player[Main.myPlayer].HeldItem.type].shootSpeed;
						Mystagogue.Output("Item now has a shoot velocity of " + Main.player[Main.myPlayer].HeldItem.shootSpeed, false);
						return;
					}
					Mystagogue.Output("Must be holding an item in the hotbar", false);
					return;
				}
			});
			new MystagogueCMD("tboost", "(New Tile Boost) Set the distance in tiles that the selected item in the hotbar can reach while modifying tiles in some way.", delegate()
			{
				if (Mystagogue.CommandArgs.Count != 1 && !Main.player[Main.myPlayer].HeldItem.IsAir)
				{
					if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
					{
						Mystagogue.Output("Must be a positive integer", false);
						return;
					}
					string text = Mystagogue.CommandArgs[1];
					while (text.StartsWith("0"))
					{
						text = text.Remove(0, 1);
					}
					int num;
					if (text.Length != 0)
					{
						if (text.Length > 2)
						{
							num = 55;
						}
						else
						{
							num = int.Parse(text);
							if (num > 55)
							{
								num = 55;
							}
						}
					}
					else
					{
						num = 0;
					}
					Main.player[Main.myPlayer].HeldItem.tileBoost = num;
					Mystagogue.Output("Item's tile boost is now " + Main.player[Main.myPlayer].HeldItem.tileBoost, false);
					return;
				}
				else
				{
					if (!Main.player[Main.myPlayer].HeldItem.IsAir)
					{
						Item item = new Item();
						item.SetDefaults(Main.player[Main.myPlayer].HeldItem.type);
						if (Main.player[Main.myPlayer].HeldItem.tileBoost == item.tileBoost)
						{
							Main.player[Main.myPlayer].HeldItem.tileBoost = 5;
						}
						else
						{
							Main.player[Main.myPlayer].HeldItem.tileBoost = item.tileBoost;
						}
						Mystagogue.Output("Item's tile boost is now " + Main.player[Main.myPlayer].HeldItem.tileBoost, false);
						return;
					}
					Mystagogue.Output("Must be holding an item in the hotbar", false);
					return;
				}
			});
			new MystagogueCMD("pick", "(New pickaxe power) Changes the pickaxe power of the item selected in the hotbar.", delegate()
			{
				if (Mystagogue.CommandArgs.Count != 1 && !Main.player[Main.myPlayer].HeldItem.IsAir)
				{
					if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
					{
						Mystagogue.Output("Must be a positive integer", false);
						return;
					}
					string text = Mystagogue.CommandArgs[1];
					while (text.StartsWith("0"))
					{
						text = text.Remove(0, 1);
					}
					int num;
					if (text.Length != 0)
					{
						if (text.Length > 3)
						{
							num = 225;
						}
						else
						{
							num = int.Parse(text);
							if (num > 225)
							{
								num = 225;
							}
						}
					}
					else
					{
						num = 0;
					}
					Main.player[Main.myPlayer].HeldItem.pick = num;
					Mystagogue.Output("Pickaxe power set to " + Main.player[Main.myPlayer].HeldItem.pick, false);
					return;
				}
				else
				{
					if (!Main.player[Main.myPlayer].HeldItem.IsAir)
					{
						Item item = new Item();
						item.SetDefaults(Main.player[Main.myPlayer].HeldItem.type);
						if (Main.player[Main.myPlayer].HeldItem.pick == item.pick)
						{
							Main.player[Main.myPlayer].HeldItem.pick = 225;
						}
						else
						{
							Main.player[Main.myPlayer].HeldItem.pick = item.pick;
						}
						Mystagogue.Output("Pickaxe power set to " + Main.player[Main.myPlayer].HeldItem.pick, false);
						return;
					}
					Mystagogue.Output("Must be holding an item in the hotbar", false);
					return;
				}
			});
			new MystagogueCMD("axe", "(New axe power) Changes the axe power of the item selected in the hotbar.", delegate()
			{
				if (Mystagogue.CommandArgs.Count != 1 && !Main.player[Main.myPlayer].HeldItem.IsAir)
				{
					if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
					{
						Mystagogue.Output("Must be a positive integer", false);
						return;
					}
					string text = Mystagogue.CommandArgs[1];
					while (text.StartsWith("0"))
					{
						text = text.Remove(0, 1);
					}
					int num;
					if (text.Length != 0)
					{
						if (text.Length > 3)
						{
							num = 175;
						}
						else
						{
							num = int.Parse(text);
							if (num > 175)
							{
								num = 175;
							}
						}
					}
					else
					{
						num = 0;
					}
					Main.player[Main.myPlayer].HeldItem.axe = (int)Math.Round((double)num / 5.0);
					Mystagogue.Output("Axe power set to " + Main.player[Main.myPlayer].HeldItem.axe * 5, false);
					return;
				}
				else
				{
					if (!Main.player[Main.myPlayer].HeldItem.IsAir)
					{
						Item item = new Item();
						item.SetDefaults(Main.player[Main.myPlayer].HeldItem.type);
						if (Main.player[Main.myPlayer].HeldItem.axe == item.axe)
						{
							Main.player[Main.myPlayer].HeldItem.axe = 35;
						}
						else
						{
							Main.player[Main.myPlayer].HeldItem.axe = item.axe;
						}
						Mystagogue.Output("Axe power set to " + Main.player[Main.myPlayer].HeldItem.axe * 5, false);
						return;
					}
					Mystagogue.Output("Must be holding an item in the hotbar", false);
					return;
				}
			});
			new MystagogueCMD("hammer", "(New hammer power) Changes the hammer power of the item selected in the hotbar.", delegate()
			{
				if (Mystagogue.CommandArgs.Count != 1 && !Main.player[Main.myPlayer].HeldItem.IsAir)
				{
					if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
					{
						Mystagogue.Output("Must be a positive integer", false);
						return;
					}
					string text = Mystagogue.CommandArgs[1];
					while (text.StartsWith("0"))
					{
						text = text.Remove(0, 1);
					}
					int num;
					if (text.Length != 0)
					{
						if (text.Length > 3)
						{
							num = 100;
						}
						else
						{
							num = int.Parse(text);
							if (num > 100)
							{
								num = 100;
							}
						}
					}
					else
					{
						num = 0;
					}
					Main.player[Main.myPlayer].HeldItem.hammer = num;
					Mystagogue.Output("Hammer power set to " + Main.player[Main.myPlayer].HeldItem.hammer, false);
					return;
				}
				else
				{
					if (!Main.player[Main.myPlayer].HeldItem.IsAir)
					{
						Item item = new Item();
						item.SetDefaults(Main.player[Main.myPlayer].HeldItem.type);
						if (Main.player[Main.myPlayer].HeldItem.hammer == item.hammer)
						{
							Main.player[Main.myPlayer].HeldItem.hammer = 100;
						}
						else
						{
							Main.player[Main.myPlayer].HeldItem.hammer = item.hammer;
						}
						Mystagogue.Output("Hammer power set to " + Main.player[Main.myPlayer].HeldItem.hammer, false);
						return;
					}
					Mystagogue.Output("Must be holding an item in the hotbar", false);
					return;
				}
			});
			new MystagogueCMD("toolgod", "(No arguments) Toggles tool god, which selects the items that the game would select and makes those tools much more powerful. This stays on and is automatic.", delegate()
			{
				Main.player[Main.myPlayer].MystagogueToolGod = !Main.player[Main.myPlayer].MystagogueToolGod;
				string str = "no longer";
				if (Main.player[Main.myPlayer].MystagogueToolGod)
				{
					str = "now";
				}
				Mystagogue.Output("Your tools are " + str + " abundant with power", false);
				Mystagogue.BuffMyTools();
			});
			new MystagogueCMD("ri", "(No arguments) Refreshes the item selected in the hotbar.", delegate()
			{
				Main.player[Main.myPlayer].HeldItem.Refresh();
				Mystagogue.Output("Held item refreshed", false);
				Mystagogue.BuffMyTools();
			});
			new MystagogueCMD("ris", "(No arguments) Refreshes all items in all inventories of the character.", delegate()
			{
				Main.player[Main.myPlayer].RefreshItems();
				Mystagogue.Output("All items refreshed", false);
				Mystagogue.BuffMyTools();
			});
			new MystagogueCMD("setstacks2b", "(No arguments) Makes all stackable items' stacks overflow to 2147483647.", delegate()
			{
				if (Main.player[Main.myPlayer].MystagogueRefills)
				{
					Mystagogue.Output("You will once again be able to deplete favorited items in your inventory", false);
					Main.player[Main.myPlayer].MystagogueRefills = false;
				}
				int num = 0;
				foreach (Item item in Main.player[Main.myPlayer].inventory.ToArray<Item>())
				{
					if (item.IsAir)
					{
						Main.player[Main.myPlayer].inventory[num] = new Item();
					}
					else if (item.maxStack > 1)
					{
						Main.player[Main.myPlayer].inventory[num].stack = int.MaxValue;
					}
					num++;
				}
				num = 0;
				foreach (Item item2 in Main.player[Main.myPlayer].miscEquips.ToArray<Item>())
				{
					if (item2.IsAir)
					{
						Main.player[Main.myPlayer].miscEquips[num] = new Item();
					}
					else if (item2.maxStack > 1)
					{
						Main.player[Main.myPlayer].miscEquips[num].stack = int.MaxValue;
					}
					num++;
				}
				num = 0;
				foreach (Item item3 in Main.player[Main.myPlayer].bank.item.ToArray<Item>())
				{
					if (item3.IsAir)
					{
						Main.player[Main.myPlayer].bank.item[num] = new Item();
					}
					else if (item3.maxStack > 1)
					{
						Main.player[Main.myPlayer].bank.item[num].stack = int.MaxValue;
					}
					num++;
				}
				num = 0;
				foreach (Item item4 in Main.player[Main.myPlayer].bank2.item.ToArray<Item>())
				{
					if (item4.IsAir)
					{
						Main.player[Main.myPlayer].bank2.item[num] = new Item();
					}
					else if (item4.maxStack > 1)
					{
						Main.player[Main.myPlayer].bank2.item[num].stack = int.MaxValue;
					}
					num++;
				}
				num = 0;
				foreach (Item item5 in Main.player[Main.myPlayer].bank3.item.ToArray<Item>())
				{
					if (item5.IsAir)
					{
						Main.player[Main.myPlayer].bank3.item[num] = new Item();
					}
					else if (item5.maxStack > 1)
					{
						Main.player[Main.myPlayer].bank3.item[num].stack = int.MaxValue;
					}
					num++;
				}
				num = 0;
				foreach (Item item6 in Main.player[Main.myPlayer].bank4.item.ToArray<Item>())
				{
					if (item6.IsAir)
					{
						Main.player[Main.myPlayer].bank4.item[num] = new Item();
					}
					else if (item6.maxStack > 1)
					{
						Main.player[Main.myPlayer].bank4.item[num].stack = int.MaxValue;
					}
					num++;
				}
				Mystagogue.Output("Stacks overflowed", false);
			});
			new MystagogueCMD("setstackslegit", "(No arguments) Makes all items with overflown stacks regulated again.", delegate()
			{
				int num = 0;
				foreach (Item item in Main.player[Main.myPlayer].inventory.ToArray<Item>())
				{
					if (item.IsAir)
					{
						Main.player[Main.myPlayer].inventory[num] = new Item();
					}
					else if (item.maxStack > 1)
					{
						Main.player[Main.myPlayer].inventory[num].stack = Main.player[Main.myPlayer].inventory[num].maxStack;
					}
					num++;
				}
				num = 0;
				foreach (Item item2 in Main.player[Main.myPlayer].armor.ToArray<Item>())
				{
					if (item2.IsAir)
					{
						Main.player[Main.myPlayer].armor[num] = new Item();
					}
					else if (item2.maxStack > 1)
					{
						Main.player[Main.myPlayer].armor[num].stack = 1;
					}
					num++;
				}
				num = 0;
				foreach (Item item3 in Main.player[Main.myPlayer].dye.ToArray<Item>())
				{
					if (item3.IsAir)
					{
						Main.player[Main.myPlayer].dye[num] = new Item();
					}
					else if (item3.maxStack > 1)
					{
						Main.player[Main.myPlayer].dye[num].stack = 1;
					}
					num++;
				}
				num = 0;
				foreach (Item item4 in Main.player[Main.myPlayer].miscEquips.ToArray<Item>())
				{
					if (item4.IsAir)
					{
						Main.player[Main.myPlayer].miscEquips[num] = new Item();
					}
					else if (item4.maxStack > 1)
					{
						Main.player[Main.myPlayer].miscEquips[num].stack = Main.player[Main.myPlayer].miscEquips[num].maxStack;
					}
					num++;
				}
				num = 0;
				foreach (Item item5 in Main.player[Main.myPlayer].miscDyes.ToArray<Item>())
				{
					if (item5.IsAir)
					{
						Main.player[Main.myPlayer].miscDyes[num] = new Item();
					}
					else if (item5.maxStack > 1)
					{
						Main.player[Main.myPlayer].miscDyes[num].stack = 1;
					}
					num++;
				}
				num = 0;
				foreach (Item item6 in Main.player[Main.myPlayer].bank.item.ToArray<Item>())
				{
					if (item6.IsAir)
					{
						Main.player[Main.myPlayer].bank.item[num] = new Item();
					}
					else if (item6.maxStack > 1)
					{
						Main.player[Main.myPlayer].bank.item[num].stack = Main.player[Main.myPlayer].bank.item[num].maxStack;
					}
					num++;
				}
				num = 0;
				foreach (Item item7 in Main.player[Main.myPlayer].bank2.item.ToArray<Item>())
				{
					if (item7.IsAir)
					{
						Main.player[Main.myPlayer].bank2.item[num] = new Item();
					}
					else if (item7.maxStack > 1)
					{
						Main.player[Main.myPlayer].bank2.item[num].stack = Main.player[Main.myPlayer].bank2.item[num].maxStack;
					}
					num++;
				}
				num = 0;
				foreach (Item item8 in Main.player[Main.myPlayer].bank3.item.ToArray<Item>())
				{
					if (item8.IsAir)
					{
						Main.player[Main.myPlayer].bank3.item[num] = new Item();
					}
					else if (item8.maxStack > 1)
					{
						Main.player[Main.myPlayer].bank3.item[num].stack = Main.player[Main.myPlayer].bank3.item[num].maxStack;
					}
					num++;
				}
				num = 0;
				foreach (Item item9 in Main.player[Main.myPlayer].bank4.item.ToArray<Item>())
				{
					if (item9.IsAir)
					{
						Main.player[Main.myPlayer].bank4.item[num] = new Item();
					}
					else if (item9.maxStack > 1)
					{
						Main.player[Main.myPlayer].bank4.item[num].stack = Main.player[Main.myPlayer].bank4.item[num].maxStack;
					}
					num++;
				}
				Mystagogue.Output("Stacks regulated", false);
			});
			new MystagogueCMD("fvt", "(No arguments) Favorites all items in the immediate inventory.", delegate()
			{
				for (int i = 0; i < Main.player[Main.myPlayer].inventory.Length; i++)
				{
					Main.player[Main.myPlayer].inventory[i].favorited = true;
				}
				Mystagogue.Output("Snap of the fingers!", false);
			});
			new MystagogueCMD("clr", "(No arguments) Clears all items that are not favorited in the inventory and all clear items in the Void inventory.", delegate()
			{
				foreach (Item item in Main.player[Main.myPlayer].inventory)
				{
					if (!item.favorited)
					{
						item.SetDefaults(0);
					}
				}
				foreach (Item item2 in Main.player[Main.myPlayer].bank4.item)
				{
					if (item2.type != 2147483647)
					{
						item2.SetDefaults(0);
					}
				}
				Mystagogue.Output("Autotrash cleaned all excess items: Not favorited in Inventory, and all Void contents.", false);
			});
			new MystagogueCMD("refills", "(No arguments) Toggles refills, which will refill all stackable favorited items if they aren't stacked full.", delegate()
			{
				Main.player[Main.myPlayer].MystagogueRefills = !Main.player[Main.myPlayer].MystagogueRefills;
				string str = "once again be able to";
				if (Main.player[Main.myPlayer].MystagogueRefills)
				{
					str = "no longer";
				}
				Mystagogue.Output("You will " + str + " deplete favorited items in your inventory", false);
			});
			new MystagogueCMD("god", "(No arguments) Will enable God Mode as seen in journey mode, stopping damageand mana depletion. You will still be able to take debuffs.", delegate()
			{
				Main.player[Main.myPlayer].MystagogueGod = !Main.player[Main.myPlayer].MystagogueGod;
				string str = "once again";
				if (Main.player[Main.myPlayer].MystagogueGod)
				{
					str = "no longer";
				}
				Mystagogue.Output("You will " + str + " take damage or lose Mana", false);
			});
			new MystagogueCMD("buddha", "(Amount per second) Heals the player artifically by the specified amount per second. Reset with no arguments,", delegate()
			{
				if (Mystagogue.CommandArgs.Count == 1)
				{
					Main.player[Main.myPlayer].MystagogueBuddha = 0;
					Mystagogue.Output("Anomalous regeneration dormant", false);
					return;
				}
				if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
				{
					Mystagogue.Output("Must be a positive integer", false);
					return;
				}
				string text = Mystagogue.CommandArgs[1];
				while (text.StartsWith("0"))
				{
					text = text.Remove(0, 1);
				}
				int num = 0;
				if (text.Length != 0)
				{
					if (text.Length > 6)
					{
						num = 200000;
					}
					else
					{
						num = int.Parse(text);
						if (num > 200000)
						{
							num = 200000;
						}
					}
				}
				if (num == 0)
				{
					Main.player[Main.myPlayer].MystagogueBuddha = 0;
					Mystagogue.Output("Anomalous regeneration dormant", false);
					return;
				}
				Main.player[Main.myPlayer].MystagogueBuddha = num;
				Mystagogue.Output("Anomalous regeneration active! " + Main.player[Main.myPlayer].MystagogueBuddha + "/second", false);
			});
			new MystagogueCMD("nomanacost", "(No arguments) Toggles whether not all mana consuming items will no longer consume any mana.", delegate()
			{
				if (Main.player[Main.myPlayer].MystagogueManaCostDeduction == 0f)
				{
					Main.player[Main.myPlayer].MystagogueManaCostDeduction = 1f;
					Mystagogue.Output("Mana will no longer be deducted when using Mana dependent items", false);
					return;
				}
				Main.player[Main.myPlayer].MystagogueManaCostDeduction = 0f;
				Mystagogue.Output("Mana will once again be deducted when using Mana dependent items", false);
			});
			new MystagogueCMD("infflight", "(No arguments) Toggles whether not all flying boots and wings will be able to fly without a timer.", delegate()
			{
				Main.player[Main.myPlayer].MystagogueInfiniteFlight = !Main.player[Main.myPlayer].MystagogueInfiniteFlight;
				if (Main.player[Main.myPlayer].MystagogueInfiniteFlight)
				{
					Mystagogue.Output("Flight is now inexhaustable!", false);
					return;
				}
				Mystagogue.Output("Flight is no longer inexhaustable", false);
			});
			new MystagogueCMD("boost", "(Speed 1-7) Changes several player movement variables at different multipliers to aid in accelerated movement. The movement will appear like lag and teleporting on other people's games when used in Multiplayer.", delegate()
			{
				if (Mystagogue.CommandArgs.Count == 1)
				{
					Main.player[Main.myPlayer].MystagogueSpeedBoost = 0;
					Mystagogue.Output("Speed boost dormant", false);
					return;
				}
				if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
				{
					Mystagogue.Output("Must be a positive integer", false);
					return;
				}
				string text = Mystagogue.CommandArgs[1];
				while (text.StartsWith("0"))
				{
					text = text.Remove(0, 1);
				}
				int num = 0;
				if (text.Length != 0)
				{
					if (text.Length > 1)
					{
						num = 7;
					}
					else
					{
						num = int.Parse(text);
						if (num > 7)
						{
							num = 7;
						}
					}
				}
				if (num == 0)
				{
					Main.player[Main.myPlayer].MystagogueSpeedBoost = 0;
					Mystagogue.Output("Speed boost dormant", false);
					return;
				}
				Main.player[Main.myPlayer].MystagogueSpeedBoost = num;
				Mystagogue.Output("Speed boost active! Power level " + Main.player[Main.myPlayer].MystagogueSpeedBoost, false);
			});
			new MystagogueCMD("jesus", "(No arguments) Simply toggles whether not the player can walk on liquids.", delegate()
			{
				Main.player[Main.myPlayer].MystagogueJesus = !Main.player[Main.myPlayer].MystagogueJesus;
				string str = "no longer";
				if (Main.player[Main.myPlayer].MystagogueJesus)
				{
					str = "now";
				}
				Mystagogue.Output("You are " + str + " able to walk on still liquid surfaces", false);
			});
			new MystagogueCMD("maxminions", "(New base max minions) Set the basic minion count up to 1000, so that you can annoy those annoying hackers who spawn as many minions as possible to lag the game and stop more projectiles from spawning by doing the same thing as them.", delegate()
			{
				if (Mystagogue.CommandArgs.Count == 1)
				{
					Main.player[Main.myPlayer].MystagoguePlayerMaxMinions = 1;
					Mystagogue.Output("Max Minions reset", false);
					return;
				}
				if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
				{
					Mystagogue.Output("Must be a positive integer", false);
					return;
				}
				string text = Mystagogue.CommandArgs[1];
				while (text.StartsWith("0"))
				{
					text = text.Remove(0, 1);
				}
				int num = 1;
				if (text.Length != 0)
				{
					if (text.Length > 4)
					{
						num = 1000;
					}
					else
					{
						num = int.Parse(text);
						if (num < 1)
						{
							num = 1;
						}
						if (num > 1000)
						{
							num = 1000;
						}
					}
				}
				if (num == 1)
				{
					Main.player[Main.myPlayer].MystagoguePlayerMaxMinions = 1;
					Mystagogue.Output("Max Minions reset", false);
					return;
				}
				Main.player[Main.myPlayer].MystagoguePlayerMaxMinions = num;
				Mystagogue.Output("Max Minions set to " + num, false);
			});
			new MystagogueCMD("givebuffs", "(Mode: melee, ranged, magic, summon) Gives the best buffs for a respective class.", delegate()
			{
				if (Main.player[Main.myPlayer].MystagogueBuffQueue == null)
				{
					Main.player[Main.myPlayer].MystagogueBuffQueue = new List<int>();
				}
				if (Mystagogue.CommandArgs.Count == 1)
				{
					Mystagogue.Output("Specify a class: \"melee\", \"ranged\", \"magic\", or \"summon\"", false);
					return;
				}
				if (Mystagogue.CommandArgs[1] == "melee" || Mystagogue.CommandArgs[1] == "ranged" || Mystagogue.CommandArgs[1] == "magic" || Mystagogue.CommandArgs[1] == "summon")
				{
					for (int i = 0; i < 22; i++)
					{
						Main.player[Main.myPlayer].buffTime[i] = 0;
						Main.player[Main.myPlayer].buffType[i] = 0;
					}
					Main.player[Main.myPlayer].MystagogueBuffQueue.Add(1);
					Main.player[Main.myPlayer].MystagogueBuffQueue.Add(17);
					Main.player[Main.myPlayer].MystagogueBuffQueue.Add(3);
					Main.player[Main.myPlayer].MystagogueBuffQueue.Add(207);
					Main.player[Main.myPlayer].MystagogueBuffQueue.Add(5);
					if (Mystagogue.CommandArgs[1] == "melee")
					{
						Main.player[Main.myPlayer].MystagogueBuffQueue.Add(25);
						Main.player[Main.myPlayer].MystagogueBuffQueue.Add(76);
						Main.player[Main.myPlayer].MystagogueBuffQueue.Add(159);
					}
					else if (Mystagogue.CommandArgs[1] == "ranged")
					{
						Main.player[Main.myPlayer].MystagogueBuffQueue.Add(16);
					}
					else if (Mystagogue.CommandArgs[1] == "magic")
					{
						Main.player[Main.myPlayer].MystagogueBuffQueue.Add(6);
						Main.player[Main.myPlayer].MystagogueBuffQueue.Add(7);
						Main.player[Main.myPlayer].MystagogueBuffQueue.Add(29);
					}
					else if (Mystagogue.CommandArgs[1] == "summon")
					{
						Main.player[Main.myPlayer].MystagogueBuffQueue.Add(76);
						Main.player[Main.myPlayer].MystagogueBuffQueue.Add(159);
						Main.player[Main.myPlayer].MystagogueBuffQueue.Add(6);
						Main.player[Main.myPlayer].MystagogueBuffQueue.Add(110);
						Main.player[Main.myPlayer].MystagogueBuffQueue.Add(150);
					}
					Main.player[Main.myPlayer].MystagogueBuffQueue.Add(115);
					Main.player[Main.myPlayer].MystagogueBuffQueue.Add(117);
					Main.player[Main.myPlayer].MystagogueBuffQueue.Add(105);
					Main.player[Main.myPlayer].MystagogueBuffQueue.Add(2);
					Main.player[Main.myPlayer].MystagogueBuffQueue.Add(114);
					Main.player[Main.myPlayer].MystagogueBuffQueue.Add(113);
					Mystagogue.Output("Buffs are being added.", false);
					return;
				}
				Mystagogue.Output("Class invalid; Must be: \"melee\", \"ranged\", \"magic\", or \"summon\"", false);
			});
			new MystagogueCMD("killdebuffs", "(No arguments) Toggles immunity to debuffs (besides tipsy).", delegate()
			{
				Main.player[Main.myPlayer].MystagogueKillDebuffs = !Main.player[Main.myPlayer].MystagogueKillDebuffs;
				string str = "no longer";
				if (Main.player[Main.myPlayer].MystagogueKillDebuffs)
				{
					str = "now";
				}
				Mystagogue.Output("You " + str + " have anomalous immunity to all debuffs", false);
			});
			new MystagogueCMD("banless", "(No arguments) Toggles immunity to the debuffs Frozen, Webbed, and Stoned, which are used to stop players from commiting certain actions on servers.", delegate()
			{
				Main.player[Main.myPlayer].MystagogueBanless = !Main.player[Main.myPlayer].MystagogueBanless;
				string str = "no longer";
				if (Main.player[Main.myPlayer].MystagogueBanless)
				{
					str = "now";
				}
				Mystagogue.Output("You " + str + " have anomalous immunity to Frozen, Webbed, and Stoned", false);
			});
			new MystagogueCMD("p2pmaphack", "(No arguments) Toggles whether you can see all players and teleport to them for no cost no matter their team or PVP status in relation to yours.", delegate()
			{
				Main.player[Main.myPlayer].MystagogueP2PMapHack = !Main.player[Main.myPlayer].MystagogueP2PMapHack;
				string str = "no longer";
				if (Main.player[Main.myPlayer].MystagogueP2PMapHack)
				{
					str = "now";
				}
				Mystagogue.Output("You " + str + " are capable of locating and teleporting to all players on the map", false);
			});
			new MystagogueCMD("magnet", "(No argments) Teleports all dropped items to you. May duplicate items that were already picked up (It's weird).", delegate()
			{
				Mystagogue.Output("Bringing items...", false);
				for (int i = 0; i < Main.item.Length; i++)
				{
					if ((Main.item[i].playerIndexTheItemIsReservedFor == Main.myPlayer && Main.netMode == 1) || (Main.item[i].playerIndexTheItemIsReservedFor == 255 || (Main.item[i].playerIndexTheItemIsReservedFor != 255 && !Main.player[Main.item[i].playerIndexTheItemIsReservedFor].active)) || Main.netMode == 0)
					{
						Main.item[i].position = Main.player[Main.myPlayer].position;
						if (Main.netMode != 0)
						{
							NetMessage.SendData(21, -1, -1, null, i, 1f, 0f, 0f, 0, 0, 0);
						}
					}
				}
			});
			new MystagogueCMD("magnodupe", "(No argments) Teleports all dropped items to you. I tried to stop duplication from happening here, unlike in [magnet].", delegate()
			{
				Mystagogue.Output("Bringing items legitnessly...", false);
				for (int i = 0; i < Main.item.Length; i++)
				{
					if (Main.item[i].active && ((Main.item[i].playerIndexTheItemIsReservedFor == Main.myPlayer && Main.netMode == 1) || (Main.item[i].playerIndexTheItemIsReservedFor == 255 || (Main.item[i].playerIndexTheItemIsReservedFor != 255 && !Main.player[Main.item[i].playerIndexTheItemIsReservedFor].active)) || Main.netMode == 0))
					{
						Main.item[i].position = Main.player[Main.myPlayer].position;
						if (Main.netMode != 0)
						{
							NetMessage.SendData(21, -1, -1, null, i, 1f, 0f, 0f, 0, 0, 0);
						}
					}
				}
			});
			new MystagogueCMD("flashlight", "(No arguments) Maybe I'd like to point a light through tiles to find stuff. Oh, the light's a square too.", delegate()
			{
				Main.player[Main.myPlayer].MystagogueFlashlight = !Main.player[Main.myPlayer].MystagogueFlashlight;
				string raw = "Aww, man...";
				if (Main.player[Main.myPlayer].MystagogueFlashlight)
				{
					raw = "Kachow!";
				}
				Mystagogue.Output(raw, false);
			});
			new MystagogueCMD("nrt", "(No arguments) Toggles whether you respawn after death instantly.", delegate()
			{
				Main.player[Main.myPlayer].MystagogueNoRespawnTimer = !Main.player[Main.myPlayer].MystagogueNoRespawnTimer;
				string str = "no longer";
				if (Main.player[Main.myPlayer].MystagogueNoRespawnTimer)
				{
					str = "now";
				}
				Mystagogue.Output("You " + str + " respawn instantly after death", false);
			});
			new MystagogueCMD("copyme", "(Name of copy) Creates a basic copy of your player with the inputted name. No name input is absolutely required, though.", delegate()
			{
				Player player = Main.player[Main.myPlayer].Duplicate();
				player.name = ((Mystagogue.CommandArgs.Count > 1) ? Mystagogue.CommandArgs[1] : ("Copy of " + player.name));
				PlayerFileData.CreateAndSave(player);
				player.active = false;
				Mystagogue.Output("Character created: " + player.name, false);
			});
			new MystagogueCMD("nameme", "(New names) Renames your character.", delegate()
			{
				Main.player[Main.myPlayer].name = ((Mystagogue.CommandArgs.Count > 1) ? Mystagogue.CommandArgs[1] : "One Face Among Many");
				Mystagogue.Output("You are " + Main.player[Main.myPlayer].name, false);
			});
			new MystagogueCMD("maxlife", "(New max life) Changes your character's intristic maximum life capacity. Doesn't seem to save anything above 500 or below 100.", delegate()
			{
				if (Mystagogue.CommandArgs.Count == 1)
				{
					Mystagogue.Output("That command requires arguments", false);
					return;
				}
				if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
				{
					Mystagogue.Output("Must be a positive integer", false);
					return;
				}
				string text = Mystagogue.CommandArgs[1];
				while (text.StartsWith("0"))
				{
					text = text.Remove(0, 1);
				}
				int num = 1;
				if (text.Length != 0)
				{
					if (text.Length > 10)
					{
						num = int.MaxValue;
					}
					else
					{
						num = (int)Math.Min(long.Parse(text), 2147483647L);
					}
				}
				Main.player[Main.myPlayer].statLifeMax = num;
				Mystagogue.Output("Max Life set to " + num, false);
			});
			new MystagogueCMD("maxmana", "(New max mana) Changes your character's intristic maximum mana capacity. The game will stop the mana from exceeding the default maximum max of 200 mana.", delegate()
			{
				if (Mystagogue.CommandArgs.Count == 1)
				{
					Mystagogue.Output("That command requires arguments", false);
					return;
				}
				if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
				{
					Mystagogue.Output("Must be a positive integer", false);
					return;
				}
				string text = Mystagogue.CommandArgs[1];
				while (text.StartsWith("0"))
				{
					text = text.Remove(0, 1);
				}
				int num = 0;
				if (text.Length != 0)
				{
					if (text.Length > 3)
					{
						num = 200;
					}
					else
					{
						num = int.Parse(text);
					}
				}
				Main.player[Main.myPlayer].statManaMax = num;
				Mystagogue.Output("Max Mana set to " + num, false);
			});
			new MystagogueCMD("setfish", "(New amount of finished angler quests) Because fishing is a fool's errand.", delegate()
			{
				if (Mystagogue.CommandArgs.Count == 1)
				{
					Mystagogue.Output("That command requires arguments", false);
					return;
				}
				if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
				{
					Mystagogue.Output("Must be a positive integer", false);
					return;
				}
				string text = Mystagogue.CommandArgs[1];
				while (text.StartsWith("0"))
				{
					text = text.Remove(0, 1);
				}
				int num = 0;
				if (text.Length != 0)
				{
					if (text.Length > 10)
					{
						num = int.MaxValue;
					}
					else
					{
						num = (int)Math.Min(long.Parse(text), 2147483647L);
					}
				}
				Main.player[Main.myPlayer].anglerQuestsFinished = num;
				Mystagogue.Output("Angler Quests Finished set to " + num, false);
			});
			new MystagogueCMD("setdd2", "(New amount of finished bartender quests) Nobody even knew DD2 quests existed!", delegate()
			{
				if (Mystagogue.CommandArgs.Count == 1)
				{
					Mystagogue.Output("That command requires arguments", false);
					return;
				}
				if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
				{
					Mystagogue.Output("Must be a positive integer", false);
					return;
				}
				string text = Mystagogue.CommandArgs[1];
				while (text.StartsWith("0"))
				{
					text = text.Remove(0, 1);
				}
				int num = 0;
				if (text.Length != 0)
				{
					if (text.Length > 10)
					{
						num = int.MaxValue;
					}
					else
					{
						num = (int)Math.Min(long.Parse(text), 2147483647L);
					}
				}
				Main.player[Main.myPlayer].bartenderQuestLog = num;
				Mystagogue.Output("Bartender Quests Finished set to " + num, false);
			});
			new MystagogueCMD("setdifficulty", "(New difficulty ID) Set the character's difficulty to Classic, Mediumcore, Hardcore, and Journey. IDs are 0, 1, 2, and 3 respectively.", delegate()
			{
				if (Mystagogue.CommandArgs.Count == 1)
				{
					Mystagogue.Output("That command requires arguments", false);
					return;
				}
				if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
				{
					Mystagogue.Output("Must be a positive integer", false);
					return;
				}
				string text = Mystagogue.CommandArgs[1];
				while (text.StartsWith("0"))
				{
					text = text.Remove(0, 1);
				}
				int num = 0;
				if (text.Length != 0)
				{
					if (text.Length > 1)
					{
						num = 3;
					}
					else
					{
						num = int.Parse(text);
						if (num > 3)
						{
							num = 3;
						}
					}
				}
				Main.player[Main.myPlayer].difficulty = (byte)num;
				Mystagogue.Output("Difficulty set to " + (((byte)num == 0) ? "Classic" : (((byte)num == 1) ? "Mediumcore" : (((byte)num == 2) ? "Hardcore" : (((byte)num == 3) ? "Journey" : "null")))), false);
			});
			new MystagogueCMD("demonheart", "(No arguments) Changes whether not the game believes your character has an extra slot for expert mode.", delegate()
			{
				Main.player[Main.myPlayer].extraAccessory = !Main.player[Main.myPlayer].extraAccessory;
				string str = "no longer";
				if (Main.player[Main.myPlayer].extraAccessory)
				{
					str = "now";
				}
				Mystagogue.Output("You " + str + " have eaten a demon heart", false);
			});
			new MystagogueCMD("charactertime", "(New time in ticks or TimeSpan format) Sets the time the current character has played in ticks (60 ticks a second) up to 999999999999999999 ticks, or a timespan of up to 10675199:59:59. Timespans can be inputted as hhhh...:mm:ss", delegate()
			{
				if (Mystagogue.CommandArgs.Count == 1)
				{
					Mystagogue.Output(string.Concat(new object[]
					{
						Main.player[Main.myPlayer].name,
						" has played for ",
						Math.Floor(Main.ActivePlayerFileData.GetPlayTime().TotalHours),
						":",
						Main.ActivePlayerFileData.GetPlayTime().Minutes,
						":",
						Main.ActivePlayerFileData.GetPlayTime().Seconds,
						" (",
						Main.ActivePlayerFileData.GetPlayTime().Ticks,
						"). To change this, you can input as a first parameter either ticks (60 per second) or a timespan with up to 10675199 hours (hhhh...:mm:ss)."
					}), false);
					return;
				}
				long ticks = 0L;
				if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
				{
					List<string> list = Mystagogue.CommandArgs[1].Split(new char[]
					{
						':'
					}).ToList<string>();
					if (new Regex("\\D").IsMatch(string.Join("", list)))
					{
						Mystagogue.Output("You can input as a first parameter either ticks (60 per second) or a timespan format with up to 10675199 hours (hhhh...:mm:ss).", false);
						return;
					}
					if (list.Count != 3)
					{
						Mystagogue.Output("Not enough/Too many colons. You can input as a first parameter either ticks (60 per second) or a timespan format with up to 10675199 hours (hhhh...:mm:ss).", false);
						return;
					}
					List<int> list2 = new List<int>();
					for (int i = 0; i < list.Count; i++)
					{
						while (list[i].StartsWith("0"))
						{
							list[i] = list[i].Remove(0, 1);
						}
						long num = 0L;
						if (list[i].Length != 0)
						{
							if (list[i].Length > 10)
							{
								num = ((i == 0) ? 10675199L : 2147483647L);
							}
							else if (i == 0)
							{
								num = ((long.Parse(list[i]) > 10675199L) ? 10675199L : num);
							}
							else
							{
								num = ((long.Parse(list[i]) > 2147483647L) ? 2147483647L : num);
							}
						}
						list2.Insert(i, (int)num);
					}
					while (list2[2] > 59)
					{
						list2[1] = list2[1] + 1;
						list2[2] = list2[2] - 60;
					}
					while (list2[1] > 59 && list2[0] < 10675199)
					{
						list2[0] = list2[0] + 1;
						list2[1] = list2[1] - 60;
					}
					if (list2[1] > 59)
					{
						list2[1] = 59;
						list2[2] = 59;
					}
					TimeSpan timeSpan = new TimeSpan(list2[0], list2[1], list2[2]);
					ticks = timeSpan.Ticks;
				}
				else
				{
					string text = Mystagogue.CommandArgs[1];
					while (text.StartsWith("0"))
					{
						text = text.Remove(0, 1);
					}
					if (text.Length != 0)
					{
						if (text.Length > 18)
						{
							ticks = long.MaxValue;
						}
						else
						{
							ticks = long.Parse(text);
						}
					}
				}
				Main.ActivePlayerFileData.MystagoguePTOp(ticks);
				Mystagogue.Output(string.Concat(new object[]
				{
					Main.player[Main.myPlayer].name,
					" had their playtime set to ",
					Math.Floor(Main.ActivePlayerFileData.GetPlayTime().TotalHours),
					":",
					Main.ActivePlayerFileData.GetPlayTime().Minutes,
					":",
					Main.ActivePlayerFileData.GetPlayTime().Seconds,
					" (",
					Main.ActivePlayerFileData.GetPlayTime().Ticks,
					")"
				}), false);
			});
			new MystagogueCMD("spawnrate", "(New spawnrate) Sets the spawnrate multiplier. Can be 0 through 1000. Diminishing returns are apparent around level 40.", delegate()
			{
				if (Mystagogue.CommandArgs.Count == 1)
				{
					Main.player[Main.myPlayer].MystagogueSpawnRate = 1;
					Mystagogue.Output("Spawn Rate Multipler reset", false);
					return;
				}
				if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
				{
					Mystagogue.Output("Must be a positive integer", false);
					return;
				}
				string text = Mystagogue.CommandArgs[1];
				while (text.StartsWith("0"))
				{
					text = text.Remove(0, 1);
				}
				int num = 0;
				if (text.Length != 0)
				{
					if (text.Length > 4)
					{
						num = 1000;
					}
					else
					{
						num = int.Parse(text);
						if (num > 1000)
						{
							num = 1000;
						}
					}
				}
				if (num == 1)
				{
					Main.player[Main.myPlayer].MystagogueSpawnRate = 1;
					Mystagogue.Output("Spawn Rate Multiplier reset", false);
					return;
				}
				Main.player[Main.myPlayer].MystagogueSpawnRate = num;
				Mystagogue.Output("Spawn Rate Multiplier set to " + num, false);
			});
			new MystagogueCMD("npc", "(Name or ID) Spawn an NPC at the cursor.", delegate()
			{
				if (Mystagogue.CommandArgs.Count == 1)
				{
					Mystagogue.Output("That command requires arguments", false);
					return;
				}
				int i = 0;
				List<string> list = new List<string>();
				if (!new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
				{
					string text = Mystagogue.CommandArgs[1];
					while (text.StartsWith("0"))
					{
						text = text.Remove(0, 1);
					}
					while (text != i.ToString())
					{
						if (i == 665)
						{
							Mystagogue.Output("Given NPC ID does not correspond to an item", false);
							return;
						}
						i++;
					}
					for (int j = 0; j < Mystagogue.CommandArgs.Count - 2; j++)
					{
						list.Add(Mystagogue.CommandArgs[2 + j]);
					}
				}
				else
				{
					i = 1;
					List<int> list2 = new List<int>();
					while (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[i]))
					{
						i++;
						if (i == Mystagogue.CommandArgs.Count)
						{
							break;
						}
					}
					string text2 = string.Join(" ", Mystagogue.CommandArgs.GetRange(1, i - 1)).ToUpper();
					int num = i;
					while (Mystagogue.CommandArgs.Count - num != 0)
					{
						list.Add(Mystagogue.CommandArgs[num]);
						num++;
					}
					for (i = 0; i < 665; i++)
					{
						if (Lang.GetNPCNameValue(i).ToUpper().StartsWith(text2))
						{
							list2.Add(i);
						}
					}
					if (list2.Count == 0)
					{
						Mystagogue.Output("No NPC names match", false);
						return;
					}
					if (list2.Count > 1)
					{
						List<string> list3 = new List<string>();
						foreach (int netID in list2)
						{
							list3.Add(Lang.GetNPCNameValue(netID).ToUpper());
						}
						List<string> list4 = new List<string>();
						foreach (int num2 in list2)
						{
							list4.Add(string.Concat(new object[]
							{
								Lang.GetNPCNameValue(num2),
								" (",
								num2,
								")"
							}));
						}
						bool flag = false;
						for (int k = 0; k < list3.Count; k++)
						{
							if (list3[k] == text2)
							{
								i = list2[k];
								list4.RemoveAt(k);
								Mystagogue.Output("Other matches include " + string.Join(", ", list4), false);
								break;
							}
							if (k + 1 == list2.Count && !flag)
							{
								Mystagogue.Output("Found " + string.Join(", ", list4), false);
								return;
							}
						}
					}
					else
					{
						i = list2[0];
					}
				}
				int num3 = 1;
				if (list.Count >= 1)
				{
					if (new Regex("\\D").IsMatch(list[0]))
					{
						Mystagogue.Output("Amount must be a positive integer", false);
						return;
					}
					string text3 = list[0];
					while (text3.StartsWith("0"))
					{
						text3 = text3.Remove(0, 1);
					}
					if (text3.Length > 3)
					{
						num3 = 200;
					}
					else if (int.Parse(text3) > 200)
					{
						num3 = 200;
					}
					else if (text3.Length > 0)
					{
						num3 = int.Parse(text3);
					}
				}
				Vector2 vector = default(Vector2);
				vector.X = (float)Main.mouseX + Main.screenPosition.X;
				if (Main.player[Main.myPlayer].gravDir == 1f)
				{
					vector.Y = (float)Main.mouseY + Main.screenPosition.Y;
				}
				else
				{
					vector.Y = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY;
				}
				int num4 = NPC.NewNPC((int)Math.Round((double)vector.X), (int)Math.Round((double)vector.Y), i, 0, 0f, 0f, 0f, 0f, 255);
				if (Main.netMode != 0)
				{
					NetMessage.SendData(23, -1, -1, null, num4, 0f, 0f, 0f, 0, 0, 0);
				}
				if (num3 > 1)
				{
					for (int l = 1; l < num3; l++)
					{
						if (Main.netMode != 0)
						{
							NetMessage.SendData(23, -1, -1, null, NPC.NewNPC((int)Math.Round((double)vector.X), (int)Math.Round((double)vector.Y), i, 0, 0f, 0f, 0f, 0f, 255), 0f, 0f, 0f, 0, 0, 0);
						}
						else
						{
							NPC.NewNPC((int)Math.Round((double)vector.X), (int)Math.Round((double)vector.Y), i, 0, 0f, 0f, 0f, 0f, 255);
						}
					}
				}
				Mystagogue.Output(string.Concat(new object[]
				{
					"Spawned ",
					(num3 > 1) ? (num3 + " ") : "",
					"new ",
					Lang.GetNPCNameValue(Main.npc[num4].type),
					" (",
					i,
					")"
				}), false);
			});
			new MystagogueCMD("searchnpc", "(Query) Returns ID of all NPCs with a name containing the concatenated arguments.", delegate()
			{
				if (Mystagogue.CommandArgs.Count == 1)
				{
					Mystagogue.Output("That command requires arguments", false);
					return;
				}
				List<int> list = new List<int>();
				string value = string.Join(" ", Mystagogue.CommandArgs.GetRange(1, Mystagogue.CommandArgs.Count - 1)).ToUpper();
				for (int i = 0; i < 665; i++)
				{
					if (Lang.GetNPCNameValue(i).ToUpper().Contains(value))
					{
						list.Add(i);
					}
				}
				if (list.Count == 0)
				{
					Mystagogue.Output("No NPC names match", false);
					return;
				}
				List<string> list2 = new List<string>();
				foreach (int num in list)
				{
					list2.Add(string.Concat(new object[]
					{
						Lang.GetNPCNameValue(num),
						" (",
						num,
						")"
					}));
				}
				Mystagogue.Output("Found " + string.Join(", ", list2), false);
			});
			new MystagogueCMD("tps", "(Teleport Setting) 0 = No Teleport on right click, 1 = Teleport on right click, 2 = Teleport every frame right click is held down.", delegate()
			{
				if (Mystagogue.CommandArgs.Count > 1)
				{
					if (Mystagogue.CommandArgs[1] == "0")
					{
						Main.player[Main.myPlayer].MystagogueTeleportSetting = 0;
						Mystagogue.Output("Guess you can't teleport. Shame...", false);
						return;
					}
					if (Mystagogue.CommandArgs[1] == "1")
					{
						Main.player[Main.myPlayer].MystagogueTeleportSetting = 1;
						Mystagogue.Output("Right click to teleport", false);
						return;
					}
					if (Mystagogue.CommandArgs[1] == "2")
					{
						Main.player[Main.myPlayer].MystagogueTeleportSetting = 2;
						Mystagogue.Output("Rapid teleport active. Right click to break the sound barrier", false);
						return;
					}
					Mystagogue.Output("That wasn't an option...", false);
					return;
				}
				else
				{
					if (Main.player[Main.myPlayer].MystagogueTeleportSetting == 0)
					{
						Main.player[Main.myPlayer].MystagogueTeleportSetting = 1;
						Mystagogue.Output("Right click to teleport", false);
						return;
					}
					Main.player[Main.myPlayer].MystagogueTeleportSetting = 0;
					Mystagogue.Output("Guess you can't teleport. Shame...", false);
					return;
				}
			});
			new MystagogueCMD("wind", "(New wind speed, Direction for wind to come from- W or E) Sets the wind speed and direction. Meant to help with achievements.", delegate()
			{
				if (Mystagogue.CommandArgs.Count < 3)
				{
					Mystagogue.Output("That command requires both arguments", false);
					return;
				}
				if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
				{
					Mystagogue.Output("Must be a positive integer", false);
					return;
				}
				string text = Mystagogue.CommandArgs[1];
				while (text.StartsWith("0"))
				{
					text = text.Remove(0, 1);
				}
				int num = 0;
				if (text.Length != 0)
				{
					if (text.Length > 2)
					{
						num = 59;
					}
					else
					{
						num = int.Parse(text);
						if (num > 59)
						{
							num = 59;
						}
					}
				}
				if (Mystagogue.CommandArgs[2].ToUpper() != "W" && Mystagogue.CommandArgs[2].ToUpper() != "E")
				{
					Mystagogue.Output("Invalid wind direction", false);
				}
				if (Mystagogue.CommandArgs[2].ToUpper() == "E")
				{
					num *= -1;
				}
				Main.windSpeedTarget = (float)num / 50f;
				Mystagogue.Output("Wind Speed set to " + num, false);
				NetMessage.TrySendData(7, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
			});
			new MystagogueCMD("time", "(New time) Sets the world's time, as a time span 24 hour format.", delegate()
			{
				if (Mystagogue.CommandArgs.Count == 1)
				{
					Mystagogue.Output("That command requires arguments", false);
					return;
				}
				List<string> list = Mystagogue.CommandArgs[1].Split(new char[]
				{
					':'
				}).ToList<string>();
				if (new Regex("\\D").IsMatch(string.Join("", list)))
				{
					Mystagogue.Output("You can input as a first parameter a timespan with up to 24 hours (hh:mm).", false);
					return;
				}
				if (list.Count != 2)
				{
					Mystagogue.Output("Not enough/Too many colons. You can input as a first parameter a timespan with up to 24 hours (hh:mm).", false);
					return;
				}
				List<int> list2 = new List<int>();
				while (list[0].StartsWith("0"))
				{
					list[0] = list[0].Remove(0, 1);
				}
				int item = 0;
				if (list[0].Length != 0)
				{
					if (list[0].Length > 2)
					{
						item = 24;
					}
					else
					{
						item = ((int.Parse(list[0]) > 24) ? 24 : int.Parse(list[0]));
					}
				}
				list2.Add(item);
				while (list[1].StartsWith("0"))
				{
					list[1] = list[1].Remove(0, 1);
				}
				item = 0;
				if (list[1].Length != 0)
				{
					if (list[1].Length > 2)
					{
						item = 59;
					}
					else
					{
						item = ((int.Parse(list[1]) > 59) ? 59 : int.Parse(list[1]));
					}
				}
				list2.Add(item);
				if (list2[0] == 24)
				{
					list2[0] = 0;
				}
				list[0] = list2[0].ToString();
				list[1] = list2[1].ToString();
				while (list2[0] > 0)
				{
					list2[0] = list2[0] - 1;
					list2[1] = list2[1] + 60;
				}
				list2[1] = list2[1] * 60 - 16200 + ((list2[1] * 60 - 16200 < 0) ? 86400 : 0);
				list2[1] = list2[1] % 86400;
				Main.dayTime = (list2[1] < 54000);
				Main.time = (double)((!Main.dayTime) ? (list2[1] - 54000) : list2[1]);
				Mystagogue.Output(string.Concat(new object[]
				{
					"Time set to ",
					list[0],
					":",
					list[1]
				}), false);
				NetMessage.TrySendData(7, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
			});
		}

		public MystagogueCMD(string Command_Name, Action Delegate_Function)
		{
			this.name = Command_Name;
			this.desc = "Description yet to be written.";
			this.func = Delegate_Function;
			MystagogueCMD.library.Add(this.name, this);
		}

		public string name;

		public string desc;

		public Action func;

		public static Dictionary<string, MystagogueCMD> library = new Dictionary<string, MystagogueCMD>();

		private static Dictionary<string, Func<Item, bool>> tags;

		private static string[] Prefixes = new string[]
		{
			"Basic",
			"Large",
			"Massive",
			"Dangerous",
			"Savage",
			"Sharp",
			"Pointy",
			"Tiny",
			"Terrible",
			"Small",
			"Dull",
			"Unhappy",
			"Bulky",
			"Shameful",
			"Heavy",
			"Light",
			"Sighted",
			"Rapid",
			"Hasty",
			"Intimidating",
			"Deadly",
			"Staunch",
			"Awful",
			"Lethargic",
			"Awkward",
			"Powerful",
			"Mystic",
			"Adept",
			"Masterful",
			"Inept",
			"Ignorant",
			"Deranged",
			"Intense",
			"Taboo",
			"Celestial",
			"Furious",
			"Keen",
			"Superior",
			"Forceful",
			"Broken",
			"Damaged",
			"Shoddy",
			"Quick",
			"Deadly",
			"Agile",
			"Nimble",
			"Murderous",
			"Slow",
			"Sluggish",
			"Lazy",
			"Annoying",
			"Nasty",
			"Manic",
			"Hurtful",
			"Strong",
			"Unpleasant",
			"Weak",
			"Ruthless",
			"Frenzying",
			"Godly",
			"Demonic",
			"Zealous",
			"Hard",
			"Guarding",
			"Armored",
			"Warding",
			"Arcane",
			"Precise",
			"Lucky",
			"Jagged",
			"Spiked",
			"Angry",
			"Menacing",
			"Brisk",
			"Fleeting",
			"Hasty",
			"Quick",
			"Wild",
			"Rash",
			"Intrepid",
			"Violent",
			"Legendary",
			"Unreal",
			"Mythical"
		};
	}
}
