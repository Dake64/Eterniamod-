using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Magic
{
    // Fire, start tier. Craftable. Shoots a fireball that burns.
    public class EmberSpark : ElementalStaff
    {
        public override int Element => 0;

        public override void SetDefaults() =>
            SetElementalDefaults(18, 24, 6, ItemRarityID.White);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Torch, 15)
                .AddIngredient(ItemID.Wood, 10)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
