using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // Swarm: tiny, cheap undead -- reserve very little life so you can field a horde,
    // but each one is weak.
    public class SwarmGrimoire : SpecializedGrimoire
    {
        public override float ReserveMult => 0.5f;
        public override float SizeMult => 0.8f;
        public override float SummonDamageMult => 0.8f;
        protected override string StyleLine => "Swarm: -50% reserved life, smaller weaker undead";

        public override void SetDefaults() =>
            SetGrimoireDefaults(46, ItemRarityID.LightRed);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Book, 1)
                .AddRecipeGroup("EterniaMythril", 8)
                .AddIngredient(ItemID.SoulofLight, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
