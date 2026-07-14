using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories
{
    // Pre-Hardmode. The pack whips itself into a fury 30% faster.
    public class AlphasFang : SummonerAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Green, 60);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<BeastTamerPlayer>().AccFerocityGainMult *= 1.30f;

            player.GetDamage(DamageClass.Summon) += 0.05f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Leather, 10)
                .AddIngredient(ItemID.Stinger, 6)
                .AddIngredient(ItemID.Vine, 3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
