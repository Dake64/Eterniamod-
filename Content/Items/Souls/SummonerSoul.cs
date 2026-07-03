using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Eternia.Content.Players;
namespace Eternia.Content.Items.Souls
{
    public class SummonerSoul : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var modPlayer = player.GetModPlayer<Eternia.Content.Players.EterniaPlayer>();

            modPlayer.summonerSoul = true;
            modPlayer.hasSoul = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Eternia.Content.Items.Souls.EmptySoul>())
                .Register();
        }
    }
}