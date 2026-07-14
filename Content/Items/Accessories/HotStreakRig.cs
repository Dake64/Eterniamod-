using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories
{
    // Hardmode. Momentum builds 45% faster -- you reach Dead Eye in a fraction of the time.
    public class HotStreakRig : RangerAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Pink, 200);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var gunner = player.GetModPlayer<GunnerPlayer>();

            gunner.AccMomentumGainMult *= 1.45f;

            player.GetAttackSpeed(DamageClass.Ranged) += 0.08f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RecoilDamper>(1)
                .AddIngredient(ItemID.RangerEmblem, 1)
                .AddIngredient(ItemID.SoulofMight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
