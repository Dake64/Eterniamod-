using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // Arcane: undead drink far less mana (sustain a large army easily), but hit softer.
    public class ArcaneGrimoire : SpecializedGrimoire
    {
        public override float ManaMult => 0.6f;
        public override float SummonDamageMult => 0.85f;
        protected override string StyleLine => "Arcane: -40% mana drain, -15% undead damage";

        public override void SetDefaults() =>
            SetGrimoireDefaults(24, ItemRarityID.Green);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Book, 1)
                .AddIngredient(ItemID.Bone, 15)
                .AddIngredient(ItemID.FallenStar, 8)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
