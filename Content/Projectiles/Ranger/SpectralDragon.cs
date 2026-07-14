using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Projectiles.Ranger
{
    // Dragonbone Bow: a spectral dragon that flies straight ahead, tearing through anything in
    // a line. Ranged so the Archer's damage bonuses apply. Reuses the LightningBolt texture.
    public class SpectralDragon : ModProjectile
    {
        public override string Texture =>
            "ETERNIA/Content/Projectiles/LightningBoltProjectile";

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 90;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (Projectile.velocity != Vector2.Zero)
            {
                Projectile.rotation =
                    Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }

            int d = Dust.NewDust(
                Projectile.position, Projectile.width, Projectile.height,
                DustID.BlueTorch, 0f, 0f, 100, default, 1.3f);

            Main.dust[d].noGravity = true;

            Lighting.AddLight(Projectile.Center, 0.2f, 0.4f, 0.7f);
        }

        public override Color? GetAlpha(Color lightColor) =>
            new Color(120, 180, 255, 120);
    }
}
