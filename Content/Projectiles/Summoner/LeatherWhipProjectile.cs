using Microsoft.Xna.Framework;

namespace Eternia.Content.Projectiles.Summoner
{
    // Early pre-Hardmode whip. Open to any Summoner (the Beast Tamer does not exist yet).
    public class LeatherWhipProjectile : BaseEterniaWhipProjectile
    {
        protected override int SegmentCount => 16;

        protected override float RangeMultiplier => 0.9f;

        protected override Color WhipColor => Color.SaddleBrown;

        protected override int TagDamage => 3;
    }
}
