using Terraria;
using Terraria.ID;

namespace Eternia.Content.Projectiles.Summoner
{
    // A soldier fused with a wisp and bound into matter. The last pre-Hardmode legionnaire --
    // slower, but its blows daze.
    public class ArcaneConstructMinion : LegionMinion
    {
        public override float MoveSpeed => 8.5f;
        public override float Inertia => 13f;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 30;
            Projectile.height = 30;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Slow, 120);
        }
    }
}
