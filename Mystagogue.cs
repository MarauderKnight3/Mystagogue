using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.Graphics.Capture;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.UI;

namespace Terraria
{
	public static class Mystagogue
	{
		public static bool TryDupe(Item slot)
		{
			bool result;
			if (Main.cursorOverride == 3)
			{
				if (Main.mouseRight && Main.mouseRightRelease && (Main.mouseItem.IsTheSameAs(slot) | Main.mouseItem.IsAir))
				{
					Main.mouseItem = new Item();
					Main.mouseItem = slot.Clone();
					Main.mouseItem.favorited = false;
					if (ItemSlot.ControlInUse && slot.maxStack != 1)
					{
						Main.mouseItem.stack = int.MaxValue;
					}
					else
					{
						Main.mouseItem.stack = Main.mouseItem.maxStack;
					}
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public static void AttemptTeleport()
		{
			if (Main.player[Main.myPlayer].MystagogueTeleportSetting != 0 && ((PlayerInput.Triggers.JustPressed.MouseRight && Main.player[Main.myPlayer].MystagogueTeleportSetting == 1) | (PlayerInput.Triggers.Current.MouseRight && Main.player[Main.myPlayer].MystagogueTeleportSetting == 2)) && !Main.player[Main.myPlayer].tileInteractionHappened && !Main.player[Main.myPlayer].mouseInterface && !CaptureManager.Instance.Active && !Main.HoveringOverAnNPC && !Main.SmartInteractShowingGenuine && !Main.player[Main.myPlayer].HeldItem.summon && Main.player[Main.myPlayer].HeldItem.type != 3384 && Main.player[Main.myPlayer].HeldItem.type != 3858 && Main.player[Main.myPlayer].HeldItem.type != 4673 && Main.player[Main.myPlayer].HeldItem.type != 3852 && Main.player[Main.myPlayer].HeldItem.type != 3611 && !Main.player[Main.myPlayer].scope)
			{
				Vector2 vector = default(Vector2);
				vector.X = (float)Main.mouseX + Main.screenPosition.X;
				if (Main.player[Main.myPlayer].gravDir == 1f)
				{
					vector.Y = (float)Main.mouseY + Main.screenPosition.Y - (float)Main.player[Main.myPlayer].height;
				}
				else
				{
					vector.Y = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY;
				}
				vector.X -= (float)(Main.player[Main.myPlayer].width / 2);
				if (vector.X > 50f && vector.X < (float)(Main.maxTilesX * 16 - 50) && vector.Y > 50f && vector.Y < (float)(Main.maxTilesY * 16 - 50))
				{
					try
					{
						Main.player[Main.myPlayer].RemoveAllGrapplingHooks();
						Main.player[Main.myPlayer].StopVanityActions(true);
						MathHelper.Clamp(1f - Main.player[Main.myPlayer].teleportTime * 0.99f, 0.01f, 1f);
						Vector2 position = Main.player[Main.myPlayer].position;
						float num = Vector2.Distance(Main.player[Main.myPlayer].position, vector);
						PressurePlateHelper.UpdatePlayerPosition(Main.player[Main.myPlayer]);
						Main.player[Main.myPlayer].position = vector;
						Main.player[Main.myPlayer].fallStart = (int)(Main.player[Main.myPlayer].position.Y / 16f);
						if (Main.player[Main.myPlayer].whoAmI == Main.myPlayer)
						{
							if (Main.player[Main.myPlayer].MystagogueTeleportSetting == 1)
							{
								if (num < new Vector2((float)Main.screenWidth, (float)Main.screenHeight).Length() / 2f + 100f)
								{
									Main.SetCameraLerp(0.1f, 2);
								}
								else
								{
									Main.BlackFadeIn = 255;
									Lighting.Clear();
									Main.screenLastPosition = Main.screenPosition;
									Main.screenPosition.X = Main.player[Main.myPlayer].position.X + (float)(Main.player[Main.myPlayer].width / 2) - (float)(Main.screenWidth / 2);
									Main.screenPosition.Y = Main.player[Main.myPlayer].position.Y + (float)(Main.player[Main.myPlayer].height / 2) - (float)(Main.screenHeight / 2);
									Main.instantBGTransitionCounter = 10;
								}
							}
							if (Main.mapTime < 5)
							{
								Main.mapTime = 5;
							}
							Main.maxQ = true;
							Main.renderNow = true;
						}
						PressurePlateHelper.UpdatePlayerPosition(Main.player[Main.myPlayer]);
						Main.player[Main.myPlayer].ResetAdvancedShadows();
						for (int i = 0; i < 3; i++)
						{
							Main.player[Main.myPlayer].UpdateSocialShadow();
						}
						Main.player[Main.myPlayer].oldPosition = Main.player[Main.myPlayer].position + Main.player[Main.myPlayer].BlehOldPositionFixer;
						Main.player[Main.myPlayer].teleportTime = 0f;
						Main.player[Main.myPlayer].teleportStyle = 2;
					}
					catch
					{
					}
					NetMessage.SendData(13, -1, -1, null, Main.myPlayer, 0f, 0f, 0f, 0, 0, 0);
				}
			}
		}

		public static void Command(string raw)
		{
			if (raw.EndsWith(";;") && raw.Length > 2)
			{
				Mystagogue.CommandArgs = raw.Substring(0, raw.Length - 2).Split(new char[]
				{
					' '
				}).ToList<string>();
				for (;;)
				{
					IL_4B:
					int num = 0;
					int num2 = 0;
					bool flag = false;
					foreach (string text in Mystagogue.CommandArgs)
					{
						if (text.StartsWith("\"") && text.EndsWith("\"") && text.Length > 2)
						{
							Mystagogue.CommandArgs[num] = Mystagogue.CommandArgs[num].Substring(1, Mystagogue.CommandArgs[num].Length - 2);
							goto IL_4B;
						}
						if (text.StartsWith("\"") && !flag)
						{
							num2 = num;
							flag = true;
						}
						else if (text.EndsWith("\"") && flag)
						{
							string text2 = Mystagogue.CommandArgs[num2];
							for (int i = 0; i < num - num2; i++)
							{
								text2 = text2 + " " + Mystagogue.CommandArgs[1 + num2];
								Mystagogue.CommandArgs.RemoveAt(1 + num2);
							}
							Mystagogue.CommandArgs[num2] = text2.Substring(1, text2.Length - 2);
							if (string.IsNullOrWhiteSpace(Mystagogue.CommandArgs[num2]))
							{
								Mystagogue.CommandArgs.RemoveAt(num2);
							}
							goto IL_4B;
						}
						num++;
					}
					break;
				}
				Mystagogue.CommandArgs = (from arg in Mystagogue.CommandArgs
				where !string.IsNullOrWhiteSpace(arg)
				select arg).ToList<string>();
				string[] array = new string[]
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
				Mystagogue.CommandArgs[0] = Mystagogue.CommandArgs[0].ToLower();
				bool flag2 = false;
				Dictionary<string, Func<Item, bool>> tags = new Dictionary<string, Func<Item, bool>>();
				tags.Add("block", (Item input) => input.createTile >= 0 && !Main.tileFrameImportant[input.createTile]);
				tags.Add("wall", (Item input) => input.createWall >= 0);
				tags.Add("object", (Item input) => input.createTile >= 0 && Main.tileFrameImportant[input.createTile]);
				tags.Add("tool", (Item input) => input.pick != 0 || input.axe != 0 || input.hammer != 0);
				tags.Add("armor", (Item input) => !input.vanity && (input.headSlot > -1 || input.bodySlot > -1 || input.legSlot > -1));
				tags.Add("acc", (Item input) => input.accessory && !input.vanity && input.createTile == -1);
				tags.Add("vanityarmor", (Item input) => input.vanity && (input.headSlot > -1 || input.bodySlot > -1 || input.legSlot > -1));
				tags.Add("vanityacc", (Item input) => input.vanity && input.headSlot == -1 && input.bodySlot == -1 && input.legSlot == -1);
				tags.Add("dye", (Item input) => GameShaders.Armor.GetShaderIdFromItemId(input.type) > 0 || GameShaders.Hair.GetShaderIdFromItemId(input.type) >= 0);
				tags.Add("damage", (Item input) => input.damage != -1 && input.pick == 0 && input.axe == 0 && input.hammer == 0);
				tags.Add("melee", (Item input) => input.melee && input.pick == 0 && input.axe == 0 && input.hammer == 0);
				tags.Add("ranged", (Item input) => input.ranged && input.ammo == AmmoID.None);
				tags.Add("magic", (Item input) => input.magic);
				tags.Add("summon", (Item input) => input.summon);
				tags.Add("potion", (Item input) => (input.buffType != 0 || input.potion || input.healMana > 0) && !Main.lightPet[input.buffType] && !Main.vanityPet[input.buffType] && !input.summon && input.mountType == -1);
				tags.Add("consumable", (Item input) => input.consumable && input.createTile == -1 && input.createWall == -1);
				tags.Add("ammo", (Item input) => input.ammo != AmmoID.None);
				tags.Add("mount", (Item input) => input.mountType != -1 && !MountID.Sets.Cart[input.mountType]);
				tags.Add("minecart", (Item input) => input.cartTrack || (input.mountType != -1 && MountID.Sets.Cart[input.mountType]));
				tags.Add("material", (Item input) => input.material && input.createTile == -1 && input.createWall == -1 && !input.accessory && input.damage == -1);
				tags.Add("quest", (Item input) => input.questItem);
				tags.Add("fishing", (Item input) => input.fishingPole >= 1 || input.bait >= 1);
				tags.Add("expert", (Item input) => input.expert);
				tags.Add("depreciated", (Item input) => ItemID.Sets.Deprecated[input.type]);
				Dictionary<string, Action> dictionary = new Dictionary<string, Action>();
				dictionary.Add("s", delegate
				{
					Mystagogue.Command("god;;");
					Mystagogue.Command("killdebuffs;;");
					Mystagogue.Command("tps;;");
					Mystagogue.Command("refills;;");
					Mystagogue.Command("infflight;;");
					Mystagogue.Command("boost 7;;");
					Mystagogue.Command("p2pmaphack;;");
					Mystagogue.Command("maxminions 200;;");
					Mystagogue.Command("toolgod;;");
					Mystagogue.Command("jesus;;");
				});
				dictionary.Add("i", delegate
				{
					if (Mystagogue.CommandArgs.Count == 1)
					{
						Mystagogue.Output("That command requires arguments");
						return;
					}
					int k = 0;
					List<string> list = new List<string>();
					if (!new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
					{
						string text3 = Mystagogue.CommandArgs[1];
						while (text3.StartsWith("0"))
						{
							text3 = text3.Remove(0, 1);
						}
						while (text3 != k.ToString())
						{
							if (k == 5044)
							{
								Mystagogue.Output("Given item ID does not correspond to an item");
								return;
							}
							k++;
						}
						for (int l = 0; l < Mystagogue.CommandArgs.Count - 2; l++)
						{
							list.Add(Mystagogue.CommandArgs[2 + l]);
						}
					}
					else
					{
						k = 1;
						List<int> list2 = new List<int>();
						while (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[k]))
						{
							k++;
							if (k == Mystagogue.CommandArgs.Count)
							{
								break;
							}
						}
						string text4 = string.Join(" ", Mystagogue.CommandArgs.GetRange(1, k - 1)).ToUpper();
						int num3 = k;
						while (Mystagogue.CommandArgs.Count - num3 != 0)
						{
							list.Add(Mystagogue.CommandArgs[num3]);
							num3++;
						}
						for (k = 0; k < 5045; k++)
						{
							if (Lang.GetItemNameValue(k).ToUpper().StartsWith(text4))
							{
								list2.Add(k);
							}
						}
						if (list2.Count == 0)
						{
							Mystagogue.Output("No item names match");
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
							foreach (int num4 in list2)
							{
								list4.Add(string.Concat(new object[]
								{
									Lang.GetItemNameValue(num4),
									" (",
									num4,
									")"
								}));
							}
							bool flag3 = false;
							for (int m = 0; m < list3.Count; m++)
							{
								if (list3[m] == text4)
								{
									k = list2[m];
									list4.RemoveAt(m);
									Mystagogue.Output("Other matches include " + string.Join(", ", list4));
									break;
								}
								if (m + 1 == list2.Count && !flag3)
								{
									Mystagogue.Output("Found " + string.Join(", ", list4));
									return;
								}
							}
						}
						else
						{
							k = list2[0];
						}
					}
					int num5 = k;
					int stack = 1;
					int num6 = 0;
					if (list.Count >= 1)
					{
						if (new Regex("\\D").IsMatch(list[0]))
						{
							Mystagogue.Output("Stack must be a positive integer");
							return;
						}
						string text5 = list[0];
						while (text5.StartsWith("0"))
						{
							text5 = text5.Remove(0, 1);
						}
						if (text5.Length > 10)
						{
							stack = int.MaxValue;
						}
						else if (Convert.ToInt64(text5) > 2147483647L)
						{
							stack = int.MaxValue;
						}
						else if (text5.Length > 0)
						{
							stack = int.Parse(text5);
						}
					}
					if (list.Count >= 2)
					{
						if (!new Regex("\\D").IsMatch(list[1]))
						{
							string text6 = list[1];
							while (text6.StartsWith("0"))
							{
								text6 = text6.Remove(0, 1);
							}
							k = 0;
							while (text6 != k.ToString())
							{
								if (k == 83)
								{
									Mystagogue.Output("Given prefix ID does not correspond to a prefix");
									return;
								}
								k++;
							}
						}
						else
						{
							string value = list[1].ToUpper();
							List<int> list5 = new List<int>();
							for (k = 0; k < array.Length; k++)
							{
								if (array[k].ToUpper().StartsWith(value) && k != 75 && k != 43 && k != 76)
								{
									list5.Add(k);
								}
							}
							if (list5.Count == 0)
							{
								Mystagogue.Output("No prefix names match");
								return;
							}
							if (list5.Count > 1)
							{
								List<string> list6 = new List<string>();
								foreach (int num7 in list5)
								{
									list6.Add(string.Concat(new object[]
									{
										array[num7],
										" (",
										num7,
										")"
									}));
								}
								Mystagogue.Output("Found " + string.Join(", ", list6));
								return;
							}
							k = list5[0];
						}
						if (k == 18 || k == 75)
						{
							if (!ContentSamples.ItemsByType[num5].accessory)
							{
								k = 18;
							}
							else
							{
								k = 75;
							}
						}
						if (k == 20 || k == 43)
						{
							if (ContentSamples.ItemsByType[num5].ranged)
							{
								k = 20;
							}
							else
							{
								k = 43;
							}
						}
						if (k == 42 || k == 76)
						{
							if (!ContentSamples.ItemsByType[num5].accessory)
							{
								k = 42;
							}
							else
							{
								k = 76;
							}
						}
						num6 = k;
					}
					Main.mouseItem.SetDefaults(num5);
					Main.mouseItem.stack = stack;
					Main.mouseItem.prefix = (byte)num6;
					Main.mouseItem.Refresh();
					string text7 = "";
					if (Main.mouseItem.prefix > 0)
					{
						text7 = " " + array[(int)Main.mouseItem.prefix];
					}
					Mystagogue.Output(string.Concat(new object[]
					{
						"Set cursor item to ",
						Main.mouseItem.stack,
						text7,
						" ",
						Lang.GetItemNameValue(Main.mouseItem.type),
						" (",
						Main.mouseItem.type,
						")"
					}));
				});
				dictionary.Add("search", delegate
				{
					if (Mystagogue.CommandArgs.Count == 1)
					{
						Mystagogue.Output("That command requires arguments");
						return;
					}
					List<int> list = new List<int>();
					string value = string.Join(" ", Mystagogue.CommandArgs.GetRange(1, Mystagogue.CommandArgs.Count - 1)).ToUpper();
					for (int k = 0; k < 5045; k++)
					{
						if (Lang.GetItemNameValue(k).ToUpper().Contains(value))
						{
							list.Add(k);
						}
					}
					if (list.Count == 0)
					{
						Mystagogue.Output("No item names match");
						return;
					}
					List<string> list2 = new List<string>();
					foreach (int num3 in list)
					{
						list2.Add(string.Concat(new object[]
						{
							Lang.GetItemNameValue(num3),
							" (",
							num3,
							")"
						}));
					}
					Mystagogue.Output("Found " + string.Join(", ", list2));
				});
				dictionary.Add("sl", delegate
				{
					if (Mystagogue.FirstFreedRecipeSlot == 0)
					{
						int num3 = Recipe.maxRecipes - 1;
						while (Main.recipe[num3].createItem.type == 0)
						{
							Mystagogue.FirstFreedRecipeSlot = num3;
							num3--;
						}
					}
					if (Mystagogue.CommandArgs.Count == 1)
					{
						Mystagogue.Output("Resetting the spawnlist");
						for (int k = Mystagogue.FirstFreedRecipeSlot; k < Recipe.maxRecipes; k++)
						{
							Main.recipe[k] = new Recipe();
						}
						Recipe.numRecipes = Mystagogue.FirstFreedRecipeSlot;
						Recipe.FindRecipes(false);
						return;
					}
					int num4 = Recipe.maxRecipes - Mystagogue.FirstFreedRecipeSlot;
					List<int> list = new List<int>();
					List<string> list10 = Mystagogue.CommandArgs.GetRange(1, Mystagogue.CommandArgs.Count - 1);
					int num5 = 0;
					string text3 = string.Join(" ", list10).Trim();
					if (text3.Contains(","))
					{
						list10 = (from arg in text3.Split(new char[]
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
					text3 = text3.Substring(0, text3.Contains(",") ? text3.IndexOf(",") : text3.Length).Trim();
					for (int l = 1; l < 5045; l++)
					{
						if (Lang.GetItemNameValue(l).ToUpper().Contains(text3.ToUpper()) || text3 == "!")
						{
							list.Add(l);
						}
					}
					if (list10.Count > 0)
					{
						bool flag3 = false;
						for (int m = 0; m < list10.Count; m++)
						{
							if (flag3)
							{
								flag3 = false;
								if (new Regex("\\D").IsMatch(list10[m]))
								{
									Mystagogue.Output("Page must be a positive integer");
								}
								else
								{
									string text4 = list10[m];
									while (text4.StartsWith("0"))
									{
										text4 = text4.Remove(0, 1);
									}
									if (text4.Length > 2)
									{
										num5 = 99;
									}
									else if (text4.Length > 0)
									{
										num5 = int.Parse(text4);
									}
									if (num5 > 0)
									{
										num5--;
									}
								}
								list10.RemoveAt(m);
								m--;
							}
							else if (list10[m] == "page")
							{
								flag3 = true;
								list10.RemoveAt(m);
								m--;
							}
						}
						bool usedCat = false;
						Func<int, List<string>, bool> Chosen = delegate(int input, List<string> args)
						{
							Item item = new Item();
							item.SetDefaults(input);
							foreach (string key in args)
							{
								if (tags.ContainsKey(key))
								{
									usedCat = true;
								}
								if (tags.ContainsKey(key) && tags[key](item))
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
							Mystagogue.Output("Categories: " + string.Join(", ", new List<string>(tags.Keys)));
						}
					}
					int count = list.Count;
					num5 = ((num5 > (int)Math.Floor((double)list.Count / (double)num4)) ? ((int)Math.Floor((double)list.Count / (double)num4)) : num5);
					list = list.GetRange(num5 * num4, (list.Count - num5 * num4 < num4) ? (list.Count - num5 * num4) : num4);
					if (list.Count == 0)
					{
						Mystagogue.Output("There was no matching items");
						return;
					}
					for (int n = Mystagogue.FirstFreedRecipeSlot; n < Recipe.maxRecipes; n++)
					{
						Main.recipe[n] = new Recipe();
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
					string text5 = "";
					string text6 = "";
					if (text3 == "!")
					{
						text5 = "items ";
					}
					else
					{
						text6 = " results for '" + text3 + "'";
					}
					Mystagogue.Output(string.Concat(new object[]
					{
						"Listing ",
						text5,
						num5 * num4 + 1,
						"-",
						num5 * num4 + ((count - num5 * num4 < num4) ? (count - num5 * num4) : num4),
						" of ",
						count,
						text6,
						(count > num4) ? string.Concat(new string[]
						{
							" (Page ",
							(num5 + 1).ToString(),
							" of ",
							((int)Math.Floor((double)count / (double)num4) + 1).ToString(),
							")"
						}) : ""
					}));
				});
				dictionary.Add("reforge", delegate
				{
					if (Main.mouseItem.IsAir)
					{
						Mystagogue.Output("Must be holding an item with the cursor");
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
						}));
						return;
					}
					int k = 0;
					if (!new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
					{
						string text3 = Mystagogue.CommandArgs[1];
						while (text3.StartsWith("0"))
						{
							text3 = text3.Remove(0, 1);
						}
						while (text3 != k.ToString())
						{
							if (k == 83)
							{
								Mystagogue.Output("Given prefix ID does not correspond to a prefix");
								return;
							}
							k++;
						}
					}
					else
					{
						string value = Mystagogue.CommandArgs[1].ToUpper();
						List<int> list = new List<int>();
						for (k = 0; k < array.Length; k++)
						{
							if (array[k].ToUpper().StartsWith(value) && k != 75 && k != 43 && k != 76)
							{
								list.Add(k);
							}
						}
						if (list.Count == 0)
						{
							Mystagogue.Output("No prefix names match");
							return;
						}
						if (list.Count > 1)
						{
							List<string> list2 = new List<string>();
							foreach (int num3 in list)
							{
								list2.Add(string.Concat(new object[]
								{
									array[num3],
									" (",
									num3,
									")"
								}));
							}
							Mystagogue.Output("Found " + string.Join(", ", list2));
							return;
						}
						k = list[0];
					}
					if (k == 18 || k == 75)
					{
						if (!ContentSamples.ItemsByType[Main.mouseItem.type].accessory)
						{
							k = 18;
						}
						else
						{
							k = 75;
						}
					}
					if (k == 20 || k == 43)
					{
						if (ContentSamples.ItemsByType[Main.mouseItem.type].ranged)
						{
							k = 20;
						}
						else
						{
							k = 43;
						}
					}
					if (k == 42 || k == 76)
					{
						if (!ContentSamples.ItemsByType[Main.mouseItem.type].accessory)
						{
							k = 42;
						}
						else
						{
							k = 76;
						}
					}
					Main.mouseItem.prefix = (byte)k;
					Main.mouseItem.Refresh();
					string text4 = "";
					if (Main.mouseItem.prefix > 0)
					{
						text4 = " " + array[(int)Main.mouseItem.prefix];
					}
					Mystagogue.Output(string.Concat(new object[]
					{
						"Reforged item ",
						Main.mouseItem.stack,
						text4,
						" ",
						Lang.GetItemNameValue(Main.mouseItem.type),
						" (",
						Main.mouseItem.type,
						")"
					}));
				});
				dictionary.Add("rename", delegate
				{
					if (Main.mouseItem.IsAir)
					{
						Mystagogue.Output("Must be holding an item with the cursor");
						return;
					}
					if (Mystagogue.CommandArgs.Count == 1)
					{
						Main.mouseItem.ClearNameOverride();
						Mystagogue.Output("Item name reset");
						return;
					}
					Main.mouseItem.SetNameOverride(string.Join(" ", Mystagogue.CommandArgs.GetRange(1, Mystagogue.CommandArgs.Count - 1)));
					Mystagogue.Output("Item renamed");
				});
				dictionary.Add("ut", delegate
				{
					if (Mystagogue.CommandArgs.Count != 1 && !Main.player[Main.myPlayer].HeldItem.IsAir)
					{
						if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
						{
							Mystagogue.Output("Must be a positive integer");
							return;
						}
						string text3 = Mystagogue.CommandArgs[1];
						while (text3.StartsWith("0"))
						{
							text3 = text3.Remove(0, 1);
						}
						int useTime;
						if (text3.Length != 0)
						{
							if (text3.Length > 2)
							{
								useTime = 99;
							}
							else
							{
								useTime = int.Parse(text3);
							}
						}
						else
						{
							useTime = 0;
						}
						Main.player[Main.myPlayer].HeldItem.useTime = useTime;
						Mystagogue.Output("Use time " + Main.player[Main.myPlayer].HeldItem.useTime);
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
							Mystagogue.Output("Use time " + Main.player[Main.myPlayer].HeldItem.useTime);
							return;
						}
						Mystagogue.Output("Must be holding an item in the hotbar");
						return;
					}
				});
				dictionary.Add("at", delegate
				{
					if (Mystagogue.CommandArgs.Count != 1 && !Main.player[Main.myPlayer].HeldItem.IsAir)
					{
						if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
						{
							Mystagogue.Output("Must be a positive integer");
							return;
						}
						string text3 = Mystagogue.CommandArgs[1];
						while (text3.StartsWith("0"))
						{
							text3 = text3.Remove(0, 1);
						}
						int num3;
						if (text3.Length != 0)
						{
							if (text3.Length > 2)
							{
								num3 = 99;
							}
							else
							{
								num3 = int.Parse(text3);
								if (num3 < 3)
								{
									num3 = 3;
								}
							}
						}
						else
						{
							num3 = 3;
						}
						Main.player[Main.myPlayer].HeldItem.useAnimation = num3;
						Mystagogue.Output("Animation time " + Main.player[Main.myPlayer].HeldItem.useAnimation);
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
							Mystagogue.Output("Animation time " + Main.player[Main.myPlayer].HeldItem.useAnimation);
							return;
						}
						Mystagogue.Output("Must be holding an item in the hotbar");
						return;
					}
				});
				dictionary.Add("shoot", delegate
				{
					if (Mystagogue.CommandArgs.Count == 1)
					{
						Main.player[Main.myPlayer].HeldItem.shoot = ContentSamples.ItemsByType[Main.player[Main.myPlayer].HeldItem.type].shoot;
						Main.player[Main.myPlayer].HeldItem.useAmmo = ContentSamples.ItemsByType[Main.player[Main.myPlayer].HeldItem.type].useAmmo;
						Mystagogue.Output("Item now shoots its original projectile");
						return;
					}
					if (Main.player[Main.myPlayer].HeldItem.IsAir)
					{
						Mystagogue.Output("Must be holding an item in the hotbar");
						return;
					}
					if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
					{
						Mystagogue.Output("Must be a positive integer");
						return;
					}
					string text3 = Mystagogue.CommandArgs[1];
					while (text3.StartsWith("0"))
					{
						text3 = text3.Remove(0, 1);
					}
					int num3 = 0;
					while (text3 != num3.ToString())
					{
						if (num3 == 949)
						{
							Mystagogue.Output("Given projectile ID does not correspond to a projectile");
							return;
						}
						num3++;
					}
					Main.player[Main.myPlayer].HeldItem.shoot = num3;
					Main.player[Main.myPlayer].HeldItem.useAmmo = AmmoID.None;
					Mystagogue.Output(string.Concat(new object[]
					{
						"Item now shoots ",
						Lang.GetProjectileName(Main.player[Main.myPlayer].HeldItem.shoot),
						" (",
						Main.player[Main.myPlayer].HeldItem.shoot,
						") if applicable"
					}));
				});
				dictionary.Add("auto", delegate
				{
					if (!Main.player[Main.myPlayer].HeldItem.IsAir)
					{
						if (Mystagogue.CommandArgs.Count > 1)
						{
							if (Mystagogue.CommandArgs[1] == "off")
							{
								Main.player[Main.myPlayer].HeldItem.autoReuse = false;
								Mystagogue.Output("Item won't autoswing");
							}
							if (Mystagogue.CommandArgs[1] == "on")
							{
								Main.player[Main.myPlayer].HeldItem.autoReuse = true;
								Mystagogue.Output("Item will autoswing");
								return;
							}
						}
						else
						{
							Main.player[Main.myPlayer].HeldItem.autoReuse = !Main.player[Main.myPlayer].HeldItem.autoReuse;
							if (Main.player[Main.myPlayer].HeldItem.autoReuse)
							{
								Mystagogue.Output("Item will autoswing");
								return;
							}
							Mystagogue.Output("Item won't autoswing");
							return;
						}
					}
					else
					{
						Mystagogue.Output("Must be holding an item in the hotbar");
					}
				});
				dictionary.Add("scale", delegate
				{
					if (Mystagogue.CommandArgs.Count != 1 && !Main.player[Main.myPlayer].HeldItem.IsAir)
					{
						if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
						{
							Mystagogue.Output("Must be a positive integer");
							return;
						}
						string text3 = Mystagogue.CommandArgs[1];
						while (text3.StartsWith("0"))
						{
							text3 = text3.Remove(0, 1);
						}
						int num3;
						if (text3.Length != 0)
						{
							if (text3.Length > 5)
							{
								num3 = 10000;
							}
							else
							{
								num3 = int.Parse(text3);
								if (num3 > 10000)
								{
									num3 = 10000;
								}
							}
						}
						else
						{
							num3 = 1;
						}
						Main.player[Main.myPlayer].HeldItem.scale = (float)(num3 / 100);
						Mystagogue.Output("Scale " + Main.player[Main.myPlayer].HeldItem.scale * 100f + "%");
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
							Mystagogue.Output("Scale " + Main.player[Main.myPlayer].HeldItem.scale * 100f + "%");
							return;
						}
						Mystagogue.Output("Must be holding an item in the hotbar");
						return;
					}
				});
				dictionary.Add("stack", delegate
				{
					if (Mystagogue.CommandArgs.Count != 1 && !Main.mouseItem.IsAir)
					{
						if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
						{
							Mystagogue.Output("Stack must be a positive integer");
							return;
						}
						string text3 = Mystagogue.CommandArgs[1];
						int stack = Main.mouseItem.maxStack;
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
						Main.mouseItem.stack = stack;
						Mystagogue.Output("Stack set to " + Main.mouseItem.stack);
						return;
					}
					else
					{
						if (!Main.mouseItem.IsAir)
						{
							Main.mouseItem.stack = Main.mouseItem.maxStack;
							Mystagogue.Output("Stack set to " + Main.mouseItem.stack);
							return;
						}
						Mystagogue.Output("Must be holding an item with the cursor");
						return;
					}
				});
				dictionary.Add("maxstack", delegate
				{
					if (Mystagogue.CommandArgs.Count != 1 && !Main.mouseItem.IsAir)
					{
						if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
						{
							Mystagogue.Output("Must be a positive integer");
							return;
						}
						string text3 = Mystagogue.CommandArgs[1];
						int maxStack = ContentSamples.ItemsByType[Main.mouseItem.type].maxStack;
						while (text3.StartsWith("0"))
						{
							text3 = text3.Remove(0, 1);
						}
						if (text3.Length > 10)
						{
							maxStack = int.MaxValue;
						}
						else if (Convert.ToInt64(text3) > 2147483647L)
						{
							maxStack = int.MaxValue;
						}
						else if (text3.Length > 0)
						{
							maxStack = int.Parse(text3);
						}
						Main.mouseItem.maxStack = maxStack;
						Mystagogue.Output("Max stack set to " + Main.mouseItem.maxStack);
						return;
					}
					else
					{
						if (!Main.mouseItem.IsAir)
						{
							Main.mouseItem.maxStack = ContentSamples.ItemsByType[Main.mouseItem.type].maxStack;
							Mystagogue.Output("Max stack set to " + Main.mouseItem.maxStack);
							return;
						}
						Mystagogue.Output("Must be holding an item with the cursor");
						return;
					}
				});
				dictionary.Add("dmg", delegate
				{
					if (Mystagogue.CommandArgs.Count != 1 && !Main.player[Main.myPlayer].HeldItem.IsAir)
					{
						if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
						{
							Mystagogue.Output("Must be a positive integer");
							return;
						}
						string text3 = Mystagogue.CommandArgs[1];
						while (text3.StartsWith("0"))
						{
							text3 = text3.Remove(0, 1);
						}
						int damage;
						if (text3.Length != 0)
						{
							if (text3.Length > 7)
							{
								damage = 999999;
							}
							else
							{
								damage = int.Parse(text3);
							}
						}
						else
						{
							damage = 0;
						}
						Main.player[Main.myPlayer].HeldItem.damage = damage;
						Mystagogue.Output("Item now deals " + Main.player[Main.myPlayer].HeldItem.damage + " damage");
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
							Mystagogue.Output("Item now deals " + Main.player[Main.myPlayer].HeldItem.damage + " damage");
							return;
						}
						Mystagogue.Output("Must be holding an item in the hotbar");
						return;
					}
				});
				dictionary.Add("crit", delegate
				{
					if (Mystagogue.CommandArgs.Count != 1 && !Main.player[Main.myPlayer].HeldItem.IsAir)
					{
						if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
						{
							Mystagogue.Output("Must be a positive integer");
							return;
						}
						string text3 = Mystagogue.CommandArgs[1];
						while (text3.StartsWith("0"))
						{
							text3 = text3.Remove(0, 1);
						}
						int num3;
						if (text3.Length != 0)
						{
							if (text3.Length > 3)
							{
								num3 = 100;
							}
							else
							{
								num3 = int.Parse(text3);
								if (num3 > 100)
								{
									num3 = 100;
								}
							}
						}
						else
						{
							num3 = 0;
						}
						Main.player[Main.myPlayer].HeldItem.crit = num3;
						Mystagogue.Output("Item now has " + Main.player[Main.myPlayer].HeldItem.crit + " critical chance");
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
							Mystagogue.Output("Item now has " + Main.player[Main.myPlayer].HeldItem.crit + " critical chance");
							return;
						}
						Mystagogue.Output("Must be holding an item in the hotbar");
						return;
					}
				});
				dictionary.Add("veloc", delegate
				{
					if (Mystagogue.CommandArgs.Count != 1 && !Main.player[Main.myPlayer].HeldItem.IsAir)
					{
						if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
						{
							Mystagogue.Output("Must be a positive integer");
							return;
						}
						string text3 = Mystagogue.CommandArgs[1];
						while (text3.StartsWith("0"))
						{
							text3 = text3.Remove(0, 1);
						}
						int num3;
						if (text3.Length != 0)
						{
							if (text3.Length > 2)
							{
								num3 = 60;
							}
							else
							{
								num3 = int.Parse(text3);
								if (num3 > 60)
								{
									num3 = 60;
								}
							}
						}
						else
						{
							num3 = 0;
						}
						Main.player[Main.myPlayer].HeldItem.shootSpeed = (float)num3;
						Mystagogue.Output("Item now has a shoot velocity of " + Main.player[Main.myPlayer].HeldItem.shootSpeed);
						return;
					}
					else
					{
						if (!Main.player[Main.myPlayer].HeldItem.IsAir)
						{
							Main.player[Main.myPlayer].HeldItem.shootSpeed = ContentSamples.ItemsByType[Main.player[Main.myPlayer].HeldItem.type].shootSpeed;
							Mystagogue.Output("Item now has a shoot velocity of " + Main.player[Main.myPlayer].HeldItem.shootSpeed);
							return;
						}
						Mystagogue.Output("Must be holding an item in the hotbar");
						return;
					}
				});
				dictionary.Add("tboost", delegate
				{
					if (Mystagogue.CommandArgs.Count != 1 && !Main.player[Main.myPlayer].HeldItem.IsAir)
					{
						if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
						{
							Mystagogue.Output("Must be a positive integer");
							return;
						}
						string text3 = Mystagogue.CommandArgs[1];
						while (text3.StartsWith("0"))
						{
							text3 = text3.Remove(0, 1);
						}
						int num3;
						if (text3.Length != 0)
						{
							if (text3.Length > 2)
							{
								num3 = 55;
							}
							else
							{
								num3 = int.Parse(text3);
								if (num3 > 55)
								{
									num3 = 55;
								}
							}
						}
						else
						{
							num3 = 0;
						}
						Main.player[Main.myPlayer].HeldItem.tileBoost = num3;
						Mystagogue.Output("Item's tile boost is now " + Main.player[Main.myPlayer].HeldItem.tileBoost);
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
							Mystagogue.Output("Item's tile boost is now " + Main.player[Main.myPlayer].HeldItem.tileBoost);
							return;
						}
						Mystagogue.Output("Must be holding an item in the hotbar");
						return;
					}
				});
				dictionary.Add("pick", delegate
				{
					if (Mystagogue.CommandArgs.Count != 1 && !Main.player[Main.myPlayer].HeldItem.IsAir)
					{
						if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
						{
							Mystagogue.Output("Must be a positive integer");
							return;
						}
						string text3 = Mystagogue.CommandArgs[1];
						while (text3.StartsWith("0"))
						{
							text3 = text3.Remove(0, 1);
						}
						int num3;
						if (text3.Length != 0)
						{
							if (text3.Length > 3)
							{
								num3 = 225;
							}
							else
							{
								num3 = int.Parse(text3);
								if (num3 > 225)
								{
									num3 = 225;
								}
							}
						}
						else
						{
							num3 = 0;
						}
						Main.player[Main.myPlayer].HeldItem.pick = num3;
						Mystagogue.Output("Pickaxe power set to " + Main.player[Main.myPlayer].HeldItem.pick);
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
							Mystagogue.Output("Pickaxe power set to " + Main.player[Main.myPlayer].HeldItem.pick);
							return;
						}
						Mystagogue.Output("Must be holding an item in the hotbar");
						return;
					}
				});
				dictionary.Add("axe", delegate
				{
					if (Mystagogue.CommandArgs.Count != 1 && !Main.player[Main.myPlayer].HeldItem.IsAir)
					{
						if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
						{
							Mystagogue.Output("Must be a positive integer");
							return;
						}
						string text3 = Mystagogue.CommandArgs[1];
						while (text3.StartsWith("0"))
						{
							text3 = text3.Remove(0, 1);
						}
						int num3;
						if (text3.Length != 0)
						{
							if (text3.Length > 3)
							{
								num3 = 175;
							}
							else
							{
								num3 = int.Parse(text3);
								if (num3 > 175)
								{
									num3 = 175;
								}
							}
						}
						else
						{
							num3 = 0;
						}
						Main.player[Main.myPlayer].HeldItem.axe = (int)Math.Round((double)num3 / 5.0);
						Mystagogue.Output("Axe power set to " + Main.player[Main.myPlayer].HeldItem.axe * 5);
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
							Mystagogue.Output("Axe power set to " + Main.player[Main.myPlayer].HeldItem.axe * 5);
							return;
						}
						Mystagogue.Output("Must be holding an item in the hotbar");
						return;
					}
				});
				dictionary.Add("hammer", delegate
				{
					if (Mystagogue.CommandArgs.Count != 1 && !Main.player[Main.myPlayer].HeldItem.IsAir)
					{
						if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
						{
							Mystagogue.Output("Must be a positive integer");
							return;
						}
						string text3 = Mystagogue.CommandArgs[1];
						while (text3.StartsWith("0"))
						{
							text3 = text3.Remove(0, 1);
						}
						int num3;
						if (text3.Length != 0)
						{
							if (text3.Length > 3)
							{
								num3 = 100;
							}
							else
							{
								num3 = int.Parse(text3);
								if (num3 > 100)
								{
									num3 = 100;
								}
							}
						}
						else
						{
							num3 = 0;
						}
						Main.player[Main.myPlayer].HeldItem.hammer = num3;
						Mystagogue.Output("Hammer power set to " + Main.player[Main.myPlayer].HeldItem.hammer);
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
							Mystagogue.Output("Hammer power set to " + Main.player[Main.myPlayer].HeldItem.hammer);
							return;
						}
						Mystagogue.Output("Must be holding an item in the hotbar");
						return;
					}
				});
				dictionary.Add("toolgod", delegate
				{
					Main.player[Main.myPlayer].MystagogueToolGod = !Main.player[Main.myPlayer].MystagogueToolGod;
					string str = "no longer";
					if (Main.player[Main.myPlayer].MystagogueToolGod)
					{
						str = "now";
					}
					Mystagogue.Output("Your tools are " + str + " abundant with power");
					Mystagogue.BuffMyTools();
				});
				dictionary.Add("ri", delegate
				{
					Main.player[Main.myPlayer].HeldItem.Refresh();
					Mystagogue.Output("Held item refreshed");
					Mystagogue.BuffMyTools();
				});
				dictionary.Add("ris", delegate
				{
					Main.player[Main.myPlayer].RefreshItems();
					Mystagogue.Output("All items refreshed");
					Mystagogue.BuffMyTools();
				});
				dictionary.Add("setstacks2b", delegate
				{
					if (Main.player[Main.myPlayer].MystagogueRefills)
					{
						Mystagogue.Output("You will once again be able to deplete favorited items in your inventory");
						Main.player[Main.myPlayer].MystagogueRefills = false;
					}
					int num3 = 0;
					foreach (Item item in Main.player[Main.myPlayer].inventory.ToArray<Item>())
					{
						if (item.IsAir)
						{
							Main.player[Main.myPlayer].inventory[num3] = new Item();
						}
						else if (item.maxStack > 1)
						{
							Main.player[Main.myPlayer].inventory[num3].stack = int.MaxValue;
						}
						num3++;
					}
					num3 = 0;
					foreach (Item item2 in Main.player[Main.myPlayer].miscEquips.ToArray<Item>())
					{
						if (item2.IsAir)
						{
							Main.player[Main.myPlayer].miscEquips[num3] = new Item();
						}
						else if (item2.maxStack > 1)
						{
							Main.player[Main.myPlayer].miscEquips[num3].stack = int.MaxValue;
						}
						num3++;
					}
					num3 = 0;
					foreach (Item item3 in Main.player[Main.myPlayer].bank.item.ToArray<Item>())
					{
						if (item3.IsAir)
						{
							Main.player[Main.myPlayer].bank.item[num3] = new Item();
						}
						else if (item3.maxStack > 1)
						{
							Main.player[Main.myPlayer].bank.item[num3].stack = int.MaxValue;
						}
						num3++;
					}
					num3 = 0;
					foreach (Item item4 in Main.player[Main.myPlayer].bank2.item.ToArray<Item>())
					{
						if (item4.IsAir)
						{
							Main.player[Main.myPlayer].bank2.item[num3] = new Item();
						}
						else if (item4.maxStack > 1)
						{
							Main.player[Main.myPlayer].bank2.item[num3].stack = int.MaxValue;
						}
						num3++;
					}
					num3 = 0;
					foreach (Item item5 in Main.player[Main.myPlayer].bank3.item.ToArray<Item>())
					{
						if (item5.IsAir)
						{
							Main.player[Main.myPlayer].bank3.item[num3] = new Item();
						}
						else if (item5.maxStack > 1)
						{
							Main.player[Main.myPlayer].bank3.item[num3].stack = int.MaxValue;
						}
						num3++;
					}
					num3 = 0;
					foreach (Item item6 in Main.player[Main.myPlayer].bank4.item.ToArray<Item>())
					{
						if (item6.IsAir)
						{
							Main.player[Main.myPlayer].bank4.item[num3] = new Item();
						}
						else if (item6.maxStack > 1)
						{
							Main.player[Main.myPlayer].bank4.item[num3].stack = int.MaxValue;
						}
						num3++;
					}
					Mystagogue.Output("Stacks overflowed");
				});
				dictionary.Add("setstackslegit", delegate
				{
					int num3 = 0;
					foreach (Item item in Main.player[Main.myPlayer].inventory.ToArray<Item>())
					{
						if (item.IsAir)
						{
							Main.player[Main.myPlayer].inventory[num3] = new Item();
						}
						else if (item.maxStack > 1)
						{
							Main.player[Main.myPlayer].inventory[num3].stack = Main.player[Main.myPlayer].inventory[num3].maxStack;
						}
						num3++;
					}
					num3 = 0;
					foreach (Item item2 in Main.player[Main.myPlayer].armor.ToArray<Item>())
					{
						if (item2.IsAir)
						{
							Main.player[Main.myPlayer].armor[num3] = new Item();
						}
						else if (item2.maxStack > 1)
						{
							Main.player[Main.myPlayer].armor[num3].stack = 1;
						}
						num3++;
					}
					num3 = 0;
					foreach (Item item3 in Main.player[Main.myPlayer].dye.ToArray<Item>())
					{
						if (item3.IsAir)
						{
							Main.player[Main.myPlayer].dye[num3] = new Item();
						}
						else if (item3.maxStack > 1)
						{
							Main.player[Main.myPlayer].dye[num3].stack = 1;
						}
						num3++;
					}
					num3 = 0;
					foreach (Item item4 in Main.player[Main.myPlayer].miscEquips.ToArray<Item>())
					{
						if (item4.IsAir)
						{
							Main.player[Main.myPlayer].miscEquips[num3] = new Item();
						}
						else if (item4.maxStack > 1)
						{
							Main.player[Main.myPlayer].miscEquips[num3].stack = Main.player[Main.myPlayer].miscEquips[num3].maxStack;
						}
						num3++;
					}
					num3 = 0;
					foreach (Item item5 in Main.player[Main.myPlayer].miscDyes.ToArray<Item>())
					{
						if (item5.IsAir)
						{
							Main.player[Main.myPlayer].miscDyes[num3] = new Item();
						}
						else if (item5.maxStack > 1)
						{
							Main.player[Main.myPlayer].miscDyes[num3].stack = 1;
						}
						num3++;
					}
					num3 = 0;
					foreach (Item item6 in Main.player[Main.myPlayer].bank.item.ToArray<Item>())
					{
						if (item6.IsAir)
						{
							Main.player[Main.myPlayer].bank.item[num3] = new Item();
						}
						else if (item6.maxStack > 1)
						{
							Main.player[Main.myPlayer].bank.item[num3].stack = Main.player[Main.myPlayer].bank.item[num3].maxStack;
						}
						num3++;
					}
					num3 = 0;
					foreach (Item item7 in Main.player[Main.myPlayer].bank2.item.ToArray<Item>())
					{
						if (item7.IsAir)
						{
							Main.player[Main.myPlayer].bank2.item[num3] = new Item();
						}
						else if (item7.maxStack > 1)
						{
							Main.player[Main.myPlayer].bank2.item[num3].stack = Main.player[Main.myPlayer].bank2.item[num3].maxStack;
						}
						num3++;
					}
					num3 = 0;
					foreach (Item item8 in Main.player[Main.myPlayer].bank3.item.ToArray<Item>())
					{
						if (item8.IsAir)
						{
							Main.player[Main.myPlayer].bank3.item[num3] = new Item();
						}
						else if (item8.maxStack > 1)
						{
							Main.player[Main.myPlayer].bank3.item[num3].stack = Main.player[Main.myPlayer].bank3.item[num3].maxStack;
						}
						num3++;
					}
					num3 = 0;
					foreach (Item item9 in Main.player[Main.myPlayer].bank4.item.ToArray<Item>())
					{
						if (item9.IsAir)
						{
							Main.player[Main.myPlayer].bank4.item[num3] = new Item();
						}
						else if (item9.maxStack > 1)
						{
							Main.player[Main.myPlayer].bank4.item[num3].stack = Main.player[Main.myPlayer].bank4.item[num3].maxStack;
						}
						num3++;
					}
					Mystagogue.Output("Stacks regulated");
				});
				dictionary.Add("fvt", delegate
				{
					for (int k = 0; k < Main.player[Main.myPlayer].inventory.Length; k++)
					{
						Main.player[Main.myPlayer].inventory[k].favorited = true;
					}
					Mystagogue.Output("Snap of the fingers!");
				});
				dictionary.Add("clr", delegate
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
					Mystagogue.Output("Autotrash cleaned all excess items: Not favorited in Inventory, and all Void contents");
				});
				dictionary.Add("refills", delegate
				{
					Main.player[Main.myPlayer].MystagogueRefills = !Main.player[Main.myPlayer].MystagogueRefills;
					string str = "once again be able to";
					if (Main.player[Main.myPlayer].MystagogueRefills)
					{
						str = "no longer";
					}
					Mystagogue.Output("You will " + str + " deplete favorited items in your inventory");
				});
				dictionary.Add("god", delegate
				{
					Main.player[Main.myPlayer].MystagogueGod = !Main.player[Main.myPlayer].MystagogueGod;
					string str = "once again";
					if (Main.player[Main.myPlayer].MystagogueGod)
					{
						str = "no longer";
					}
					Mystagogue.Output("You will " + str + " take damage or lose Mana");
				});
				dictionary.Add("buddha", delegate
				{
					if (Mystagogue.CommandArgs.Count == 1)
					{
						Main.player[Main.myPlayer].MystagogueBuddha = 0;
						Mystagogue.Output("Anomalous regeneration dormant");
						return;
					}
					if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
					{
						Mystagogue.Output("Must be a positive integer");
						return;
					}
					string text3 = Mystagogue.CommandArgs[1];
					while (text3.StartsWith("0"))
					{
						text3 = text3.Remove(0, 1);
					}
					int num3 = 0;
					if (text3.Length != 0)
					{
						if (text3.Length > 5)
						{
							num3 = 36000;
						}
						else
						{
							num3 = int.Parse(text3);
							if (num3 > 36000)
							{
								num3 = 36000;
							}
						}
					}
					if (num3 == 0)
					{
						Main.player[Main.myPlayer].MystagogueBuddha = 0;
						Mystagogue.Output("Anomalous regeneration dormant");
						return;
					}
					Main.player[Main.myPlayer].MystagogueBuddha = num3;
					Mystagogue.Output("Anomalous regeneration active! " + Main.player[Main.myPlayer].MystagogueBuddha + "/second");
				});
				dictionary.Add("nomanacost", delegate
				{
					if (Main.player[Main.myPlayer].MystagogueManaCostDeduction == 0f)
					{
						Main.player[Main.myPlayer].MystagogueManaCostDeduction = 1f;
						Mystagogue.Output("Mana will no longer be deducted when using Mana dependent items");
						return;
					}
					Main.player[Main.myPlayer].MystagogueManaCostDeduction = 0f;
					Mystagogue.Output("Mana will once again be deducted when using Mana dependent items");
				});
				dictionary.Add("infflight", delegate
				{
					Main.player[Main.myPlayer].MystagogueInfiniteFlight = !Main.player[Main.myPlayer].MystagogueInfiniteFlight;
					if (Main.player[Main.myPlayer].MystagogueInfiniteFlight)
					{
						Mystagogue.Output("Flight is now inexhaustable!");
						return;
					}
					Mystagogue.Output("Flight is no longer inexhaustable");
				});
				dictionary.Add("boost", delegate
				{
					if (Mystagogue.CommandArgs.Count == 1)
					{
						Main.player[Main.myPlayer].MystagogueSpeedBoost = 0;
						Mystagogue.Output("Speed boost dormant");
						return;
					}
					if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
					{
						Mystagogue.Output("Must be a positive integer");
						return;
					}
					string text3 = Mystagogue.CommandArgs[1];
					while (text3.StartsWith("0"))
					{
						text3 = text3.Remove(0, 1);
					}
					int num3 = 0;
					if (text3.Length != 0)
					{
						if (text3.Length > 1)
						{
							num3 = 7;
						}
						else
						{
							num3 = int.Parse(text3);
							if (num3 > 7)
							{
								num3 = 7;
							}
						}
					}
					if (num3 == 0)
					{
						Main.player[Main.myPlayer].MystagogueSpeedBoost = 0;
						Mystagogue.Output("Speed boost dormant");
						return;
					}
					Main.player[Main.myPlayer].MystagogueSpeedBoost = num3;
					Mystagogue.Output("Speed boost active! Power level " + Main.player[Main.myPlayer].MystagogueSpeedBoost);
				});
				dictionary.Add("jesus", delegate
				{
					Main.player[Main.myPlayer].MystagogueJesus = !Main.player[Main.myPlayer].MystagogueJesus;
					string str = "no longer";
					if (Main.player[Main.myPlayer].MystagogueJesus)
					{
						str = "now";
					}
					Mystagogue.Output("You are " + str + " able to walk on still liquid surfaces");
				});
				dictionary.Add("maxminions", delegate
				{
					if (Mystagogue.CommandArgs.Count == 1)
					{
						Main.player[Main.myPlayer].MystagoguePlayerMaxMinions = 1;
						Mystagogue.Output("Max Minions reset");
						return;
					}
					if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
					{
						Mystagogue.Output("Must be a positive integer");
						return;
					}
					string text3 = Mystagogue.CommandArgs[1];
					while (text3.StartsWith("0"))
					{
						text3 = text3.Remove(0, 1);
					}
					int num3 = 1;
					if (text3.Length != 0)
					{
						if (text3.Length > 4)
						{
							num3 = 1000;
						}
						else
						{
							num3 = int.Parse(text3);
							if (num3 < 1)
							{
								num3 = 1;
							}
							if (num3 > 1000)
							{
								num3 = 1000;
							}
						}
					}
					if (num3 == 1)
					{
						Main.player[Main.myPlayer].MystagoguePlayerMaxMinions = 1;
						Mystagogue.Output("Max Minions reset");
						return;
					}
					Main.player[Main.myPlayer].MystagoguePlayerMaxMinions = num3;
					Mystagogue.Output("Max Minions set to " + num3);
				});
				dictionary.Add("givebuffs", delegate
				{
					if (Main.player[Main.myPlayer].MystagogueBuffQueue == null)
					{
						Main.player[Main.myPlayer].MystagogueBuffQueue = new List<int>();
					}
					if (Mystagogue.CommandArgs.Count == 1)
					{
						Mystagogue.Output("Specify a class: \"melee\", \"ranged\", \"magic\", or \"summon\"");
						return;
					}
					if (Mystagogue.CommandArgs[1] == "melee" || Mystagogue.CommandArgs[1] == "ranged" || Mystagogue.CommandArgs[1] == "magic" || Mystagogue.CommandArgs[1] == "summon")
					{
						for (int k = 0; k < 22; k++)
						{
							Main.player[Main.myPlayer].buffTime[k] = 0;
							Main.player[Main.myPlayer].buffType[k] = 0;
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
						Mystagogue.Output("Buffs are being added.");
						return;
					}
					Mystagogue.Output("Class invalid; Must be: \"melee\", \"ranged\", \"magic\", or \"summon\"");
				});
				dictionary.Add("killdebuffs", delegate
				{
					Main.player[Main.myPlayer].MystagogueKillDebuffs = !Main.player[Main.myPlayer].MystagogueKillDebuffs;
					string str = "no longer";
					if (Main.player[Main.myPlayer].MystagogueKillDebuffs)
					{
						str = "now";
					}
					Mystagogue.Output("You " + str + " have anomalous immunity to all debuffs");
				});
				dictionary.Add("banless", delegate
				{
					Main.player[Main.myPlayer].MystagogueBanless = !Main.player[Main.myPlayer].MystagogueBanless;
					string str = "no longer";
					if (Main.player[Main.myPlayer].MystagogueBanless)
					{
						str = "now";
					}
					Mystagogue.Output("You " + str + " have anomalous immunity to Frozen, Webbed, and Stoned");
				});
				dictionary.Add("p2pmaphack", delegate
				{
					Main.player[Main.myPlayer].MystagogueP2PMapHack = !Main.player[Main.myPlayer].MystagogueP2PMapHack;
					string str = "no longer";
					if (Main.player[Main.myPlayer].MystagogueP2PMapHack)
					{
						str = "now";
					}
					Mystagogue.Output("You " + str + " are capable of locating and teleporting to all players on the map");
				});
				dictionary.Add("magnet", delegate
				{
					Mystagogue.Output("Bringing items...");
					for (int k = 0; k < Main.item.Length; k++)
					{
						if ((Main.item[k].playerIndexTheItemIsReservedFor == Main.myPlayer && Main.netMode == 1) || (Main.item[k].playerIndexTheItemIsReservedFor == 255 || (Main.item[k].playerIndexTheItemIsReservedFor != 255 && !Main.player[Main.item[k].playerIndexTheItemIsReservedFor].active)) || Main.netMode == 0)
						{
							Main.item[k].position = Main.player[Main.myPlayer].position;
							if (Main.netMode != 0)
							{
								NetMessage.SendData(21, -1, -1, null, k, 1f, 0f, 0f, 0, 0, 0);
							}
						}
					}
				});
				dictionary.Add("magnodupe", delegate
				{
					Mystagogue.Output("Bringing items legitnessly...");
					for (int k = 0; k < Main.item.Length; k++)
					{
						if (Main.item[k].active && ((Main.item[k].playerIndexTheItemIsReservedFor == Main.myPlayer && Main.netMode == 1) || (Main.item[k].playerIndexTheItemIsReservedFor == 255 || (Main.item[k].playerIndexTheItemIsReservedFor != 255 && !Main.player[Main.item[k].playerIndexTheItemIsReservedFor].active)) || Main.netMode == 0))
						{
							Main.item[k].position = Main.player[Main.myPlayer].position;
							if (Main.netMode != 0)
							{
								NetMessage.SendData(21, -1, -1, null, k, 1f, 0f, 0f, 0, 0, 0);
							}
						}
					}
				});
				dictionary.Add("flashlight", delegate
				{
					Main.player[Main.myPlayer].MystagogueFlashlight = !Main.player[Main.myPlayer].MystagogueFlashlight;
					string raw2 = "Aww, man...";
					if (Main.player[Main.myPlayer].MystagogueFlashlight)
					{
						raw2 = "Kachow!";
					}
					Mystagogue.Output(raw2);
				});
				dictionary.Add("nrt", delegate
				{
					Main.player[Main.myPlayer].MystagogueNoRespawnTimer = !Main.player[Main.myPlayer].MystagogueNoRespawnTimer;
					string str = "no longer";
					if (Main.player[Main.myPlayer].MystagogueNoRespawnTimer)
					{
						str = "now";
					}
					Mystagogue.Output("You " + str + " respawn instantly after death");
				});
				dictionary.Add("tps", delegate
				{
					if (Mystagogue.CommandArgs.Count > 1)
					{
						if (Mystagogue.CommandArgs[1] == "0")
						{
							Main.player[Main.myPlayer].MystagogueTeleportSetting = 0;
							Mystagogue.Output("Guess you can't teleport. Shame...");
							return;
						}
						if (Mystagogue.CommandArgs[1] == "1")
						{
							Main.player[Main.myPlayer].MystagogueTeleportSetting = 1;
							Mystagogue.Output("Right click to teleport");
							return;
						}
						if (Mystagogue.CommandArgs[1] == "2")
						{
							Main.player[Main.myPlayer].MystagogueTeleportSetting = 2;
							Mystagogue.Output("Rapid teleport active. Right click to break the sound barrier");
							return;
						}
					}
					else
					{
						if (Main.player[Main.myPlayer].MystagogueTeleportSetting == 0)
						{
							Main.player[Main.myPlayer].MystagogueTeleportSetting = 1;
							Mystagogue.Output("Right click to teleport");
							return;
						}
						Main.player[Main.myPlayer].MystagogueTeleportSetting = 0;
						Mystagogue.Output("Guess you can't teleport. Shame...");
					}
				});
				dictionary.Add("copyme", delegate
				{
					Player player = Main.player[Main.myPlayer].Duplicate();
					player.name = ((Mystagogue.CommandArgs.Count > 1) ? Mystagogue.CommandArgs[1] : ("Copy of " + player.name));
					PlayerFileData.CreateAndSave(player);
					player.active = false;
					Mystagogue.Output("Character created: " + player.name);
				});
				dictionary.Add("nameme", delegate
				{
					Main.player[Main.myPlayer].name = ((Mystagogue.CommandArgs.Count > 1) ? Mystagogue.CommandArgs[1] : "One Face Among Many");
					Mystagogue.Output("You are " + Main.player[Main.myPlayer].name);
				});
				dictionary.Add("maxlife", delegate
				{
					if (Mystagogue.CommandArgs.Count == 1)
					{
						Mystagogue.Output("That command requires arguments");
						return;
					}
					if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
					{
						Mystagogue.Output("Must be a positive integer");
						return;
					}
					string text3 = Mystagogue.CommandArgs[1];
					while (text3.StartsWith("0"))
					{
						text3 = text3.Remove(0, 1);
					}
					int num3 = 1;
					if (text3.Length != 0)
					{
						if (text3.Length > 10)
						{
							num3 = int.MaxValue;
						}
						else
						{
							num3 = (int)Math.Min(long.Parse(text3), 2147483647L);
						}
					}
					Main.player[Main.myPlayer].statLifeMax = num3;
					Mystagogue.Output("Max Life set to " + num3);
				});
				dictionary.Add("maxmana", delegate
				{
					if (Mystagogue.CommandArgs.Count == 1)
					{
						Mystagogue.Output("That command requires arguments");
						return;
					}
					if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
					{
						Mystagogue.Output("Must be a positive integer");
						return;
					}
					string text3 = Mystagogue.CommandArgs[1];
					while (text3.StartsWith("0"))
					{
						text3 = text3.Remove(0, 1);
					}
					int num3 = 0;
					if (text3.Length != 0)
					{
						if (text3.Length > 3)
						{
							num3 = 200;
						}
						else
						{
							num3 = int.Parse(text3);
						}
					}
					Main.player[Main.myPlayer].statManaMax = num3;
					Mystagogue.Output("Max Mana set to " + num3);
				});
				dictionary.Add("setfish", delegate
				{
					if (Mystagogue.CommandArgs.Count == 1)
					{
						Mystagogue.Output("That command requires arguments");
						return;
					}
					if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
					{
						Mystagogue.Output("Must be a positive integer");
						return;
					}
					string text3 = Mystagogue.CommandArgs[1];
					while (text3.StartsWith("0"))
					{
						text3 = text3.Remove(0, 1);
					}
					int num3 = 0;
					if (text3.Length != 0)
					{
						if (text3.Length > 10)
						{
							num3 = int.MaxValue;
						}
						else
						{
							num3 = (int)Math.Min(long.Parse(text3), 2147483647L);
						}
					}
					Main.player[Main.myPlayer].anglerQuestsFinished = num3;
					Mystagogue.Output("Angler Quests Finished set to " + num3);
				});
				dictionary.Add("setdd2", delegate
				{
					if (Mystagogue.CommandArgs.Count == 1)
					{
						Mystagogue.Output("That command requires arguments");
						return;
					}
					if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
					{
						Mystagogue.Output("Must be a positive integer");
						return;
					}
					string text3 = Mystagogue.CommandArgs[1];
					while (text3.StartsWith("0"))
					{
						text3 = text3.Remove(0, 1);
					}
					int num3 = 0;
					if (text3.Length != 0)
					{
						if (text3.Length > 10)
						{
							num3 = int.MaxValue;
						}
						else
						{
							num3 = (int)Math.Min(long.Parse(text3), 2147483647L);
						}
					}
					Main.player[Main.myPlayer].bartenderQuestLog = num3;
					Mystagogue.Output("Bartender Quests Finished set to " + num3);
				});
				dictionary.Add("setdifficulty", delegate
				{
					if (Mystagogue.CommandArgs.Count == 1)
					{
						Mystagogue.Output("That command requires arguments");
						return;
					}
					if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
					{
						Mystagogue.Output("Must be a positive integer");
						return;
					}
					string text3 = Mystagogue.CommandArgs[1];
					while (text3.StartsWith("0"))
					{
						text3 = text3.Remove(0, 1);
					}
					int num3 = 0;
					if (text3.Length != 0)
					{
						if (text3.Length > 1)
						{
							num3 = 3;
						}
						else
						{
							num3 = int.Parse(text3);
							if (num3 > 3)
							{
								num3 = 3;
							}
						}
					}
					Main.player[Main.myPlayer].difficulty = (byte)num3;
					Mystagogue.Output("Difficulty set to " + (((byte)num3 == 0) ? "Classic" : (((byte)num3 == 1) ? "Mediumcore" : (((byte)num3 == 2) ? "Hardcore" : (((byte)num3 == 3) ? "Journey" : "null")))));
				});
				dictionary.Add("demonheart", delegate
				{
					Main.player[Main.myPlayer].extraAccessory = !Main.player[Main.myPlayer].extraAccessory;
					string str = "no longer";
					if (Main.player[Main.myPlayer].extraAccessory)
					{
						str = "now";
					}
					Mystagogue.Output("You " + str + " have eaten a demon heart");
				});
				dictionary.Add("charactertime", delegate
				{
					if (Mystagogue.CommandArgs.Count == 1)
					{
						Mystagogue.Output(string.Concat(new object[]
						{
							Main.player[Main.myPlayer].name,
							" has played for ",
							Main.ActivePlayerFileData.GetPlayTime(),
							" (",
							Main.ActivePlayerFileData.GetPlayTime().Ticks,
							")"
						}));
						return;
					}
					if (new Regex("\\D").IsMatch(Mystagogue.CommandArgs[1]))
					{
						Mystagogue.Output("Must be a positive integer");
						return;
					}
					string text3 = Mystagogue.CommandArgs[1];
					while (text3.StartsWith("0"))
					{
						text3 = text3.Remove(0, 1);
					}
					long ticks = 0L;
					if (text3.Length != 0)
					{
						if (text3.Length > 18)
						{
							ticks = long.MaxValue;
						}
						else
						{
							ticks = long.Parse(text3);
						}
					}
					Main.ActivePlayerFileData.MystagoguePTOp(ticks);
					Mystagogue.Output(string.Concat(new object[]
					{
						Main.player[Main.myPlayer].name,
						" had their playtime set to ",
						Main.ActivePlayerFileData.GetPlayTime(),
						" (",
						Main.ActivePlayerFileData.GetPlayTime().Ticks,
						")"
					}));
				});
				for (int j = 0; j < dictionary.Count; j++)
				{
					if (dictionary.ElementAt(j).Key == Mystagogue.CommandArgs[0])
					{
						dictionary[Mystagogue.CommandArgs[0]]();
						break;
					}
					if (dictionary.Count - 1 == j)
					{
						flag2 = true;
					}
				}
				if (!flag2)
				{
					Main.chatText = "";
				}
				Mystagogue.PlayerRefreshTimer = -1;
				Mystagogue.TrySyncingMyPlayer();
			}
		}

		public static void Output(string raw)
		{
			Main.NewText("Mystagogue: " + raw, 92, 247, 172);
		}

		private static void TrySyncingMyPlayer()
		{
			if (Main.netMode != 0)
			{
				Player clientPlayer = Main.clientPlayer;
				bool flag = false;
				for (int i = 0; i < 59; i++)
				{
					if (Main.player[Main.myPlayer].inventory[i].IsNotTheSameAs(clientPlayer.inventory[i]))
					{
						flag = true;
						NetMessage.SendData(5, -1, -1, null, Main.myPlayer, (float)i, (float)Main.player[Main.myPlayer].inventory[i].prefix, 0f, 0, 0, 0);
					}
				}
				for (int j = 0; j < Main.player[Main.myPlayer].armor.Length; j++)
				{
					if (Main.player[Main.myPlayer].armor[j].IsNotTheSameAs(clientPlayer.armor[j]))
					{
						flag = true;
						NetMessage.SendData(5, -1, -1, null, Main.myPlayer, (float)(59 + j), (float)Main.player[Main.myPlayer].armor[j].prefix, 0f, 0, 0, 0);
					}
				}
				for (int k = 0; k < Main.player[Main.myPlayer].miscEquips.Length; k++)
				{
					if (Main.player[Main.myPlayer].miscEquips[k].IsNotTheSameAs(clientPlayer.miscEquips[k]))
					{
						flag = true;
						NetMessage.SendData(5, -1, -1, null, Main.myPlayer, (float)(58 + Main.player[Main.myPlayer].armor.Length + Main.player[Main.myPlayer].dye.Length + 1 + k), (float)Main.player[Main.myPlayer].miscEquips[k].prefix, 0f, 0, 0, 0);
					}
				}
				for (int l = 0; l < Main.player[Main.myPlayer].miscDyes.Length; l++)
				{
					if (Main.player[Main.myPlayer].miscDyes[l].IsNotTheSameAs(clientPlayer.miscDyes[l]))
					{
						flag = true;
						NetMessage.SendData(5, -1, -1, null, Main.myPlayer, (float)(58 + Main.player[Main.myPlayer].armor.Length + Main.player[Main.myPlayer].dye.Length + Main.player[Main.myPlayer].miscEquips.Length + 1 + l), (float)Main.player[Main.myPlayer].miscDyes[l].prefix, 0f, 0, 0, 0);
					}
				}
				for (int m = 0; m < Main.player[Main.myPlayer].bank.item.Length; m++)
				{
					if (Main.player[Main.myPlayer].bank.item[m].IsNotTheSameAs(clientPlayer.bank.item[m]))
					{
						flag = true;
						NetMessage.SendData(5, -1, -1, null, Main.myPlayer, (float)(58 + Main.player[Main.myPlayer].armor.Length + Main.player[Main.myPlayer].dye.Length + Main.player[Main.myPlayer].miscEquips.Length + Main.player[Main.myPlayer].miscDyes.Length + 1 + m), (float)Main.player[Main.myPlayer].bank.item[m].prefix, 0f, 0, 0, 0);
					}
				}
				for (int n = 0; n < Main.player[Main.myPlayer].bank2.item.Length; n++)
				{
					if (Main.player[Main.myPlayer].bank2.item[n].IsNotTheSameAs(clientPlayer.bank2.item[n]))
					{
						flag = true;
						NetMessage.SendData(5, -1, -1, null, Main.myPlayer, (float)(58 + Main.player[Main.myPlayer].armor.Length + Main.player[Main.myPlayer].dye.Length + Main.player[Main.myPlayer].miscEquips.Length + Main.player[Main.myPlayer].miscDyes.Length + Main.player[Main.myPlayer].bank.item.Length + 1 + n), (float)Main.player[Main.myPlayer].bank2.item[n].prefix, 0f, 0, 0, 0);
					}
				}
				if (Main.player[Main.myPlayer].trashItem.IsNotTheSameAs(clientPlayer.trashItem))
				{
					flag = true;
					NetMessage.SendData(5, -1, -1, null, Main.myPlayer, (float)(58 + Main.player[Main.myPlayer].armor.Length + Main.player[Main.myPlayer].dye.Length + Main.player[Main.myPlayer].miscEquips.Length + Main.player[Main.myPlayer].miscDyes.Length + Main.player[Main.myPlayer].bank.item.Length + Main.player[Main.myPlayer].bank2.item.Length + 1), (float)Main.player[Main.myPlayer].trashItem.prefix, 0f, 0, 0, 0);
				}
				for (int num = 0; num < Main.player[Main.myPlayer].bank3.item.Length; num++)
				{
					if (Main.player[Main.myPlayer].bank3.item[num].IsNotTheSameAs(clientPlayer.bank3.item[num]))
					{
						flag = true;
						NetMessage.SendData(5, -1, -1, null, Main.myPlayer, (float)(58 + Main.player[Main.myPlayer].armor.Length + Main.player[Main.myPlayer].dye.Length + Main.player[Main.myPlayer].miscEquips.Length + Main.player[Main.myPlayer].miscDyes.Length + Main.player[Main.myPlayer].bank.item.Length + Main.player[Main.myPlayer].bank2.item.Length + 2 + num), (float)Main.player[Main.myPlayer].bank3.item[num].prefix, 0f, 0, 0, 0);
					}
				}
				for (int num2 = 0; num2 < Main.player[Main.myPlayer].bank4.item.Length; num2++)
				{
					if (Main.player[Main.myPlayer].bank4.item[num2].IsNotTheSameAs(clientPlayer.bank4.item[num2]))
					{
						flag = true;
						NetMessage.SendData(5, -1, -1, null, Main.myPlayer, (float)(58 + Main.player[Main.myPlayer].armor.Length + Main.player[Main.myPlayer].dye.Length + Main.player[Main.myPlayer].miscEquips.Length + Main.player[Main.myPlayer].miscDyes.Length + Main.player[Main.myPlayer].bank.item.Length + Main.player[Main.myPlayer].bank2.item.Length + Main.player[Main.myPlayer].bank3.item.Length + 2 + num2), (float)Main.player[Main.myPlayer].bank4.item[num2].prefix, 0f, 0, 0, 0);
					}
				}
				for (int num3 = 0; num3 < Main.player[Main.myPlayer].dye.Length; num3++)
				{
					if (Main.player[Main.myPlayer].dye[num3].IsNotTheSameAs(clientPlayer.dye[num3]))
					{
						flag = true;
						NetMessage.SendData(5, -1, -1, null, Main.myPlayer, (float)(58 + Main.player[Main.myPlayer].armor.Length + 1 + num3), (float)Main.player[Main.myPlayer].dye[num3].prefix, 0f, 0, 0, 0);
					}
				}
				if (Main.player[Main.myPlayer].chest != clientPlayer.chest && Main.player[Main.myPlayer].chest < 0 && clientPlayer.chest >= 0)
				{
					if (Main.player[Main.myPlayer].editedChestName)
					{
						if (Main.chest[clientPlayer.chest] != null)
						{
							NetMessage.SendData(33, -1, -1, NetworkText.FromLiteral(Main.chest[clientPlayer.chest].name), Main.player[Main.myPlayer].chest, 1f, 0f, 0f, 0, 0, 0);
						}
						else
						{
							NetMessage.SendData(33, -1, -1, null, Main.player[Main.myPlayer].chest, 0f, 0f, 0f, 0, 0, 0);
						}
						Main.player[Main.myPlayer].editedChestName = false;
					}
					else
					{
						NetMessage.SendData(33, -1, -1, null, Main.player[Main.myPlayer].chest, 0f, 0f, 0f, 0, 0, 0);
					}
				}
				if (Main.player[Main.myPlayer].talkNPC != clientPlayer.talkNPC)
				{
					NetMessage.SendData(40, -1, -1, null, Main.myPlayer, 0f, 0f, 0f, 0, 0, 0);
				}
				if (Main.LocalPlayer.tileEntityAnchor.interactEntityID != clientPlayer.tileEntityAnchor.interactEntityID && Main.LocalPlayer.tileEntityAnchor.interactEntityID < 0)
				{
					NetMessage.SendData(122, -1, -1, null, -1, (float)Main.myPlayer, 0f, 0f, 0, 0, 0);
				}
				bool flag2 = false;
				if (Main.player[Main.myPlayer].zone1 != clientPlayer.zone1)
				{
					flag2 = true;
				}
				if (Main.player[Main.myPlayer].zone2 != clientPlayer.zone2)
				{
					flag2 = true;
				}
				if (Main.player[Main.myPlayer].zone3 != clientPlayer.zone3)
				{
					flag2 = true;
				}
				if (Main.player[Main.myPlayer].zone4 != clientPlayer.zone4)
				{
					flag2 = true;
				}
				if (flag2)
				{
					NetMessage.SendData(36, -1, -1, null, Main.myPlayer, 0f, 0f, 0f, 0, 0, 0);
				}
				if (Main.player[Main.myPlayer].statLife != clientPlayer.statLife || Main.player[Main.myPlayer].statLifeMax != clientPlayer.statLifeMax)
				{
					Main.player[Main.myPlayer].netLife = false;
					NetMessage.SendData(16, -1, -1, null, Main.myPlayer, 0f, 0f, 0f, 0, 0, 0);
				}
				if (Main.player[Main.myPlayer].statMana != clientPlayer.statMana || Main.player[Main.myPlayer].statManaMax != clientPlayer.statManaMax)
				{
					Main.player[Main.myPlayer].netMana = false;
					NetMessage.SendData(42, -1, -1, null, Main.myPlayer, 0f, 0f, 0f, 0, 0, 0);
				}
				bool flag3 = false;
				for (int num4 = 0; num4 < 22; num4++)
				{
					if (Main.player[Main.myPlayer].buffType[num4] != clientPlayer.buffType[num4])
					{
						flag3 = true;
					}
				}
				if (flag3)
				{
					NetMessage.SendData(50, -1, -1, null, Main.myPlayer, 0f, 0f, 0f, 0, 0, 0);
				}
				if (Main.player[Main.myPlayer].MinionRestTargetPoint != clientPlayer.MinionRestTargetPoint)
				{
					NetMessage.SendData(99, -1, -1, null, Main.myPlayer, 0f, 0f, 0f, 0, 0, 0);
				}
				if (Main.player[Main.myPlayer].MinionAttackTargetNPC != clientPlayer.MinionAttackTargetNPC)
				{
					NetMessage.SendData(115, -1, -1, null, Main.myPlayer, 0f, 0f, 0f, 0, 0, 0);
				}
				if (flag)
				{
					NetMessage.SendData(138, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
				}
				NetMessage.SendData(13, -1, -1, null, Main.myPlayer, 0f, 0f, 0f, 0, 0, 0);
				Main.clientPlayer = (Player)Main.player[Main.myPlayer].clientClone();
			}
		}

		public static void BuffMyTools()
		{
			if (!Main.player[Main.myPlayer].MystagogueToolGod)
			{
				for (int i = 0; i < Main.player[Main.myPlayer].inventory.Length; i++)
				{
					if (Main.player[Main.myPlayer].inventory[i].pick > 0)
					{
						Main.player[Main.myPlayer].inventory[i].Refresh();
					}
				}
				for (int j = 0; j < Main.player[Main.myPlayer].inventory.Length; j++)
				{
					if (Main.player[Main.myPlayer].inventory[j].axe > 0)
					{
						Main.player[Main.myPlayer].inventory[j].Refresh();
					}
				}
				for (int k = 0; k < Main.player[Main.myPlayer].inventory.Length; k++)
				{
					if (Main.player[Main.myPlayer].inventory[k].hammer > 0)
					{
						Main.player[Main.myPlayer].inventory[k].Refresh();
					}
				}
				return;
			}
			int num = 0;
			for (int l = 0; l < Main.player[Main.myPlayer].inventory.Length; l++)
			{
				if (Main.player[Main.myPlayer].inventory[l].pick > 0)
				{
					Main.player[Main.myPlayer].inventory[l].pick = ContentSamples.ItemsByType[2786].pick;
					Main.player[Main.myPlayer].inventory[l].useTime = 0;
					Main.player[Main.myPlayer].inventory[l].useAnimation = 7;
					Main.player[Main.myPlayer].inventory[l].tileBoost = 15;
					num = l + 1;
					break;
				}
			}
			for (int m = num; m < Main.player[Main.myPlayer].inventory.Length; m++)
			{
				if (Main.player[Main.myPlayer].inventory[m].pick > 0)
				{
					Main.player[Main.myPlayer].inventory[m].Refresh();
				}
			}
			for (int n = 0; n < Main.player[Main.myPlayer].inventory.Length; n++)
			{
				if (Main.player[Main.myPlayer].inventory[n].axe > 0)
				{
					Main.player[Main.myPlayer].inventory[n].axe = ContentSamples.ItemsByType[1305].axe;
					Main.player[Main.myPlayer].inventory[n].useTime = 0;
					Main.player[Main.myPlayer].inventory[n].useAnimation = 7;
					Main.player[Main.myPlayer].inventory[n].tileBoost = 15;
					num = n + 1;
					break;
				}
			}
			for (int num2 = num; num2 < Main.player[Main.myPlayer].inventory.Length; num2++)
			{
				if (Main.player[Main.myPlayer].inventory[num2].axe > 0)
				{
					Main.player[Main.myPlayer].inventory[num2].Refresh();
				}
			}
			for (int num3 = 0; num3 < Main.player[Main.myPlayer].inventory.Length; num3++)
			{
				if (Main.player[Main.myPlayer].inventory[num3].hammer > 0)
				{
					Main.player[Main.myPlayer].inventory[num3].hammer = ContentSamples.ItemsByType[1305].hammer;
					Main.player[Main.myPlayer].inventory[num3].useTime = 0;
					Main.player[Main.myPlayer].inventory[num3].useAnimation = 7;
					Main.player[Main.myPlayer].inventory[num3].tileBoost = 4;
					num = num3 + 1;
					return;
				}
			}
			for (int num4 = num; num4 < Main.player[Main.myPlayer].inventory.Length; num4++)
			{
				if (Main.player[Main.myPlayer].inventory[num4].hammer > 0)
				{
					Main.player[Main.myPlayer].inventory[num4].Refresh();
				}
			}
		}

		public static void ResetEffectsMod()
		{
			if (Main.player[Main.myPlayer].MystagogueFlashlight)
			{
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
				for (int i = -6; i < 7; i++)
				{
					for (int j = -6; j < 7; j++)
					{
						Lighting.AddLight(new Vector2(vector.X + (float)(16 * i), vector.Y + (float)(16 * j)), 1f, 1f, 1f);
					}
				}
			}
			if (Main.player[Main.myPlayer].MystagogueRefills)
			{
				foreach (Item item in Main.player[Main.myPlayer].inventory)
				{
					if (!item.IsAir && item.favorited && item.stack < item.maxStack)
					{
						item.stack = item.maxStack;
					}
				}
				foreach (Item item2 in Main.player[Main.myPlayer].miscEquips)
				{
					if (!item2.IsAir && item2.favorited && item2.stack < item2.maxStack)
					{
						item2.stack = item2.maxStack;
					}
				}
			}
			if (Main.player[Main.myPlayer].MystagogueBuffQueue == null)
			{
				Main.player[Main.myPlayer].MystagogueBuffQueue = new List<int>();
			}
			else if (Main.player[Main.myPlayer].MystagogueBuffQueue.Count > 0 && Mystagogue.BuffQueueTimer == 0)
			{
				Main.player[Main.myPlayer].AddBuff(Main.player[Main.myPlayer].MystagogueBuffQueue[0], int.MaxValue, true, false);
				Main.player[Main.myPlayer].MystagogueBuffQueue.RemoveAt(0);
				Mystagogue.BuffQueueTimer = 10;
			}
			else if (Mystagogue.BuffQueueTimer > 0)
			{
				Mystagogue.BuffQueueTimer--;
			}
			if (Main.player[Main.myPlayer].MystagoguePlayerMaxMinions <= 0)
			{
				Main.player[Main.myPlayer].MystagoguePlayerMaxMinions = 1;
			}
			Main.player[Main.myPlayer].maxMinions = Main.player[Main.myPlayer].MystagoguePlayerMaxMinions;
			Main.player[Main.myPlayer].maxTurrets = Main.player[Main.myPlayer].MystagoguePlayerMaxMinions;
			if (Main.player[Main.myPlayer].MystagogueSpeedBoost > 0)
			{
				Main.player[Main.myPlayer].moveSpeed = 1f + (float)Main.player[Main.myPlayer].MystagogueSpeedBoost * 1.6f;
				Main.player[Main.myPlayer].runSlowdown += (float)Main.player[Main.myPlayer].MystagogueSpeedBoost * 0.03f;
				Main.player[Main.myPlayer].gravity += (float)Main.player[Main.myPlayer].MystagogueSpeedBoost * 0.075f;
				Main.player[Main.myPlayer].maxFallSpeed += (float)Main.player[Main.myPlayer].MystagogueSpeedBoost * 7f;
				Main.player[Main.myPlayer].jumpSpeedBoost = 0f + (float)Main.player[Main.myPlayer].MystagogueSpeedBoost * 2.5f;
			}
			if (Main.player[Main.myPlayer].MystagogueInfiniteFlight)
			{
				Main.player[Main.myPlayer].wingTime = 30f;
				Main.player[Main.myPlayer].rocketTime = 30;
			}
			Main.player[Main.myPlayer].manaCost = 1f - Main.player[Main.myPlayer].MystagogueManaCostDeduction;
			Main.player[Main.myPlayer].waterWalk = Main.player[Main.myPlayer].MystagogueJesus;
			Main.player[Main.myPlayer].creativeGodMode = Main.player[Main.myPlayer].MystagogueGod;
			if (Main.player[Main.myPlayer].MystagogueBuddha > 0 && !Main.player[Main.myPlayer].dead)
			{
				Main.player[Main.myPlayer].statLife += (int)Math.Floor((double)Main.player[Main.myPlayer].MystagogueBuddha / 60.0);
				if (Mystagogue.BuddhaCounter >= 60 / (Main.player[Main.myPlayer].MystagogueBuddha % 60))
				{
					Main.player[Main.myPlayer].statLife++;
					Mystagogue.BuddhaCounter = 0;
				}
				else
				{
					Mystagogue.BuddhaCounter++;
				}
			}
			if (Mystagogue.PlayerRefreshTimer > 0)
			{
				Mystagogue.PlayerRefreshTimer--;
				return;
			}
			if (Mystagogue.PlayerRefreshTimer == 0)
			{
				Mystagogue.TrySyncingMyPlayer();
				if (Main.player[Main.myPlayer].MystagogueToolGod)
				{
					Mystagogue.BuffMyTools();
				}
				Mystagogue.PlayerRefreshTimer = -1;
				return;
			}
			if (Mystagogue.PlayerRefreshTimer == -1 && (Main.player[Main.myPlayer].MystagogueBuddha > 0 || Main.player[Main.myPlayer].MystagogueBuffQueue.Count > 0 || Main.player[Main.myPlayer].MystagogueGod || Main.player[Main.myPlayer].MystagogueKillDebuffs || Main.player[Main.myPlayer].MystagogueManaCostDeduction > 0f || Main.player[Main.myPlayer].MystagogueNoRespawnTimer || Main.player[Main.myPlayer].MystagoguePlayerMaxMinions != 1 || Main.player[Main.myPlayer].MystagogueRefills || Main.player[Main.myPlayer].MystagogueSpeedBoost > 0 || Main.player[Main.myPlayer].MystagogueInfiniteFlight || Main.player[Main.myPlayer].MystagogueJesus || Main.player[Main.myPlayer].MystagogueToolGod))
			{
				Mystagogue.PlayerRefreshTimer = 20;
			}
		}

		private static int BuffQueueTimer;

		private static int PlayerRefreshTimer;

		public static int FirstFreedRecipeSlot;

		private static int BuddhaCounter;

		private static List<string> CommandArgs;

		public static int ExtraSlots;
	}
}
