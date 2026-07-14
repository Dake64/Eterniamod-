using Terraria;
using Terraria.ModLoader;

using Eternia.Content.Items.Weapons.Ranger;

namespace Eternia.Content.Globals
{
    // Carries which Archer bow fired an arrow and whether it was a Perfect Shot, then dispatches
    // the per-tick and on-hit behaviour back to that bow's virtuals. This is what lets each bow
    // keep its own identity in its own file instead of one giant switch.
    public class ArcherBowGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public int bowType = -1;
        public bool perfectArrow;

        // How many enemies this arrow has already pierced (Eclipse Recurve ramps on it).
        public int pierceHits;

        private ArcherBow Bow =>
            bowType >= 0 ? ModContent.GetModItem(bowType) as ArcherBow : null;

        private Player Owner(Projectile projectile)
        {
            if (projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
            {
                return null;
            }

            Player player = Main.player[projectile.owner];

            return player != null && player.active ? player : null;
        }

        public override void AI(Projectile projectile)
        {
            ArcherBow bow = Bow;

            if (bow == null)
            {
                return;
            }

            Player player = Owner(projectile);

            if (player != null)
            {
                bow.UpdateArrow(projectile, player);
            }
        }

        public override void ModifyHitNPC(
            Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            ArcherBow bow = Bow;
            Player player = Owner(projectile);

            if (bow != null && player != null)
            {
                bow.ModifyArrowHit(projectile, target, player, ref modifiers);
            }
        }

        public override void OnHitNPC(
            Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            ArcherBow bow = Bow;
            Player player = Owner(projectile);

            if (bow != null && player != null)
            {
                bow.OnArrowHit(projectile, target, player, perfectArrow, hit, damageDone);
            }

            pierceHits++;
        }
    }
}
