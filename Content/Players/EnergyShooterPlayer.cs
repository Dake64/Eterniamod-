using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Eternia.Content.Souls;

namespace Eternia.Content.Players
{
    public class EnergyShooterPlayer : ModPlayer
    {
        // =================================================
        // ENERGY HEAT
        // =================================================

        public float Heat;

        public float MaxHeat = 100f;

        public bool Overheated;

        // =================================================
        // OVERDRIVE
        // =================================================

        public bool Overdrive;

        public int OverdriveTimer;

        // =================================================
        // RESET
        // =================================================

        public override void ResetEffects()
        {
            if (!IsActiveEnergyGunner())
            {
                Heat = 0f;

                Overheated = false;

                Overdrive = false;

                OverdriveTimer = 0;
            }
        }

        // =================================================
        // POST UPDATE
        // =================================================

        public override void PostUpdate()
        {
            if (!IsActiveEnergyGunner())
            {
                return;
            }

            // =============================================
            // COOLING
            // =============================================

            if (!Player.channel)
            {
                Heat -= Overdrive
                    ? 0.15f
                    : 0.55f;
            }

            if (Heat < 0f)
            {
                Heat = 0f;
            }

            // =============================================
            // OVERDRIVE ACTIVATION
            // =============================================

            if (EterniaKeybinds.SkillKey
                .JustPressed
                && Heat >= 50f
                && !Overheated
                && !Overdrive)
            {
                Overdrive = true;

                OverdriveTimer = 300;
            }

            // =============================================
            // OVERDRIVE
            // =============================================

            if (Overdrive)
            {
                OverdriveTimer--;

                // =========================================
                // SMALL HEAT GENERATION
                // =========================================

                Heat += 0.18f;

                // =========================================
                // ELECTRIC FX
                // =========================================

                if (Main.rand.NextBool(3))
                {
                    Dust.NewDust(
                        Player.position,
                        Player.width,
                        Player.height,
                        DustID.Electric
                    );
                }

                // =========================================
                // END OVERDRIVE
                // =========================================

                if (OverdriveTimer <= 0)
                {
                    Overdrive = false;
                }

                // =========================================
                // OVERHEAT
                // =========================================

                if (Heat >= MaxHeat)
                {
                    Heat = MaxHeat;

                    Overheated = true;

                    Overdrive = false;
                }
            }

            // =============================================
            // OVERHEAT RESET
            // =============================================

            if (Overheated
                && Heat <= 20f)
            {
                Overheated = false;
            }

            // =============================================
            // OVERHEAT FX
            // =============================================

            if (Overheated
                && Main.rand.NextBool(4))
            {
                Dust.NewDust(
                    Player.position,
                    Player.width,
                    Player.height,
                    DustID.Smoke
                );
            }
        }

        // =================================================
        // CAN USE ITEM
        // =================================================

        public override bool CanUseItem(Item item)
        {
            if (!IsActiveEnergyGunner())
            {
                return true;
            }

            // =============================================
            // ONLY RANGED
            // =============================================

            if (!item.DamageType
                .CountsAsClass(DamageClass.Ranged))
            {
                return true;
            }

            // =============================================
            // OVERHEATED
            // =============================================

            if (Overheated)
            {
                return false;
            }

            // =============================================
            // BUILD HEAT
            // =============================================

            Heat += 5.5f;

            if (Heat >= MaxHeat)
            {
                Heat = MaxHeat;

                Overheated = true;
            }

            return true;
        }

        // =================================================
        // MODIFY WEAPON DAMAGE
        // =================================================

        public override void ModifyWeaponDamage(
            Item item,
            ref StatModifier damage)
        {
            if (!IsActiveEnergyGunner())
            {
                return;
            }

            if (!item.DamageType
                .CountsAsClass(DamageClass.Ranged))
            {
                return;
            }

            // =============================================
            // HEAT DAMAGE BONUS
            // =============================================

            float heatPercent =
                Heat / MaxHeat;

            damage +=
                heatPercent * 0.35f;

            // =============================================
            // OVERDRIVE BONUS
            // =============================================

            if (Overdrive)
            {
                damage += 0.20f;
            }
        }

        // =================================================
        // USE SPEED
        // =================================================

        public override float UseSpeedMultiplier(
            Item item)
        {
            if (!IsActiveEnergyGunner())
            {
                return 1f;
            }

            if (!item.DamageType
                .CountsAsClass(DamageClass.Ranged))
            {
                return 1f;
            }

            if (Overdrive)
            {
                return 1.25f;
            }

            return 1f;
        }

        // =================================================
        // PROJECTILE HIT
        // =================================================

        public override void OnHitNPCWithProj(
            Projectile proj,
            NPC target,
            NPC.HitInfo hit,
            int damageDone)
        {
            if (!IsActiveEnergyGunner())
            {
                return;
            }

            // =============================================
            // ONLY RANGED
            // =============================================

            if (!proj.DamageType
                .CountsAsClass(DamageClass.Ranged))
            {
                return;
            }

            // =============================================
            // HIGH HEAT FX
            // =============================================

            float heatPercent =
                Heat / MaxHeat;

            if (heatPercent >= 0.7f)
            {
                for (int i = 0; i < 6; i++)
                {
                    Dust.NewDust(
                        target.position,
                        target.width,
                        target.height,
                        DustID.Electric
                    );
                }
            }

            // =============================================
            // OVERDRIVE HIT FX
            // =============================================

            if (Overdrive)
            {
                for (int i = 0; i < 10; i++)
                {
                    Dust.NewDust(
                        target.position,
                        target.width,
                        target.height,
                        DustID.BlueTorch
                    );
                }
            }
        }

        public bool IsActiveEnergyGunner()
        {
            var soul =
                Player.GetModPlayer<EterniaPlayer>();

            return soul.HasClassSoul &&
                soul.ActiveSoul == SoulId.Ranger &&
                Player.GetModPlayer<SubclassPlayer>().CurrentSubclass ==
                "Energy Gunner";
        }
    }
}
