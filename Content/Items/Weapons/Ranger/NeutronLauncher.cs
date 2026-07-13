using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Projectiles.Ranger;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Post-Golem. Launches large guided neutron bolts that hunt down targets. Heavy hitter,
    // heavy heat.
    public class NeutronLauncher : EnergyWeapon
    {
        public override float HeatPerShot => 13f;

        protected override float ProjScale => 1.4f;

        protected override int EnergyProjectile =>
            ModContent.ProjectileType<EnergyHomingBolt>();

        public override void SetDefaults() =>
            SetEnergyDefaults(110, 18, ItemRarityID.Cyan, shootSpeed: 12f, knockBack: 4f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SpectreBar, 12)
                .AddIngredient<AncientBattery>(8)
                .AddIngredient<PlasmaCore>(10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
