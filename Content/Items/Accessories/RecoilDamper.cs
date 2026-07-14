using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories
{
    // Pre-Hardmode. Momentum bleeds away 30% slower, so a missed beat does not cost you the run.
    public class RecoilDamper : RangerAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Green, 60);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GunnerPlayer>().AccMomentumDecayMult *= 0.70f;

            player.GetAttackSpeed(DamageClass.Ranged) += 0.05f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Shackle, 1)
                .AddRecipeGroup("IronBar", 12)
                .AddIngredient(ItemID.Leather, 6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
