using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Magic
{
    // The Elemental Mage's ultimate weapon (post Moon Lord). Craftable. Fires the
    // active affinity's element (Element = -1), fully changing its behaviour with the
    // Elemental Affinity: fire explosions, freezing storms, lightning chains, piercing
    // gusts, or rock meteors.
    public class HeartOfGaia : ElementalStaff
    {
        public override int Element => -1;

        public override void SetDefaults() =>
            SetElementalDefaults(155, 18, 16, ItemRarityID.Purple);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GrimoireOfFiveElements>(1)
                .AddIngredient(ItemID.LunarBar, 15)
                .AddIngredient(ItemID.FragmentNebula, 8)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
