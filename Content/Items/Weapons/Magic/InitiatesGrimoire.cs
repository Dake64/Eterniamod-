using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Magic
{
    // First curse weapon. Low damage, low cost, refunds a little Cursed Energy on hit --
    // it teaches you to keep casting without draining out.
    public class InitiatesGrimoire : CurseWeapon
    {
        public override int EnergyCost => 3;
        protected override int RefundOnHit => 2;

        public override void SetDefaults() =>
            SetCurseDefaults(14, 22, ItemRarityID.White);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Wood, 15)
                .AddIngredient(ItemID.Cobweb, 10)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
