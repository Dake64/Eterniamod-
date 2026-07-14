using Terraria;
using Terraria.ID;

namespace Eternia.Content.Projectiles.Summoner
{
    // The seed of the legion. Tiny and darting -- and the raw material every fusion starts from.
    public class WispMinion : LegionMinion
    {
        public override float MoveSpeed => 11.5f;
        public override float Inertia => 9f;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 18;
            Projectile.height = 18;
        }

        public override void AI()
        {
            base.AI();

            if (Main.rand.NextBool(6))
            {
                Dust d = Dust.NewDustDirect(
                    Projectile.position, Projectile.width, Projectile.height,
                    DustID.PurpleTorch);

                d.noGravity = true;
            }
        }
    }
}
