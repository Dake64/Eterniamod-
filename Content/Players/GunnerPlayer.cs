using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Eternia.Content.Souls;

namespace Eternia.Content.Players
{
    public class GunnerPlayer : ModPlayer
    {
        // =================================================
        // SWEET SPOT
        // =================================================

        public float SweetSpotValue;

        public bool GoingRight = true;

        // =================================================
        // DEAD EYE
        // =================================================

        public bool DeadEye;

        public int DeadEyeTimer;

        public float DeadEyeEnergy;

        public const float MaxDeadEyeEnergy = 100f;

        // =================================================
        // RESET
        // =================================================

        public override void ResetEffects()
        {
            if (!IsActiveGunner())
            {
                SweetSpotValue = 0f;

                DeadEye = false;

                DeadEyeTimer = 0;

                DeadEyeEnergy = 0f;
            }
        }

        // =================================================
        // POST UPDATE
        // =================================================

        public override void PostUpdate()
        {
            if (!IsActiveGunner())
            {
                return;
            }

            // =============================================
            // SWEET SPOT OSCILLATION
            // =============================================

            if (GoingRight)
            {
                SweetSpotValue += 0.035f;
            }
            else
            {
                SweetSpotValue -= 0.035f;
            }

            if (SweetSpotValue >= 1f)
            {
                SweetSpotValue = 1f;

                GoingRight = false;
            }

            if (SweetSpotValue <= 0f)
            {
                SweetSpotValue = 0f;

                GoingRight = true;
            }

            // =============================================
            // DEAD EYE
            // =============================================

            if (DeadEye)
            {
                DeadEyeEnergy -= 0.08f;

                // =========================================
                // PARTICLES
                // =========================================

                if (Main.rand.NextBool(3))
                {
                    Dust.NewDust(
                        Player.position,
                        Player.width,
                        Player.height,
                        DustID.GoldFlame
                    );
                }

                // =========================================
                // END DEAD EYE
                // =========================================

                if (DeadEyeEnergy <= 0f)
                {
                    DeadEyeEnergy = 0f;

                    DeadEye = false;
                }
            }
        }

        // =================================================
        // MODIFY SHOOT STATS
        // =================================================

        public override void ModifyShootStats(
            Item item,
            ref Vector2 position,
            ref Vector2 velocity,
            ref int type,
            ref int damage,
            ref float knockback)
        {
            if (!IsActiveGunner())
            {
                return;
            }

            // =============================================
            // ONLY BULLET WEAPONS
            // =============================================

            if (item.useAmmo != AmmoID.Bullet)
            {
                return;
            }

            // =============================================
            // PERFECT ZONE
            // =============================================

            bool perfectZone =
                SweetSpotValue >= 0.38f
                && SweetSpotValue <= 0.62f;

            // =============================================
            // ACTIVATE DEAD EYE
            // =============================================

            if (perfectZone
                && !DeadEye)
            {
                DeadEye = true;

                DeadEyeTimer = 999999;

                DeadEyeEnergy = MaxDeadEyeEnergy;

                CombatText.NewText(
                    Player.Hitbox,
                    Color.Gold,
                    "DEAD EYE"
                );

                // =========================================
                // ACTIVATION FX
                // =========================================

                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(
                        Player.position,
                        Player.width,
                        Player.height,
                        DustID.GoldFlame
                    );
                }
            }

            // =============================================
            // DEAD EYE BONUSES
            // =============================================

            if (DeadEye)
            {
                damage =
                    (int)(damage * 1.35f);

                velocity *= 1.15f;

                // =========================================
                // ENERGY CONSUMPTION
                // =========================================

                float consumeAmount = 0f;

                // =========================================
                // FAST WEAPONS CONSUME MORE
                // =========================================

                if (item.useTime <= 10)
                {
                    consumeAmount = 8f;
                }
                else if (item.useTime <= 20)
                {
                    consumeAmount = 5f;
                }
                else
                {
                    consumeAmount = 2.5f;
                }

                DeadEyeEnergy -= consumeAmount;

                if (DeadEyeEnergy <= 0f)
                {
                    DeadEyeEnergy = 0f;

                    DeadEye = false;
                }
            }
        }

        // =================================================
        // CRIT BONUS
        // =================================================

        public override void ModifyWeaponCrit(
            Item item,
            ref float crit)
        {
            if (!IsActiveGunner())
            {
                return;
            }

            if (item.useAmmo != AmmoID.Bullet)
            {
                return;
            }

            if (DeadEye)
            {
                crit += 20f;
            }
        }

        // =================================================
        // ATTACK SPEED
        // =================================================

        public override float UseSpeedMultiplier(
            Item item)
        {
            if (!IsActiveGunner())
            {
                return 1f;
            }

            if (item.useAmmo != AmmoID.Bullet)
            {
                return 1f;
            }

            // =============================================
            // NO BONUS FOR VERY FAST WEAPONS
            // =============================================

            if (DeadEye)
            {
                if (item.useTime <= 10)
                {
                    return 1f;
                }

                return 1.15f;
            }

            return 1f;
        }

        // =================================================
        // HIT EFFECTS
        // =================================================

        public override void OnHitNPCWithProj(
            Projectile proj,
            NPC target,
            NPC.HitInfo hit,
            int damageDone)
        {
            if (!IsActiveGunner())
            {
                return;
            }

            if (Player.HeldItem.useAmmo
                != AmmoID.Bullet)
            {
                return;
            }

            if (DeadEye)
            {
                for (int i = 0; i < 8; i++)
                {
                    Dust.NewDust(
                        target.position,
                        target.width,
                        target.height,
                        DustID.Torch
                    );
                }
            }
        }

        public bool IsActiveGunner()
        {
            var soul =
                Player.GetModPlayer<EterniaPlayer>();

            return soul.HasClassSoul &&
                soul.ActiveSoul == SoulId.Ranger &&
                Player.GetModPlayer<SubclassPlayer>().CurrentSubclass ==
                    "Gunner";
        }
    }
}
