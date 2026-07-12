using Microsoft.Xna.Framework;
using Terraria;
using Eternia.Content.Souls;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Players
{
    public class GuardianPlayer : ModPlayer
    {
        // =================================================
        // REFLECT RADIUS
        // =================================================

        public const float ReflectRadius = 180f;

        // =================================================
        // RESET EFFECTS
        // =================================================

        public override void ResetEffects()
        {
            // =============================================
            // ONLY GUARDIAN
            // =============================================

            if (!IsActiveGuardian())
            {
                return;
            }

            // =============================================
            // BONUS DEFENSE
            // =============================================

            Player.statDefense += 8;

            // =============================================
            // BONUS REGEN
            // =============================================

            Player.lifeRegen += 5;
        }

        // =================================================
        // ON HURT
        // =================================================

        public override void OnHurt(
            Player.HurtInfo info)
        {
            // =============================================
            // ONLY GUARDIAN
            // =============================================

            if (!IsActiveGuardian())
            {
                return;
            }

            // =============================================
            // REFLECT DAMAGE
            // =============================================

            int reflectDamage =
                (info.Damage / 2)
                + ((Player.statDefense));

            // =============================================
            // SEARCH ENEMIES
            // =============================================

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                // =========================================
                // VALID NPC
                // =========================================

                if (!npc.active
                    || npc.friendly
                    || npc.dontTakeDamage)
                {
                    continue;
                }

                // =========================================
                // DISTANCE
                // =========================================

                float distance =
                    Vector2.Distance(
                        Player.Center,
                        npc.Center
                    );

                // =========================================
                // INSIDE AURA
                // =========================================

                if (distance <= ReflectRadius)
                {
                    npc.SimpleStrikeNPC(
                        reflectDamage,
                        0
                    );

                    // =====================================
                    // VISUAL FX
                    // =====================================

                    for (int d = 0; d < 15; d++)
                    {
                        Dust.NewDust(
                            npc.position,
                            npc.width,
                            npc.height,
                            DustID.Iron
                        );
                    }
                }
            }

            // =============================================
            // GUARDIAN FX
            // =============================================

            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(
                    Player.position,
                    Player.width,
                    Player.height,
                    DustID.Iron
                );
            }
        }

        // =================================================
        // SHIELD AURA PAYOFF (Escudero)
        // =================================================

        // Any class can wield a shield and project its Defensive Aura; the Guardian is
        // the one who gets the most out of it. Payoff = a flat +25% aura damage (it is
        // the Escudero's own weapon), the aura scaling with Defense ("Escalado del daño
        // con la Defensa", +1% per 4 defense), AND the Defense passive tree shaping the
        // aura (damage/radius/pulse-speed/effects). All aura bonuses are Guardian-only.
        public float AuraDamageMultiplier()
        {
            if (!IsActiveGuardian())
            {
                return 1f;
            }

            float mult = 1.25f + (Player.statDefense * 0.01f / 4f);

            if (HasDefensePassive("Iron Wall")) mult += 0.10f;
            if (HasDefensePassive("Fortress Body")) mult += 0.15f;
            if (HasDefensePassive("Aegis")) mult += 0.20f;

            return mult;
        }

        // The Guardian projects a larger aura; the Defense tree widens it further.
        public float AuraRadiusMultiplier()
        {
            if (!IsActiveGuardian())
            {
                return 1f;
            }

            float mult = 1.15f;

            if (HasDefensePassive("Shield Training")) mult += 0.10f;
            if (HasDefensePassive("Bulwark")) mult += 0.15f;

            return mult;
        }

        // The Defense tree makes the Guardian's aura pulse faster (<1 = shorter gap).
        public float AuraPulseMultiplier()
        {
            if (!IsActiveGuardian())
            {
                return 1f;
            }

            float mult = 1f;

            if (HasDefensePassive("Unbreakable")) mult *= 0.85f;
            if (HasDefensePassive("Stonewall")) mult *= 0.85f;

            return mult;
        }

        // Per-pulse Guardian passive effects: Last Bastion sustains the wielder while
        // guarding. Called by the aura on the owner's client only.
        public void ApplyGuardianAuraPulse(Player owner)
        {
            if (!IsActiveGuardian())
            {
                return;
            }

            if (HasDefensePassive("Last Bastion") &&
                owner.statLife < owner.statLifeMax2 &&
                Main.rand.NextBool(2))
            {
                owner.statLife += 2;
                owner.HealEffect(2);
            }
        }

        private bool HasDefensePassive(string node)
        {
            var stats = Player.GetModPlayer<EterniaStatsPlayer>();
            var soul = Player.GetModPlayer<EterniaPlayer>();

            return stats.HasActivePassive(soul.ActiveSoul, node);
        }

        public bool IsActiveGuardian()
        {
            var soul =
                Player.GetModPlayer<EterniaPlayer>();

            return soul.HasClassSoul &&
                soul.ActiveSoul == SoulId.Warrior &&
                Player.GetModPlayer<SubclassPlayer>().CurrentSubclass ==
                    "Guardian";
        }
    }
}
