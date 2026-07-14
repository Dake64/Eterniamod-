using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories
{
    // Pre-Hardmode. Concentration charges 25% faster -- you draw a good shot sooner.
    public class HuntersLens : RangerAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Green, 60);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<ArcherPlayer>().AccFocusRegenMult *= 1.25f;

            player.GetCritChance(DamageClass.Ranged) += 4f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Aglet, 1)
                .AddIngredient(ItemID.Leather, 8)
                .AddIngredient(ItemID.Feather, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
