using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Buffs;

namespace Eternia.Content.Items.Consumables
{
    public class BloodlustDraught : EterniaConsumable
    {
        protected override int BuffType => ModContent.BuffType<BloodlustDraughtBuff>();

        protected override int Rarity => ItemRarityID.LightPurple;

        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ItemID.Deathweed)
                .AddIngredient(ItemID.Waterleaf)
                .AddTile(TileID.Bottles)
                .Register();
        }
    }
}
