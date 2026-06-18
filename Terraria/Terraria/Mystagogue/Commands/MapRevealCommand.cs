using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class MapRevealCommand : Command
{
	public MapRevealCommand() : base("map", "Fully reveals all loaded parts of the world on the map.") { }
	protected internal override void Execute(List<string> args)
	{
		Output("Revealing map...");

		for (int x = 0; x < Main.Map.MaxWidth; x++) {
			for (int y = 0; y < Main.Map.MaxHeight; y++) {
				Main.Map.Update(x, y, 255);
			}
		}
		Main.refreshMap = true;
	}
}
