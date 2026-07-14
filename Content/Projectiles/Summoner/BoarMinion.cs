using Terraria;
using Terraria.ID;

namespace Eternia.Content.Projectiles.Summoner
{
    // A heavy tusker. Slower, bulkier, and its charges knock foes back hard.
    public class BoarMinion : BeastMinion
    {
        public override float MoveSpeed => 8f;
        public override float Inertia => 14f;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 42;
            Projectile.height = 34;
            Projectile.knockBack = 6f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Slow, 120);
        }
    }
}
