using Terraria;
using Terraria.ID;
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
            var subclass =
                Player.GetModPlayer<SubclassPlayer>();

            if (subclass.CurrentSubclass
                != "Stunner")
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
            var subclass =
                Player.GetModPlayer<SubclassPlayer>();

            if (subclass.CurrentSubclass
                != "Stunner")
            {
                return;
            }

            // =============================================
            // BUILD CHARGE
            // =============================================

            if (Player.HeldItem.damage > 0
                && Player.HeldItem.DamageType
                    == DamageClass.Melee)
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
            var subclass =
                Player.GetModPlayer<SubclassPlayer>();

            if (subclass.CurrentSubclass
                != "Stunner")
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
            var subclass =
                Player.GetModPlayer<SubclassPlayer>();

            if (subclass.CurrentSubclass
                != "Stunner")
            {
                return;
            }

            // =============================================
            // FULL CHARGE EFFECT
            // =============================================

            if (FullyCharged)
            {
                // =========================================
                // SLOW EFFECT
                // =========================================

                target.GetGlobalNPC<
                        Eternia.Content.NPCs.StunnedNPC>()
                    .stunTimer = 120;

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
    }
}