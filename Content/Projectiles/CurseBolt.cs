using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Projectiles
{
    // Shared curse-weapon bolt. ai[0] = Cursed Energy refunded on hit; ai[1] = 1 for an
    // AoE burst on hit (returns a little extra energy per foe caught). Pierce is set by
    // the firing weapon. Reuses the CursedBoltProjectile texture.
    public class CurseBolt : ModProjectile
    {
        public override string Texture =>
            "ETERNIA/Content/Projectiles/CursedBoltProjectile";

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Dust.NewDust(
                Projectile.position,
                Projectile.width,
                Projectile.height,
                DustID.Shadowflame);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(170, 90, 210);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.ShadowFlame, 120);

            var cursed = Main.player[Projectile.owner]
                .GetModPlayer<CursedMagePlayer>();

            int refund = (int)Projectile.ai[0];

            if (refund > 0 && cursed.IsActiveMage())
            {
                cursed.GainEnergy(refund);
            }

            // AoE burst: reward hitting groups with damage + a trickle of energy back.
            if (Projectile.ai[1] == 1f && Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];

                    if (!npc.active || npc.friendly ||
                        npc.dontTakeDamage || npc.whoAmI == target.whoAmI)
                    {
                        continue;
                    }

                    if (Vector2.Distance(target.Center, npc.Center) > 96f)
                    {
                        continue;
                    }

                    npc.SimpleStrikeNPC(
                        System.Math.Max(1, damageDone / 2),
                        0, false, 0f, DamageClass.Magic);

                    if (cursed.IsActiveMage())
                    {
                        cursed.GainEnergy(1);
                    }
                }
            }
        }
    }
}
