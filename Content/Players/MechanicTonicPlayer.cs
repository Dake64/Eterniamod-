using Terraria;
using Terraria.ModLoader;

using Eternia.Content.Buffs;

namespace Eternia.Content.Players
{
    // Applies the subclass MECHANIC tonics. Each tonic feeds the same public Acc* hooks that the
    // accessories and armor sets use, so they all stack cleanly. Done here in PostUpdateEquips --
    // which always runs before every subclass reads its hooks in PostUpdate -- so the effect is
    // guaranteed to land the same frame, whatever the mod load order.
    //
    // A tonic whose subclass you are not playing simply does nothing (the hook goes unread).
    public class MechanicTonicPlayer : ModPlayer
    {
        public override void PostUpdateEquips()
        {
            if (Player.HasBuff(ModContent.BuffType<CoolantDraughtBuff>()))
            {
                var e = Player.GetModPlayer<EnergyShooterPlayer>();
                e.AccCoolRateMult += 0.6f;
                e.AccHeatPerShotMult *= 0.7f;
                e.AccOverheatShield = true;
            }

            if (Player.HasBuff(ModContent.BuffType<AdrenalineDraughtBuff>()))
            {
                var g = Player.GetModPlayer<GunnerPlayer>();
                g.AccMomentumGainMult += 0.5f;
                g.AccMomentumDecayMult *= 0.55f;
            }

            if (Player.HasBuff(ModContent.BuffType<FocusDraughtBuff>()))
            {
                var a = Player.GetModPlayer<ArcherPlayer>();
                a.AccFocusRegenMult += 0.5f;
                a.AccFocusLossMult *= 0.6f;
            }

            if (Player.HasBuff(ModContent.BuffType<WarcryDraughtBuff>()))
            {
                var f = Player.GetModPlayer<FighterPlayer>();
                f.AccBonusMaxCombo += 5;
                f.AccBonusComboDuration += 120;
            }

            if (Player.HasBuff(ModContent.BuffType<BloodlustDraughtBuff>()))
            {
                var b = Player.GetModPlayer<BeastTamerPlayer>();
                b.AccFerocityGainMult += 0.5f;
                b.AccFerocityDecayMult *= 0.55f;
            }

            if (Player.HasBuff(ModContent.BuffType<OverdriveDraughtBuff>()))
            {
                var t = Player.GetModPlayer<TechSummonerPlayer>();
                t.AccCoreRateMult += 0.5f;
                t.AccOverdriveBonusTicks += 180;
            }
        }
    }
}
