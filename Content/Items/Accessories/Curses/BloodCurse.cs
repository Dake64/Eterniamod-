using Eternia.Content.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Accessories.Curses
{
    public class BloodCurse : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 1);
        }

        public override void UpdateAccessory(
            Player player,
            bool hideVisual)
        {
            var cursedPlayer =
                player.GetModPlayer<CursedMagePlayer>();

            cursedPlayer.BaseCorruption += 35;
            player.statLifeMax2 -= 20;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Vertebrae, 5)
                .AddIngredient(ItemID.FallenStar, 3)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
