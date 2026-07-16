using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Buffs;

namespace Eternia.Content.Items.Consumables
{
    public class PackleadersBrew : EterniaConsumable
    {
        protected override int BuffType => ModContent.BuffType<PackleadersBrewBuff>();

        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ItemID.Waterleaf)
                .AddTile(TileID.Bottles)
                .Register();
        }
    }
}
