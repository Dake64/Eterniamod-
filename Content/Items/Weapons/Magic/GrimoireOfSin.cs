using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Magic
{
    // Early Hardmode. Generates little Corruption and refunds energy on hit -- an easy
    // introduction to the two-resource game.
    public class GrimoireOfSin : CurseWeapon
    {
        public override int EnergyCost => 6;
        protected override int CorruptionPerCast => 2;
        protected override int RefundOnHit => 3;

        public override void SetDefaults() =>
            SetCurseDefaults(60, 20, ItemRarityID.LightRed, 12f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("EterniaMythril", 12)
                .AddIngredient(ItemID.SoulofNight, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
