using Terraria.ID;

namespace Eternia.Content.Items.Materials
{
    // The airframe. Forged from salvaged tech scrap -- every drone starts here.
    public class DroneChassis : DroneComponent
    {
        protected override int Rarity => ItemRarityID.Blue;
        protected override int SellCopper => 1200;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<EnergeticFragment>(10)
                .AddIngredient<DamagedCircuit>(5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
