using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Projectiles
{
    public class FireBoltProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;

            Projectile.friendly = true;
            Projectile.hostile = false;

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
                DustID.Torch);
        }

        public override void OnHitNPC(
            NPC target,
            NPC.HitInfo hit,
            int damageDone)
        {
            Player player =
                Main.player[Projectile.owner];

            var elementalist =
                player.GetModPlayer<ElementalistPlayer>();

            if (!elementalist.IsActiveElementalist())
            {
                return;
            }

            int burnTime = 180;

            if (elementalist.FireLevel >= 2)
            {
                burnTime += 60;
            }

            target.AddBuff(
                BuffID.OnFire,
                burnTime);

            int dustAmount =
                elementalist.FireLevel >= 3
                    ? 30
                    : 15;

            for (int i = 0; i < dustAmount; i++)
            {
                Dust.NewDust(
                    target.position,
                    target.width,
                    target.height,
                    DustID.Torch);
            }

            elementalist.GainCharge(5);
            elementalist.GainAffinity(0);

            
        }
    }
}
