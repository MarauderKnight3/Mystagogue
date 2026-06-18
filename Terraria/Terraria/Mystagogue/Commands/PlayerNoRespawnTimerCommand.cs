using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class PlayerNoRespawnTimerCommand : Command
{
	internal static bool NoRespawnTimerMode;

	public PlayerNoRespawnTimerCommand() : base("nrt", "Makes the respawn timer end almost instantly during death.") { }
	protected internal override void Execute(List<string> args)
	{
		NoRespawnTimerMode = !NoRespawnTimerMode;
		Output("Instant respawn toggled " + (NoRespawnTimerMode ? "on" : "off") + ".");
	}

	protected internal override void ResetVariables() => NoRespawnTimerMode = false;

	protected internal override void PreUpdateDeadHook()
	{
		if (NoRespawnTimerMode && Main.LocalPlayer.respawnTimer > 3) {
			Main.LocalPlayer.respawnTimer = 3;
		}
	}
}
