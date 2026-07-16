using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Bosses;
using Eternia.Content.Items.Materials;
using Eternia.Content.Items.Weapons.Boss;
using Eternia.Content.Projectiles.Bosses;

namespace Eternia.Content.NPCs.Bosses
{
    // PROTOTYPE-01. The first Eternia boss: an ancient civilisation's failed attempt at a vessel
    // for an artificial Soul. It never spoke; it only wants to finish a mission it no longer
    // remembers. Fast, aggressive, and built around swapping weapon modules on the fly.
    //
    // Design goals (from the brief): ULTRAKILL / NieR:Automata / MGR energy -- constant weapon
    // changes, dashes, unrelenting pressure, three phases that escalate as the core is exposed.
    //
    // NOTE: placeholder art. The body borrows the vanilla Golem sprite, tinted, with a glowing
    // Soul core drawn over the chest that grows brighter as the machine breaks apart. This is a
    // stand-in until real art exists -- the whole mod is in this state.
    public class Prototype01 : ModNPC
    {
        // --- AI state (server-authoritative; clients derive visuals from life) ---------------
        private enum Action
        {
            Intro,
            Reposition,
            SwordDash,
            PlasmaVolley,
            LanceCharge,
            DroneSpawn,
            CoreVent
        }

        private Action action = Action.Intro;
        private int actionTimer;
        private int step;
        private int reps;
        private int phase = 1;
        private int invulnTimer;
        private float coreGlow;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }

        public override string BossHeadTexture =>
            "ETERNIA/Content/Items/Souls/EmptySoul";

        // Use a valid existing texture so the NPC autoloads; PreDraw replaces it anyway.
        public override string Texture =>
            "ETERNIA/Content/Items/Souls/EmptySoul";

        public override void SetDefaults()
        {
            NPC.width = 92;
            NPC.height = 112;
            NPC.damage = 42;
            NPC.defense = 20;
            NPC.lifeMax = 6000;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.value = Item.buyPrice(gold: 8);
            NPC.boss = true;
            NPC.npcSlots = 10f;
            NPC.aiStyle = -1;
            NPC.lavaImmune = true;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PrototypeCore>(), 1, 8, 15));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SoulAlloy>(), 1, 2, 4));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SoulforgedSabre>()));
        }

        // ==============================================================================
        // AI
        // ==============================================================================

        public override void AI()
        {
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];

            if (!player.active || player.dead)
            {
                Despawn();
                return;
            }

            // Face the player and keep a slight banking tilt.
            NPC.spriteDirection = player.Center.X > NPC.Center.X ? 1 : -1;
            NPC.rotation = MathHelper.Clamp(NPC.velocity.X * 0.02f, -0.25f, 0.25f);

            coreGlow = 0.3f + (1f - LifeFraction) * 0.7f;
            Lighting.AddLight(NPC.Center, 0.3f * coreGlow, 0.6f * coreGlow, 0.9f * coreGlow);
            AmbientDust();

            UpdatePhase(player);

            if (invulnTimer > 0)
            {
                invulnTimer--;
                NPC.dontTakeDamage = true;
                NPC.velocity *= 0.9f;
                actionTimer++;
                return;
            }

            NPC.dontTakeDamage = false;

            // The core is more exposed as it breaks: less defense, more contact damage.
            NPC.defense = phase == 1 ? 20 : phase == 2 ? 14 : 6;
            NPC.damage = phase == 3 ? 64 : 42;

            switch (action)
            {
                case Action.Intro: DoIntro(player); break;
                case Action.Reposition: DoReposition(player); break;
                case Action.SwordDash: DoSwordDash(player); break;
                case Action.PlasmaVolley: DoPlasmaVolley(player); break;
                case Action.LanceCharge: DoLanceCharge(player); break;
                case Action.DroneSpawn: DoDroneSpawn(player); break;
                case Action.CoreVent: DoCoreVent(player); break;
            }

            actionTimer++;
        }

        private float LifeFraction =>
            NPC.lifeMax <= 0 ? 0f : MathHelper.Clamp(NPC.life / (float)NPC.lifeMax, 0f, 1f);

        private void UpdatePhase(Player player)
        {
            int newPhase =
                LifeFraction > 0.70f ? 1 :
                LifeFraction > 0.35f ? 2 : 3;

            if (newPhase > phase)
            {
                phase = newPhase;
                OnPhaseChange(player);
            }
        }

        private void OnPhaseChange(Player player)
        {
            invulnTimer = 60;
            action = Action.Reposition;
            actionTimer = 0;
            step = 0;

            SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
            SoundEngine.PlaySound(SoundID.Item122, NPC.Center);

            if (!Main.dedServ)
            {
                Main.instance.CameraModifiers.Add(
                    new PunchCameraModifier(
                        NPC.Center,
                        Main.rand.NextVector2Unit(),
                        14f, 6f, 22, 1600f, "Prototype01Phase"));
            }

            for (int i = 0; i < 80; i++)
            {
                Dust d = Dust.NewDustPerfect(
                    NPC.Center,
                    DustID.BlueTorch,
                    Main.rand.NextVector2Circular(10f, 10f),
                    100, default, 2f);

                d.noGravity = true;
            }

            // Phase 3 announces itself with a vent.
            if (phase == 3 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(
                    NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero,
                    ModContent.ProjectileType<PrototypeShockwave>(),
                    ScaledDamage(28), 0f, Main.myPlayer);
            }
        }

        // --- Actions ------------------------------------------------------------------------

        private void DoIntro(Player player)
        {
            MoveToward(player.Center + new Vector2(0f, -240f), 8f, 0.25f);

            if (actionTimer == 6)
            {
                SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
            }

            if (actionTimer > 70)
            {
                BeginReposition();
            }
        }

        private void DoReposition(Player player)
        {
            // Hover to a point beside/above the player, then pick the next module.
            float side = NPC.Center.X < player.Center.X ? 1f : -1f;
            Vector2 anchor = player.Center + new Vector2(side * 300f, -180f);

            MoveToward(anchor, phase == 3 ? 13f : phase == 2 ? 11f : 9f, 0.3f);

            int hover = phase == 3 ? 26 : phase == 2 ? 36 : 48;

            if (actionTimer > hover)
            {
                ChooseAttack(player);
            }
        }

        private void ChooseAttack(Player player)
        {
            // Weighted by phase; never repeat the same module back to back.
            System.Collections.Generic.List<Action> pool =
                new System.Collections.Generic.List<Action>
                {
                    Action.SwordDash,
                    Action.PlasmaVolley,
                    Action.LanceCharge
                };

            if (phase >= 2)
            {
                pool.Add(Action.DroneSpawn);
                pool.Add(Action.SwordDash); // bias toward aggression
            }

            if (phase >= 3)
            {
                pool.Add(Action.CoreVent);
                pool.Add(Action.LanceCharge);
            }

            Action next;

            do
            {
                next = pool[Main.rand.Next(pool.Count)];
            }
            while (next == action && pool.Count > 1);

            action = next;
            actionTimer = 0;
            step = 0;
            reps = 0;
        }

        private void DoSwordDash(Player player)
        {
            int dashCount = phase == 3 ? 4 : phase == 2 ? 3 : 2;

            switch (step)
            {
                case 0: // telegraph
                    MoveToward(player.Center, 6f, 0.25f);
                    NPC.velocity *= 0.85f;

                    if (actionTimer > (phase == 3 ? 10 : 16))
                    {
                        Vector2 dir =
                            (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX);

                        NPC.velocity = dir * (phase == 3 ? 26f : 21f);
                        SoundEngine.PlaySound(SoundID.Item71, NPC.Center);
                        StepTo(1);
                    }

                    break;

                case 1: // dash
                    if (actionTimer % 5 == 0 &&
                        Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(
                            NPC.GetSource_FromAI(), NPC.Center, NPC.velocity * 0.25f,
                            ModContent.ProjectileType<PrototypeEnergySlash>(),
                            ScaledDamage(22), 4f, Main.myPlayer);
                    }

                    if (actionTimer > 16)
                    {
                        StepTo(2);
                    }

                    break;

                case 2: // recover
                    NPC.velocity *= 0.9f;

                    if (actionTimer > 10)
                    {
                        reps++;

                        if (reps >= dashCount)
                        {
                            BeginReposition();
                        }
                        else
                        {
                            StepTo(0);
                        }
                    }

                    break;
            }
        }

        private void DoPlasmaVolley(Player player)
        {
            int volleys = phase == 3 ? 4 : 3;
            int shots = phase == 3 ? 7 : phase == 2 ? 5 : 3;
            int cadence = phase == 3 ? 16 : 22;

            // Strafe slowly to stay a moving target while firing.
            float side = NPC.Center.X < player.Center.X ? 1f : -1f;
            MoveToward(player.Center + new Vector2(side * 320f, -120f), 7f, 0.2f);

            if (actionTimer % cadence == cadence - 1)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 baseDir =
                        (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);

                    float spread = MathHelper.ToRadians(shots * 4f);

                    for (int i = 0; i < shots; i++)
                    {
                        float lerp = shots == 1 ? 0.5f : i / (float)(shots - 1);
                        float angle = MathHelper.Lerp(-spread, spread, lerp);

                        Vector2 vel = baseDir.RotatedBy(angle) * (phase == 3 ? 9f : 7.5f);

                        Projectile.NewProjectile(
                            NPC.GetSource_FromAI(), NPC.Center, vel,
                            ModContent.ProjectileType<PrototypePlasmaBolt>(),
                            ScaledDamage(20), 2f, Main.myPlayer);
                    }
                }

                SoundEngine.PlaySound(SoundID.Item33, NPC.Center);
                step++;

                if (step >= volleys)
                {
                    BeginReposition();
                }
            }
        }

        private void DoLanceCharge(Player player)
        {
            switch (step)
            {
                case 0: // line up on the player's row
                    Vector2 lineUp =
                        new Vector2(
                            NPC.Center.X < player.Center.X ? player.Center.X - 480f : player.Center.X + 480f,
                            player.Center.Y);

                    MoveToward(lineUp, 15f, 0.5f);

                    if (actionTimer > (phase == 3 ? 22 : 30) ||
                        System.Math.Abs(NPC.Center.Y - player.Center.Y) < 30f && actionTimer > 14)
                    {
                        float dir = player.Center.X > NPC.Center.X ? 1f : -1f;
                        NPC.velocity = new Vector2(dir * (phase == 3 ? 33f : 27f), 0f);
                        SoundEngine.PlaySound(SoundID.Item122, NPC.Center);
                        StepTo(1);
                    }

                    break;

                case 1: // the charge -- shed plasma along the lance's path
                    if (actionTimer % 4 == 0 &&
                        Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int s = -1; s <= 1; s += 2)
                        {
                            Projectile.NewProjectile(
                                NPC.GetSource_FromAI(), NPC.Center,
                                new Vector2(0f, s * 5f),
                                ModContent.ProjectileType<PrototypePlasmaBolt>(),
                                ScaledDamage(18), 1f, Main.myPlayer);
                        }
                    }

                    if (actionTimer > 34)
                    {
                        BeginReposition();
                    }

                    break;
            }
        }

        private void DoDroneSpawn(Player player)
        {
            MoveToward(player.Center + new Vector2(0f, -260f), 8f, 0.25f);

            int droneCount = phase == 3 ? 5 : 3;

            if (actionTimer % 12 == 11 && step < droneCount)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 vel = Main.rand.NextVector2Circular(4f, 4f);

                    Projectile.NewProjectile(
                        NPC.GetSource_FromAI(),
                        NPC.Center + Main.rand.NextVector2Circular(40f, 40f),
                        vel,
                        ModContent.ProjectileType<PrototypeDrone>(),
                        ScaledDamage(24), 2f, Main.myPlayer);
                }

                SoundEngine.PlaySound(SoundID.Item92, NPC.Center);
                step++;
            }

            if (step >= droneCount && actionTimer % 12 == 0)
            {
                BeginReposition();
            }
        }

        private void DoCoreVent(Player player)
        {
            // The desperate phase-3 explosion: overlapping shockwaves + a radial plasma burst.
            NPC.velocity *= 0.9f;

            if (actionTimer == 20 || actionTimer == 44)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(
                        NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero,
                        ModContent.ProjectileType<PrototypeShockwave>(),
                        ScaledDamage(30), 0f, Main.myPlayer);
                }

                SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
            }

            if (actionTimer == 32 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                const int spokes = 14;

                for (int i = 0; i < spokes; i++)
                {
                    Vector2 vel =
                        (MathHelper.TwoPi * i / spokes).ToRotationVector2() * 7f;

                    Projectile.NewProjectile(
                        NPC.GetSource_FromAI(), NPC.Center, vel,
                        ModContent.ProjectileType<PrototypePlasmaBolt>(),
                        ScaledDamage(22), 2f, Main.myPlayer);
                }
            }

            if (actionTimer > 64)
            {
                BeginReposition();
            }
        }

        // --- Helpers ------------------------------------------------------------------------

        private void BeginReposition()
        {
            action = Action.Reposition;
            actionTimer = 0;
            step = 0;
            reps = 0;
        }

        private void StepTo(int next)
        {
            step = next;
            actionTimer = 0;
        }

        private void MoveToward(Vector2 target, float maxSpeed, float accel)
        {
            Vector2 delta = target - NPC.Center;

            if (delta.Length() > 12f)
            {
                Vector2 dir = delta.SafeNormalize(Vector2.Zero);
                NPC.velocity += dir * accel;

                if (NPC.velocity.Length() > maxSpeed)
                {
                    NPC.velocity = NPC.velocity.SafeNormalize(Vector2.Zero) * maxSpeed;
                }
            }
            else
            {
                NPC.velocity *= 0.88f;
            }
        }

        // Scales an attack's base damage with the boss's rolled rarity / master mode feel by
        // leaning on the NPC's own damage curve. Kept simple: expert/master bump via GameModeData.
        private int ScaledDamage(int baseDamage)
        {
            float mult = Main.masterMode ? 1.5f : Main.expertMode ? 1.2f : 1f;
            return (int)(baseDamage * mult);
        }

        private void AmbientDust()
        {
            if (!Main.rand.NextBool(3))
            {
                return;
            }

            Dust d = Dust.NewDustPerfect(
                NPC.Center + Main.rand.NextVector2Circular(NPC.width * 0.4f, NPC.height * 0.4f),
                DustID.BlueTorch,
                new Vector2(0f, -Main.rand.NextFloat(0.5f, 1.5f)),
                150,
                default,
                0.8f + coreGlow);

            d.noGravity = true;
        }

        private void Despawn()
        {
            NPC.velocity.Y -= 0.3f;
            NPC.velocity.X *= 0.98f;
            NPC.EncourageDespawn(10);

            if (NPC.timeLeft > 20)
            {
                NPC.timeLeft = 20;
            }
        }

        // ==============================================================================
        // Draw -- placeholder mechanical body + a Soul core that opens up as it breaks
        // ==============================================================================

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Main.instance.LoadNPC(NPCID.Golem);

            Texture2D body = TextureAssets.Npc[NPCID.Golem].Value;

            int frames = System.Math.Max(1, Main.npcFrameCount[NPCID.Golem]);
            int frameHeight = body.Height / frames;
            Rectangle src = new Rectangle(0, 0, body.Width, frameHeight);

            float scale = NPC.width / (float)body.Width * 1.6f;

            Vector2 origin = new Vector2(body.Width / 2f, frameHeight / 2f);

            Vector2 pos = NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY);

            // Phase-3 jitter: the machine is falling apart.
            if (phase == 3)
            {
                pos += Main.rand.NextVector2Circular(2.2f, 2.2f);
            }

            SpriteEffects fx =
                NPC.spriteDirection == 1
                    ? SpriteEffects.FlipHorizontally
                    : SpriteEffects.None;

            // Tint: cold steel that reddens and darkens as it breaks.
            Color tint = Color.Lerp(new Color(150, 190, 220), new Color(120, 60, 70), 1f - LifeFraction);

            spriteBatch.Draw(body, pos, src, tint * (NPC.Opacity <= 0 ? 1f : NPC.Opacity), NPC.rotation, origin, scale, fx, 0f);

            DrawCore(spriteBatch, pos);

            return false;
        }

        private void DrawCore(SpriteBatch spriteBatch, Vector2 pos)
        {
            Texture2D glow = TextureAssets.Npc[Type].Value; // the soul icon placeholder

            float pulse =
                0.7f + 0.3f * (float)System.Math.Sin(Main.GlobalTimeWrappedHourly * 6f);

            float intensity = coreGlow * pulse;

            Vector2 coreOrigin = new Vector2(glow.Width / 2f, glow.Height / 2f);

            // The core sits at the chest and blooms outward as it is exposed.
            Vector2 corePos = pos + new Vector2(0f, -6f);

            for (int i = 0; i < 6; i++)
            {
                float angle = MathHelper.TwoPi * i / 6f + Main.GlobalTimeWrappedHourly;
                Vector2 offset = angle.ToRotationVector2() * (4f + intensity * 8f);

                spriteBatch.Draw(
                    glow, corePos + offset, null,
                    new Color(120, 220, 255) * (0.35f * intensity),
                    0f, coreOrigin, 1.1f + intensity, SpriteEffects.None, 0f);
            }

            spriteBatch.Draw(
                glow, corePos, null,
                Color.White * (0.6f + 0.4f * intensity),
                0f, coreOrigin, 0.8f + intensity * 0.6f, SpriteEffects.None, 0f);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 6; i++)
            {
                Dust d = Dust.NewDustDirect(
                    NPC.position, NPC.width, NPC.height,
                    Main.rand.NextBool() ? DustID.BlueTorch : DustID.Iron,
                    hit.HitDirection, -1f, 100, default, 1.2f);

                d.noGravity = Main.rand.NextBool();
            }

            if (NPC.life <= 0 && !Main.dedServ)
            {
                for (int i = 0; i < 60; i++)
                {
                    Dust d = Dust.NewDustPerfect(
                        NPC.Center,
                        Main.rand.NextBool() ? DustID.BlueTorch : DustID.Smoke,
                        Main.rand.NextVector2Circular(8f, 8f),
                        100, default, 2f);

                    d.noGravity = Main.rand.NextBool();
                }
            }
        }
    }
}
