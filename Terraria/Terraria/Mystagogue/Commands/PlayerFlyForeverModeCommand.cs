using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class PlayerFlyForeverModeCommand : Command
{
	internal bool FlyForeverMode;

	public PlayerFlyForeverModeCommand() : base("fly", "Enables or disables the endless flight duration of rocket boots and wings.") { }
	protected internal override void Execute(List<string> args)
	{
		FlyForeverMode = !FlyForeverMode;
		Output("Flying forever toggled " + (FlyForeverMode ? "on" : "off") + ".");
	}

	protected internal override void ResetVariables() => FlyForeverMode = false;

	protected internal override void PostResetEffectsHook()
	{
		if (FlyForeverMode) {
			Main.player[Main.myPlayer].wingTime = 30f;
			Main.player[Main.myPlayer].rocketTime = 30;
		}
	}
}
