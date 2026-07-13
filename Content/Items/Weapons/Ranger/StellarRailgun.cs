using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Post-Moon Lord. The ultimate precision weapon: slow, staggering single shots that pierce
    // almost anything. The single hardest-hitting energy weapon.
    public class StellarRailgun : EnergyWeapon
    {
        public override float HeatPerShot => 22f;

        protected override int Pierce => 10;
        protected override float ProjScale => 2f;

        public override void SetDefaults() =>
            SetEnergyDefaults(260, 34, ItemRarityID.Purple, shootSpeed: 22f, knockBack: 7f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LunarBar, 18)
                .AddIngredient(ItemID.FragmentSolar, 20)
                .AddIngredient<AncientBattery>(12)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
