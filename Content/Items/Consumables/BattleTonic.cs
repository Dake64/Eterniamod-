using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Buffs;

namespace Eternia.Content.Items.Consumables
{
    public class BattleTonic : EterniaConsumable
    {
        protected override int BuffType => ModContent.BuffType<BattleTonicBuff>();

        public override void AddRecipes()
        {
            // Every herb in one bottle -- an edge for all four souls at once.
            CreateRecipe(2)
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ItemID.Daybloom)
                .AddIngredient(ItemID.Moonglow)
                .AddIngredient(ItemID.Blinkroot)
                .AddIngredient(ItemID.Waterleaf)
                .AddTile(TileID.Bottles)
                .Register();
        }
    }
}
