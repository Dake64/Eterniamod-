using Terraria;

namespace Eternia.Content.Projectiles.Necromancer
{
    // A lesser echo of the Eye of Cthulhu: fast, flighty, moderate cost. Reuses the
    // Skeleton texture until real art exists.
    public class EyeSpiritMinion : BaseNecroMinion
    {
        public override string Texture =>
            "ETERNIA/Content/Projectiles/Necromancer/SkeletonMinion";

        public override int ManaDrain => 6;
        public override int ReservePercent => 20;
        public override float MoveSpeed => 9f;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 36;
            Projectile.height = 36;
        }
    }
}
