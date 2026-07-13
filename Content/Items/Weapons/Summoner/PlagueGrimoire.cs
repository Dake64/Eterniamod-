using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // Plague: undead inflict Venom on hit, but strike a little softer.
    public class PlagueGrimoire : SpecializedGrimoire
    {
        public override int OnHitDebuff => BuffID.Venom;
        public override float SummonDamageMult => 0.9f;
        protected override string StyleLine => "Plague: undead inflict Venom, -10% damage";

        public override void SetDefaults() =>
            SetGrimoireDefaults(50, ItemRarityID.LightRed);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Book, 1)
                .AddRecipeGroup("EterniaMythril", 8)
                .AddIngredient(ItemID.CursedFlame, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.Book, 1)
                .AddRecipeGroup("EterniaMythril", 8)
                .AddIngredient(ItemID.Ichor, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
