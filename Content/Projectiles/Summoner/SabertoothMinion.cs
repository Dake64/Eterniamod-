using Terraria;
using Terraria.ID;

namespace Eternia.Content.Projectiles.Summoner
{
    // An apex predator. Fast, savage bites that bleed foes and feed a little life back to the
    // Tamer.
    public class SabertoothMinion : BeastMinion
    {
        public override float MoveSpeed => 11.5f;
        public override float Inertia => 9f;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Bleeding, 240);

            Player owner = Main.player[Projectile.owner];

            if (owner.whoAmI == Main.myPlayer &&
                owner.statLife < owner.statLifeMax2 &&
                Main.rand.NextBool(4))
            {
                owner.statLife += 2;
                owner.HealEffect(2);
            }
        }
    }
}
