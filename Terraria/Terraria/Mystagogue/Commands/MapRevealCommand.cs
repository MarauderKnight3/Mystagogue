using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class MapRevealCommand : Command
{
	public MapRevealCommand() : base("map", "Reveals the whole map.") { }
	protected internal override void Execute(List<string> args)
	{
		Output("Revealing map...");

		int xlen = Main.Map.MaxWidth;
		int ylen = Main.Map.MaxHeight;
		for (int x = 0; x < xlen; x++) {
			for (int y = 0; y < ylen; y++) {
				if (Main.tile[x, y] != null &&
					(x - 1 < 0 || Main.tile[x - 1, y] != null) &&
					(x + 1 > xlen || Main.tile[x + 1, y] != null) &&
					(y - 1 < 0 || Main.tile[x, y - 1] != null) &&
					(y + 1 > ylen || Main.tile[x, y + 1] != null)) {
					Main.Map.Update(x, y, 255);
				}
			}
		}
		Main.refreshMap = true;
	}
}
