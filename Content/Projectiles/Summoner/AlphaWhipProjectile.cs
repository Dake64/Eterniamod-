using Microsoft.Xna.Framework;

namespace Eternia.Content.Projectiles.Summoner
{
    // The Beast Tamer's endgame whip: long reach and a heavy summon tag.
    public class AlphaWhipProjectile : BaseEterniaWhipProjectile
    {
        protected override int SegmentCount => 24;

        protected override float RangeMultiplier => 1.35f;

        protected override Color WhipColor => new Color(255, 140, 40);

        protected override int TagDamage => 26;

        protected override string RequiredSubclass => "Beast Tamer";
    }
}
