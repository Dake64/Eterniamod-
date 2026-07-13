using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Projectiles.Ranger
{
    // Plasma bolt: detonates on impact, damaging everything nearby. For plasma/nova/fusion
    // weapons. Ranged so the temperature bonuses and Plasma Conductors global apply.
    public class EnergyPlasmaBolt : ModProjectile
    {
        private const float BlastRadius = 96f;

        public override string Texture =>
            "ETERNIA/Content/Projectiles/LightningBoltProjectile";

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            if (Projectile.velocity != Vector2.Zero)
            {
                Projectile.rotation =
                    Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }

            Lighting.AddLight(Projectile.Center, 0.4f, 0.5f, 0.2f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 170, 90, 160);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner != Main.myPlayer)
            {
                return;
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (!npc.active || npc.friendly ||
                    npc.dontTakeDamage || npc.whoAmI == target.whoAmI)
                {
                    continue;
                }

                if (Vector2.Distance(target.Center, npc.Center) > BlastRadius)
                {
                    continue;
                }

                npc.SimpleStrikeNPC(
                    System.Math.Max(1, damageDone / 2),
                    0, false, 0f, DamageClass.Ranged);
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 18; i++)
            {
                Dust d = Dust.NewDustDirect(
                    Projectile.position, Projectile.width, Projectile.height,
                    DustID.Torch, 0f, 0f, 100, default, 1.3f);

                d.noGravity = true;
                d.velocity *= 2.2f;
            }
        }
    }
}
