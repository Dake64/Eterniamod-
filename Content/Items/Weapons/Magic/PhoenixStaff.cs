using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Magic
{
    // Fire, start of Hardmode. Craftable. Fiery explosions and enhanced burn.
    public class PhoenixStaff : ElementalStaff
    {
        public override int Element => 0;

        public override void SetDefaults() =>
            SetElementalDefaults(72, 22, 10, ItemRarityID.LightRed);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("EterniaMythril", 12)
                .AddIngredient(ItemID.HellstoneBar, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
