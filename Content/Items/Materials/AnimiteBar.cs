using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Placeable;

namespace Eternia.Content.Items.Materials
{
    // Tier 2 soul-metal bar. Smelted from Animite at any Furnace.
    public class AnimiteBar : ModItem
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.MythrilBar;

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(copper: 180);
            Item.material = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnimiteOre>(4)
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}
