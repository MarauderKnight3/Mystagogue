using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Terraria.Mystagogue.Commands;
internal class MapRevealCommand : Command
{
	public MapRevealCommand() : base("map", "Reveals the whole map.") { }
	protected internal override void Execute(List<string> args)
	{
		Output("Revealing map...");

		for (int x = 0; x < Main.Map.MaxWidth; x++) {
			for (int y = 0; y < Main.Map.MaxHeight; y++) {
				try {
					Main.Map.Update(x, y, 255);
				}
				catch (IndexOutOfRangeException e) {
					Debug.WriteLine("Out of bounds from " + x + ", " + y);
				}
			}
		}
		Main.refreshMap = true;
	}
}
