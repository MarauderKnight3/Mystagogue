using System.Collections.Generic;
using System.Linq;
using Terraria.ID;

namespace Terraria.Mystagogue.Commands;
internal class PlayerAutoMiscBuffCommand : Command
{
	internal static bool AutoMiscBuff;

	public PlayerAutoMiscBuffCommand() : base("misc", "Toggles the application of some buffs.") { }
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
						BuffID.CatBast, BuffID.Campfire, BuffID.HeartLamp, BuffID.StarInBottle,
						BuffID.Sunflower, BuffID.SugarRush, BuffID.Kite, BuffID.Sharpened,
						BuffID.AmmoBox, BuffID.Clairvoyance, BuffID.Bewitched, BuffID.WarTable
					}) {
				if (!Main.LocalPlayer.buffType.Contains(buffID))
					Main.LocalPlayer.AddBuff(buffID, 7200, false);
			}
		}
	}
}
