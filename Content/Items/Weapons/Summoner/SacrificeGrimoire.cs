using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // Sacrifice: undead reserve far less life (field a bigger army), but drink more mana.
    public class SacrificeGrimoire : SpecializedGrimoire
    {
        public override float ReserveMult => 0.6f;
        public override float ManaMult => 1.5f;
        protected override string StyleLine => "Sacrifice: -40% reserved life, +50% mana drain";

        public override void SetDefaults() =>
            SetGrimoireDefaults(24, ItemRarityID.Green);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Book, 1)
                .AddIngredient(ItemID.Bone, 15)
                .AddIngredient(ItemID.LifeCrystal, 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
