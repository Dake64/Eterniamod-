using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Placeable;

namespace Eternia.Content.Items.Materials
{
    // Tier 3 soul-metal bar -- the last thing Eternia gives you before Hellstone.
    public class ReveniteBar : ModItem
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.AdamantiteBar;

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(copper: 320);
            Item.material = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ReveniteOre>(5)
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}
