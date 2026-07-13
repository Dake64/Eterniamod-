using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Magic
{
    // Very high damage, and a big power spike once Corruption passes 150 -- it rewards
    // riding the edge of the Cursed Collapse.
    public class BookOfCollapse : CurseWeapon
    {
        public override int EnergyCost => 14;
        protected override int CorruptionPerCast => 5;

        protected override float CorruptionDamageMultiplier(int corruption) =>
            corruption > 150 ? 1.5f : 1f;

        public override void SetDefaults() =>
            SetCurseDefaults(95, 24, ItemRarityID.Yellow);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteBar, 14)
                .AddIngredient(ItemID.SoulofFright, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
