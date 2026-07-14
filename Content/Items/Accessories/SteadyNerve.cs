using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Items.Accessories
{
    // Hardmode. The Archer's weakness is getting hit -- a blow shatters your Concentration.
    // This cuts that loss by 65%, so you can hold a charge through a brawl. The survivalist pick.
    public class SteadyNerve : RangerAccessory
    {
        public override void SetDefaults() =>
            SetAccessoryDefaults(ItemRarityID.Yellow, 260);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var archer = player.GetModPlayer<ArcherPlayer>();

            archer.AccFocusLossMult *= 0.35f;

            player.GetDamage(DamageClass.Ranged) += 0.08f;
            player.statDefense += 5;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<HuntersLens>(1)
                .AddIngredient(ItemID.CrossNecklace, 1)
                .AddIngredient(ItemID.HallowedBar, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
