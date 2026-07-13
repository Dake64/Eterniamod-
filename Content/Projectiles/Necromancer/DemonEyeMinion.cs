using Terraria;

namespace Eternia.Content.Projectiles.Necromancer
{
    // A fast, flighty demon eye: cheap on life but flits around quickly. Reuses the
    // Skeleton texture until real art exists.
    public class DemonEyeMinion : BaseNecroMinion
    {
        public override string Texture =>
            "ETERNIA/Content/Projectiles/Necromancer/SkeletonMinion";

        public override int ManaDrain => 3;
        public override int ReservePercent => 10;
        public override float MoveSpeed => 8f;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 28;
            Projectile.height = 28;
        }
    }
}
