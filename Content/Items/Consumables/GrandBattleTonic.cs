using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Buffs;

namespace Eternia.Content.Items.Consumables
{
    public class GrandBattleTonic : EterniaConsumable
    {
        protected override int BuffType => ModContent.BuffType<GrandBattleTonicBuff>();

        protected override int Rarity => ItemRarityID.LightRed;

        public override void AddRecipes()
        {
            // The hardmode upgrade: the base tonic empowered with the three mechanical souls.
            CreateRecipe(2)
                .AddIngredient<BattleTonic>(2)
                .AddIngredient(ItemID.SoulofMight)
                .AddIngredient(ItemID.SoulofSight)
                .AddIngredient(ItemID.SoulofFright)
                .AddTile(TileID.Bottles)
                .Register();
        }
    }
}
