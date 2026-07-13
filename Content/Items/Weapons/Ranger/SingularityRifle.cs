using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Post-Moon Lord. A colossal singularity bolt that tears straight through everything.
    // Enormous damage and heat -- built to be ridden in the critical zone.
    public class SingularityRifle : EnergyWeapon
    {
        public override float HeatPerShot => 20f;

        protected override int Pierce => 8;
        protected override float ProjScale => 1.8f;

        public override void SetDefaults() =>
            SetEnergyDefaults(200, 20, ItemRarityID.Red, shootSpeed: 20f, knockBack: 6f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LunarBar, 14)
                .AddIngredient(ItemID.FragmentVortex, 18)
                .AddIngredient<AncientBattery>(10)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
