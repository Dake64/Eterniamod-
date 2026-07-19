using Microsoft.Xna.Framework;
using Terraria;
using Eternia.Content.Progression;
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

        // --- Accessory hooks (reset every frame; accessories re-apply them) -------------
        public float AccAuraDamage;
        public float AccAuraRadius;
        public float AccAuraPulseMult = 1f;

        public override void ResetEffects()
        {
            AccAuraDamage = 0f;
            AccAuraRadius = 0f;
            AccAuraPulseMult = 1f;

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
            //
            // The wall learns to answer harder as the world hardens:
            //   AWAKENED  (Muro)      it returns half of what struck you.
            //   DEEPENED  (Plantera)  it returns the blow in FULL, and the answer staggers
            //                         whatever it touches -- attackers bounce off you.
            //   PERFECTED (Moon Lord) the answer exceeds the wound and reaches much further.

            int tier = MechanicTier.Current();

            int reflectDamage =
                (tier >= MechanicTier.Deepened ? info.Damage : info.Damage / 2)
                + Player.statDefense;

            if (tier >= MechanicTier.Perfected)
            {
                reflectDamage = (int)(reflectDamage * 1.5f);
            }

            float reflectRange =
                ReflectRadius * (tier >= MechanicTier.Perfected ? 1.6f : 1f);

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

                if (distance <= reflectRange)
                {
                    npc.SimpleStrikeNPC(
                        reflectDamage,
                        0
                    );

                    // Deepened onward the answer also staggers, so a crowd that piles onto
                    // the Guardian keeps getting knocked out of its own rhythm.
                    if (tier >= MechanicTier.Deepened)
                    {
                        npc.GetGlobalNPC<Eternia.Content.NPCs.StunnedNPC>()
                            .stunTimer = tier >= MechanicTier.Perfected ? 120 : 60;
                    }

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

            return mult + AccAuraDamage;
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

            return mult + AccAuraRadius;
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

            return mult * AccAuraPulseMult;
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

            return soul.HasClassSoulNow &&
                soul.EffectiveSoul == SoulId.Warrior &&
                Player.GetModPlayer<SubclassPlayer>().CurrentSubclass ==
                    "Guardian";
        }
    }
}
