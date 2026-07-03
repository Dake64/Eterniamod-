using Microsoft.Xna.Framework;

namespace Eternia.Content.Projectiles.Summoner
{
    public class TrainingWhipProjectile : BaseEterniaWhipProjectile
    {
        protected override int SegmentCount => 14;

        protected override float RangeMultiplier => 0.82f;

        protected override Color WhipColor => Color.MediumPurple;

        protected override int TagDamage => 1;
    }
}
