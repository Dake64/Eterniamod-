using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Projectiles.Ranger;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Post-mechs. Chain-lightning cannon: each bolt arcs to nearby enemies. Fast, low heat --
    // superb crowd clear.
    public class TeslaCannon : EnergyWeapon
    {
        public override float HeatPerShot => 5f;

        protected override int EnergyProjectile =>
            ModContent.ProjectileType<EnergyChainBolt>();

        public override void SetDefaults() =>
            SetEnergyDefaults(40, 9, ItemRarityID.LightPurple, shootSpeed: 13f, knockBack: 2f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 12)
                .AddIngredient<EnergyCrystal>(14)
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
