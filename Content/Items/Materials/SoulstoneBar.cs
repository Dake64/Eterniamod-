using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Placeable;

namespace Eternia.Content.Items.Materials
{
    // Tier 1 soul-metal bar. Smelted from Soulstone at any Furnace.
    public class SoulstoneBar : ModItem
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.CobaltBar;

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(copper: 90);
            Item.material = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SoulstoneOre>(3)
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}
