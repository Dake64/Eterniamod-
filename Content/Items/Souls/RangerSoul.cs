using Terraria;
using Eternia.Content.Players;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Souls
{
    public class RangerSoul : ModItem
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
            var modPlayer = player.GetModPlayer<EterniaPlayer>();

            modPlayer.hasSoul = true;
            modPlayer.rangerSoul = true; // o la que corresponda
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Eternia.Content.Items.Souls.EmptySoul>())
                .Register();
        }
    }
}