using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Buffs;

namespace Eternia.Content.Items.Consumables
{
    public class EternalFeast : EterniaConsumable
    {
        protected override int BuffType => ModContent.BuffType<EternalFeastBuff>();

        // A long meal, meant to stay up: 20 minutes.
        protected override int BuffDuration => 72000;

        protected override bool IsFood => true;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Mushroom, 5)
                .AddIngredient(ItemID.Daybloom, 2)
                .AddTile(TileID.CookingPots)
                .Register();
        }
    }
}
