using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Magic
{
    // The Cursed Mage's ultimate weapon. Scales directly with current Corruption,
    // generates enormous Corruption, and greatly amplifies Cursed Burst -- the peak of
    // the class's risk/reward. Crafted from the Scepter of the End.
    public class NecronomiconOfEternia : CurseWeapon
    {
        public override int EnergyCost => 18;
        protected override int CorruptionPerCast => 10;
        protected override int Pierce => 2;
        public override float BurstMultiplier => 1.5f;

        protected override float CorruptionDamageMultiplier(int corruption) =>
            1f + corruption * 0.004f; // +80% at 200

        public override void SetDefaults() =>
            SetCurseDefaults(140, 20, ItemRarityID.Purple, 12f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ScepterOfTheEnd>(1)
                .AddIngredient(ItemID.LunarBar, 15)
                .AddIngredient(ItemID.FragmentNebula, 8)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
