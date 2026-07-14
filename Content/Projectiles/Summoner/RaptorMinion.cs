using Terraria;
using Terraria.ID;

namespace Eternia.Content.Projectiles.Summoner
{
    // Blistering speed. Its slashing bites make foes bleed.
    public class RaptorMinion : BeastMinion
    {
        public override float MoveSpeed => 12.5f;
        public override float Inertia => 8f;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Bleeding, 180);
        }
    }
}
