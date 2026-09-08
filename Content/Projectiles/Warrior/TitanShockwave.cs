using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Projectiles.Warrior
{
    // Titan's Gutcleaver's bespoke mechanic: the colossus's blow SHAKES THE GROUND. On a landed
    // hit the cleaver sends a low wave rolling outward along the ground (a left/right pair), and
    // anything it rolls over takes a melee hit -- so the wave inherits the Swordsman's bleed and
    // Crimson Trail pipeline for free (WarriorBleedPlayer / SwordsmanPlayer.OnHitNPCWithProj).
    //
    // It is spawned once per swing, not once per enemy struck: SwordIdentityGlobalItem scans for
    // a wave this owner spawned in the last few frames before making another. Drawn entirely with
    // dust (no sprite yet), so it reads as an earthquake rather than a thrown missile.
    public class TitanShockwave : ModProjectile
    {
        // Placeholder path: a valid, already-shipped texture so the loader is happy. PreDraw
        // returns false, so the sprite is never actually drawn -- the wave is all dust.
        public override string Texture => "ETERNIA/Content/Projectiles/Warrior/CrimsonSlash";

        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;          // rolls through everything in its path
            Projectile.timeLeft = 30;
            Projectile.tileCollide = false;      // a shockwave shouldn't die on the first slope
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.light = 0.2f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;  // can clip a straggler twice as it passes
        }

        public override void AI()
        {
            // Grow tall as it rolls, so it reads as a spreading wall of debris.
            Projectile.scale = 1f + (30 - Projectile.timeLeft) * 0.03f;
            int h = (int)(36 * Projectile.scale);

            if (h != Projectile.height)
            {
                // Keep the base on the ground while it grows upward.
                Projectile.position.Y += Projectile.height - h;
                Projectile.height = h;
            }

            // Bleed off speed so it doesn't outrun the screen.
            Projectile.velocity.X *= 0.985f;

            // Ground-shake dust: earth kicked up along the base, plus a little blood since it is a
            // Swordsman wave. Themed to the Titan's amber husk, not a generic sparkle.
            if (Main.rand.NextBool())
            {
                Dust dirt = Dust.NewDustDirect(
                    new Vector2(Projectile.Center.X, Projectile.Bottom.Y - 6f),
                    Projectile.width, 6,
                    DustID.Dirt,
                    Projectile.velocity.X * 0.2f, -Main.rand.NextFloat(1.5f, 3.5f),
                    120, default, Main.rand.NextFloat(1.1f, 1.7f));
                dirt.noGravity = false;
            }

            if (Main.rand.NextBool(3))
            {
                Dust ember = Dust.NewDustPerfect(
                    Projectile.Center + Main.rand.NextVector2Circular(Projectile.width * 0.4f, Projectile.height * 0.4f),
                    DustID.Blood, new Vector2(Projectile.velocity.X * 0.1f, -1f), 40, default, 1.2f);
                ember.noGravity = true;
            }

            Lighting.AddLight(Projectile.Center, 0.4f, 0.28f, 0.12f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // A gout of earth and blood where the wave connects.
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDustPerfect(
                    target.Center,
                    Main.rand.NextBool() ? DustID.Dirt : DustID.Blood,
                    Main.rand.NextVector2Circular(4f, 4f) + new Vector2(0f, -2f),
                    30, default, 1.3f);
            }
        }

        // All dust -- the placeholder sprite is never drawn.
        public override bool PreDraw(ref Color lightColor) => false;
    }
}
