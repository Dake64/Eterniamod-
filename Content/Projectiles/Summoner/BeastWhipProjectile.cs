using Microsoft.Xna.Framework;

namespace Eternia.Content.Projectiles.Summoner
{
    public class BeastWhipProjectile : BaseEterniaWhipProjectile
    {
        protected override int SegmentCount => 18;

        protected override float RangeMultiplier => 0.95f;

        protected override Color WhipColor => Color.SandyBrown;

        protected override int TagDamage => 12;

        protected override string RequiredSubclass => "Beast Tamer";
    }
}
