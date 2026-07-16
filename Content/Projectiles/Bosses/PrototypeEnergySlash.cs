using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Projectiles.Bosses
{
    // A thrown energy crescent from the sword module. Short-lived and fast -- it punctuates
    // Prototype-01's dash-slashes. Fades as it flies.
    public class PrototypeEnergySlash : ModProjectile
    {
        public override string Texture =>
            "ETERNIA/Content/Projectiles/Warrior/CrimsonSlash";

        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 44;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 45;
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

            // Decelerate into a fade so the slash reads as a swing, not a bullet.
            Projectile.velocity *= 0.96f;
            Projectile.alpha += 5;

            Lighting.AddLight(Projectile.Center, 0.5f, 0.7f, 0.9f);

            if (Main.rand.NextBool())
            {
                Dust d = Dust.NewDustPerfect(
                    Projectile.Center,
                    DustID.IceTorch,
                    Main.rand.NextVector2Circular(1.5f, 1.5f),
                    100,
                    default,
                    1.2f);

                d.noGravity = true;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(150, 230, 255, 90) * (1f - Projectile.alpha / 255f);
        }
    }
}
