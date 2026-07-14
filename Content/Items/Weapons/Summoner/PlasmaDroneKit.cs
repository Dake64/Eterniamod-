using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Projectiles.Summoner;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // First Hardmode assembly. Heavy plasma bolts that set targets ablaze.
    public class PlasmaDroneKit : DroneKit
    {
        protected override int DroneType => ModContent.ProjectileType<PlasmaDrone>();

        public override void SetDefaults() =>
            SetKitDefaults(30, 30, ItemRarityID.LightPurple, mana: 12);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<DroneChassis>(3)
                .AddIngredient<ServoCore>(3)
                .AddIngredient<CommandChip>(2)
                .AddIngredient<PlasmaCore>(8)
                .AddRecipeGroup("EterniaMythril", 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
