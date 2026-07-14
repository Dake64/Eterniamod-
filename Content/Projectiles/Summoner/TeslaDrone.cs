using Terraria;
using Terraria.ID;

namespace Eternia.Content.Projectiles.Summoner
{
    // The best pre-Hardmode build. Its bolts electrify what they hit.
    public class TeslaDrone : DroneMinion
    {
        public override int ShotCooldown => 38;
        public override float ShotSpeed => 12f;
        public override float Range => 640f;

        protected override void ConfigureShot(Projectile shot)
        {
            if (shot.ModProjectile is DroneLaser laser)
            {
                laser.DebuffType = BuffID.Electrified;
                laser.DebuffTime = 120;
            }
        }
    }
}
