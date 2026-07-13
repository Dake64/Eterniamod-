using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Projectiles.Ranger
{
    // Guided bolt: curves toward the nearest enemy. For ion/neutron/quantum guided weapons.
    public class EnergyHomingBolt : ModProjectile
    {
        private const float SightRange = 480f;
        private const float TurnRate = 0.09f;

        public override string Texture =>
            "ETERNIA/Content/Projectiles/LightningBoltProjectile";

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            NPC target = FindTarget();

            if (target != null)
            {
                Vector2 desired =
                    Vector2.Normalize(target.Center - Projectile.Center)
                    * Projectile.velocity.Length();

                Projectile.velocity =
                    Vector2.Lerp(Projectile.velocity, desired, TurnRate);
            }

            if (Projectile.velocity != Vector2.Zero)
            {
                Projectile.rotation =
                    Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }

            if (Main.rand.NextBool(2))
            {
                Dust d = Dust.NewDustDirect(
                    Projectile.position, Projectile.width, Projectile.height,
                    DustID.Electric, 0f, 0f, 100, default, 0.9f);
                d.noGravity = true;
            }

            Lighting.AddLight(Projectile.Center, 0.2f, 0.5f, 0.5f);
        }

        private NPC FindTarget()
        {
            NPC nearest = null;
            float best = SightRange;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (!npc.active || npc.friendly ||
                    npc.dontTakeDamage || !npc.CanBeChasedBy(Projectile))
                {
                    continue;
                }

                float dist = Vector2.Distance(Projectile.Center, npc.Center);

                if (dist < best)
                {
                    best = dist;
                    nearest = npc;
                }
            }

            return nearest;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(140, 235, 210, 160);
        }
    }
}
