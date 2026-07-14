using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories
{
    // Hardmode. Dead Eye runs 4 seconds longer and Momentum barely decays -- the "stay in the
    // rampage" build, instead of the "reach it faster" one.
    public class DeadEyeRegulator : RangerAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Yellow, 280);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var gunner = player.GetModPlayer<GunnerPlayer>();

            gunner.AccDeadEyeBonusTicks += 240; // +4s
            gunner.AccMomentumDecayMult *= 0.55f;

            player.GetCritChance(DamageClass.Ranged) += 6f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RecoilDamper>(1)
                .AddIngredient(ItemID.SniperScope, 1)
                .AddIngredient(ItemID.SoulofSight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
