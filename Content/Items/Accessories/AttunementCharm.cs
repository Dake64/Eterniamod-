using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories
{
    // Pre-Hardmode. You flow between elements faster (switch cooldown -10 ticks).
    public class AttunementCharm : MageAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Green, 60);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<ElementalistPlayer>().AccSwitchCooldownCut += 10;

            player.GetDamage(DamageClass.Magic) += 0.05f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BandofStarpower, 1)
                .AddIngredient(ItemID.FallenStar, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
