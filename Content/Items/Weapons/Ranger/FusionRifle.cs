using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Post-Golem. A fusion rifle: fast, piercing, high-damage bolts. Runs hot.
    public class FusionRifle : EnergyWeapon
    {
        public override float HeatPerShot => 12f;

        protected override int Pierce => 3;

        public override void SetDefaults() =>
            SetEnergyDefaults(90, 11, ItemRarityID.Yellow, shootSpeed: 16f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SpectreBar, 14)
                .AddIngredient<PlasmaCore>(10)
                .AddIngredient<AncientBattery>(5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
