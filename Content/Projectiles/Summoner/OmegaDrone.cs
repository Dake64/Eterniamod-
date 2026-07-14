using Terraria;
using Terraria.ID;

namespace Eternia.Content.Projectiles.Summoner
{
    // The engineer's masterpiece. Relentless fire, long reach, piercing bolts -- a full fleet of
    // these is the Tech Summoner's endgame.
    public class OmegaDrone : DroneMinion
    {
        public override int ShotCooldown => 24;
        public override float ShotSpeed => 16f;
        public override float Range => 780f;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 34;
            Projectile.height = 34;
        }

        protected override void ConfigureShot(Projectile shot)
        {
            shot.penetrate = 2;
            shot.scale = 1.2f;

            if (shot.ModProjectile is DroneLaser laser)
            {
                laser.DebuffType = BuffID.Electrified;
                laser.DebuffTime = 180;
            }
        }
    }
}
