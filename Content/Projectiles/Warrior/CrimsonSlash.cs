using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items;

namespace Eternia.Content.Projectiles.Warrior
{
    // The bleeding slash thrown by every Swordsman blade -- ONE projectile that behaves genuinely
    // differently per weapon, driven by the firing sword's SlashStyle. There is exactly one style
    // per weapon, so no two blades fly alike.
    //
    // One class rather than nineteen: the motions are variations on "a slash travels and cuts", so
    // nineteen near-identical ModProjectiles would be duplication, not identity. A mechanic that
    // is NOT a slash gets its own class instead (ThornSeed, EmberPatch, TitanShockwave, BoneSpike,
    // SanguineGuillotine, RequiemDetonation).
    //
    // Style, colour and scale are captured ONCE from the firing weapon, so switching weapons
    // mid-flight cannot mutate a slash already in the air.
    //
    //   ai[0] -- style scratch timer (Return uses it)
    //   ai[1] -- variant: 0 normal, 1 shard (never shatters again), 2 echo (ghost, never echoes)
    public class CrimsonSlash : ModProjectile
    {
        private Color slashColor = new Color(200, 45, 50);
        private SlashStyle style = SlashStyle.Clean;
        private bool configured;
        private float baseScale = 1f;
        private int age;
        private bool lunged;

        private bool IsShard => Projectile.ai[1] == 1f;
        private bool IsEcho => Projectile.ai[1] == 2f;

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 40;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.light = 0.3f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1; // hit each enemy once by default
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            age++;

            if (!configured)
            {
                if (owner != null && owner.active && owner.HeldItem?.ModItem is IBleedWeapon bleed)
                {
                    slashColor = bleed.SlashColor;
                    style = bleed.Style;
                    baseScale = bleed.SlashScale;
                    Projectile.scale = bleed.SlashScale;
                }

                // Shards and echoes are offcuts of a parent slash: they keep the look but never
                // re-trigger the behaviour that produced them.
                if (IsShard)
                {
                    baseScale *= 0.6f;
                    Projectile.penetrate = 1;
                    Projectile.timeLeft = Math.Min(Projectile.timeLeft, 22);
                }
                else if (IsEcho)
                {
                    baseScale *= 0.8f;
                }
                else
                {
                    ConfigureForStyle();
                }

                configured = true;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();

            if (!IsShard && !IsEcho)
            {
                DriveStyle(owner);
            }

            Lighting.AddLight(Projectile.Center, slashColor.ToVector3() * 0.35f);
            EmitStyleDust();
        }

        // Spawn-time properties. Each style gets its own reach, weight and rhythm.
        private void ConfigureForStyle()
        {
            switch (style)
            {
                case SlashStyle.Clean:                       // neutral reference
                    Projectile.timeLeft = 30;
                    break;

                case SlashStyle.Ripping:                     // one of three in a burst: small, brief
                    Projectile.penetrate = 1;
                    Projectile.timeLeft = 24;
                    baseScale *= 0.75f;
                    break;

                case SlashStyle.Lance:                       // precise: fast, thin, runs the line
                    Projectile.penetrate = -1;
                    Projectile.tileCollide = false;
                    Projectile.timeLeft = 26;
                    Projectile.velocity *= 1.7f;
                    Projectile.localNPCHitCooldown = 10;
                    break;

                case SlashStyle.Lunge:                       // predatory: leaps at wounded prey
                    Projectile.penetrate = 3;
                    break;

                case SlashStyle.Serpent:                     // chaotic: weaves as it goes
                    Projectile.timeLeft = 46;
                    break;

                case SlashStyle.Growing:                     // heavy: swells as it advances
                    Projectile.penetrate = -1;
                    Projectile.timeLeft = 36;
                    Projectile.velocity *= 0.65f;
                    Projectile.localNPCHitCooldown = 12;
                    break;

                case SlashStyle.Seeding:                     // deliberate: plants thorns
                    Projectile.penetrate = 3;
                    Projectile.timeLeft = 42;
                    break;

                case SlashStyle.Quick:                       // snappy little cut
                    Projectile.timeLeft = 26;
                    Projectile.velocity *= 1.3f;
                    baseScale *= 0.85f;
                    break;

                case SlashStyle.Braking:                     // weighty: punches through, then bogs
                    Projectile.penetrate = -1;
                    Projectile.timeLeft = 42;
                    Projectile.localNPCHitCooldown = 12;
                    break;

                case SlashStyle.Ember:                        // smouldering: sheds burning slag
                    Projectile.penetrate = 3;
                    Projectile.timeLeft = 42;
                    break;

                case SlashStyle.Echo:                         // elegant: a ghost trails it
                    Projectile.timeLeft = 40;
                    break;

                case SlashStyle.Cross:                        // one fang of an X pair
                    Projectile.timeLeft = 30;
                    Projectile.velocity *= 1.15f;
                    break;

                case SlashStyle.Wide:                         // brutal guillotine crescent
                    ResizeCrescent(1.6f, 46);
                    Projectile.velocity *= 0.45f;
                    Projectile.timeLeft = 28;
                    Projectile.penetrate = -1;
                    Projectile.localNPCHitCooldown = 12;
                    break;

                case SlashStyle.Quake:                        // the widest, slowest cleave
                    ResizeCrescent(1.9f, 52);
                    Projectile.velocity *= 0.35f;
                    Projectile.timeLeft = 32;
                    Projectile.penetrate = -1;
                    Projectile.localNPCHitCooldown = 14;
                    break;

                case SlashStyle.Shatter:                      // radiant: bursts at the apex
                    Projectile.timeLeft = 34;
                    break;

                case SlashStyle.Return:                       // uncanny boomerang
                    Projectile.penetrate = -1;
                    Projectile.tileCollide = false;
                    Projectile.timeLeft = 120;
                    Projectile.localNPCHitCooldown = 16;
                    break;

                case SlashStyle.Homing:                       // relentless hunter
                    Projectile.penetrate = 3;
                    Projectile.timeLeft = 52;
                    break;

                case SlashStyle.Mark:                         // brands, then the Requiem detonates
                    Projectile.timeLeft = 40;
                    break;

                case SlashStyle.Phantom:                      // spectral drifter
                    Projectile.penetrate = 3;
                    Projectile.timeLeft = 52;
                    break;
            }
        }

        private void ResizeCrescent(float mult, int baseSize)
        {
            baseScale *= mult;
            Projectile.scale *= mult;
            int s = (int)(baseSize * Projectile.scale);
            if (s < 8)
            {
                s = 8;
            }

            Projectile.Resize(s, s);
        }

        // Per-frame motion. This is where the blades stop feeling alike.
        private void DriveStyle(Player owner)
        {
            switch (style)
            {
                case SlashStyle.Serpent:
                {
                    // Weaves across its own line of travel -- corruption never runs straight.
                    Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.UnitX);
                    Vector2 perp = new Vector2(-dir.Y, dir.X);
                    Projectile.position += perp * (float)Math.Cos(age * 0.34f) * 2.4f;
                    break;
                }

                case SlashStyle.Growing:
                {
                    float g = 1f + age * 0.030f;
                    Projectile.scale = baseScale * g;
                    int s = (int)(46 * Projectile.scale);
                    Projectile.Resize(Math.Max(8, s), Math.Max(8, s));
                    break;
                }

                case SlashStyle.Quake:
                {
                    float g = 1f + age * 0.018f;
                    Projectile.scale = baseScale * g;
                    int s = (int)(52 * Projectile.scale);
                    Projectile.Resize(Math.Max(8, s), Math.Max(8, s));
                    break;
                }

                case SlashStyle.Braking:
                    Projectile.velocity *= 0.955f;   // heavy metal bogging down
                    break;

                case SlashStyle.Lunge:
                {
                    // ONE decisive leap at a bleeding foe, then it commits. Not a homing curve --
                    // a predator's pounce.
                    if (!lunged && age > 4)
                    {
                        NPC prey = FindTarget(430f, true);
                        if (prey != null)
                        {
                            float speed = Projectile.velocity.Length() * 1.65f;
                            Projectile.velocity =
                                (prey.Center - Projectile.Center).SafeNormalize(Projectile.velocity) * speed;
                            lunged = true;
                        }
                    }
                    break;
                }

                case SlashStyle.Homing:
                    SteerTowardBleeding(0.15f, 900f);
                    break;

                case SlashStyle.Phantom:
                    SteerTowardBleeding(0.085f, 760f);
                    break;

                case SlashStyle.Return:
                {
                    ReturnAI(owner);
                    // It comes back LARGER -- the void gives it back heavier than it left.
                    if (Projectile.ai[0] > 22f)
                    {
                        Projectile.scale = baseScale * 1.45f;
                    }
                    break;
                }

                case SlashStyle.Seeding:
                    if (age % 10 == 0 && Main.myPlayer == Projectile.owner)
                    {
                        Projectile.NewProjectile(
                            Projectile.GetSource_FromAI(),
                            Projectile.Center, Vector2.Zero,
                            ModContent.ProjectileType<ThornSeed>(),
                            Math.Max(1, Projectile.damage / 3),
                            0f, Projectile.owner);
                    }
                    break;

                case SlashStyle.Ember:
                    if (age % 9 == 0 && Main.myPlayer == Projectile.owner)
                    {
                        Projectile.NewProjectile(
                            Projectile.GetSource_FromAI(),
                            Projectile.Center,
                            new Vector2(Projectile.velocity.X * 0.1f, 0.4f),
                            ModContent.ProjectileType<EmberPatch>(),
                            Math.Max(1, Projectile.damage / 4),
                            0f, Projectile.owner);
                    }
                    break;

                case SlashStyle.Shatter:
                    // Bursts at the apex of its arc rather than fizzling out at the end.
                    if (age >= 20)
                    {
                        Projectile.Kill();
                    }
                    break;
            }
        }

        private void SteerTowardBleeding(float turn, float range)
        {
            NPC target = FindTarget(range, false);

            if (target == null)
            {
                return;
            }

            float speed = Projectile.velocity.Length();
            Vector2 want =
                (target.Center - Projectile.Center).SafeNormalize(Projectile.velocity) * speed;

            Projectile.velocity = Vector2.Lerp(Projectile.velocity, want, turn);
        }

        // Prefer a bleeding foe -- this is a Swordsman slash, it hunts the wounded.
        // requireBleeding: the Lunge only pounces on prey that is ALREADY bleeding.
        private NPC FindTarget(float range, bool requireBleeding)
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

                bool bleeds = npc.HasBuff(bleedType);

                if (requireBleeding && !bleeds)
                {
                    continue;
                }

                float d = Vector2.DistanceSquared(npc.Center, Projectile.Center);

                if (d > range * range)
                {
                    continue;
                }

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

        // Particles are per-identity, not "red dust for everyone".
        private void EmitStyleDust()
        {
            switch (style)
            {
                case SlashStyle.Clean:                  // dull metal sparks
                    if (Main.rand.NextBool(4))
                    {
                        Dust.NewDustPerfect(Projectile.Center, DustID.Silver,
                            -Projectile.velocity * 0.08f, 80, default, 0.8f);
                    }
                    break;

                case SlashStyle.Ripping:                // iron filings torn loose
                    if (Main.rand.NextBool(2))
                    {
                        Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f),
                            DustID.Iron, Main.rand.NextVector2Circular(1.5f, 1.5f), 60, default, 0.9f);
                    }
                    break;

                case SlashStyle.Lance:                  // speed lines, no spray
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Silver,
                        -Projectile.velocity * 0.22f, 120, default, 0.75f);
                    d.noGravity = true;
                    break;
                }

                case SlashStyle.Lunge:                  // green tracking motes
                    if (Main.rand.NextBool(3))
                    {
                        Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Grass,
                            -Projectile.velocity * 0.1f, 60, default, 1f);
                        d.noGravity = true;
                    }
                    break;

                case SlashStyle.Serpent:                // corrupt spores + lime sparks
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center,
                        Main.rand.NextBool(3) ? DustID.CursedTorch : DustID.PurpleTorch,
                        Main.rand.NextVector2Circular(1f, 1f), 60, default, 1.1f);
                    d.noGravity = true;
                    break;
                }

                case SlashStyle.Growing:                // dark dread mist
                    if (Main.rand.NextBool(2))
                    {
                        Dust d = Dust.NewDustPerfect(
                            Projectile.Center + Main.rand.NextVector2Circular(Projectile.width * 0.35f, Projectile.height * 0.35f),
                            DustID.Smoke, -Projectile.velocity * 0.05f, 90, default, 1.4f);
                        d.noGravity = true;
                    }
                    break;

                case SlashStyle.Seeding:                // leaf litter
                    if (Main.rand.NextBool(3))
                    {
                        Dust.NewDustPerfect(Projectile.Center, DustID.Grass,
                            Main.rand.NextVector2Circular(1.2f, 1.2f), 60, default, 1f);
                    }
                    break;

                case SlashStyle.Quick:                  // bone chips
                    if (Main.rand.NextBool(3))
                    {
                        Dust.NewDustPerfect(Projectile.Center, DustID.Bone,
                            Main.rand.NextVector2Circular(1.4f, 1.4f), 0, default, 0.9f);
                    }
                    break;

                case SlashStyle.Braking:                // heavy metal shards
                    if (Main.rand.NextBool(2))
                    {
                        Dust.NewDustPerfect(Projectile.Center, DustID.Stone,
                            Main.rand.NextVector2Circular(1.6f, 1.6f), 40, default, 1.1f);
                    }
                    break;

                case SlashStyle.Ember:                  // rising embers
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(6f, 6f),
                        DustID.Torch, new Vector2(0f, -Main.rand.NextFloat(0.4f, 1.4f)), 0, default, 1.2f);
                    d.noGravity = true;
                    break;
                }

                case SlashStyle.Echo:                   // falling blood droplets
                    if (Main.rand.NextBool(2))
                    {
                        Dust.NewDustPerfect(Projectile.Center, DustID.Blood,
                            new Vector2(Main.rand.NextFloat(-0.6f, 0.6f), 0.8f), 30, slashColor, 1f);
                    }
                    break;

                case SlashStyle.Cross:                  // liquid-metal threads
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Silver,
                        -Projectile.velocity * 0.14f, 100, default, 0.9f);
                    d.noGravity = true;
                    break;
                }

                case SlashStyle.Wide:                   // thick spray
                    Dust.NewDustPerfect(
                        Projectile.Center + Main.rand.NextVector2Circular(10f, 10f),
                        DustID.Blood, Main.rand.NextVector2Circular(2f, 2f), 0, default, 1.3f);
                    break;

                case SlashStyle.Quake:                  // kicked-up earth
                    if (Main.rand.NextBool(2))
                    {
                        Dust.NewDustPerfect(
                            Projectile.Center + Main.rand.NextVector2Circular(12f, 12f),
                            Main.rand.NextBool() ? DustID.Dirt : DustID.Stone,
                            Main.rand.NextVector2Circular(1.6f, 1.6f), 30, default, 1.3f);
                    }
                    break;

                case SlashStyle.Shatter:                // gathering light
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GoldCoin,
                        Main.rand.NextVector2Circular(0.8f, 0.8f), 80, default, 1f);
                    d.noGravity = true;
                    break;
                }

                case SlashStyle.Return:                 // sinking void motes
                    if (Main.rand.NextBool(2))
                    {
                        Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Shadowflame,
                            -Projectile.velocity * 0.05f, 120, default, 0.9f);
                        d.noGravity = true;
                    }
                    break;

                case SlashStyle.Homing:                 // living spores over blood
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center,
                        Main.rand.NextBool(3) ? DustID.Blood : DustID.Grass,
                        -Projectile.velocity * 0.08f, 60, default, 1f);
                    d.noGravity = true;
                    break;
                }

                case SlashStyle.Mark:                   // crimson sigil motes
                    if (Main.rand.NextBool(2))
                    {
                        Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Blood,
                            new Vector2(0f, -0.9f), 60, slashColor, 1.1f);
                        d.noGravity = true;
                    }
                    break;

                case SlashStyle.Phantom:                // afterimages trailing behind
                    if (Main.rand.NextBool(2))
                    {
                        Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Blood,
                            -Projectile.velocity * 0.3f, 150, slashColor, 1.3f);
                        d.noGravity = true;
                    }
                    break;

                default:
                    if (Main.rand.NextBool(2))
                    {
                        Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Blood,
                            -Projectile.velocity * 0.1f, 40, slashColor, 1f);
                        d.noGravity = true;
                    }
                    break;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // The hit spray matches the identity too, rather than blood for everything.
            int dust = DustID.Blood;
            int count = 6;

            switch (style)
            {
                case SlashStyle.Ripping: dust = DustID.Iron; count = 8; break;
                case SlashStyle.Lance: dust = DustID.Silver; count = 4; break;
                case SlashStyle.Quick: dust = DustID.Bone; count = 5; break;
                case SlashStyle.Braking: dust = DustID.Stone; count = 7; break;
                case SlashStyle.Ember: dust = DustID.Torch; count = 7; break;
                case SlashStyle.Serpent: dust = DustID.CursedTorch; count = 6; break;
                case SlashStyle.Shatter: dust = DustID.GoldCoin; count = 6; break;
                case SlashStyle.Quake: dust = DustID.Dirt; count = 9; break;
                case SlashStyle.Seeding: dust = DustID.Grass; count = 5; break;
            }

            for (int i = 0; i < count; i++)
            {
                Dust.NewDustPerfect(
                    target.Center, dust,
                    Main.rand.NextVector2Circular(3f, 3f), 30, slashColor, 1.1f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            // Hallowed Bloodletter: bursts into radiant shards. Shards carry ai1 = 1 so they never
            // shatter again. Owner-only spawn so multiplayer stays in sync.
            if (style == SlashStyle.Shatter && !IsShard && !IsEcho &&
                Main.myPlayer == Projectile.owner)
            {
                Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.UnitX);

                for (int i = -2; i <= 2; i++)
                {
                    Vector2 v = dir.RotatedBy(MathHelper.ToRadians(22 * i)) * 8f;

                    Projectile.NewProjectile(
                        Projectile.GetSource_Death(),
                        Projectile.Center, v,
                        ModContent.ProjectileType<CrimsonSlash>(),
                        Math.Max(1, Projectile.damage * 2 / 5),
                        Projectile.knockBack * 0.4f,
                        Projectile.owner,
                        ai1: 1f);
                }

                for (int i = 0; i < 12; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GoldCoin,
                        Main.rand.NextVector2Circular(4f, 4f), 60, default, 1.2f);
                    d.noGravity = true;
                }
            }
        }

        // Shape follows behaviour: heavy cleaves use the fat crescent, the lance uses the thrust
        // lens, the boomerang spins. Colour follows the blade.
        public override bool PreDraw(ref Color lightColor)
        {
            string path = "ETERNIA/Content/Projectiles/Warrior/CrimsonSlash";
            float styleBase = 0.8f;
            float extraRot = 0f;

            switch (style)
            {
                case SlashStyle.Wide:
                case SlashStyle.Growing:
                    path += "_Heavy"; styleBase = 1.1f; break;
                case SlashStyle.Quake:
                    path += "_Heavy"; styleBase = 1.25f; break;
                case SlashStyle.Lance:
                    path += "_Pierce"; styleBase = 1.0f; break;
                case SlashStyle.Return:
                    path += "_Return"; styleBase = 0.9f;
                    extraRot = Projectile.timeLeft * -0.22f; // the boomerang spins
                    break;
                case SlashStyle.Cross:
                    extraRot = Projectile.timeLeft * -0.10f; // the fangs tumble
                    break;
            }

            Texture2D tex = ModContent.Request<Texture2D>(path).Value;
            Vector2 origin = new Vector2(tex.Width, tex.Height) / 2f;

            float fade = MathHelper.Clamp(Projectile.timeLeft / 10f, 0f, 1f);
            float pop = MathHelper.Clamp(age / 5f, 0.4f, 1f);
            float drawScale = styleBase * baseScale * pop;

            // The echo is a ghost of the real cut: dimmer and translucent.
            Color draw = slashColor * fade;
            if (IsEcho)
            {
                draw = slashColor * (fade * 0.45f);
            }

            Main.EntitySpriteDraw(
                tex,
                Projectile.Center - Main.screenPosition,
                null,
                draw,
                Projectile.rotation + extraRot,
                origin,
                drawScale,
                SpriteEffects.None);

            return false;
        }
    }
}
