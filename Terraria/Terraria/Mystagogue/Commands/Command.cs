using System;
using System.Collections.Generic;

namespace Terraria.Mystagogue.Commands;

internal abstract class Command(string name, string description)
{
	protected internal string Name { get; } = name;
	protected internal string Description { get; } = description;
	protected internal static SortedDictionary<string, Command> Library = [];

	static Command()
	{
		// Initialize the command library
		foreach (var type in typeof(Command).Assembly.GetTypes()) {
			if (type.IsSubclassOf(typeof(Command)) && !type.IsAbstract) {
				var commandInstance = Activator.CreateInstance(type) as Command;
				Library[commandInstance.Name] = commandInstance;
				commandInstance.ResetVariables();
			}
		}
	}

	protected internal abstract void Execute(List<string> args);
	protected internal virtual void ResetVariables() { }
	protected internal virtual void PreUpdateDeadHook() { }
	protected internal virtual void PreUpdateBuffsHook() { }
	protected internal virtual void PostResetEffectsHook() { }

	protected internal void Output(string message, bool isError = false) => Anomaly.Output(message, isError);
}
