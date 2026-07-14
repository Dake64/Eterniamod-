using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Projectiles.Ranger
{
    // Celestial Horizon: a falling star from the Perfect-Shot star rain. Drops onto the target
    // and bursts. Ranged so the Archer's damage bonuses apply. Reuses the LightningBolt texture.
    public class StarShard : ModProjectile
    {
        public override string Texture =>
            "ETERNIA/Content/Projectiles/LightningBoltProjectile";

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.rotation += 0.3f;

            int d = Dust.NewDust(
                Projectile.position, Projectile.width, Projectile.height,
                DustID.GoldFlame, 0f, 0f, 100, default, 1.2f);

            Main.dust[d].noGravity = true;

            Lighting.AddLight(Projectile.Center, 0.6f, 0.5f, 0.2f);
        }

        public override Color? GetAlpha(Color lightColor) =>
            new Color(255, 220, 120, 120);
    }
}
