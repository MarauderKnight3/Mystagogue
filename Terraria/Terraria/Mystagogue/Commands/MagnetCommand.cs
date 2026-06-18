using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class MagnetCommand : Command
{
	public MagnetCommand() : base("magnet", "Teleports all items on the map to you.") { }
	protected internal override void Execute(List<string> args)
	{
		Output("Bringing items...");

		for (int i = 0; i < Main.item.Length; i++) {
			if (Main.item[i].active && Main.LocalPlayer.active && (Main.item[i].playerIndexTheItemIsReservedFor == Main.myPlayer || Main.item[i].playerIndexTheItemIsReservedFor == 255 || Main.item[i].playerIndexTheItemIsReservedFor != 255 && !Main.player[Main.item[i].playerIndexTheItemIsReservedFor].active || Main.netMode == 0)) {
				Main.item[i].position = Main.player[Main.myPlayer].position;
				if (Main.netMode != 0) {
					NetMessage.SendData(21, number: i, number2: 255);
				}
			}
		}
	}
}
