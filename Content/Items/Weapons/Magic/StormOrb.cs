using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Magic
{
    // Lightning, early Hardmode. Craftable. Fast bolts that bounce between foes (the
    // Lightning affinity arcs even more in Hardmode).
    public class StormOrb : ElementalStaff
    {
        public override int Element => 2;

        public override void SetDefaults() =>
            SetElementalDefaults(88, 16, 11, ItemRarityID.Lime, 16f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("EterniaAdamantite", 12)
                .AddIngredient(ItemID.SoulofLight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
