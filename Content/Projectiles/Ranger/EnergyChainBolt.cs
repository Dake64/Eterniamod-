using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Projectiles.Ranger
{
    // Chain-lightning bolt: on hit it arcs to nearby enemies. For Tesla/Arc/Ion weapons.
    // ai[0] carries how many extra jumps are left; each jump strikes for reduced damage.
    public class EnergyChainBolt : ModProjectile
    {
        private const float JumpRange = 200f;

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
            Projectile.extraUpdates = 1;
        }

        public override void OnSpawn(Terraria.DataStructures.IEntitySource source)
        {
            // Default to two arcs if the weapon did not set a jump count.
            if (Projectile.ai[0] <= 0f)
            {
                Projectile.ai[0] = 2f;
            }
        }

        public override void AI()
        {
            if (Projectile.velocity != Vector2.Zero)
            {
                Projectile.rotation =
                    Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }

            if (Main.rand.NextBool())
            {
                Dust d = Dust.NewDustDirect(
                    Projectile.position, Projectile.width, Projectile.height,
                    DustID.Electric, 0f, 0f, 100, default, 1f);
                d.noGravity = true;
            }

            Lighting.AddLight(Projectile.Center, 0.2f, 0.4f, 0.6f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(150, 220, 255, 150);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner != Main.myPlayer || Projectile.ai[0] <= 0f)
            {
                return;
            }

            NPC nearest = null;
            float best = JumpRange;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (!npc.active || npc.friendly ||
                    npc.dontTakeDamage || npc.whoAmI == target.whoAmI)
                {
                    continue;
                }

                float dist = Vector2.Distance(target.Center, npc.Center);

                if (dist < best)
                {
                    best = dist;
                    nearest = npc;
                }
            }

            if (nearest == null)
            {
                return;
            }

            Vector2 dir = Vector2.Normalize(nearest.Center - target.Center);

            int p = Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                target.Center,
                dir * Projectile.velocity.Length(),
                Projectile.type,
                System.Math.Max(1, (int)(Projectile.damage * 0.75f)),
                Projectile.knockBack,
                Projectile.owner);

            Main.projectile[p].ai[0] = Projectile.ai[0] - 1f;
        }
    }
}
