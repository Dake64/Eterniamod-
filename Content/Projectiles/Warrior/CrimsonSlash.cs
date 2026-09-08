using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items;

namespace Eternia.Content.Projectiles.Warrior
{
    // The bleeding slash thrown by every Swordsman edge weapon -- ONE projectile that behaves
    // GENUINELY differently per weapon, driven by the firing sword's SlashStyle. This is what
    // makes the blades feel distinct without a clone projectile per sword; a mechanic that needs
    // truly bespoke behaviour (ground shockwave, lingering fire, mark-detonation) gets its own
    // class instead of a style here.
    //
    // The style, colour and scale are captured ONCE from the firing weapon (so switching weapons
    // mid-flight can't mutate a slash already in the air). ai[1] == 1 tags a Split fragment so
    // fragments never split again.
    public class CrimsonSlash : ModProjectile
    {
        // Uses the existing Content/Projectiles/Warrior/CrimsonSlash.png (default texture path).

        private Color slashColor = new Color(200, 45, 50);
        private SlashStyle style = SlashStyle.Straight;
        private bool configured;

        private bool IsFragment => Projectile.ai[1] == 1f;

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 55;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.light = 0.3f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1; // hit each enemy once
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            // Capture the firing sword's identity once, then configure per style.
            if (!configured)
            {
                if (IsFragment)
                {
                    style = SlashStyle.Straight;
                    configured = true;
                }
                else if (owner != null && owner.active &&
                    owner.HeldItem?.ModItem is IBleedWeapon bleed)
                {
                    slashColor = bleed.SlashColor;
                    style = bleed.Style;
                    Projectile.scale = bleed.SlashScale;
                    ConfigureForStyle();
                    configured = true;
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation();

            switch (style)
            {
                case SlashStyle.Homing:
                    SteerTowardBleeding(0.055f, 560f);
                    break;
                case SlashStyle.HomingAggressive:
                    SteerTowardBleeding(0.14f, 900f);
                    break;
                case SlashStyle.Return:
                    ReturnAI(owner);
                    break;
            }

            Lighting.AddLight(Projectile.Center, slashColor.ToVector3() * 0.35f);
            EmitStyleDust();
        }

        // Set spawn-time properties that differ by style. Runs on the first frame the weapon is
        // known; the one-frame default before that is harmless for a slash.
        private void ConfigureForStyle()
        {
            switch (style)
            {
                case SlashStyle.Pierce:
                    Projectile.penetrate = -1;      // runs through the whole line
                    Projectile.localNPCHitCooldown = 10;
                    break;
                case SlashStyle.Wide:
                    Projectile.scale *= 1.6f;
                    int s = (int)(46 * Projectile.scale);
                    Projectile.Resize(s, s);
                    Projectile.velocity *= 0.45f;   // barely travels
                    Projectile.timeLeft = 28;
                    Projectile.penetrate = -1;      // the crescent sweeps everything in it
                    Projectile.localNPCHitCooldown = 12;
                    break;
                case SlashStyle.Return:
                    Projectile.penetrate = -1;
                    Projectile.tileCollide = false; // a boomerang shouldn't die on a wall
                    Projectile.timeLeft = 120;
                    Projectile.localNPCHitCooldown = 16;
                    break;
                case SlashStyle.HomingAggressive:
                case SlashStyle.Homing:
                    Projectile.penetrate = 3;
                    break;
            }
        }

        private void SteerTowardBleeding(float turn, float range)
        {
            NPC target = FindTarget(range);

            if (target == null)
            {
                return;
            }

            float speed = Projectile.velocity.Length();
            Vector2 want =
                (target.Center - Projectile.Center).SafeNormalize(Projectile.velocity) * speed;

            Projectile.velocity = Vector2.Lerp(Projectile.velocity, want, turn);
        }

        // Prefer a bleeding foe (this is a Swordsman slash -- it hunts the wounded), else the
        // nearest hostile.
        private NPC FindTarget(float range)
        {
            int bleedType = ModContent.BuffType<Content.Buffs.BleedDebuff>();

            NPC best = null;
            float bestDist = range * range;
            bool bestBleeds = false;

            foreach (NPC npc in Main.npc)
            {
                if (!npc.active || npc.friendly || npc.dontTakeDamage ||
                    npc.immortal || npc.CountsAsACritter)
                {
                    continue;
                }

                float d = Vector2.DistanceSquared(npc.Center, Projectile.Center);

                if (d > range * range)
                {
                    continue;
                }

                bool bleeds = npc.HasBuff(bleedType);

                // A bleeding target always beats a non-bleeding one; among equals, nearest wins.
                if ((bleeds && !bestBleeds) || (bleeds == bestBleeds && d < bestDist))
                {
                    best = npc;
                    bestDist = d;
                    bestBleeds = bleeds;
                }
            }

            return best;
        }

        private void ReturnAI(Player owner)
        {
            Projectile.ai[0]++;

            if (Projectile.ai[0] < 22f || owner == null || !owner.active)
            {
                return;
            }

            Vector2 toOwner = owner.Center - Projectile.Center;

            if (toOwner.Length() < 42f)
            {
                Projectile.Kill();
                return;
            }

            Vector2 want = toOwner.SafeNormalize(Vector2.Zero) * 16f;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, want, 0.12f);
        }

        private void EmitStyleDust()
        {
            // One dust most frames, themed by style -- present but never a festival (per design).
            switch (style)
            {
                case SlashStyle.Return: // void reaver: dark, sinking motes
                    if (Main.rand.NextBool(2))
                    {
                        Dust d = Dust.NewDustPerfect(
                            Projectile.Center, DustID.Shadowflame,
                            -Projectile.velocity * 0.05f, 120, default, 0.9f);
                        d.noGravity = true;
                    }
                    break;

                case SlashStyle.Wide: // heavy cleave: thick blood spray
                    if (Main.rand.NextBool(1))
                    {
                        Dust.NewDustPerfect(
                            Projectile.Center + Main.rand.NextVector2Circular(10f, 10f),
                            DustID.Blood, Main.rand.NextVector2Circular(2f, 2f), 0, default, 1.3f);
                    }
                    break;

                default: // blood mist trailing the slash
                    if (Main.rand.NextBool(2))
                    {
                        Dust d = Dust.NewDustPerfect(
                            Projectile.Center, DustID.Blood,
                            -Projectile.velocity * 0.1f, 40, slashColor, 1f);
                        d.noGravity = true;
                    }
                    break;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // A small blood burst on every landed slash, coloured to the blade.
            for (int i = 0; i < 6; i++)
            {
                Dust.NewDustPerfect(
                    target.Center, DustID.Blood,
                    Main.rand.NextVector2Circular(3f, 3f), 40, slashColor, 1.1f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            // Split: burst into three short fragment slashes. Fragments carry ai[1]=1 so they
            // never split again. Spawned only by the owner so multiplayer stays in sync.
            if (style == SlashStyle.Split && !IsFragment &&
                Main.myPlayer == Projectile.owner)
            {
                Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.UnitX);

                for (int i = -1; i <= 1; i++)
                {
                    Vector2 v = dir.RotatedBy(MathHelper.ToRadians(28 * i)) * 7f;

                    Projectile.NewProjectile(
                        Projectile.GetSource_Death(),
                        Projectile.Center, v,
                        ModContent.ProjectileType<CrimsonSlash>(),
                        System.Math.Max(1, Projectile.damage * 2 / 3),
                        Projectile.knockBack * 0.5f,
                        Projectile.owner,
                        ai1: 1f);
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            float fade = MathHelper.Clamp(Projectile.timeLeft / 10f, 0f, 1f);
            return slashColor * fade;
        }
    }
}
