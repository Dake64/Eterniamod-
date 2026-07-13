using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Projectiles.Ranger;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Post-mechs. A five-bolt plasma blast; each pellet detonates on impact. High burst heat,
    // devastating point-blank.
    public class PlasmaShotgun : EnergyWeapon
    {
        public override float HeatPerShot => 7f;

        protected override int ShotCount => 5;
        protected override float SpreadDegrees => 14f;

        protected override int EnergyProjectile =>
            ModContent.ProjectileType<EnergyPlasmaBolt>();

        public override void SetDefaults() =>
            SetEnergyDefaults(22, 26, ItemRarityID.LightPurple, shootSpeed: 12f, knockBack: 4f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("EterniaAdamantite", 12)
                .AddIngredient<PlasmaCore>(8)
                .AddIngredient(ItemID.SoulofMight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
