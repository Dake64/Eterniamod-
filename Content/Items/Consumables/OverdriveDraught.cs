using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Buffs;

namespace Eternia.Content.Items.Consumables
{
    public class OverdriveDraught : EterniaConsumable
    {
        protected override int BuffType => ModContent.BuffType<OverdriveDraughtBuff>();

        protected override int Rarity => ItemRarityID.LightPurple;

        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ItemID.Shiverthorn)
                .AddIngredient(ItemID.Waterleaf)
                .AddTile(TileID.Bottles)
                .Register();
        }
    }
}
