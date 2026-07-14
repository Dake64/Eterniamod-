using Terraria;
using Terraria.ModLoader;

using Eternia.Content.Items.Weapons.Ranger;

namespace Eternia.Content.Globals
{
    // Carries which Gunner gun fired a bullet, then dispatches its on-hit signature back to the
    // gun (burn, chill, extra Momentum...). Keeps each gun's effect in its own file.
    public class GunnerGunGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public int gunType = -1;

        public override void OnHitNPC(
            Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (gunType < 0 ||
                projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
            {
                return;
            }

            Player player = Main.player[projectile.owner];

            if (player == null || !player.active)
            {
                return;
            }

            if (ModContent.GetModItem(gunType) is GunnerGun gun)
            {
                gun.OnBulletHit(projectile, target, player, hit, damageDone);
            }
        }
    }
}
