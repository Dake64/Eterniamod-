using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories
{
    // Hardmode. Vents 60% faster and burns 20% less heat per shot -- built for a gunner who
    // wants to LIVE in the critical zone.
    public class HeatSinkArray : RangerAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Pink, 200);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var energy = player.GetModPlayer<EnergyShooterPlayer>();

            energy.AccHeatPerShotMult *= 0.80f;
            energy.AccCoolRateMult *= 1.60f;

            player.GetCritChance(DamageClass.Ranged) += 6f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CoolantRig>(1)
                .AddIngredient(ItemID.RangerEmblem, 1)
                .AddIngredient<AncientBattery>(8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
