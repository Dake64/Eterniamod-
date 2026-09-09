using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Projectiles.Warrior
{
    // Thornrender's identity: the slash does not just pass through, it PLANTS. Every few frames
    // the travelling slash drops one of these -- a thorn that roots where it lands and keeps
    // biting anything that walks into it.
    //
    // That makes Thornrender the zone-control blade: its damage is not in the slash itself but in
    // the ground it has already covered, so it rewards cutting across a lane rather than aiming
    // at one target.
    //
    // Melee + player-owned, so it inherits the Swordsman bleed / Crimson Trail pipeline.
    public class ThornSeed : ModProjectile
    {
        public override string Texture => "ETERNIA/Content/Projectiles/Warrior/CrimsonSlash";

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 110;          // it lingers, but not long enough to carpet the screen
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30; // bites a loiterer repeatedly, but slowly
        }

        public override void AI()
        {
            Projectile.velocity = Vector2.Zero;  // rooted

            // Sprouting: a quick flick of leaves, then it just sits and waits.
            if (Projectile.timeLeft > 142)
            {
                for (int i = 0; i < 2; i++)
                {
                    Dust d = Dust.NewDustPerfect(
                        Projectile.Center,
                        DustID.Grass,
                        new Vector2(Main.rand.NextFloat(-1.2f, 1.2f), Main.rand.NextFloat(-1.8f, -0.4f)),
                        0, default, 1.1f);
                    d.noGravity = true;
                }
            }
            else if (Main.rand.NextBool(20))
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Grass, Vector2.Zero, 120, default, 0.8f);
            }

            Lighting.AddLight(Projectile.Center, 0.10f, 0.20f, 0.08f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 4; i++)
            {
                Dust.NewDustPerfect(
                    target.Center, DustID.Grass,
                    Main.rand.NextVector2Circular(2.5f, 2.5f), 0, default, 1f);
            }
        }

        public override bool PreDraw(ref Color lightColor) => false; // drawn as dust
    }
}
