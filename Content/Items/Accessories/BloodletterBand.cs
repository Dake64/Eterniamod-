using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories
{
    // Pre-Hardmode. You bank Crimson Trail 30% faster off every bleeding wound.
    public class BloodletterBand : WarriorAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Green, 60);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CrimsonTrailPlayer>().AccTrailGainMult *= 1.30f;

            player.GetCritChance(DamageClass.Melee) += 4f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Shackle, 1)
                .AddRecipeGroup("EterniaEvilBar", 8)
                .AddRecipeGroup("EterniaEvilScale", 6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
