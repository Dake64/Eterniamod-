using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Players
{
    public class BerserkerPlayer : ModPlayer
    {
        // =================================================
        // RAGE
        // =================================================

        public int Rage;

        public int RageTimer;

        public bool Overrage;

        // =================================================
        // CONSTANTS
        // =================================================

        private const int MaxRage = 100;

        private const int RageDuration = 180;

        // =================================================
        // RESET EFFECTS
        // =================================================

        public override void ResetEffects()
        {
            var subclassPlayer =
                Player.GetModPlayer<SubclassPlayer>();

            // =============================================
            // ONLY BERSERKER
            // =============================================

            if (subclassPlayer.CurrentSubclass
                != "Berserker")
            {
                Rage = 0;

                RageTimer = 0;

                Overrage = false;
            }
        }

        // =================================================
        // POST UPDATE
        // =================================================

        public override void PostUpdate()
        {
            var subclassPlayer =
                Player.GetModPlayer<SubclassPlayer>();

            if (subclassPlayer.CurrentSubclass
                != "Berserker")
            {
                return;
            }

            // =============================================
            // RAGE DECAY
            // =============================================

            if (Rage > 0)
            {
                RageTimer--;

                if (RageTimer <= 0)
                {
                    Rage--;

                    RageTimer = 20;

                    if (Rage < 0)
                    {
                        Rage = 0;
                    }
                }
            }

            // =============================================
            // DAMAGE BONUS
            // =============================================

            Player.GetDamage(
                DamageClass.Melee)
                += Rage * 0.004f;

            // =============================================
            // ATTACK SPEED
            // =============================================

            Player.GetAttackSpeed(
                DamageClass.Melee)
                += Rage * 0.003f;

            // =============================================
            // DEFENSE REDUCTION
            // =============================================

            Player.statDefense -=
                Rage / 10;

            // =============================================
            // LOW HP BONUS
            // =============================================

            float lifePercent =
                (float)Player.statLife
                / Player.statLifeMax2;

            if (lifePercent <= 0.35f)
            {
                Player.GetDamage(
                    DamageClass.Melee)
                    += 0.20f;
            }

            // =============================================
            // MID RAGE FX
            // =============================================

            if (Rage >= 30
                && Main.rand.NextBool(6))
            {
                Dust.NewDust(
                    Player.position,
                    Player.width,
                    Player.height,
                    DustID.Blood
                );
            }

            // =============================================
            // HIGH RAGE FX
            // =============================================

            if (Rage >= 70
                && Main.rand.NextBool(3))
            {
                Dust.NewDust(
                    Player.position,
                    Player.width,
                    Player.height,
                    DustID.Torch
                );
            }

            // =============================================
            // OVERRAGE MODE
            // =============================================

            if (Rage >= MaxRage)
            {
                Overrage = true;
            }
            else
            {
                Overrage = false;
            }

            // =============================================
            // OVERRAGE EFFECTS
            // =============================================

            if (Overrage)
            {
                // DAMAGE

                Player.GetDamage(
                    DamageClass.Melee)
                    += 0.35f;

                // SPEED

                Player.GetAttackSpeed(
                    DamageClass.Melee)
                    += 0.20f;

                // MOVE SPEED

                Player.moveSpeed += 0.25f;

                // LIFE DRAIN

                if (Main.GameUpdateCount % 30 == 0)
                {
                    Player.statLife--;

                    if (Player.statLife <= 1)
                    {
                        Player.statLife = 1;
                    }
                }

                // VISUAL FX

                if (Main.rand.NextBool(2))
                {
                    Dust.NewDust(
                        Player.position,
                        Player.width,
                        Player.height,
                        DustID.Torch
                    );
                }

                if (Main.rand.NextBool(3))
                {
                    Dust.NewDust(
                        Player.position,
                        Player.width,
                        Player.height,
                        DustID.Blood
                    );
                }
            }
        }

        // =================================================
        // ON HIT NPC WITH ITEM
        // =================================================

        public override void OnHitNPCWithItem(
            Item item,
            NPC target,
            NPC.HitInfo hit,
            int damageDone)
        {
            var subclassPlayer =
                Player.GetModPlayer<SubclassPlayer>();

            if (subclassPlayer.CurrentSubclass
                != "Berserker")
            {
                return;
            }

            // =============================================
            // RAGE FROM DAMAGE
            // =============================================

            int rageGain =
                damageDone / 20;

            if (rageGain < 1)
            {
                rageGain = 1;
            }

            AddRage(rageGain);

            // =============================================
            // BLOOD HEAL
            // =============================================

            if (Main.rand.NextBool(8))
            {
                Player.statLife += 1;

                Player.HealEffect(1);
            }
        }

        // =================================================
        // ON HIT PROJECTILE
        // =================================================

        public override void OnHitNPCWithProj(
            Projectile proj,
            NPC target,
            NPC.HitInfo hit,
            int damageDone)
        {
            var subclassPlayer =
                Player.GetModPlayer<SubclassPlayer>();

            if (subclassPlayer.CurrentSubclass
                != "Berserker")
            {
                return;
            }

            // =============================================
            // ONLY MELEE PROJECTILES
            // =============================================

            if (!proj.DamageType.CountsAsClass(
                DamageClass.Melee))
            {
                return;
            }

            // =============================================
            // RAGE FROM DAMAGE
            // =============================================

            int rageGain =
                damageDone / 20;

            if (rageGain < 1)
            {
                rageGain = 1;
            }

            AddRage(rageGain);
        }

        // =================================================
        // ON HURT
        // =================================================

        public override void OnHurt(
            Player.HurtInfo info)
        {
            var subclassPlayer =
                Player.GetModPlayer<SubclassPlayer>();

            if (subclassPlayer.CurrentSubclass
                != "Berserker")
            {
                return;
            }

            // =============================================
            // GAIN RAGE WHEN HIT
            // =============================================

            AddRage(info.Damage / 5);
        }

        // =================================================
        // ADD RAGE
        // =================================================

        public void AddRage(int amount)
        {
            Rage += amount;

            if (Rage > MaxRage)
            {
                Rage = MaxRage;
            }

            RageTimer = RageDuration;
        }
    }
}