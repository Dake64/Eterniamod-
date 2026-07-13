using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Post-Moon Lord. A relentless celestial beam: extremely fast, deeply piercing bolts that
    // read as one continuous ray. Sustained annihilation.
    public class CelestialBeam : EnergyWeapon
    {
        public override float HeatPerShot => 16f;

        protected override int Pierce => 10;
        protected override float ProjScale => 1.4f;

        public override void SetDefaults() =>
            SetEnergyDefaults(120, 6, ItemRarityID.Red, shootSpeed: 22f, knockBack: 1.5f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LunarBar, 14)
                .AddIngredient(ItemID.FragmentNebula, 18)
                .AddIngredient<AncientBattery>(8)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
