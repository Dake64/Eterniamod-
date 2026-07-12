using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Magic
{
    // Wind tier. Craftable. Fires an air blade that pierces.
    public class GaleScepter : ElementalStaff
    {
        public override int Element => 3;

        public override void SetDefaults() =>
            SetElementalDefaults(34, 22, 7, ItemRarityID.Green, 14f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Cloud, 25)
                .AddIngredient(ItemID.Wood, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
