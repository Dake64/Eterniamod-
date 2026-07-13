using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Magic
{
    // A rapid, low-cost stream (a "continuous beam" approximated as fast auto-fire) that
    // drains Cursed Energy gradually while you hold it. Teaches sustained management.
    public class EclipseStaff : CurseWeapon
    {
        public override int EnergyCost => 2;

        public override void SetDefaults() =>
            SetCurseDefaults(10, 8, ItemRarityID.Green, 14f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("EterniaEvilBar", 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
