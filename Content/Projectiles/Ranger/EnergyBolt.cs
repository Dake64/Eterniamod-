using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Projectiles.Ranger
{
    // Shared bolt for the Energy Gunner's Hardmode energy arsenal. Ranged damage so the
    // temperature-zone bonuses and the Plasma Conductors global apply. Its tint (ai[0] as a
    // packed RGB hint isn't worth it here) stays a uniform energy cyan; weapons differ by
    // fire rate, damage and shoot speed. Reuses the LightningBoltProjectile texture.
    public class EnergyBolt : ModProjectile
    {
        public override string Texture =>
            "ETERNIA/Content/Projectiles/LightningBoltProjectile";

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1; // snappy, laser-like travel
        }

        public override void AI()
        {
            if (Projectile.velocity != Vector2.Zero)
            {
                Projectile.rotation =
                    Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }

            if (Main.rand.NextBool(2))
            {
                int d = Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Electric,
                    0f, 0f, 100, default, 0.9f);

                Main.dust[d].noGravity = true;
            }

            Lighting.AddLight(Projectile.Center, 0.2f, 0.5f, 0.6f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(120, 220, 255, 160);
        }
    }
}
