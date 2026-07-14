using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;

namespace Eternia.Content.Projectiles.Summoner
{
    // The Tech Summoner's endgame whip. Paints a target for the whole fleet.
    public class OmegaWhipProjectile : BaseEterniaWhipProjectile
    {
        protected override int SegmentCount => 24;

        protected override float RangeMultiplier => 1.3f;

        protected override Color WhipColor => new Color(120, 230, 255);

        protected override int TagDamage => 24;

        protected override string RequiredSubclass => "Tech Summoner";

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            target.AddBuff(BuffID.Electrified, 240);
        }
    }
}
