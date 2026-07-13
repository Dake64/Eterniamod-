using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // Lich: undead steal life for you, but reserve more of it.
    public class LichGrimoire : SpecializedGrimoire
    {
        public override bool Lifesteal => true;
        public override float ReserveMult => 1.3f;
        protected override string StyleLine => "Lich: undead lifesteal, +30% reserved life";

        public override void SetDefaults() =>
            SetGrimoireDefaults(50, ItemRarityID.LightRed);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Book, 1)
                .AddRecipeGroup("EterniaMythril", 8)
                .AddIngredient(ItemID.SoulofNight, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
