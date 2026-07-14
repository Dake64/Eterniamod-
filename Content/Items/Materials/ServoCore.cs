using Terraria.ID;

namespace Eternia.Content.Items.Materials
{
    // The motor. What makes a chassis fly and track a target.
    public class ServoCore : DroneComponent
    {
        protected override int Rarity => ItemRarityID.Green;
        protected override int SellCopper => 1800;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<EnergyCrystal>(5)
                .AddIngredient<DamagedCircuit>(4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
