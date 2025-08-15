using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;
internal class AnomalyResetCommand : Command
{
	public AnomalyResetCommand() : base("resetall", "Disables any enabled persistent player cheats. This does not include item cheats.") { }
	protected internal override void Execute(List<string> args)
	{
		Anomaly.Reset();
		Output("Atypical playerstate normalized.");
	}
}
