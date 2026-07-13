using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Projectiles
{
    // The Doom Orb: a homing orb that lingers, raising Corruption as it hits and dealing
    // more damage the more Corruption the wielder holds. Reuses the CursedBoltProjectile
    // texture.
    public class CurseOrb : ModProjectile
    {
        public override string Texture =>
            "ETERNIA/Content/Projectiles/CursedBoltProjectile";

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300; // ~5s
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            NPC target = FindTarget();

            if (target != null)
            {
                Vector2 dir = Vector2.Normalize(target.Center - Projectile.Center);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, dir * 9f, 0.07f);
            }

            Projectile.rotation += 0.2f;

            Dust.NewDust(
                Projectile.position,
                Projectile.width,
                Projectile.height,
                DustID.Shadowflame);

            Lighting.AddLight(Projectile.Center, new Vector3(0.4f, 0.1f, 0.5f));
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(180, 90, 220);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            int corruption = Main.player[Projectile.owner]
                .GetModPlayer<CursedMagePlayer>().TotalCorruption;

            // The orb hits harder the closer to the corruption limit you sit.
            modifiers.SourceDamage *= 1f + corruption * 0.004f; // +80% at 200
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.ShadowFlame, 120);

            var cursed = Main.player[Projectile.owner]
                .GetModPlayer<CursedMagePlayer>();

            // While it stays out attacking, it slowly raises Corruption.
            if (cursed.IsActiveCursedMage())
            {
                cursed.AddTemporaryCorruption(1);
            }
        }

        private NPC FindTarget()
        {
            NPC best = null;
            float bestDist = 700f;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (!npc.active || npc.friendly ||
                    npc.dontTakeDamage || !npc.CanBeChasedBy())
                {
                    continue;
                }

                float dist = Vector2.Distance(Projectile.Center, npc.Center);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = npc;
                }
            }

            return best;
        }
    }
}
