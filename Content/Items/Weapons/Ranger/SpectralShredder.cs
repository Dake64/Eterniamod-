using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Post-Plantera. Spectre-forged: heavy damage at a brutal fire rate.
    public class SpectralShredder : GunnerGun
    {
        public override void SetDefaults() =>
            SetGunDefaults(48, 5, ItemRarityID.Yellow, knockBack: 2f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SpectreBar, 14)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
