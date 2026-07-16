using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Projectiles.Bosses
{
    // An energy "copy" the core spits out in phase 2+. It winds up briefly, then homes on the
    // nearest player like a guided missile and detonates on contact or when its time runs out.
    public class PrototypeDrone : ModProjectile
    {
        private const int WindupTicks = 45;
        private const float HomingAccel = 0.35f;
        private const float MaxSpeed = 11f;

        public override string Texture =>
            "ETERNIA/Content/Projectiles/IceBoltProjectile";

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 360;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Projectile.ai[0]++;

            Lighting.AddLight(Projectile.Center, 0.3f, 0.5f, 0.9f);

            if (Main.rand.NextBool())
            {
                Dust d = Dust.NewDustPerfect(
                    Projectile.Center,
                    DustID.BlueTorch,
                    Main.rand.NextVector2Circular(1f, 1f),
                    120,
                    default,
                    1f);

                d.noGravity = true;
            }

            // Wind-up: hover and pulse before locking on, so it is a readable threat.
            if (Projectile.ai[0] < WindupTicks)
            {
                Projectile.velocity *= 0.94f;
                Projectile.rotation += 0.3f;
                return;
            }

            Player target = FindTarget();

            if (target != null)
            {
                Vector2 toTarget =
                    (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitX);

                Projectile.velocity =
                    (Projectile.velocity + toTarget * HomingAccel)
                        .SafeNormalize(Vector2.UnitX) *
                    System.Math.Min(MaxSpeed, Projectile.velocity.Length() + 0.3f);
            }

            Projectile.rotation =
                Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        private Player FindTarget()
        {
            Player best = null;
            float bestDist = float.MaxValue;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player p = Main.player[i];

                if (!p.active || p.dead)
                {
                    continue;
                }

                float dist = Vector2.DistanceSquared(p.Center, Projectile.Center);

                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = p;
                }
            }

            return best;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(140, 200, 255, 120);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 16; i++)
            {
                Dust d = Dust.NewDustDirect(
                    Projectile.position, Projectile.width, Projectile.height,
                    DustID.BlueTorch, 0f, 0f, 100, default, 1.4f);

                d.noGravity = true;
                d.velocity *= 2.4f;
            }
        }
    }
}
