using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Start of Hardmode. Very fast, very low heat per shot -- a light spray weapon that reaches
    // the hot zone gradually.
    public class PlasmaCarbine : EnergyWeapon
    {
        public override float HeatPerShot => 3f;

        public override void SetDefaults() =>
            SetEnergyDefaults(24, 7, ItemRarityID.LightRed, shootSpeed: 13f, knockBack: 1.5f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("EterniaMythril", 8)
                .AddIngredient<EnergyCrystal>(12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
