using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Projectiles.Warrior
{
    // The Sanguine Cleaver's charged payload: a single, brutal blood guillotine released when the
    // player lets go of the alt-fire charge (see SanguineCharge). Its SIZE and DAMAGE are baked in
    // at spawn by the charger; ai[0] carries the charge fraction (0..1) only so the visual can
    // scale to match. Slow, penetrates the whole line, lingers -- one heavy execution stroke.
    //
    // Melee + player-owned, so it inherits the Swordsman's bleed / Crimson Trail pipeline like any
    // slash. Drawn with the shared slash sprite (default draw), tinted deep crimson.
    public class SanguineGuillotine : ModProjectile
    {
        public override string Texture => "ETERNIA/Content/Projectiles/Warrior/CrimsonSlash";

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 46;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.light = 0.5f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12; // can bite a lingering foe ~twice
        }

        public override void AI()
        {
            // Scale to the charge that made it: a tapped release is a big crescent, a full charge
            // is enormous. Set the hitbox to match on the first frame.
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                float frac = MathHelper.Clamp(Projectile.ai[0], 0f, 1f);
                Projectile.scale = 1.4f + 1.7f * frac;
                int s = (int)(60 * Projectile.scale);
                Projectile.Resize(s, s);
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity *= 0.95f; // the stroke slows as it bites down

            // Heavy blood -- a thick spray, not a sparkle. It's a guillotine.
            for (int i = 0; i < 2; i++)
            {
                if (Main.rand.NextBool())
                {
                    Dust d = Dust.NewDustPerfect(
                        Projectile.Center + Main.rand.NextVector2Circular(Projectile.width * 0.4f, Projectile.height * 0.4f),
                        DustID.Blood,
                        Main.rand.NextVector2Circular(3f, 3f),
                        30, default, Main.rand.NextFloat(1.2f, 1.9f));
                    d.noGravity = true;
                }
            }

            Lighting.AddLight(Projectile.Center, 0.6f, 0.1f, 0.14f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 14; i++)
            {
                Dust.NewDustPerfect(
                    target.Center,
                    DustID.Blood,
                    Main.rand.NextVector2Circular(6f, 6f),
                    20, default, 1.6f);
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            float fade = MathHelper.Clamp(Projectile.timeLeft / 12f, 0f, 1f);
            return new Color(150, 20, 28) * fade;
        }
    }
}
