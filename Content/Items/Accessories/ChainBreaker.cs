using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories
{
    // Hardmode. Raises the Combo ceiling by 8 -- a bigger cap means a bigger payoff at the top.
    public class ChainBreaker : WarriorAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Pink, 200);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var fighter = player.GetModPlayer<FighterPlayer>();

            fighter.AccBonusMaxCombo += 8;

            player.GetAttackSpeed(DamageClass.Melee) += 0.08f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BrawlersWraps>(1)
                .AddIngredient(ItemID.WarriorEmblem, 1)
                .AddIngredient(ItemID.HallowedBar, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
