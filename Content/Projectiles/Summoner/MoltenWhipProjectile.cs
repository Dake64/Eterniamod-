using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;

namespace Eternia.Content.Projectiles.Summoner
{
    // Hellstone whip. The best pre-Hardmode lash -- sets tagged foes ablaze.
    public class MoltenWhipProjectile : BaseEterniaWhipProjectile
    {
        protected override int SegmentCount => 20;

        protected override float RangeMultiplier => 1.1f;

        protected override Color WhipColor => Color.OrangeRed;

        protected override int TagDamage => 9;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            target.AddBuff(BuffID.OnFire, 240);
        }
    }
}
