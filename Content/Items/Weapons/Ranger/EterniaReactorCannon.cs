using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Projectiles.Ranger;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Post-Moon Lord. The Energy Gunner's signature: a twin-barrel reactor cannon firing paired
    // plasma detonations. Colossal heat, colossal reward. Also a rare Moon Lord drop.
    public class EterniaReactorCannon : EnergyWeapon
    {
        public override float HeatPerShot => 18f;

        protected override int ShotCount => 2;
        protected override float SpreadDegrees => 5f;
        protected override float ProjScale => 1.3f;

        protected override int EnergyProjectile =>
            ModContent.ProjectileType<EnergyPlasmaBolt>();

        public override void SetDefaults() =>
            SetEnergyDefaults(180, 12, ItemRarityID.Purple, shootSpeed: 17f, knockBack: 5f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LunarBar, 18)
                .AddIngredient(ItemID.FragmentStardust, 20)
                .AddIngredient<AncientBattery>(15)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
