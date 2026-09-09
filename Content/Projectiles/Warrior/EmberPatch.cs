using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Projectiles.Warrior
{
    // Molten Gutripper's identity: the slash SHEDS. It drops burning slag as it flies, so the
    // Gutripper's damage keeps working after the swing is over -- a smouldering lane rather than a
    // clean cut. Shorter-lived than Thornrender's seeds and it burns rather than bites, so the two
    // "leave something behind" blades still feel different: thorns are a trap, embers are a fire.
    //
    // Melee + player-owned, so it inherits the Swordsman bleed / Crimson Trail pipeline.
    public class EmberPatch : ModProjectile
    {
        public override string Texture => "ETERNIA/Content/Projectiles/Warrior/CrimsonSlash";

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 70;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            // Slag falls and settles rather than hanging in the air.
            Projectile.velocity.X *= 0.92f;
            if (Projectile.velocity.Y < 3f)
            {
                Projectile.velocity.Y += 0.12f;
            }

            if (Main.rand.NextBool(2))
            {
                Dust d = Dust.NewDustPerfect(
                    Projectile.Center + Main.rand.NextVector2Circular(6f, 6f),
                    DustID.Torch,
                    new Vector2(Main.rand.NextFloat(-0.4f, 0.4f), -Main.rand.NextFloat(0.6f, 1.8f)),
                    0, default, Main.rand.NextFloat(0.9f, 1.5f));
                d.noGravity = true;
            }

            Lighting.AddLight(Projectile.Center, 0.55f, 0.28f, 0.06f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 120);

            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(
                    target.Center, DustID.Torch,
                    Main.rand.NextVector2Circular(3f, 3f), 0, default, 1.2f);
            }
        }

        public override bool PreDraw(ref Color lightColor) => false; // drawn as dust
    }
}
