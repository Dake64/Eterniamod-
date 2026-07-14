using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Projectiles.Summoner;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // Post-mechs assembly. A long-range sniper platform whose railshots pierce a whole line.
    public class RailDroneKit : DroneKit
    {
        protected override int DroneType => ModContent.ProjectileType<RailDrone>();

        public override void SetDefaults() =>
            SetKitDefaults(48, 30, ItemRarityID.Pink, mana: 14);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<DroneChassis>(4)
                .AddIngredient<ServoCore>(3)
                .AddIngredient<CommandChip>(3)
                .AddIngredient<AncientBattery>(6)
                .AddIngredient(ItemID.HallowedBar, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
