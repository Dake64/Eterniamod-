using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Projectiles
{
    public class CursedBoltProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;

            Projectile.friendly = true;

            Projectile.DamageType =
                DamageClass.Magic;

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
                DustID.Shadowflame
            );
        }

        public override void OnHitNPC(
            NPC target,
            NPC.HitInfo hit,
            int damageDone)
        {
            Player player =
                Main.player[Projectile.owner];

            var cursed =
                player.GetModPlayer<CursedMagePlayer>();

            cursed.GainEnergy(2);

            target.AddBuff(
                BuffID.ShadowFlame,
                120);
        }
    }
}