using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Buffs;

namespace Eternia.Content.Items.Consumables
{
    public class ArcanistsBrew : EterniaConsumable
    {
        protected override int BuffType => ModContent.BuffType<ArcanistsBrewBuff>();

        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ItemID.Moonglow)
                .AddTile(TileID.Bottles)
                .Register();
        }
    }
}
