using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Buffs;

namespace Eternia.Content.Items.Consumables
{
    public class AdrenalineDraught : EterniaConsumable
    {
        protected override int BuffType => ModContent.BuffType<AdrenalineDraughtBuff>();

        protected override int Rarity => ItemRarityID.LightPurple;

        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ItemID.Daybloom)
                .AddIngredient(ItemID.Blinkroot)
                .AddTile(TileID.Bottles)
                .Register();
        }
    }
}
