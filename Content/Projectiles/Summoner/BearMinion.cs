using Terraria;
using Terraria.ID;

namespace Eternia.Content.Projectiles.Summoner
{
    // A hulking bruiser. Hits like a truck and shrugs off the front line.
    public class BearMinion : BeastMinion
    {
        public override float MoveSpeed => 8.5f;
        public override float Inertia => 13f;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 46;
            Projectile.height = 42;
            Projectile.knockBack = 5f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Slow, 120);
        }
    }
}
