using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Projectiles.Ranger;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Dropped by Duke Fishron. Each arrow looses a spectral dragon that streaks straight ahead,
    // shredding anything in its path -- brutal on bosses and lines of enemies alike.
    public class DragonboneBow : ArcherBow
    {
        public override void SetDefaults() =>
            SetBowDefaults(112, 15, ItemRarityID.Yellow, shootSpeed: 14f, knockBack: 3.5f);

        public override void OnArrowSpawn(Projectile arrow, Player player, bool perfect)
        {
            if (player.whoAmI != Main.myPlayer)
            {
                return;
            }

            Projectile.NewProjectile(
                player.GetSource_ItemUse(player.HeldItem),
                arrow.Center,
                arrow.velocity,
                ModContent.ProjectileType<SpectralDragon>(),
                (int)(arrow.damage * 0.6f),
                arrow.knockBack,
                player.whoAmI);
        }
    }
}
