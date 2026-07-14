using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;

namespace Eternia.Content.Projectiles.Summoner
{
    // Post-mechs Beast Tamer whip. A savage lash that leaves foes bleeding for the pack.
    public class FeralWhipProjectile : BaseEterniaWhipProjectile
    {
        protected override int SegmentCount => 22;

        protected override float RangeMultiplier => 1.2f;

        protected override Color WhipColor => Color.Orange;

        protected override int TagDamage => 16;

        protected override string RequiredSubclass => "Beast Tamer";

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            target.AddBuff(BuffID.Bleeding, 360);
        }
    }
}
