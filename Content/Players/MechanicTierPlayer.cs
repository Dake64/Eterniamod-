using Terraria.ModLoader;

using Eternia.Content.Progression;

namespace Eternia.Content.Players
{
    // Every subclass mechanic DEEPENS with the world's hardmode milestones, not just the
    // Swordsman's. This feeds the same public Acc* hooks that accessories, armor sets and the
    // mechanic tonics already use, so the growth stacks cleanly with all of them and NO
    // subclass file needed changing -- the tuning of each mechanic stays where it belongs.
    //
    // Applied in PostUpdateEquips, which always runs before subclasses read their hooks in
    // PostUpdate, so it lands the same frame regardless of mod load order (the same reasoning
    // as MechanicTonicPlayer, deliberately mirrored).
    //
    // At tier 1 (Wall of Flesh) steps is 0 and every value below is untouched, so the original
    // balance is exactly preserved until Plantera.
    public class MechanicTierPlayer : ModPlayer
    {
        public override void PostUpdateEquips()
        {
            int steps = MechanicTier.Steps();

            if (steps <= 0)
            {
                return;
            }

            // Shared curves, so no subclass ends up quietly scaling faster than another.
            float gain = 0.20f * steps;             // resources build +20% / +40%
            float slower = 1f - 0.15f * steps;      // decay/loss falls to 85% / 70%
            float power = 0.10f * steps;            // payoff damage +10% / +20%
            int window = 60 * steps;                // effect windows +1s / +2s

            // --- WARRIOR ---------------------------------------------------------------

            Player.GetModPlayer<CrimsonTrailPlayer>().AccTrailGainMult += gain;

            var fighter = Player.GetModPlayer<FighterPlayer>();
            fighter.AccBonusMaxCombo += 3 * steps;
            fighter.AccBonusComboDuration += window;

            var guardian = Player.GetModPlayer<GuardianPlayer>();
            guardian.AccAuraDamage += power;
            guardian.AccAuraRadius += 0.12f * steps;

            // --- MAGE ------------------------------------------------------------------

            var elementalist = Player.GetModPlayer<ElementalistPlayer>();
            elementalist.AccSurgeBonusTicks += window;
            elementalist.AccSwitchCooldownCut += 15 * steps;

            // Lower reserve = the same army costs less of your maximum life.
            Player.GetModPlayer<NecromancerPlayer>().AccReserveMult *= slower;

            // --- RANGER ----------------------------------------------------------------

            var energy = Player.GetModPlayer<EnergyShooterPlayer>();
            energy.AccCoolRateMult += gain;
            energy.AccHeatPerShotMult *= slower;

            var archer = Player.GetModPlayer<ArcherPlayer>();
            archer.AccFocusRegenMult += gain;
            archer.AccFocusLossMult *= slower;
            archer.AccPerfectDamage += power;

            var gunner = Player.GetModPlayer<GunnerPlayer>();
            gunner.AccMomentumGainMult += gain;
            gunner.AccMomentumDecayMult *= slower;
            gunner.AccDeadEyeBonusTicks += window;

            // --- SUMMONER --------------------------------------------------------------

            var beast = Player.GetModPlayer<BeastTamerPlayer>();
            beast.AccFerocityGainMult += gain;
            beast.AccFerocityDecayMult *= slower;
            beast.AccFrenzyDamage += power;

            var advanced = Player.GetModPlayer<AdvancedSummonerPlayer>();
            advanced.AccCommandRateMult += gain;
            advanced.AccLegionScaleBonus += 0.05f * steps;
            advanced.AccOverclockBonusTicks += window;

            var tech = Player.GetModPlayer<TechSummonerPlayer>();
            tech.AccCoreRateMult += gain;
            tech.AccOverdriveBonusTicks += window;
            tech.AccOverdriveDefense += 8 * steps;
        }
    }
}
