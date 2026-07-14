using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Projectiles.Summoner;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // A rapid-fire platform: twin servos, chattering guns.
    public class RepeaterDroneKit : DroneKit
    {
        protected override int DroneType => ModContent.ProjectileType<RepeaterDrone>();

        public override void SetDefaults() =>
            SetKitDefaults(13, 30, ItemRarityID.Green);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<DroneChassis>(2)
                .AddIngredient<ServoCore>(2)
                .AddIngredient<CommandChip>(1)
                .AddIngredient<EnergeticFragment>(15)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
