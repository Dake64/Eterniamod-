using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // The first true spray gun. Low damage, blistering rate -- perfect for building Momentum
    // fast on crowds.
    public class RattlerSMG : GunnerGun
    {
        protected override float SpreadDegrees => 4f;

        public override void SetDefaults() =>
            SetGunDefaults(9, 7, ItemRarityID.Blue, knockBack: 1f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("IronBar", 14)
                .AddIngredient(ItemID.Gel, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
