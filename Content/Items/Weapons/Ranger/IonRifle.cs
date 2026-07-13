using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Projectiles.Ranger;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Post-mechs. Fires guided ion bolts that curve into the nearest enemy -- forgiving aim,
    // great against erratic targets.
    public class IonRifle : EnergyWeapon
    {
        public override float HeatPerShot => 6f;

        protected override int EnergyProjectile =>
            ModContent.ProjectileType<EnergyHomingBolt>();

        public override void SetDefaults() =>
            SetEnergyDefaults(55, 12, ItemRarityID.LightPurple, shootSpeed: 12f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 12)
                .AddIngredient<PlasmaCore>(6)
                .AddIngredient(ItemID.SoulofSight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
