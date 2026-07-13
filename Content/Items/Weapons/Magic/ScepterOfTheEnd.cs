using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Magic
{
    // Base-endgame curse weapon. Generates huge Corruption and its bolts get stronger the
    // closer you sit to the Corruption limit.
    public class ScepterOfTheEnd : CurseWeapon
    {
        public override int EnergyCost => 16;
        protected override int CorruptionPerCast => 8;
        protected override int Pierce => 2;

        protected override float CorruptionDamageMultiplier(int corruption) =>
            1f + corruption * 0.003f; // +60% at 200

        public override void SetDefaults() =>
            SetCurseDefaults(115, 22, ItemRarityID.Red, 12f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LunarBar, 12)
                .AddIngredient(ItemID.FragmentNebula, 6)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
