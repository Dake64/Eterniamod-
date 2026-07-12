using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items;
using Eternia.Content.Players;

namespace Eternia.Content.Projectiles.Guardian
{
    // The Defensive Aura every shield projects while its owner holds the shield up
    // (left-click channel). It is a controller/visual: it sticks to the owner, waits
    // ~0.5s to spin up, then pulses damage to every enemy inside a small radius and
    // applies the shield's personality effect. It vanishes the instant the shield is
    // lowered.
    //
    // DamageClass.Generic so ANY class can wield a shield. The Guardian (Escudero) is
    // simply the one who gets the most out of it: GuardianPlayer scales the aura's
    // damage with defense and its radius with Guardian passives.
    public class DefensiveAuraProjectile : ModProjectile
    {
        // Never actually drawn (the ring is dust); reuse an existing texture so the
        // content loader is happy.
        public override string Texture =>
            "ETERNIA/Content/Projectiles/Warrior/CrimsonSlash";

        private const int WarmupTicks = 30; // ~0.5s before the aura deals damage

        // ai[0] = lifetime tick counter (spin-up). localAI[0] = pulse timer.
        public override void SetDefaults()
        {
            Projectile.width = 88;
            Projectile.height = 88;
            Projectile.friendly = false;   // damage is dealt manually per pulse
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 6;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.aiStyle = -1;
        }

        // The aura never collides on its own; all damage is manual per pulse.
        public override bool? CanDamage() => false;

        // The aura is drawn as a dust ring (DrawRing); the reused placeholder texture
        // must NOT be painted, or it shows up as a stray white slash behind the player.
        public override bool PreDraw(ref Color lightColor) => false;

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            // If the owner stopped channelling a shield, the aura disappears at once.
            if (owner == null || !owner.active || owner.dead ||
                !owner.channel || owner.noItems || owner.CCed ||
                owner.HeldItem?.ModItem is not IShieldWeapon shield)
            {
                Projectile.Kill();
                return;
            }

            // Keep the shield "in use" so the channel does not drop between use cycles,
            // and stick the aura to the wielder.
            owner.heldProj = Projectile.whoAmI;
            owner.itemTime = 2;
            owner.itemAnimation = 2;

            var guardian = owner.GetModPlayer<GuardianPlayer>();
            float radius = shield.AuraRadius * guardian.AuraRadiusMultiplier();

            Projectile.width = Projectile.height = (int)(radius * 2f);
            Projectile.Center = owner.Center;
            Projectile.timeLeft = 6;
            Projectile.ai[0]++;

            bool warmedUp = Projectile.ai[0] >= WarmupTicks;

            DrawRing(shield.AuraColor, radius, warmedUp);

            if (!warmedUp)
            {
                return;
            }

            // Pulse on the shield's interval (the Guardian's Defense tree can speed it).
            int interval = Math.Max(
                6,
                (int)(shield.AuraPulseInterval * guardian.AuraPulseMultiplier()));

            Projectile.localAI[0]++;
            if (Projectile.localAI[0] < interval)
            {
                return;
            }
            Projectile.localAI[0] = 0f;

            Pulse(owner, shield, guardian, radius);
        }

        private void Pulse(
            Player owner,
            IShieldWeapon shield,
            GuardianPlayer guardian,
            float radius)
        {
            // Only the owner runs damage and effects (avoids double-hits/heals in MP).
            if (Projectile.owner != Main.myPlayer)
            {
                return;
            }

            // Per-pulse effects: the shield's own (Holy ally regen) + Guardian passives
            // (Last Bastion sustain).
            shield.OnAuraPulse(owner);
            guardian.ApplyGuardianAuraPulse(owner);

            int damage = Math.Max(1, (int)(Projectile.damage * guardian.AuraDamageMultiplier()));

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (!npc.active || npc.friendly ||
                    npc.dontTakeDamage || npc.immortal)
                {
                    continue;
                }

                // Circle test against the enemy's edge.
                if (Vector2.Distance(owner.Center, npc.Center)
                    > radius + npc.width * 0.5f)
                {
                    continue;
                }

                int hitDir = Math.Sign(npc.Center.X - owner.Center.X);
                if (hitDir == 0)
                {
                    hitDir = owner.direction;
                }

                npc.SimpleStrikeNPC(
                    damage,
                    hitDir,
                    false,
                    Projectile.knockBack,
                    DamageClass.Generic,
                    true,
                    0f,
                    false);

                shield.OnAuraHit(owner, npc);
            }
        }

        // A ring of tinted dust marking the aura's edge; brighter once it is armed.
        private void DrawRing(Color color, float radius, bool warmedUp)
        {
            int points = warmedUp ? 10 : 6;

            for (int i = 0; i < points; i++)
            {
                if (!Main.rand.NextBool(warmedUp ? 2 : 3))
                {
                    continue;
                }

                float angle = MathHelper.TwoPi * i / points
                    + Projectile.ai[0] * 0.05f;

                Vector2 pos = Projectile.Center
                    + angle.ToRotationVector2() * radius;

                Dust dust = Dust.NewDustPerfect(
                    pos,
                    DustID.GemDiamond,
                    Vector2.Zero,
                    120,
                    color,
                    warmedUp ? 1.1f : 0.8f);

                dust.noGravity = true;
                dust.noLight = false;
            }

            Lighting.AddLight(Projectile.Center, color.ToVector3() * 0.2f);
        }
    }
}
