using Terraria;
using Terraria.ModLoader;
using Eternia.Content.Souls;
using Eternia.Content.Globals;
using Eternia.Content.Buffs;
using Eternia.Content.NPCs;

namespace Eternia.Content.Players
{
    public class SwordsmanPlayer : ModPlayer
    {
        // Crimson Trail is banked from blood that is ALREADY running. The strike that opens a
        // wound earns nothing; every strike after it collects. That is the Swordsman's creed --
        // "open the wound, then bank the blood" -- and the old split rewarded the opposite, paying
        // double for first blood and turning the subclass into a hit-and-move-on crowd farmer.
        private const int TrailPerBleedingHit = 6;

        // Blood already drawn keeps paying: once a second, every enemy still bleeding from YOUR
        // wound feeds the Trail. Opening wounds and then keeping them open IS the mechanic, so
        // the resource should flow from the field bleeding, not only from swinging at it.
        private const int TrailPerBleedingEnemy = 1;

        // Without a ceiling this breaks at the Hemorrhage tier, where one press bleeds a whole
        // 28-tile zone: during an event thirty bleeding enemies would refill the bar faster than
        // the technique's own cooldown, turning it into a held button.
        private const int MaxBleedingEnemiesCounted = 8;

        private int bleedIncomeTimer;

        // The Swordsman is the bleed MASTER: their edge-weapon hits always inflict
        // bleed, bypassing the hidden chance other Warriors roll. Every edge hit also
        // feeds the Crimson Trail resource, which is spent by the Swordsman technique.
        public override void OnHitNPCWithItem(
            Item item,
            NPC target,
            NPC.HitInfo hit,
            int damageDone)
        {
            if (!IsActiveSwordsman())
            {
                return;
            }

            if (!EterniaGlobalItem.CanInflictBleed(item))
            {
                return;
            }

            bool wasBleeding =
                target.HasBuff(ModContent.BuffType<BleedDebuff>());

            Player.GetModPlayer<WarriorBleedPlayer>().ApplyBleed(target);

            // The opening strike only draws the wound -- it banks nothing.
            if (!wasBleeding)
            {
                return;
            }

            // Milestones deepen the mechanic: extra Crimson Trail gained per hit.
            int milestoneBonus =
                Player.GetModPlayer<MilestonePlayer>().Milestones;

            Player.GetModPlayer<CrimsonTrailPlayer>()
                .Add(TrailPerBleedingHit + milestoneBonus);
        }

        // The Swordsman's bleeding slash also inflicts guaranteed bleed and feeds
        // Crimson Trail, exactly like a direct edge hit -- so throwing the slash is a
        // real ranged extension of the mechanic, not a watered-down poke.
        public override void OnHitNPCWithProj(
            Projectile proj,
            NPC target,
            NPC.HitInfo hit,
            int damageDone)
        {
            if (!IsActiveSwordsman())
            {
                return;
            }

            if (!proj.DamageType.CountsAsClass(DamageClass.Melee))
            {
                return;
            }

            if (!EterniaGlobalItem.CanInflictBleed(Player.HeldItem))
            {
                return;
            }

            bool wasBleeding =
                target.HasBuff(ModContent.BuffType<BleedDebuff>());

            Player.GetModPlayer<WarriorBleedPlayer>().ApplyBleed(target);

            // Same rule as a direct strike: the slash that opens the wound banks nothing.
            if (!wasBleeding)
            {
                return;
            }

            int milestoneBonus =
                Player.GetModPlayer<MilestonePlayer>().Milestones;

            Player.GetModPlayer<CrimsonTrailPlayer>()
                .Add(TrailPerBleedingHit + milestoneBonus);
        }

        public override void PostUpdate()
        {
            if (!IsActiveSwordsman())
            {
                bleedIncomeTimer = 0;
                return;
            }

            bleedIncomeTimer++;

            if (bleedIncomeTimer < 60)
            {
                return;
            }

            bleedIncomeTimer = 0;

            int bleedType =
                ModContent.BuffType<BleedDebuff>();

            int bleeding = 0;

            foreach (NPC npc in Main.npc)
            {
                if (!npc.active
                    || npc.friendly
                    || npc.life <= 0
                    || !npc.HasBuff(bleedType))
                {
                    continue;
                }

                // Only YOUR wounds pay you -- another Warrior's bleed is their income.
                if (npc.GetGlobalNPC<BleedGlobalNPC>().BleedOwner != Player.whoAmI)
                {
                    continue;
                }

                bleeding++;

                if (bleeding >= MaxBleedingEnemiesCounted)
                {
                    break;
                }
            }

            if (bleeding > 0)
            {
                Player.GetModPlayer<CrimsonTrailPlayer>()
                    .Add(bleeding * TrailPerBleedingEnemy);
            }
        }

        public bool IsActiveSwordsman()
        {
            var soul =
                Player.GetModPlayer<EterniaPlayer>();

            return soul.HasClassSoulNow &&
                soul.EffectiveSoul == SoulId.Warrior &&
                Player.GetModPlayer<SubclassPlayer>().CurrentSubclass ==
                    "Swordsman";
        }
    }
}
