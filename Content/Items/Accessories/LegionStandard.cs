using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories
{
    // Hardmode. Makes a FULL roster worth far more: the Legion bonus scales up by a further 10%,
    // and Command charges 40% faster. Pairs with the half-slot legion, which fills fast.
    public class LegionStandard : SummonerAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Pink, 220);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var legion = player.GetModPlayer<AdvancedSummonerPlayer>();

            legion.AccCommandRateMult *= 1.40f;
            legion.AccLegionScaleBonus += 0.10f;

            player.maxMinions += 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CommandersSigil>(1)
                .AddIngredient(ItemID.SummonerEmblem, 1)
                .AddIngredient(ItemID.NecromanticScroll, 1)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
