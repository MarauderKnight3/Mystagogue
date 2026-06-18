using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class PlayerFlyForeverModeCommand : Command
{
	internal bool FlyForeverMode;

	public PlayerFlyForeverModeCommand() : base("fly", "Toggles endless flight stamina.") { }
	protected internal override void Execute(List<string> args)
	{
		FlyForeverMode = !FlyForeverMode;
		Output("Flying forever toggled " + (FlyForeverMode ? "on" : "off") + ".");
	}

	protected internal override void ResetVariables() => FlyForeverMode = false;

	protected internal override void PostResetEffectsHook()
	{
		if (FlyForeverMode) {
			Main.LocalPlayer.wingTime = 30f;
			Main.LocalPlayer.rocketTime = 30;
		}
	}
}
