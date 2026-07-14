using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories
{
    // Pre-Hardmode. +3 max Combo and a longer window before it drops.
    public class BrawlersWraps : WarriorAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Green, 60);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var fighter = player.GetModPlayer<FighterPlayer>();

            fighter.AccBonusMaxCombo += 3;
            fighter.AccBonusComboDuration += 45; // +0.75s

            player.GetAttackSpeed(DamageClass.Melee) += 0.05f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Leather, 10)
                .AddRecipeGroup("IronBar", 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
