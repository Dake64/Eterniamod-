using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Placeable;

namespace Eternia.Content.Items.Materials
{
    // Hardmode tier 2 soul-metal bar. Smelted at an Adamantite/Titanium Forge.
    public class AetheriumBar : ModItem
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.OrichalcumBar;

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(silver: 9);
            Item.material = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AetheriumOre>(4)
                .AddTile(TileID.AdamantiteForge)
                .Register();
        }
    }
}
