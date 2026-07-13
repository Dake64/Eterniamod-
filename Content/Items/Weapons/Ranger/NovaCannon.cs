using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Projectiles.Ranger;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Post-Golem. Fires a huge plasma nova that detonates on impact. Massive area damage and a
    // lot of heat per shot.
    public class NovaCannon : EnergyWeapon
    {
        public override float HeatPerShot => 14f;

        protected override float ProjScale => 1.5f;

        protected override int EnergyProjectile =>
            ModContent.ProjectileType<EnergyPlasmaBolt>();

        public override void SetDefaults() =>
            SetEnergyDefaults(120, 16, ItemRarityID.Yellow, shootSpeed: 13f, knockBack: 4f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SpectreBar, 12)
                .AddIngredient<PlasmaCore>(12)
                .AddIngredient<AncientBattery>(6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
