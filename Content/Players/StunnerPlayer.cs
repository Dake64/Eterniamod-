using Terraria;
using Terraria.ID;
using Eternia.Content.Souls;
using Terraria.ModLoader;

namespace Eternia.Content.Players
{
    public class StunnerPlayer : ModPlayer
    {
        // =================================================
        // CHARGE
        // =================================================

        public int Charge;

        public int MaxCharge = 300;

        public bool FullyCharged;

        // =================================================
        // RESET
        // =================================================

        public override void ResetEffects()
        {
            if (!IsActiveStunner())
            {
                Charge = 0;

                FullyCharged = false;
            }
        }

        // =================================================
        // POST UPDATE
        // =================================================

        public override void PostUpdate()
        {
            if (!IsActiveStunner())
            {
                return;
            }

            // =============================================
            // BUILD CHARGE
            // =============================================

            if (Player.HeldItem.damage > 0
                && Player.HeldItem.DamageType
                    .CountsAsClass(DamageClass.Melee))
            {
                if (Charge < MaxCharge)
                {
                    Charge++;
                }
                else
                {
                    Charge = MaxCharge;

                    FullyCharged = true;
                }
            }
            else
            {
                Charge = 0;

                FullyCharged = false;
            }

            // =============================================
            // FULL CHARGE EFFECT
            // =============================================

            if (FullyCharged
                && Main.rand.NextBool(3))
            {
                Dust.NewDust(
                    Player.position,
                    Player.width,
                    Player.height,
                    DustID.Torch
                );
            }
        }

        // =================================================
        // MODIFY HIT
        // =================================================

        public override void ModifyHitNPCWithItem(
            Item item,
            NPC target,
            ref NPC.HitModifiers modifiers)
        {
            if (!IsActiveStunner())
            {
                return;
            }

            // =============================================
            // DAMAGE BONUS
            // =============================================

            float chargePercent =
                (float)Charge / MaxCharge;

            modifiers.SourceDamage +=
                chargePercent * 1.2f;

            if (FullyCharged &&
                HasActivePassive("Tyrant Smash"))
            {
                modifiers.SourceDamage += 0.20f;
            }

            // =============================================
            // ARMOR BREAK
            // =============================================

            modifiers.DefenseEffectiveness
                *= 1f - (chargePercent * 0.5f);
        }

        // =================================================
        // ON HIT
        // =================================================

        public override void OnHitNPCWithItem(
            Item item,
            NPC target,
            NPC.HitInfo hit,
            int damageDone)
        {
            if (!IsActiveStunner())
            {
                return;
            }

            // =============================================
            // FULL CHARGE EFFECT
            // =============================================

            if (FullyCharged)
            {
                int stunDuration =
                    HasActivePassive("Concussion")
                        ? 180
                        : 120;

                // =========================================
                // SLOW EFFECT
                // =========================================

                target.GetGlobalNPC<
                        Eternia.Content.NPCs.StunnedNPC>()
                    .stunTimer = stunDuration;

                // =========================================
                // VISUAL FX
                // =========================================

                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(
                        target.position,
                        target.width,
                        target.height,
                        DustID.Smoke
                    );
                }
            }

            // =============================================
            // RESET CHARGE
            // =============================================

            Charge = 0;

            FullyCharged = false;
        }

        private bool HasActivePassive(string passiveName)
        {
            var soul =
                Player.GetModPlayer<EterniaPlayer>();

            return Player.GetModPlayer<EterniaStatsPlayer>()
                .HasActivePassive(
                    soul.ActiveSoul,
                    passiveName);
        }

        public bool IsActiveStunner()
        {
            var soul =
                Player.GetModPlayer<EterniaPlayer>();

            return soul.HasClassSoul &&
                soul.ActiveSoul == SoulId.Warrior &&
                Player.GetModPlayer<SubclassPlayer>().CurrentSubclass ==
                    "Stunner";
        }
    }
}
