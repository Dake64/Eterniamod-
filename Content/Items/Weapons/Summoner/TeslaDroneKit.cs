using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Projectiles.Summoner;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // The best pre-Hardmode build. A plasma core in the barrel: its bolts electrify.
    public class TeslaDroneKit : DroneKit
    {
        protected override int DroneType => ModContent.ProjectileType<TeslaDrone>();

        public override void SetDefaults() =>
            SetKitDefaults(19, 30, ItemRarityID.Orange, mana: 12);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<DroneChassis>(2)
                .AddIngredient<ServoCore>(2)
                .AddIngredient<CommandChip>(2)
                .AddIngredient<PlasmaCore>(4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
