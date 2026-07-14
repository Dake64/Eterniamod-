namespace Eternia.Content.Projectiles.Summoner
{
    // Two wisps fused into one. Steadier and harder-hitting -- the legion's rank and file.
    public class SpiritSoldierMinion : LegionMinion
    {
        public override float MoveSpeed => 9.5f;
        public override float Inertia => 12f;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 26;
            Projectile.height = 30;
        }
    }
}
