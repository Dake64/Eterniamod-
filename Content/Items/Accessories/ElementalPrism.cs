using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories
{
    // Hardmode. Element switching is almost instant (-22 ticks) -- built for a mage who wants to
    // weave all five elements in a single fight.
    public class ElementalPrism : MageAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Pink, 220);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<ElementalistPlayer>().AccSwitchCooldownCut += 22;

            player.GetDamage(DamageClass.Magic) += 0.08f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AttunementCharm>(1)
                .AddIngredient(ItemID.SorcererEmblem, 1)
                .AddIngredient(ItemID.SoulofLight, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
