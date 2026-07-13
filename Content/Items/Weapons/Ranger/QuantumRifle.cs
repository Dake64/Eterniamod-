using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Projectiles.Ranger;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Post-Plantera. Guided quantum bolts that pierce as they track -- high, reliable DPS.
    public class QuantumRifle : EnergyWeapon
    {
        public override float HeatPerShot => 8f;

        protected override int Pierce => 2;

        protected override int EnergyProjectile =>
            ModContent.ProjectileType<EnergyHomingBolt>();

        public override void SetDefaults() =>
            SetEnergyDefaults(70, 12, ItemRarityID.Yellow, shootSpeed: 13f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteBar, 16)
                .AddIngredient<PlasmaCore>(10)
                .AddIngredient<AncientBattery>(4)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
