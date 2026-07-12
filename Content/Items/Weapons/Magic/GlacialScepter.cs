using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Magic
{
    // Ice, Hardmode ice biome. Craftable. Giant crystals with a chance to freeze.
    public class GlacialScepter : ElementalStaff
    {
        public override int Element => 1;

        public override void SetDefaults() =>
            SetElementalDefaults(80, 24, 11, ItemRarityID.Lime);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("EterniaAdamantite", 12)
                .AddIngredient(ItemID.IceBlock, 25)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
