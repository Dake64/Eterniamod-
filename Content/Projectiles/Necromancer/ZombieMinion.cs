using Terraria;

namespace Eternia.Content.Projectiles.Necromancer
{
    // A slow, heavy zombie: reserves more life but drains little mana. Reuses the
    // TrainingWhip... no, reuses the Skeleton texture until real art exists.
    public class ZombieMinion : BaseNecroMinion
    {
        public override string Texture =>
            "ETERNIA/Content/Projectiles/Necromancer/SkeletonMinion";

        public override int ManaDrain => 4;
        public override int ReservePercent => 20;
        public override float MoveSpeed => 4.5f;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 34;
            Projectile.height = 42;
        }
    }
}
