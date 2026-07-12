using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Magic
{
    // Crafted from the five Hardmode elemental staves. Fires the player's ACTIVE
    // affinity (Element = -1), or a random element if none is active.
    public class GrimoireOfFiveElements : ElementalStaff
    {
        public override int Element => -1;

        public override void SetDefaults() =>
            SetElementalDefaults(118, 22, 14, ItemRarityID.Cyan);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<PhoenixStaff>(1)
                .AddIngredient<GlacialScepter>(1)
                .AddIngredient<StormOrb>(1)
                .AddIngredient<HurricaneStaff>(1)
                .AddIngredient<SeismicScepter>(1)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
