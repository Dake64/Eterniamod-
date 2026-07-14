using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Starter sidearm. Cheap and steady -- your first taste of building Momentum.
    public class ScrapPistol : GunnerGun
    {
        public override void SetDefaults() =>
            SetGunDefaults(8, 13, ItemRarityID.White, knockBack: 1.5f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("IronBar", 8)
                .AddIngredient(ItemID.Wood, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
