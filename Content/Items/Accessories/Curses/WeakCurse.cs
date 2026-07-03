using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories.Curses
{
    public class WeakCurse : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;

            Item.accessory = true;

            Item.rare =
                ItemRarityID.Blue;

            Item.value =
                Item.buyPrice(
                    silver: 50);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.RottenChunk, 5)
                .AddIngredient(ItemID.FallenStar, 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }

        public override void UpdateAccessory(
            Player player,
            bool hideVisual)
        {
            var cursedPlayer =
                player.GetModPlayer<CursedMagePlayer>();

            cursedPlayer.BaseCorruption += 10;

            player.statDefense -= 2;
        }
    }
}