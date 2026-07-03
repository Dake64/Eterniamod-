using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Projectiles
{
    public class IceBoltProjectile : ModProjectile
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
                DustID.Ice);
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

            int frostTime = 180;

            if (elementalist.IceLevel >= 3)
            {
                frostTime += 60;
            }

            target.AddBuff(
                BuffID.Frostburn,
                frostTime);

            float slowMultiplier = 0.5f;

            if (elementalist.IceLevel >= 2)
            {
                slowMultiplier = 0.3f;
            }

            target.velocity *= slowMultiplier;

            elementalist.GainCharge(5);
            elementalist.GainAffinity(1);

            
        }
    }
}
