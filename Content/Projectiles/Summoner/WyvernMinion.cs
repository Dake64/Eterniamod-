using Terraria;
using Terraria.ID;

namespace Eternia.Content.Projectiles.Summoner
{
    // The Beast Tamer's apex: a soaring wyvern. Blinding speed and deep, bleeding strikes.
    public class WyvernMinion : BeastMinion
    {
        public override float MoveSpeed => 13.5f;
        public override float Inertia => 7f;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 40;
            Projectile.height = 38;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Bleeding, 300);
        }
    }
}
