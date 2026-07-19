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

        // The ceiling has to be LOW, because from the Hemorrhage tier the technique makes its own
        // fuel: one press bleeds a whole 28-tile zone, and every enemy it just wounded then pays
        // you back. Playtest at a cap of 8 gave +8/s, refilling the 50 cost in about six seconds
        // with no input at all -- the technique fired itself. At 3 the same crowd takes roughly
        // seventeen seconds to refill passively, so a press has to be earned by swinging again.
        private const int MaxBleedingEnemiesCounted = 3;

        private int bleedIncomeTimer;

        // OnHitNPCWithItem fires once PER ENEMY STRUCK, so a wide swing through five bleeding
        // enemies banked five times over -- +30 from a single swing, which is why hordes filled
        // the bar instantly while a boss crawled. A short window caps how many of those hits can
        // pay, so cleaving a crowd is still worth more than hitting one target, just not fivefold.
        // A lone boss is completely unaffected: its swings are far further apart than the window.
        private const int TrailWindowFrames = 10;
        private const int MaxTrailGainsPerWindow = 2;

        private int trailWindowTimer;
        private int trailGainsInWindow;

        private bool CanBankTrail()
        {
            if (trailWindowTimer <= 0)
            {
                trailWindowTimer = TrailWindowFrames;
                trailGainsInWindow = 0;
            }

            if (trailGainsInWindow >= MaxTrailGainsPerWindow)
            {
                return false;
            }

            trailGainsInWindow++;

            return true;
        }

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

            // A wide swing through a crowd must not bank once per enemy it clips.
            if (!CanBankTrail())
            {
                return;
            }

            // Milestones deepen the mechanic: extra Crimson Trail gained per hit.
            int milestoneBonus =
                Player.GetModPlayer<MilestonePlayer>().Milestones;

            Player.GetModPlayer<CrimsonTrailPlayer>()
                .Add(TrailPerHit() + milestoneBonus);
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

            // A wide swing through a crowd must not bank once per enemy it clips.
            if (!CanBankTrail())
            {
                return;
            }

            int milestoneBonus =
                Player.GetModPlayer<MilestonePlayer>().Milestones;

            Player.GetModPlayer<CrimsonTrailPlayer>()
                .Add(TrailPerHit() + milestoneBonus);
        }

        // Blood Tithe (deep Bleed node) is the only passive that pays into the Trail per swing.
        private int TrailPerHit()
        {
            return TrailPerBleedingHit
                + (HasActivePassive("Blood Tithe") ? 2 : 0);
        }

        // Open Veins widens how much of the field can pay you at once.
        private int BleedingEnemyCap()
        {
            return MaxBleedingEnemiesCounted
                + (HasActivePassive("Open Veins") ? 2 : 0);
        }

        public bool HasActivePassive(string passiveName)
        {
            var soul =
                Player.GetModPlayer<EterniaPlayer>();

            return Player.GetModPlayer<EterniaStatsPlayer>()
                .HasActivePassive(soul.EffectiveSoul, passiveName);
        }

        public bool HasKeystone(string keystone)
        {
            return Player.GetModPlayer<EterniaStatsPlayer>()
                .UnlockedPassives.Contains(keystone);
        }

        public override void PostUpdate()
        {
            if (trailWindowTimer > 0)
            {
                trailWindowTimer--;
            }

            if (!IsActiveSwordsman())
            {
                bleedIncomeTimer = 0;
                return;
            }

            // KEYSTONE Hemorrhagic Frenzy: raw melee power, paid for at the finisher (see
            // SwordsmanSkillPlayer.EffectiveCost) rather than with attack speed, which would
            // have starved the very resource the keystone sits on top of.
            if (HasKeystone("Hemorrhagic Frenzy"))
            {
                Player.GetDamage(DamageClass.Melee) += 0.20f;
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

                if (bleeding >= BleedingEnemyCap())
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
