using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories
{
    // Hardmode. Does nothing for building Ferocity -- it makes the PAYOFF enormous: Primal Roar
    // grants a further +25% summon damage. The burst build.
    public class PrimalTotem : SummonerAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Yellow, 280);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<BeastTamerPlayer>().AccFrenzyDamage += 0.25f;

            player.maxMinions += 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AlphasFang>(1)
                .AddIngredient(ItemID.PapyrusScarab, 1)
                .AddIngredient(ItemID.SoulofFright, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
