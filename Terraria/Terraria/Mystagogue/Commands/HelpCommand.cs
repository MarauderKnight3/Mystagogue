using System.Collections.Generic;
using System.Linq;
using Terraria.Mystagogue.Utils;

namespace Terraria.Mystagogue.Commands;
internal class HelpCommand : Command
{
	public HelpCommand() : base("help", "[Command Name] Gives helpful information, a list of commands, and more. You can specify a command to read about it.") { }
	protected internal override void Execute(List<string> args)
	{
		if (args.Count > 0) {
			Library.TryGetValue(args[0].ToLower(), out Command command);
			if (command is null) {
				Output($"Command [{args[0]}] not found.", true);
				return;
			}
			Output($"You chose to read about [{command.Name}].\n>> {command.Description}");
			return;
		}

		string commandList = string.Join("], [", Library.Select(c => c.Key)).ReplaceEveryXthOccurrence("], [", "],\n[", 10);
		Output($@"There are {Library.Count} commands loaded. Learn about a command with `help [Command Name]`.
List of commands: [{commandList}]");
	}
}
