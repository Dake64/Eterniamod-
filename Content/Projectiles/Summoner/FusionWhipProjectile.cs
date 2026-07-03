using Microsoft.Xna.Framework;

namespace Eternia.Content.Projectiles.Summoner
{
    public class FusionWhipProjectile : BaseEterniaWhipProjectile
    {
        protected override int SegmentCount => 20;

        protected override float RangeMultiplier => 1.05f;

        protected override Color WhipColor => Color.LightSteelBlue;

        protected override int TagDamage => 3;

        protected override string RequiredSubclass => "Advanced Summoner";
    }
}
