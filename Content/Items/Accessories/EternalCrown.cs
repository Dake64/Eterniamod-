using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Accessories
{
    // Post-Moon Lord. The Summoner's capstone -- it drives all three Summoner mechanics at once
    // (Ferocity, the Power Core, Command/Legion).
    public class EternalCrown : SummonerAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Red, 600);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // Beast Tamer
            var tamer = player.GetModPlayer<Players.BeastTamerPlayer>();
            tamer.AccFerocityGainMult *= 1.30f;
            tamer.AccFrenzyDamage += 0.12f;

            // Tech Summoner
            var tech = player.GetModPlayer<Players.TechSummonerPlayer>();
            tech.AccCoreRateMult *= 1.30f;
            tech.AccOverdriveDefense += 10;

            // Advanced Summoner
            var legion = player.GetModPlayer<Players.AdvancedSummonerPlayer>();
            legion.AccCommandRateMult *= 1.30f;
            legion.AccLegionScaleBonus += 0.06f;

            player.GetDamage(DamageClass.Summon) += 0.14f;
            player.maxMinions += 2;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SoulOfThePack>(1)
                .AddIngredient(ItemID.AvengerEmblem, 1)
                .AddIngredient(ItemID.LunarBar, 12)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
