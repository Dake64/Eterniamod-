using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Projectiles
{
    public class BasicCardProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 28;

            Projectile.friendly = true;

            Projectile.DamageType =
                DamageClass.Magic;

            Projectile.penetrate = 1;

            Projectile.timeLeft = 300;

            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.rotation += 0.25f;

            Dust.NewDust(
                Projectile.position,
                Projectile.width,
                Projectile.height,
                DustID.GoldCoin);
        }
    }
}