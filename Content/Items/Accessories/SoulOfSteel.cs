using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Accessories
{
    // Pre-Hardmode, for ANY Warrior -- something to wear before you know which subclass you are.
    public class SoulOfSteel : WarriorAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Blue, 40);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Melee) += 0.06f;
            player.GetCritChance(DamageClass.Melee) += 3f;
            player.statDefense += 4;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("IronBar", 12)
                .AddIngredient(ItemID.Leather, 6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
