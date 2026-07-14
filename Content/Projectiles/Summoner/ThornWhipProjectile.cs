using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;

namespace Eternia.Content.Projectiles.Summoner
{
    // Jungle whip. Its barbs poison whatever they tag.
    public class ThornWhipProjectile : BaseEterniaWhipProjectile
    {
        protected override int SegmentCount => 18;

        protected override float RangeMultiplier => 1f;

        protected override Color WhipColor => Color.OliveDrab;

        protected override int TagDamage => 5;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            target.AddBuff(BuffID.Poisoned, 300);
        }
    }
}
