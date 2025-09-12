using System.Collections.Generic;
using System.Linq;
using Terraria.ID;

namespace Terraria.Mystagogue.Commands;
internal class PlayerAutoMiscBuffCommand : Command
{
	internal static bool AutoMiscBuff;

	public PlayerAutoMiscBuffCommand() : base("misc", "Enables or disables whether your character gains a collection of environmental and table buffs automatically.") { }
	protected internal override void Execute(List<string> args)
	{
		AutoMiscBuff = !AutoMiscBuff;
		Output("Auto misc buffs toggled " + (AutoMiscBuff ? "on" : "off") + ".");
	}

	protected internal override void ResetVariables() => AutoMiscBuff = false;

	protected internal static void TryApplyBuffs()
	{
		if (AutoMiscBuff) {
			foreach (int buffID in new List<int> {
						BuffID.CatBast, BuffID.Campfire, BuffID.Sunflower, BuffID.HeartLamp,
						BuffID.StarInBottle, BuffID.SugarRush, BuffID.Sharpened,
						BuffID.AmmoBox, BuffID.Clairvoyance, BuffID.Bewitched, BuffID.WarTable
					}) {
				if (!Main.player[Main.myPlayer].buffType.Contains(buffID))
					Main.player[Main.myPlayer].AddBuff(buffID, 7200, false);
			}
		}
	}
}
