using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories
{
    // Hardmode. Leans all the way into the Tech Summoner's defensive, engineered feel: +10 defense
    // always, and the Overdrive shield hardens by a further 20.
    public class ReactivePlating : SummonerAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Yellow, 280);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var tech = player.GetModPlayer<TechSummonerPlayer>();

            tech.AccOverdriveDefense += 20;

            player.statDefense += 10;
            player.endurance += 0.04f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CapacitorBank>(1)
                .AddIngredient(ItemID.ObsidianShield, 1)
                .AddIngredient<PlasmaCore>(10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
