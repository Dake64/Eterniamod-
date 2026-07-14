using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Lunar-tier. A blistering fire rate that keeps Momentum pinned near the top.
    public class VortexRipper : GunnerGun
    {
        public override void SetDefaults() =>
            SetGunDefaults(54, 4, ItemRarityID.Cyan, knockBack: 1.5f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentVortex, 16)
                .AddIngredient(ItemID.LunarBar, 6)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
