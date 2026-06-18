using System.Collections.Generic;
using Terraria.ID;

namespace Terraria.Mystagogue.Commands;
internal class PlayerForceImmunityCommand : Command
{
	internal static bool ForceDebuffImmunity;

	public PlayerForceImmunityCommand() : base("immune", "Toggles debuff immunity.") { }
	protected internal override void Execute(List<string> args)
	{
		ForceDebuffImmunity = !ForceDebuffImmunity;
		Output("Immunity toggled " + (ForceDebuffImmunity ? "on" : "off") + ".");
	}

	protected internal override void ResetVariables() => ForceDebuffImmunity = false;

	protected internal override void PreUpdateBuffsHook()
	{
		if (ForceDebuffImmunity) {
			foreach (int num in new List<int>
			{
				BuffID.Poisoned, BuffID.PotionSickness, BuffID.Darkness, BuffID.Cursed,
				BuffID.OnFire, BuffID.Bleeding, BuffID.Confused, BuffID.Slow, BuffID.Weak,
				BuffID.Silenced, BuffID.BrokenArmor, BuffID.Horrified, BuffID.TheTongue,
				BuffID.CursedInferno, BuffID.Frostburn, BuffID.Chilled, BuffID.Frozen,
				BuffID.Burning, BuffID.Suffocation, BuffID.Ichor, BuffID.Venom, BuffID.Midas,
				BuffID.Blackout, BuffID.WaterCandle, BuffID.ChaosState, BuffID.ManaSickness,
				BuffID.Lovestruck, BuffID.Stinky, BuffID.Slimed, BuffID.Electrified,
				BuffID.MoonLeech, BuffID.Rabies, BuffID.Webbed, BuffID.ShadowFlame,
				BuffID.Stoned, BuffID.Dazed, BuffID.Obstructed, BuffID.VortexDebuff,
				BuffID.BoneJavelin, BuffID.StardustMinionBleed, BuffID.DryadsWardDebuff,
				BuffID.Daybreak, BuffID.WindPushed, BuffID.WitheredArmor, BuffID.WitheredWeapon,
				BuffID.OgreSpit, BuffID.NoBuilding, BuffID.BetsysCurse, BuffID.Oiled,
				BuffID.GelBalloonBuff, BuffID.OnFire3, BuffID.Frostburn2, BuffID.NeutralHunger,
				BuffID.Hunger, BuffID.Starving, BuffID.TentacleSpike, BuffID.BloodButcherer,
				BuffID.Shimmer, BuffID.EelWhipNPCDebuff, BuffID.Hemorrhage
			}) {
				Main.LocalPlayer.buffImmune[num] = true;
			}
		}
	}
}
