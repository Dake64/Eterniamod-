using Terraria;
using Terraria.ModLoader;
using Eternia.Content.Souls;
using Eternia.Content.Globals;
using Eternia.Content.Buffs;

namespace Eternia.Content.Players
{
    public class SwordsmanPlayer : ModPlayer
    {
        // Crimson Trail earned for drawing first blood vs. sustaining an existing bleed.
        private const int FirstBloodGain = 12;
        private const int SustainGain = 6;

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

            // Reward drawing first blood more than sustaining an existing bleed.
            // Milestones deepen the mechanic: extra Crimson Trail gained per hit.
            int milestoneBonus =
                Player.GetModPlayer<MilestonePlayer>().Milestones;

            Player.GetModPlayer<CrimsonTrailPlayer>()
                .Add((wasBleeding ? SustainGain : FirstBloodGain) + milestoneBonus);
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

            int milestoneBonus =
                Player.GetModPlayer<MilestonePlayer>().Milestones;

            Player.GetModPlayer<CrimsonTrailPlayer>()
                .Add((wasBleeding ? SustainGain : FirstBloodGain) + milestoneBonus);
        }

        public bool IsActiveSwordsman()
        {
            var soul =
                Player.GetModPlayer<EterniaPlayer>();

            return soul.HasClassSoul &&
                soul.ActiveSoul == SoulId.Warrior &&
                Player.GetModPlayer<SubclassPlayer>().CurrentSubclass ==
                    "Swordsman";
        }
    }
}
