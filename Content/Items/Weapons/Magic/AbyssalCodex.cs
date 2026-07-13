using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Magic
{
    // The last pre-Hardmode curse weapon. High damage, very high Cursed Energy cost --
    // a preview of the high-stakes Hardmode style.
    public class AbyssalCodex : CurseWeapon
    {
        public override int EnergyCost => 18;

        public override void SetDefaults() =>
            SetCurseDefaults(40, 26, ItemRarityID.Orange);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HellstoneBar, 12)
                .AddIngredient(ItemID.Obsidian, 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
