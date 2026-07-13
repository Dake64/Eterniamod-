using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Projectiles.Ranger;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Post-Plantera. A heavy arc cannon: powerful bolts that chain lightning between foes.
    // Stronger and hotter than the Tesla Cannon.
    public class ArcCannon : EnergyWeapon
    {
        public override float HeatPerShot => 9f;

        protected override int EnergyProjectile =>
            ModContent.ProjectileType<EnergyChainBolt>();

        public override void SetDefaults() =>
            SetEnergyDefaults(60, 14, ItemRarityID.Lime, shootSpeed: 13f, knockBack: 3f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteBar, 14)
                .AddIngredient<PlasmaCore>(8)
                .AddIngredient(ItemID.SoulofFright, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
