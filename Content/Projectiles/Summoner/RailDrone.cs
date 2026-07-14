using Terraria;

namespace Eternia.Content.Projectiles.Summoner
{
    // A sniper platform. Slow and long-ranged, and its railshots punch through a line of enemies.
    public class RailDrone : DroneMinion
    {
        public override int ShotCooldown => 62;
        public override float ShotSpeed => 18f;
        public override float Range => 820f;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 30;
            Projectile.height = 30;
        }

        protected override void ConfigureShot(Projectile shot)
        {
            shot.penetrate = 3;
            shot.scale = 1.3f;
        }
    }
}
