using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Fires two bullets per shot in a small spread. More burst per trigger pull.
    public class TwinDerringer : GunnerGun
    {
        protected override int BulletsPerShot => 2;
        protected override float SpreadDegrees => 9f;

        public override void SetDefaults() =>
            SetGunDefaults(11, 15, ItemRarityID.Blue, knockBack: 2f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("EterniaSilver", 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
