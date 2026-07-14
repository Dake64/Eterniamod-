using Terraria;
using Terraria.ID;

namespace Eternia.Content.Projectiles.Summoner
{
    // Golem fused with construct, refined into arc energy. Blistering speed; strikes electrify.
    public class ArcSentinelMinion : LegionMinion
    {
        public override float MoveSpeed => 13f;
        public override float Inertia => 8f;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Electrified, 120);
        }
    }
}
