using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Projectiles
{
    public class LightningBoltProjectile : ModProjectile
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

            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Dust.NewDust(
                Projectile.position,
                Projectile.width,
                Projectile.height,
                DustID.YellowTorch);
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

            target.AddBuff(
                BuffID.Electrified,
                300);

            float chainRange = 250f;

            if (elementalist.LightningLevel >= 5)
            {
                chainRange = 350f;
            }

            foreach (NPC npc in Main.npc)
            {
                if (!npc.active ||
                    npc.friendly ||
                    npc.whoAmI == target.whoAmI)
                {
                    continue;
                }

                float distance =
                    Vector2.Distance(
                        target.Center,
                        npc.Center);

                if (distance <= chainRange)
                {
                    int dustAmount =
                        elementalist.LightningLevel >= 3
                        ? 25
                        : 10;

                    for (int i = 0; i < dustAmount; i++)
                    {
                        Dust.NewDust(
                            npc.position,
                            npc.width,
                            npc.height,
                            DustID.YellowTorch);
                    }

                    break;
                }
            }

            int chargeGain =
                elementalist.LightningLevel >= 2
                ? 10
                : 5;

            elementalist.GainCharge(chargeGain);

            elementalist.LightningAffinity++;

           
        }
    }
}