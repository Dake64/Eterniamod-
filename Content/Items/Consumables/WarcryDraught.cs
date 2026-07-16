using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Buffs;

namespace Eternia.Content.Items.Consumables
{
    public class WarcryDraught : EterniaConsumable
    {
        protected override int BuffType => ModContent.BuffType<WarcryDraughtBuff>();

        protected override int Rarity => ItemRarityID.LightPurple;

        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ItemID.Daybloom)
                .AddIngredient(ItemID.Deathweed)
                .AddTile(TileID.Bottles)
                .Register();
        }
    }
}
