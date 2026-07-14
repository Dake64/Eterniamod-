using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Projectiles.Ranger
{
    // Eternal Horizon's signature: spawned by each Perfect Shot. Pierces infinitely, halves the
    // target's defense, grows stronger the farther it has flown, leaves a stellar trail and
    // bursts on every hit. The Archer's mastery of precision and distance made manifest.
    public class ConstellationArrow : ModProjectile
    {
        public override string Texture =>
            "ETERNIA/Content/Projectiles/LightningBoltProjectile";

        private Vector2 startPos;
        private bool started;

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
        }

        public override void AI()
        {
            if (!started)
            {
                startPos = Projectile.Center;
                started = true;
            }

            if (Projectile.velocity != Vector2.Zero)
            {
                Projectile.rotation =
                    Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }

            for (int i = 0; i < 2; i++)
            {
                int d = Dust.NewDust(
                    Projectile.position, Projectile.width, Projectile.height,
                    DustID.YellowStarDust, 0f, 0f, 100, default, 1.3f);

                Main.dust[d].noGravity = true;
            }

            Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 0.3f);
        }

        // Farther travelled = harder hit (up to +100% at ~60 blocks).
        private float DistanceScale()
        {
            float blocks = Vector2.Distance(startPos, Projectile.Center) / 16f;

            return 1f + MathHelper.Clamp(blocks / 60f, 0f, 1f);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.DefenseEffectiveness *= 0.5f; // ignores 50% of defense
            modifiers.SourceDamage *= DistanceScale();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Stellar burst on impact.
            for (int i = 0; i < 12; i++)
            {
                int d = Dust.NewDust(
                    target.position, target.width, target.height,
                    DustID.YellowStarDust, 0f, 0f, 100, default, 1.5f);

                Main.dust[d].noGravity = true;
            }
        }

        public override Color? GetAlpha(Color lightColor) =>
            new Color(255, 240, 160, 120);
    }
}
