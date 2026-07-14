using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories
{
    // Pre-Hardmode. Energy weapons run 15% cooler -- you can ride the hot zone a little longer
    // before the reactor bites back.
    public class CoolantRig : RangerAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Green, 60);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<EnergyShooterPlayer>().AccHeatPerShotMult *= 0.85f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<EnergeticFragment>(12)
                .AddIngredient<DamagedCircuit>(8)
                .AddIngredient(ItemID.Shackle, 1)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
