using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories
{
    // Hardmode. Built for the payoff: Perfect Shots hit 25% harder and the bar fills 40% faster,
    // so you land far more of them.
    public class FalconEye : RangerAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Pink, 200);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var archer = player.GetModPlayer<ArcherPlayer>();

            archer.AccFocusRegenMult *= 1.40f;
            archer.AccPerfectDamage += 0.25f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<HuntersLens>(1)
                .AddIngredient(ItemID.MagicQuiver, 1)
                .AddIngredient(ItemID.RangerEmblem, 1)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
