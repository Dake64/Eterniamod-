using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Projectiles.Bosses
{
    // The plasma-cannon shot. Fast, glowing, ignores tiles so an arena's terrain never eats it.
    // Spawned by Prototype-01's cannon and drone modules; damage is set by the boss at spawn.
    public class PrototypePlasmaBolt : ModProjectile
    {
        public override string Texture =>
            "ETERNIA/Content/Projectiles/LightningBoltProjectile";

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            if (Projectile.velocity != Vector2.Zero)
            {
                Projectile.rotation =
                    Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }

            Lighting.AddLight(Projectile.Center, 0.2f, 0.6f, 0.8f);

            if (Main.rand.NextBool(2))
            {
                Dust d = Dust.NewDustPerfect(
                    Projectile.Center,
                    DustID.BlueTorch,
                    Projectile.velocity * -0.15f,
                    120,
                    default,
                    1.1f);

                d.noGravity = true;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(120, 220, 255, 150);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust d = Dust.NewDustDirect(
                    Projectile.position, Projectile.width, Projectile.height,
                    DustID.BlueTorch, 0f, 0f, 120, default, 1.2f);

                d.noGravity = true;
                d.velocity *= 1.8f;
            }
        }
    }
}
