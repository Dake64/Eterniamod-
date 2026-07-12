using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Magic
{
    // Fire, Underworld tier. Craftable from Hellstone. The strongest fire staff before
    // the Elemental Sage Scepter.
    public class MagmaTome : ElementalStaff
    {
        public override int Element => 0;

        public override void SetDefaults() =>
            SetElementalDefaults(47, 28, 9, ItemRarityID.Orange);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HellstoneBar, 15)
                .AddIngredient(ItemID.Obsidian, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
