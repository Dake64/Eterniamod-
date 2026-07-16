using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Projectiles.Bosses
{
    // The energy crescent thrown by the Soulforged Sabre (Prototype-01's signature drop). A
    // friendly mirror of the boss's own sword arc.
    public class SoulSlash : ModProjectile
    {
        public override string Texture =>
            "ETERNIA/Content/Projectiles/Warrior/CrimsonSlash";

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 40;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
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

            Projectile.velocity *= 0.97f;
            Projectile.alpha += 6;

            Lighting.AddLight(Projectile.Center, 0.4f, 0.6f, 0.8f);

            if (Main.rand.NextBool())
            {
                Dust d = Dust.NewDustPerfect(
                    Projectile.Center,
                    DustID.IceTorch,
                    Main.rand.NextVector2Circular(1.2f, 1.2f),
                    100,
                    default,
                    1.1f);

                d.noGravity = true;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(150, 230, 255, 80) * (1f - Projectile.alpha / 255f);
        }
    }
}
