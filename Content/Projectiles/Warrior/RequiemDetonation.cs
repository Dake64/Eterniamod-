using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Projectiles.Warrior
{
    // Crimson Requiem's bespoke mechanic pays off here: when a foe's Requiem marks hit the
    // threshold, the marks DETONATE -- a crimson burst centred on that foe that strikes everything
    // in the radius once (group execution). Melee + player-owned, so the blast bleeds and banks
    // Crimson Trail on everything it catches, exactly like a slash. Very short-lived: it is a
    // pulse, not a lingering field. Drawn with an expanding ring of crimson dust.
    public class RequiemDetonation : ModProjectile
    {
        public override string Texture => "ETERNIA/Content/Projectiles/Warrior/CrimsonSlash";

        public override void SetDefaults()
        {
            Projectile.width = 180;
            Projectile.height = 180;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 12;             // a pulse
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.light = 0.7f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;  // each foe in the blast is struck exactly once
        }

        public override void AI()
        {
            // Expanding ring of crimson: a hard pulse outward on the first frames.
            float t = 1f - Projectile.timeLeft / 12f;
            float radius = Projectile.width * 0.5f * MathHelper.Clamp(t * 1.4f, 0f, 1f);

            int rays = 26;
            for (int i = 0; i < rays; i++)
            {
                if (!Main.rand.NextBool(2))
                {
                    continue;
                }

                Vector2 dir = (MathHelper.TwoPi * i / rays).ToRotationVector2();
                Dust d = Dust.NewDustPerfect(
                    Projectile.Center + dir * radius,
                    DustID.Blood,
                    dir * Main.rand.NextFloat(1f, 4f),
                    30, default, Main.rand.NextFloat(1.3f, 2f));
                d.noGravity = true;
            }

            Lighting.AddLight(Projectile.Center, 0.9f, 0.1f, 0.16f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(
                    target.Center,
                    DustID.Blood,
                    Main.rand.NextVector2Circular(5f, 5f),
                    20, default, 1.5f);
            }
        }

        public override bool PreDraw(ref Color lightColor) => false; // dust ring only
    }
}
