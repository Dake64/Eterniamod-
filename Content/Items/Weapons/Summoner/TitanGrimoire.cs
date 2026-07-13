using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // Titan: few, huge undead -- each reserves a lot of life but hits like a truck.
    public class TitanGrimoire : SpecializedGrimoire
    {
        public override float ReserveMult => 1.6f;
        public override float SizeMult => 1.6f;
        public override float SummonDamageMult => 1.6f;
        protected override string StyleLine => "Titan: +60% reserved life, giant undead, +60% damage";

        public override void SetDefaults() =>
            SetGrimoireDefaults(56, ItemRarityID.LightRed);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Book, 1)
                .AddRecipeGroup("EterniaAdamantite", 8)
                .AddIngredient(ItemID.SoulofMight, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
