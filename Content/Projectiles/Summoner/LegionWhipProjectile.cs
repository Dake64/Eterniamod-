using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;

namespace Eternia.Content.Projectiles.Summoner
{
    // The Advanced Summoner's endgame whip. Mark one foe and the entire legion erases it.
    public class LegionWhipProjectile : BaseEterniaWhipProjectile
    {
        protected override int SegmentCount => 24;

        protected override float RangeMultiplier => 1.3f;

        protected override Color WhipColor => new Color(190, 140, 255);

        protected override int TagDamage => 24;

        protected override string RequiredSubclass => "Advanced Summoner";

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            target.AddBuff(BuffID.ShadowFlame, 240);
        }
    }
}
