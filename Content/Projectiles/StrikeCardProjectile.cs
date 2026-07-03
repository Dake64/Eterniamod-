using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Projectiles
{
    public class StrikeCardProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 32;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.DamageType =
                DamageClass.Magic;

            Projectile.penetrate = 1;

            Projectile.timeLeft = 300;

            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.rotation += 0.3f;

            Dust.NewDust(
                Projectile.position,
                Projectile.width,
                Projectile.height,
                DustID.GemRuby);
        }
    }
}