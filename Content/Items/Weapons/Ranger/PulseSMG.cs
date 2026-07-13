using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Start of Hardmode. The fastest-firing early energy weapon. Tiny heat per shot, but it
    // fires so fast it still climbs -- pure sustained fire.
    public class PulseSMG : EnergyWeapon
    {
        public override float HeatPerShot => 3f;

        public override void SetDefaults() =>
            SetEnergyDefaults(20, 6, ItemRarityID.LightRed, shootSpeed: 14f, knockBack: 1.5f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("EterniaMythril", 8)
                .AddIngredient<EnergyCrystal>(14)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
