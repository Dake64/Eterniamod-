using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories
{
    // Hardmode. The aura pulses 30% faster and reaches 25% wider -- fewer, bigger hits become
    // many, constant ones. The crowd-control build.
    public class ResonatingWard : WarriorAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Yellow, 280);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var guardian = player.GetModPlayer<GuardianPlayer>();

            guardian.AccAuraPulseMult *= 0.70f; // shorter gap between pulses
            guardian.AccAuraRadius += 0.25f;

            player.statDefense += 5;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BulwarkCharm>(1)
                .AddIngredient(ItemID.ObsidianShield, 1)
                .AddIngredient(ItemID.SoulofMight, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
