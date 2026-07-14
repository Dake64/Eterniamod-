using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Projectiles.Summoner;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // The engineer's masterpiece. Every part you know how to build, poured into one drone.
    public class OmegaDroneKit : DroneKit
    {
        protected override int DroneType => ModContent.ProjectileType<OmegaDrone>();

        public override void SetDefaults() =>
            SetKitDefaults(64, 30, ItemRarityID.Red, mana: 16);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<DroneChassis>(6)
                .AddIngredient<ServoCore>(5)
                .AddIngredient<CommandChip>(5)
                .AddIngredient<AncientBattery>(15)
                .AddIngredient(ItemID.LunarBar, 10)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
