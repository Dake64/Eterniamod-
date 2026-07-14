using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories
{
    // Hardmode. Ferocity builds 55% faster and lingers far longer -- the pack essentially never
    // calms down, so Primal Roar is always close.
    public class BeastlordsCollar : SummonerAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Pink, 200);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var tamer = player.GetModPlayer<BeastTamerPlayer>();

            tamer.AccFerocityGainMult *= 1.55f;
            tamer.AccFerocityDecayMult *= 0.50f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AlphasFang>(1)
                .AddIngredient(ItemID.SummonerEmblem, 1)
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
