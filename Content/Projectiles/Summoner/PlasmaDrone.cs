using Terraria;
using Terraria.ID;

namespace Eternia.Content.Projectiles.Summoner
{
    // First Hardmode build. Heavy plasma bolts that set targets ablaze.
    public class PlasmaDrone : DroneMinion
    {
        public override int ShotCooldown => 40;
        public override float ShotSpeed => 13f;
        public override float Range => 680f;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 30;
            Projectile.height = 30;
        }

        protected override void ConfigureShot(Projectile shot)
        {
            shot.scale = 1.2f;

            if (shot.ModProjectile is DroneLaser laser)
            {
                laser.DebuffType = BuffID.OnFire;
                laser.DebuffTime = 180;
            }
        }
    }
}
