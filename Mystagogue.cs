using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.Graphics.Capture;
using Terraria.ID;
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
			if (Main.player[Main.myPlayer].MystagoguePlayerMaxMinions < 1)
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
				Mystagogue.BuddhaReservedHealth += (double)(Main.player[Main.myPlayer].MystagogueBuddha / 60);
				if (Mystagogue.BuddhaReservedHealth >= 1.0)
				{
					Main.player[Main.myPlayer].statLife += (int)Math.Floor(Mystagogue.BuddhaReservedHealth);
					Mystagogue.BuddhaReservedHealth %= 1.0;
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

		public static void Output(string raw, bool hideMystagogue = false)
		{
			Main.NewText(hideMystagogue ? "" : ("Mystagogue: " + raw), 92, 247, 172);
		}

		public static void CatchWildHelpCommand()
		{
			if (Main.chatText == "/help" || Main.chatText == "/?")
			{
				Mystagogue.Command("help;;", false);
			}
		}

		public static void Command(string raw, bool clear)
		{
			if (raw.EndsWith(";;") && raw.Length > 2)
			{
				Mystagogue.CommandArgs = raw.Substring(0, raw.Length - 2).Split(new char[]
				{
					' '
				}).ToList<string>();
				for (;;)
				{
					IL_45:
					int num = 0;
					int num2 = 0;
					bool flag = false;
					foreach (string text in Mystagogue.CommandArgs)
					{
						if (text.StartsWith("\"") && text.EndsWith("\"") && text.Length > 2)
						{
							Mystagogue.CommandArgs[num] = Mystagogue.CommandArgs[num].Substring(1, Mystagogue.CommandArgs[num].Length - 2);
							goto IL_45;
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
							goto IL_45;
						}
						num++;
					}
					break;
				}
				Mystagogue.CommandArgs = (from arg in Mystagogue.CommandArgs
				where !string.IsNullOrWhiteSpace(arg)
				select arg).ToList<string>();
				Mystagogue.CommandArgs[0] = Mystagogue.CommandArgs[0].ToLower();
				int j = 0;
				while (j < MystagogueCMD.library.Count)
				{
					if (MystagogueCMD.library.ElementAt(j).Key == Mystagogue.CommandArgs[0])
					{
						MystagogueCMD.library[Mystagogue.CommandArgs[0]].func();
						if (clear)
						{
							Main.chatText = "";
							break;
						}
						break;
					}
					else
					{
						j++;
					}
				}
				Mystagogue.PlayerRefreshTimer = -1;
				Mystagogue.TrySyncingMyPlayer();
			}
		}

		private static int BuffQueueTimer;

		private static int PlayerRefreshTimer;

		public static int FirstFreedRecipeSlot;

		public static List<string> CommandArgs;

		public static int ExtraSlots;

		private static double BuddhaReservedHealth;
	}
}
