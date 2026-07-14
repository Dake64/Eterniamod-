using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories
{
    // Hardmode. Crimson Trail floods in 60% faster -- your technique comes back around constantly.
    public class HemophageSigil : WarriorAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Pink, 200);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CrimsonTrailPlayer>().AccTrailGainMult *= 1.60f;

            player.GetDamage(DamageClass.Melee) += 0.08f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BloodletterBand>(1)
                .AddIngredient(ItemID.WarriorEmblem, 1)
                .AddIngredient(ItemID.SoulofNight, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
