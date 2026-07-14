using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Accessories
{
    // Pre-Hardmode, for ANY Ranger -- something to wear before you know which subclass you are.
    public class SoulOfTheHunt : RangerAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Blue, 40);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Ranged) += 0.06f;
            player.GetCritChance(DamageClass.Ranged) += 3f;
            player.moveSpeed += 0.05f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Leather, 8)
                .AddIngredient(ItemID.Feather, 12)
                .AddRecipeGroup("IronBar", 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
