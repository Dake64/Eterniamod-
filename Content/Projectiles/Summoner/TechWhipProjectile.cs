using Microsoft.Xna.Framework;

namespace Eternia.Content.Projectiles.Summoner
{
    public class TechWhipProjectile : BaseEterniaWhipProjectile
    {
        protected override int SegmentCount => 22;

        protected override float RangeMultiplier => 1.15f;

        protected override Color WhipColor => Color.Cyan;

        protected override int TagDamage => 5;

        protected override string RequiredSubclass => "Tech Summoner";
    }
}
