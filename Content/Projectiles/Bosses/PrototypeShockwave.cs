using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Projectiles.Bosses
{
    // The core venting: a ring of energy that expands outward from a fixed point. Phase 3's
    // "grandes explosiones de energia". The hitbox grows with the ring, so you have to move out of
    // the way, not just tank it.
    public class PrototypeShockwave : ModProjectile
    {
        private const int Lifetime = 42;
        private const float MaxRadius = 220f;

        private Vector2 Origin
        {
            get => new Vector2(Projectile.ai[0], Projectile.ai[1]);
            set
            {
                Projectile.ai[0] = value.X;
                Projectile.ai[1] = value.Y;
            }
        }

        public override string Texture =>
            "ETERNIA/Content/Items/Souls/EmptySoul";

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Lifetime;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
        }

        public override void OnSpawn(Terraria.DataStructures.IEntitySource source)
        {
            Origin = Projectile.Center;
        }

        public override void AI()
        {
            float t = 1f - Projectile.timeLeft / (float)Lifetime;
            int radius = (int)(MaxRadius * t);

            // Grow the hitbox as a thick ring, kept centered on the origin.
            int size = System.Math.Max(8, radius * 2);
            Vector2 center = Origin;
            Projectile.position = center - new Vector2(size / 2f);
            Projectile.width = size;
            Projectile.height = size;

            Lighting.AddLight(center, 0.6f, 0.8f, 1f);

            for (int i = 0; i < 3; i++)
            {
                float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                Vector2 pos = center + angle.ToRotationVector2() * radius;

                Dust d = Dust.NewDustPerfect(pos, DustID.BlueTorch, Vector2.Zero, 100, default, 1.6f);
                d.noGravity = true;
                d.velocity = angle.ToRotationVector2() * 2f;
            }
        }

        // Only the leading edge of the ring hurts -- the hollow center is safe.
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float t = 1f - Projectile.timeLeft / (float)Lifetime;
            float radius = MaxRadius * t;

            Vector2 target = targetHitbox.Center.ToVector2();
            float dist = Vector2.Distance(target, Origin);

            return System.Math.Abs(dist - radius) < 26f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // A soft expanding ring of the boss's colour.
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            float t = 1f - Projectile.timeLeft / (float)Lifetime;
            float radius = MaxRadius * t;
            Color color = new Color(120, 210, 255) * (1f - t) * 0.9f;

            int segments = 48;

            for (int i = 0; i < segments; i++)
            {
                float angle = MathHelper.TwoPi * i / segments;
                Vector2 pos = Origin + angle.ToRotationVector2() * radius - Main.screenPosition;

                Main.spriteBatch.Draw(
                    pixel,
                    pos,
                    null,
                    color,
                    angle,
                    new Vector2(0.5f),
                    new Vector2(6f, 3f),
                    SpriteEffects.None,
                    0f);
            }

            return false;
        }
    }
}
