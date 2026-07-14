using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories
{
    // Pre-Hardmode. Your defensive aura projects 12% wider.
    public class BulwarkCharm : WarriorAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Green, 60);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GuardianPlayer>().AccAuraRadius += 0.12f;

            player.statDefense += 4;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Shackle, 1)
                .AddRecipeGroup("IronBar", 14)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
