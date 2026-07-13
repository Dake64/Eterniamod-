using Terraria;
using Terraria.ModLoader;

using Eternia.Content.Necromancy;
using Eternia.Content.Projectiles.Necromancer;
using Eternia.Content.Souls;

namespace Eternia.Content.Players
{
    // The Necromancer does NOT use minion slots. Instead each undead RESERVES a slice of
    // the player's maximum life (temporarily lowering it while the summon lives) and
    // DRAINS mana every second. If mana runs dry the weakest undead crumble first. This
    // trades the usual "summon count" cap for a life + mana juggling act.
    public class NecromancerPlayer : ModPlayer
    {
        public int ActiveNecroSummons;
        public int ManaDrainPerSecond;

        // Fraction of max life currently reserved by active undead (0..0.9).
        public float ReservedLifeFraction;

        // The equipped (held / last-held) Grimoire that reshapes the whole army.
        public ISpecializedGrimoire ActiveGrimoire;

        public override void PostUpdateEquips()
        {
            ActiveNecroSummons = 0;
            ManaDrainPerSecond = 0;
            ReservedLifeFraction = 0f;

            if (!IsActiveNecromancer())
            {
                return;
            }

            // The held Grimoire is the "main weapon": it reshapes the whole army. Keep the
            // last one active so switching to a spell weapon does not reset your style.
            if (Player.HeldItem?.ModItem is ISpecializedGrimoire held)
            {
                ActiveGrimoire = held;
            }

            float reserveMult = ActiveGrimoire?.ReserveMult ?? 1f;
            float manaMult = ActiveGrimoire?.ManaMult ?? 1f;

            float reserve = 0f;

            foreach (Projectile proj in Main.projectile)
            {
                if (!proj.active || proj.owner != Player.whoAmI)
                {
                    continue;
                }

                if (proj.ModProjectile is BaseNecroMinion minion)
                {
                    ActiveNecroSummons++;
                    ManaDrainPerSecond += minion.ManaDrain;
                    reserve += minion.ReservePercent / 100f;
                }
            }

            // The Grimoire scales the mana toll and the Necromancer's defense.
            ManaDrainPerSecond = (int)(ManaDrainPerSecond * manaMult);
            Player.statDefense += ActiveGrimoire?.DefenseDelta ?? 0;

            // Shadow affinity, Bone Conduit and milestones all EASE the life toll, so a
            // deeper Necromancer fields more undead for the same reserved life.
            var soul = Player.GetModPlayer<EterniaPlayer>();
            var stats = Player.GetModPlayer<EterniaStatsPlayer>();

            float ease = 1f;

            if (stats.HasActivePassive(soul.ActiveSoul, "Bone Conduit"))
            {
                ease *= 0.8f;
            }

            // Up to -40% reserve at 100 Shadow affinity.
            ease *= 1f - System.Math.Min(0.4f, stats.ShadowAffinity * 0.004f);

            // Up to -30% reserve from milestones.
            ease *= 1f - System.Math.Min(
                0.3f,
                Player.GetModPlayer<MilestonePlayer>().Milestones * 0.03f);

            reserve *= ease * reserveMult;

            ReservedLifeFraction = System.Math.Min(0.9f, reserve);

            // Reserve the life: lower the effective maximum while the undead live.
            Player.statLifeMax2 -=
                (int)(Player.statLifeMax2 * ReservedLifeFraction);
        }

        public override void PostUpdate()
        {
            if (!IsActiveNecromancer())
            {
                return;
            }

            // Every second the undead drink mana; run dry and the weakest crumble.
            if (Main.GameUpdateCount % 60 == 0 && ManaDrainPerSecond > 0)
            {
                Player.statMana -= ManaDrainPerSecond;

                if (Player.statMana < 0)
                {
                    Player.statMana = 0;
                    DespawnWeakest();
                }
            }
        }

        // Kill the least-important undead (lowest reserve = weakest) so stronger ones
        // survive the mana shortage.
        private void DespawnWeakest()
        {
            Projectile weakest = null;
            int lowest = int.MaxValue;

            foreach (Projectile proj in Main.projectile)
            {
                if (!proj.active || proj.owner != Player.whoAmI)
                {
                    continue;
                }

                if (proj.ModProjectile is BaseNecroMinion minion &&
                    minion.ReservePercent < lowest)
                {
                    lowest = minion.ReservePercent;
                    weakest = proj;
                }
            }

            weakest?.Kill();
        }

        public bool IsActiveNecromancer()
        {
            var soul =
                Player.GetModPlayer<EterniaPlayer>();

            // The Necromancer is a MAGE subclass (dark magic that raises the dead), so it
            // runs on the Mage Soul.
            return soul.HasClassSoul &&
                soul.ActiveSoul == SoulId.Mage &&
                Player.GetModPlayer<SubclassPlayer>().CurrentSubclass ==
                    "Necromancer";
        }
    }
}
