using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Magic
{
    // Area attack. Rewards catching multiple enemies by returning extra Cursed Energy
    // per foe in the burst.
    public class ForbiddenTome : CurseWeapon
    {
        public override int EnergyCost => 8;
        protected override int RefundOnHit => 2;
        protected override bool AoEOnHit => true;

        public override void SetDefaults() =>
            SetCurseDefaults(24, 28, ItemRarityID.Green);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("EterniaEvilBar", 8)
                .AddRecipeGroup("EterniaEvilScale", 6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
