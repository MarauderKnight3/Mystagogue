using System.Collections.Generic;
using Terraria.IO;

namespace Terraria.Mystagogue.Commands;
internal class CopyPlayerFileCommand : Command
{
	public CopyPlayerFileCommand() : base("copyme", "[Name of Copy] Saves a copy of the player character you're playing as.") { }
	protected internal override void Execute(List<string> args)
	{
		Player player = Main.player[Main.myPlayer].Duplicate();
		player.active = false;
		player.name = args.Count > 0 ? args[0] : "Copy of " + player.name;
		PlayerFileData.CreateAndSave(player);
		Output("Copy created: " + player.name);
	}
}
