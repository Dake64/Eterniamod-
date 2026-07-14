using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Globals
{
    // During Dead Eye a promoted Gunner's bullets pierce more and punch through armor. Reads the
    // Gunner's live Dead Eye state (and the Full Auto / Armor Piercing passives) at spawn.
    public class GunnerGlobalProjectile : GlobalProjectile
    {
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
            {
                return;
            }

            Player player = Main.player[projectile.owner];

            if (player == null || !player.active ||
                !projectile.DamageType.CountsAsClass(DamageClass.Ranged) ||
                player.HeldItem.useAmmo != AmmoID.Bullet)
            {
                return;
            }

            var gunner = player.GetModPlayer<GunnerPlayer>();

            if (!gunner.IsActiveGunner() || !gunner.DeadEye)
            {
                return;
            }

            if (projectile.penetrate > 0)
            {
                projectile.penetrate += gunner.DeadEyePierce;
            }

            projectile.ArmorPenetration += gunner.DeadEyeArmorPen;
        }
    }
}
