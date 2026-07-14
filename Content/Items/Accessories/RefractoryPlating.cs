using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories
{
    // Hardmode. THE build-changer: an overheat no longer hurts you at all. The weapon still locks
    // out to vent, but you can shove it straight into the red without fear -- exactly the reckless
    // play the Energy Gunner wants. Costs you cooling, so the lockouts hurt in tempo instead.
    public class RefractoryPlating : RangerAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Yellow, 320);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var energy = player.GetModPlayer<EnergyShooterPlayer>();

            energy.AccOverheatShield = true;
            energy.AccCoolRateMult *= 0.80f; // the plating traps heat

            player.statDefense += 6;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CoolantRig>(1)
                .AddIngredient(ItemID.ObsidianShield, 1)
                .AddIngredient<PlasmaCore>(10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
