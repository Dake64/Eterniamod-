using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Magic
{
    // Area attacks, moderate Corruption. Excellent against groups.
    public class TomeOfTheAbyss : CurseWeapon
    {
        public override int EnergyCost => 10;
        protected override int CorruptionPerCast => 4;
        protected override bool AoEOnHit => true;

        public override void SetDefaults() =>
            SetCurseDefaults(78, 26, ItemRarityID.Lime);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("EterniaAdamantite", 12)
                .AddIngredient(ItemID.SoulofNight, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
