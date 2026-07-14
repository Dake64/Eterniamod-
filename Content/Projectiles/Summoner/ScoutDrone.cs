namespace Eternia.Content.Projectiles.Summoner
{
    // Your first build. A light scout: modest fire rate, reliable.
    public class ScoutDrone : DroneMinion
    {
        public override int ShotCooldown => 48;
        public override float Range => 560f;
    }
}
