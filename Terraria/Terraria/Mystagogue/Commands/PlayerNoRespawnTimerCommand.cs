using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class PlayerNoRespawnTimerCommand : Command
{
	internal static bool NoRespawnTimerMode;

	public PlayerNoRespawnTimerCommand() : base("nrt", "Enables or disables the removal of the delay to respawn after death.") { }
	protected internal override void Execute(List<string> args)
	{
		NoRespawnTimerMode = !NoRespawnTimerMode;
		Output("Respawn timer removal toggled " + (NoRespawnTimerMode ? "on" : "off") + ".");
	}

	protected internal override void ResetVariables() => NoRespawnTimerMode = false;

	protected internal override void PreUpdateDeadHook()
	{
		if (NoRespawnTimerMode && Main.player[Main.myPlayer].respawnTimer > 3) {
			Main.player[Main.myPlayer].respawnTimer = 3;
		}
	}
}
