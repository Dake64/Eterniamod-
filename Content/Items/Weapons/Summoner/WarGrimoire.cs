using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // War: harder-hitting undead, at the cost of the Necromancer's own defense.
    public class WarGrimoire : SpecializedGrimoire
    {
        public override float SummonDamageMult => 1.25f;
        public override int DefenseDelta => -8;
        protected override string StyleLine => "War: +25% undead damage, -8 defense";

        public override void SetDefaults() =>
            SetGrimoireDefaults(26, ItemRarityID.Green);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Book, 1)
                .AddIngredient(ItemID.Bone, 15)
                .AddIngredient(ItemID.RottenChunk, 8)
                .AddTile(TileID.WorkBenches)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.Book, 1)
                .AddIngredient(ItemID.Bone, 15)
                .AddIngredient(ItemID.Vertebrae, 8)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
