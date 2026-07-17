using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Placeable;

namespace Eternia.Content.Items.Materials
{
    // Hardmode tier 3 soul-metal bar -- the top of the Eternia ore ladder. Adamantite/Titanium Forge.
    public class NullsteelBar : ModItem
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.TitaniumBar;

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(silver: 14);
            Item.material = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<NullsteelOre>(5)
                .AddTile(TileID.AdamantiteForge)
                .Register();
        }
    }
}
