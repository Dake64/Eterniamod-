using Terraria;

namespace Eternia.Content.Projectiles.Necromancer
{
    // A lesser echo of King Slime: tanky, slow, reserves a lot of life. Reuses the
    // Skeleton texture until real art exists.
    public class GuardianSlimeMinion : BaseNecroMinion
    {
        public override string Texture =>
            "ETERNIA/Content/Projectiles/Necromancer/SkeletonMinion";

        public override int ManaDrain => 8;
        public override int ReservePercent => 30;
        public override float MoveSpeed => 5f;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 44;
            Projectile.height = 44;
        }
    }
}
