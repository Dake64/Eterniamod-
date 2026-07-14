using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories
{
    // Pre-Hardmode. A curse accessory: it carries +15 BASE Corruption, so you start every fight
    // already tainted. Corruption is the Cursed Mage's power AND its poison -- this is the first
    // step onto that ladder.
    public class CursedTalisman : MageAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Green, 70);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CursedMagePlayer>().BaseCorruption += 15;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Shackle, 1)
                .AddRecipeGroup("EterniaEvilBar", 8)
                .AddIngredient(ItemID.RottenChunk, 6)
                .AddTile(TileID.Anvils)
                .Register();

            // Crimson worlds use Vertebrae instead of Rotten Chunks.
            CreateRecipe()
                .AddIngredient(ItemID.Shackle, 1)
                .AddRecipeGroup("EterniaEvilBar", 8)
                .AddIngredient(ItemID.Vertebrae, 6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
