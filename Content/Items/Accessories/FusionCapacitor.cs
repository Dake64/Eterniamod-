using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories
{
    // Hardmode. The core charges 60% faster AND the Overdrive Protocol runs 4 seconds longer --
    // the uptime build.
    public class FusionCapacitor : SummonerAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Pink, 200);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var tech = player.GetModPlayer<TechSummonerPlayer>();

            tech.AccCoreRateMult *= 1.60f;
            tech.AccOverdriveBonusTicks += 240; // +4s
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CapacitorBank>(1)
                .AddIngredient(ItemID.SummonerEmblem, 1)
                .AddIngredient<AncientBattery>(10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
