using Terraria.ID;

namespace Eternia.Content.Items.Materials
{
    // The brain. Without one, a drone is just a hovering brick.
    public class CommandChip : DroneComponent
    {
        protected override int Rarity => ItemRarityID.Orange;
        protected override int SellCopper => 2400;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<DamagedCircuit>(8)
                .AddIngredient<EnergyCrystal>(3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
