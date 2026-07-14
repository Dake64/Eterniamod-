using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Projectiles.Summoner;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // Your first assembly. One of each part -- the simplest drone that flies.
    public class ScoutDroneKit : DroneKit
    {
        protected override int DroneType => ModContent.ProjectileType<ScoutDrone>();

        public override void SetDefaults() =>
            SetKitDefaults(10, 30, ItemRarityID.Blue);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<DroneChassis>(1)
                .AddIngredient<ServoCore>(1)
                .AddIngredient<CommandChip>(1)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
