using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Projectiles.Warrior
{
    // Bonewarden Sabre's bespoke mechanic: the warden of the dead makes the bone TURN ON its
    // owner. On a landed hit a jagged spike erupts through the struck foe and lingers, biting a
    // couple more times before it crumbles -- a small pocket of zone denial fixed to the wound.
    //
    // It is a melee projectile owned by the player, so it inherits the Swordsman's bleed / Crimson
    // Trail pipeline like any slash. Stationary, short-lived, drawn as a burst of bone dust.
    public class BoneSpike : ModProjectile
    {
        public override string Texture => "ETERNIA/Content/Projectiles/Warrior/CrimsonSlash";

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 44;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 42;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.light = 0.15f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14; // ~3 bites over its life
        }

        public override void AI()
        {
            Projectile.velocity = Vector2.Zero; // erupts in place and holds

            // Erupt fast on the first frames, then hold: a quick spike of bone dust upward.
            if (Projectile.timeLeft > 34)
            {
                for (int i = 0; i < 3; i++)
                {
                    Dust b = Dust.NewDustPerfect(
                        Projectile.Bottom - new Vector2(0f, Main.rand.NextFloat(Projectile.height)),
                        DustID.Bone,
                        new Vector2(Main.rand.NextFloat(-0.6f, 0.6f), -Main.rand.NextFloat(1f, 3f)),
                        0, default, Main.rand.NextFloat(1f, 1.5f));
                    b.noGravity = true;
                }
            }
            else if (Main.rand.NextBool(3))
            {
                Dust b = Dust.NewDustPerfect(
                    Projectile.Center + Main.rand.NextVector2Circular(6f, Projectile.height * 0.4f),
                    DustID.Bone, Vector2.Zero, 60, default, 1f);
                b.noGravity = true;
            }

            Lighting.AddLight(Projectile.Center, 0.28f, 0.26f, 0.2f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(
                    target.Center,
                    DustID.Bone,
                    Main.rand.NextVector2Circular(3f, 3f),
                    0, default, 1.2f);
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}
