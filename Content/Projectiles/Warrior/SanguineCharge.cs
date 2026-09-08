using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Weapons.Warrior;

namespace Eternia.Content.Projectiles.Warrior
{
    // The Sanguine Cleaver's alt-fire charge. Held at the player's hand while the alt-fire button
    // is channelled; blood gathers on the blade as it builds. On release it unleashes ONE
    // SanguineGuillotine scaled and empowered by how long it charged, then dies.
    //
    // Owner-authoritative: only the owning client reads its own channel/aim and spawns the payload
    // (multiplayer-safe). Non-owner copies just pin to the hand and self-clean via the timeLeft
    // safety, so a missed kill packet can't leave a charge stuck on a remote screen.
    public class SanguineCharge : ModProjectile
    {
        public override string Texture => "ETERNIA/Content/Projectiles/Warrior/CrimsonSlash";

        // Frames to a full charge (~1.5s), and the smallest charge a release will bother firing.
        private const float MaxCharge = 90f;
        private const float MinReleaseCharge = 12f;

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = false;      // the charge itself doesn't hit; the payload does
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = (int)MaxCharge + 40; // safety cap so a stuck copy self-cleans
        }

        // We pin the position by hand every frame.
        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            // Dead, gone, or no longer holding the cleaver: stop charging.
            if (!owner.active || owner.dead ||
                owner.HeldItem?.ModItem is not SanguineCleaver)
            {
                Projectile.Kill();
                return;
            }

            float frac = MathHelper.Clamp(Projectile.ai[0] / MaxCharge, 0f, 1f);

            // Aim from the player toward the cursor (owner) or the synced aim (others).
            Vector2 aim =
                (owner.Center - Projectile.Center).LengthSquared() > 0f && Projectile.owner != Main.myPlayer
                    ? Projectile.velocity.SafeNormalize(new Vector2(owner.direction, 0f))
                    : (Main.MouseWorld - owner.Center).SafeNormalize(new Vector2(owner.direction, 0f));

            // Pin to the hand, blade pointing at the aim; keep the swing pose alive.
            Projectile.Center = owner.MountedCenter + aim * 26f;
            Projectile.rotation = aim.ToRotation();
            Projectile.velocity = aim;                 // store aim so remotes can face it
            owner.heldProj = Projectile.whoAmI;
            owner.direction = aim.X >= 0f ? 1 : -1;

            // Gathering blood: more, and faster, the fuller the charge. A red glow at full.
            int dustCount = 1 + (int)(frac * 3f);
            for (int i = 0; i < dustCount; i++)
            {
                Vector2 ring = Main.rand.NextVector2CircularEdge(26f, 26f) * (1f - 0.4f * frac);
                Dust d = Dust.NewDustPerfect(
                    Projectile.Center + ring,
                    DustID.Blood,
                    -ring * 0.06f, 40, default,
                    1f + frac);
                d.noGravity = true;
            }

            Lighting.AddLight(Projectile.Center, 0.4f * frac + 0.15f, 0.05f, 0.08f);

            // ---- Owner drives charge and release; everyone else just holds the pose ----
            if (Projectile.owner != Main.myPlayer)
            {
                return;
            }

            owner.SetDummyItemTime(2); // keep the "using" animation while we channel

            if (owner.channel)
            {
                if (Projectile.ai[0] < MaxCharge)
                {
                    Projectile.ai[0]++;
                }

                Projectile.timeLeft = 2; // stay alive as long as the owner keeps channelling
                return;
            }

            // Released: fire the guillotine scaled to the charge, then die.
            FireGuillotine(owner, frac, aim);
            Projectile.Kill();
        }

        private void FireGuillotine(Player owner, float frac, Vector2 aim)
        {
            if (Projectile.ai[0] < MinReleaseCharge)
            {
                return; // a mere tap fizzles -- no free hit for flicking the button
            }

            // Damage scales from ~1.3x a swing at the floor to ~3.4x at full charge, using the
            // held cleaver's real (class-boosted) weapon damage as the base.
            int baseDmg = owner.GetWeaponDamage(owner.HeldItem);
            int dmg = (int)(baseDmg * (1.3f + 2.1f * frac));
            float kb = 8f + 4f * frac;

            Projectile.NewProjectile(
                owner.GetSource_ItemUse(owner.HeldItem),
                owner.MountedCenter + aim * 30f,
                aim * (5f + 2f * frac),
                ModContent.ProjectileType<SanguineGuillotine>(),
                System.Math.Max(1, dmg),
                kb,
                owner.whoAmI,
                ai0: frac);

            // A wet burst at the point of release.
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDustPerfect(
                    owner.MountedCenter + aim * 24f,
                    DustID.Blood,
                    aim.RotatedByRandom(0.6f) * Main.rand.NextFloat(2f, 7f),
                    20, default, 1.6f);
            }
        }

        public override bool PreDraw(ref Color lightColor) => false; // all dust while charging
    }
}
