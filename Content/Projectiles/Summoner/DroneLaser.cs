using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Projectiles.Summoner
{
    // The bolt a drone fires. Summon damage, so it rides the Tech Summoner's Power Core bonuses
    // and the whip tag. Reuses the LightningBolt texture.
    public class DroneLaser : ModProjectile
    {
        // Set by the firing drone (see DroneMinion.ConfigureShot): the debuff this bolt inflicts.
        public int DebuffType = -1;
        public int DebuffTime = 120;

        public override string Texture =>
            "ETERNIA/Content/Projectiles/LightningBoltProjectile";

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (Projectile.velocity != Vector2.Zero)
            {
                Projectile.rotation =
                    Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }

            if (Main.rand.NextBool(2))
            {
                Dust d = Dust.NewDustDirect(
                    Projectile.position, Projectile.width, Projectile.height,
                    DustID.Electric, 0f, 0f, 100, default, 0.9f);

                d.noGravity = true;
            }

            Lighting.AddLight(Projectile.Center, 0.2f, 0.5f, 0.6f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (DebuffType >= 0)
            {
                target.AddBuff(DebuffType, DebuffTime);
            }
        }

        public override Color? GetAlpha(Color lightColor) =>
            new Color(140, 230, 255, 160);
    }
}
