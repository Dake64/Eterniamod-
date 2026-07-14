using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories
{
    // Hardmode. The Combo window runs 3 seconds longer -- you can reposition, dodge, and come
    // back without losing your chain. The "never drop it" build.
    public class EndlessRhythm : WarriorAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Yellow, 260);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var fighter = player.GetModPlayer<FighterPlayer>();

            fighter.AccBonusComboDuration += 180; // +3s
            fighter.AccBonusMaxCombo += 3;

            player.GetDamage(DamageClass.Melee) += 0.08f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BrawlersWraps>(1)
                .AddIngredient(ItemID.PowerGlove, 1)
                .AddIngredient(ItemID.SoulofMight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
