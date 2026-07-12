using Microsoft.Xna.Framework;

using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Projectiles
{
    // One projectile for the whole Elemental Mage arsenal, parameterized by element in
    // ai[0] (0 Fire, 1 Ice, 2 Lightning, 3 Wind, 4 Earth). Every elemental staff fires
    // this; the affinity/cycle staves pass the player's ACTIVE element.
    //
    // The base element effect (burn/slow/electrify/pierce/burst) applies to ANY caster,
    // so pre-Hardmode these read as normal elemental weapons. In Hardmode the Elemental
    // Affinity globals (ElementalAffinityGlobalProjectile/Item) overlay the active
    // affinity on top, so the same arsenal plays completely differently per affinity.
    public class ElementalArsenalBolt : ModProjectile
    {
        public override string Texture =>
            "ETERNIA/Content/Projectiles/FireBoltProjectile";

        private int Element => (int)Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            // Wind pierces a little on its own (the Wind affinity adds more in Hardmode).
            if (Element == 3)
            {
                Projectile.penetrate = 2;
            }
        }

        public override void AI()
        {
            Dust.NewDust(
                Projectile.position,
                Projectile.width,
                Projectile.height,
                ElementDust(Element));

            Lighting.AddLight(
                Projectile.Center,
                ElementalistPlayer.ElementColor(Element).ToVector3() * 0.3f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return ElementalistPlayer.ElementColor(Element);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Base element effect -- applies to any caster (works pre-Hardmode).
            switch (Element)
            {
                case 0: // Fire
                    target.AddBuff(BuffID.OnFire, 240);
                    break;

                case 1: // Ice: chill + slow
                    target.AddBuff(BuffID.Chilled, 120);
                    target.AddBuff(BuffID.Frostburn, 120);
                    break;

                case 2: // Lightning
                    target.AddBuff(BuffID.Electrified, 180);
                    break;

                case 3: // Wind: pure pierce, no debuff
                    break;

                case 4: // Earth: a little rock burst
                    for (int i = 0; i < 14; i++)
                    {
                        Dust.NewDust(target.position, target.width, target.height, DustID.Dirt);
                    }
                    break;
            }

            // Feed the per-element leveling / charge if the caster is an Elementalist.
            var ele = Main.player[Projectile.owner].GetModPlayer<ElementalistPlayer>();
            if (ele.IsActiveElementalist())
            {
                ele.GainCharge(5);
                ele.GainAffinity(Element);
            }
        }

        private static int ElementDust(int element)
        {
            return element switch
            {
                0 => DustID.Torch,
                1 => DustID.Ice,
                2 => DustID.YellowTorch,
                3 => DustID.Cloud,
                4 => DustID.Dirt,
                _ => DustID.Torch
            };
        }
    }
}
