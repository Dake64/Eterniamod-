namespace Eternia.Content.Projectiles.Summoner
{
    // A rapid-fire platform. Weaker shots, but it never stops chattering.
    public class RepeaterDrone : DroneMinion
    {
        public override int ShotCooldown => 26;
        public override float ShotSpeed => 12f;
        public override float Range => 600f;
    }
}
