using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Accessories
{
    // Post-Moon Lord. The Ranger's capstone -- it feeds all three Ranger mechanics at once
    // (Temperature, Concentration, Momentum).
    public class EternalSight : RangerAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Red, 600);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // Energy Gunner
            var energy = player.GetModPlayer<Players.EnergyShooterPlayer>();
            energy.AccHeatPerShotMult *= 0.85f;
            energy.AccCoolRateMult *= 1.30f;

            // Archer
            var archer = player.GetModPlayer<Players.ArcherPlayer>();
            archer.AccFocusRegenMult *= 1.30f;
            archer.AccPerfectDamage += 0.15f;

            // Gunner
            var gunner = player.GetModPlayer<Players.GunnerPlayer>();
            gunner.AccMomentumGainMult *= 1.30f;
            gunner.AccMomentumDecayMult *= 0.70f;

            player.GetDamage(DamageClass.Ranged) += 0.14f;
            player.GetCritChance(DamageClass.Ranged) += 8f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SoulOfTheHunt>(1)
                .AddIngredient(ItemID.AvengerEmblem, 1)
                .AddIngredient(ItemID.LunarBar, 12)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
