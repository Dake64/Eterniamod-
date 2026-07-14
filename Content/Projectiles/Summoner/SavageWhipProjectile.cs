using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;

namespace Eternia.Content.Projectiles.Summoner
{
    // Post-Plantera Beast Tamer whip. Long reach, heavy tag, deep wounds.
    public class SavageWhipProjectile : BaseEterniaWhipProjectile
    {
        protected override int SegmentCount => 24;

        protected override float RangeMultiplier => 1.25f;

        protected override Color WhipColor => Color.DarkOrange;

        protected override int TagDamage => 20;

        protected override string RequiredSubclass => "Beast Tamer";

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            target.AddBuff(BuffID.Bleeding, 420);
            target.AddBuff(BuffID.Poisoned, 300);
        }
    }
}
