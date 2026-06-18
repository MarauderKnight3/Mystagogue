using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class AnomalyResetCommand : Command
{
	public AnomalyResetCommand() : base("reset", "Toggles off player cheats.") { }
	protected internal override void Execute(List<string> args)
	{
		Anomaly.Reset();
		Output("Atypical playerstate normalized.");
	}
}
