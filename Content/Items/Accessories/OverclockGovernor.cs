using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories
{
    // Hardmode. Overclock -- the window where you exceed your minion cap -- runs 5 seconds
    // longer. The "flood the field" build.
    public class OverclockGovernor : SummonerAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Yellow, 300);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var legion = player.GetModPlayer<AdvancedSummonerPlayer>();

            legion.AccOverclockBonusTicks += 300; // +5s

            player.GetAttackSpeed(DamageClass.Summon) += 0.08f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CommandersSigil>(1)
                .AddIngredient(ItemID.PapyrusScarab, 1)
                .AddIngredient(ItemID.SoulofSight, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
