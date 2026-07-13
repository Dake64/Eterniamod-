using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Projectiles.Ranger;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Post-Moon Lord. A weaponized antimatter cannon: large plasma detonations, very high
    // damage and heat.
    public class AntimatterCannon : EnergyWeapon
    {
        public override float HeatPerShot => 16f;

        protected override float ProjScale => 1.4f;

        protected override int EnergyProjectile =>
            ModContent.ProjectileType<EnergyPlasmaBolt>();

        public override void SetDefaults() =>
            SetEnergyDefaults(150, 13, ItemRarityID.Red, shootSpeed: 16f, knockBack: 4f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LunarBar, 12)
                .AddIngredient(ItemID.FragmentVortex, 15)
                .AddIngredient<AncientBattery>(8)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
