using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Souls;

namespace Eternia.Content.Players
{
    // The Gunner is the rapid-fire gunslinger. It runs on MOMENTUM (0-100): every bullet that
    // LANDS builds it, and it decays fast the moment you stop hitting (stop firing or miss). The
    // higher the Momentum the bigger the damage/fire-rate/crit bonus, and hitting 100 triggers
    // DEAD EYE -- a short overdrive that then drops you back to zero. Fast guns (machine guns,
    // SMGs) build Momentum through sheer volume; slow rifles barely move it, by design -- the
    // pure sniper fantasy is reserved for a future dedicated class. Bullet weapons only.
    //   0-39   : no bonus
    //   40-69  : +8% damage, +8% fire rate
    //   70-99  : +15% damage, +15% fire rate, +5% crit
    //   100    : DEAD EYE overdrive
    public class GunnerPlayer : ModPlayer
    {
        public float Momentum;
        public const float MaxMomentum = 100f;

        public bool DeadEye;
        public int DeadEyeTimer;

        private int ticksSinceHit;

        public float MomentumPercent => Momentum / MaxMomentum * 100f;

        // 0 none, 1 warmed, 2 hot.
        public int Tier =>
            DeadEye ? 2 :
            Momentum >= 70f ? 2 :
            Momentum >= 40f ? 1 : 0;

        public override void ResetEffects()
        {
            if (!IsActiveGunner())
            {
                Momentum = 0f;
                DeadEye = false;
                DeadEyeTimer = 0;
            }
        }

        public override void PostUpdate()
        {
            if (!IsActiveGunner())
            {
                return;
            }

            ticksSinceHit++;

            if (DeadEye)
            {
                // Overdrive burns down on a timer; Momentum is pinned at the top meanwhile.
                Momentum = MaxMomentum;
                DeadEyeTimer--;

                if (Main.rand.NextBool(3))
                {
                    Dust.NewDust(
                        Player.position, Player.width, Player.height, DustID.GoldFlame);
                }

                if (DeadEyeTimer <= 0)
                {
                    DeadEye = false;
                    Momentum = 0f;
                }

                return;
            }

            // Reaching the top ignites Dead Eye.
            if (Momentum >= MaxMomentum)
            {
                ActivateDeadEye();
                return;
            }

            // Momentum bleeds away once you stop landing shots.
            if (ticksSinceHit > 30)
            {
                float decay = HasGun("Rapid Chamber") ? 1.0f : 1.5f;
                Momentum -= decay;

                if (Momentum < 0f)
                {
                    Momentum = 0f;
                }
            }
        }

        private void ActivateDeadEye()
        {
            DeadEye = true;
            DeadEyeTimer = 300 + (HasGun("Deadshot") ? 120 : 0);
            Momentum = MaxMomentum;

            CombatText.NewText(Player.Hitbox, Color.Gold, "DEAD EYE");

            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(
                    Player.position, Player.width, Player.height, DustID.GoldFlame);
            }
        }

        public override void OnHitNPCWithProj(
            Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!IsActiveGunner() || Player.HeldItem.useAmmo != AmmoID.Bullet)
            {
                return;
            }

            ticksSinceHit = 0;

            if (!DeadEye)
            {
                // Landing a shot builds Momentum. Quick Trigger builds it faster.
                Momentum += HasGun("Quick Trigger") ? 6f : 4f;

                if (Momentum > MaxMomentum)
                {
                    Momentum = MaxMomentum;
                }
            }
            else if (target.life <= 0 && HasKeystone("Trigger Discipline"))
            {
                // Trigger Discipline: kills during Dead Eye keep the rampage going.
                DeadEyeTimer += 60;
            }
        }

        public override void ModifyShootStats(
            Item item,
            ref Vector2 position,
            ref Vector2 velocity,
            ref int type,
            ref int damage,
            ref float knockback)
        {
            if (!IsActiveGunner() || item.useAmmo != AmmoID.Bullet)
            {
                return;
            }

            float dmgBonus = DamageBonus();

            damage = (int)(damage * (1f + dmgBonus));
            velocity *= 1f + (DeadEye ? 0.15f : 0f);
        }

        public override void ModifyWeaponCrit(Item item, ref float crit)
        {
            if (!IsActiveGunner() || item.useAmmo != AmmoID.Bullet)
            {
                return;
            }

            if (DeadEye)
            {
                crit += HasGun("Executioner") ? 28f : 20f;
            }
            else if (Tier == 2)
            {
                crit += 5f;
            }
        }

        public override float UseSpeedMultiplier(Item item)
        {
            if (!IsActiveGunner() || item.useAmmo != AmmoID.Bullet)
            {
                return 1f;
            }

            float bonus = SpeedBonus();

            // Hair Trigger: sharper fire-rate scaling.
            if (HasGun("Hair Trigger"))
            {
                bonus *= 1.35f;
            }

            return 1f + bonus;
        }

        private float DamageBonus()
        {
            float bonus = DeadEye ? 0.30f : Tier == 2 ? 0.15f : Tier == 1 ? 0.08f : 0f;

            // Bullet Storm: bigger damage at every Momentum tier.
            if (bonus > 0f && HasGun("Bullet Storm"))
            {
                bonus += 0.05f;
            }

            return bonus;
        }

        private float SpeedBonus()
        {
            return DeadEye ? 0.25f : Tier == 2 ? 0.15f : Tier == 1 ? 0.08f : 0f;
        }

        // Dead Eye makes bullets pierce and ignore armor (read by GunnerGlobalProjectile).
        public int DeadEyePierce =>
            !DeadEye ? 0 : HasGun("Full Auto") ? 2 : 1;

        public int DeadEyeArmorPen =>
            !DeadEye ? 0 : HasGun("Armor Piercing") ? 20 : 8;

        // Lets a signature gun (e.g. the Suppressor) feed extra Momentum on hit. No effect
        // during Dead Eye (Momentum is pinned there).
        public void AddMomentum(float amount)
        {
            if (!IsActiveGunner() || DeadEye)
            {
                return;
            }

            Momentum += amount;

            if (Momentum > MaxMomentum)
            {
                Momentum = MaxMomentum;
            }
        }

        private bool HasGun(string node)
        {
            var soul = Player.GetModPlayer<EterniaPlayer>();
            var stats = Player.GetModPlayer<EterniaStatsPlayer>();

            return stats.HasActivePassive(soul.ActiveSoul, node);
        }

        private bool HasKeystone(string keystone)
        {
            return Player.GetModPlayer<EterniaStatsPlayer>()
                .UnlockedPassives.Contains(keystone);
        }

        public bool IsActiveGunner()
        {
            var soul = Player.GetModPlayer<EterniaPlayer>();

            return soul.HasClassSoul &&
                soul.ActiveSoul == SoulId.Ranger &&
                Player.GetModPlayer<SubclassPlayer>().CurrentSubclass == "Gunner";
        }
    }
}
