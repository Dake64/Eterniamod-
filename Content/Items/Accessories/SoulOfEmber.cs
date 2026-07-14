using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Accessories
{
    // Pre-Hardmode, for ANY Mage -- something to wear before you know which subclass you are.
    public class SoulOfEmber : MageAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Blue, 40);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Magic) += 0.06f;
            player.statManaMax2 += 40;
            player.manaRegenBonus += 4;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FallenStar, 12)
                .AddRecipeGroup("EterniaSilver", 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
