using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Buffs;

namespace Eternia.Content.Items.Consumables
{
    public class CoolantDraught : EterniaConsumable
    {
        protected override int BuffType => ModContent.BuffType<CoolantDraughtBuff>();

        protected override int Rarity => ItemRarityID.LightPurple;

        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ItemID.Shiverthorn)
                .AddIngredient(ItemID.Blinkroot)
                .AddTile(TileID.Bottles)
                .Register();
        }
    }
}
