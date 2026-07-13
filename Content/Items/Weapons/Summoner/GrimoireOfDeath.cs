using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // The basic Grimoire: raises dominated creatures with no army modifiers. The
    // specialized grimoires reshape how they behave.
    public class GrimoireOfDeath : SpecializedGrimoire
    {
        protected override string StyleLine => "Basic: no army modifiers";

        public override void SetDefaults() =>
            SetGrimoireDefaults(26, ItemRarityID.LightRed);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Book, 1)
                .AddIngredient(ItemID.Bone, 15)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
