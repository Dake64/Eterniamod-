using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Buffs;

namespace Eternia.Content.Items.Consumables
{
    public class FocusDraught : EterniaConsumable
    {
        protected override int BuffType => ModContent.BuffType<FocusDraughtBuff>();

        protected override int Rarity => ItemRarityID.LightPurple;

        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ItemID.Moonglow)
                .AddIngredient(ItemID.Blinkroot)
                .AddTile(TileID.Bottles)
                .Register();
        }
    }
}
