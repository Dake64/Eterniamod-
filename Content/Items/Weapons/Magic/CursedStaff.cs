using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Magic
{
    // Scales its damage once Corruption passes 50 -- the player's introduction to the
    // high-risk game.
    public class CursedStaff : CurseWeapon
    {
        public override int EnergyCost => 8;
        protected override int CorruptionPerCast => 3;

        protected override float CorruptionDamageMultiplier(int corruption) =>
            corruption > 50 ? 1.3f : 1f;

        public override void SetDefaults() =>
            SetCurseDefaults(68, 22, ItemRarityID.LightRed);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("EterniaMythril", 12)
                .AddIngredient(ItemID.SoulofNight, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
