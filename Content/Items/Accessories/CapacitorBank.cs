using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories
{
    // Pre-Hardmode. The Power Core charges 30% faster.
    public class CapacitorBank : SummonerAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Green, 60);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<TechSummonerPlayer>().AccCoreRateMult *= 1.30f;

            player.statDefense += 3;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<EnergyCrystal>(8)
                .AddIngredient<DamagedCircuit>(10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
