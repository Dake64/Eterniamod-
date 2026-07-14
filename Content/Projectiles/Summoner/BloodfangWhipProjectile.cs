using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;

namespace Eternia.Content.Projectiles.Summoner
{
    // Evil-biome whip. Tears wounds open -- tagged foes bleed.
    public class BloodfangWhipProjectile : BaseEterniaWhipProjectile
    {
        protected override int SegmentCount => 20;

        protected override float RangeMultiplier => 1.05f;

        protected override Color WhipColor => Color.Crimson;

        protected override int TagDamage => 7;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            target.AddBuff(BuffID.Bleeding, 300);
        }
    }
}
