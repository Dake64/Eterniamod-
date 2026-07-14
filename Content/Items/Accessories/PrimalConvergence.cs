using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories
{
    // Hardmode. The opposite build to the Prism: switching stays slow, but every switch SURGE
    // lasts 4 seconds longer -- so you commit to an element and ride the wave.
    public class PrimalConvergence : MageAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Yellow, 300);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<ElementalistPlayer>().AccSurgeBonusTicks += 240; // +4s

            player.GetDamage(DamageClass.Magic) += 0.12f;
            player.statManaMax2 += 40;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AttunementCharm>(1)
                .AddIngredient(ItemID.ManaFlower, 1)
                .AddIngredient(ItemID.SoulofSight, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
