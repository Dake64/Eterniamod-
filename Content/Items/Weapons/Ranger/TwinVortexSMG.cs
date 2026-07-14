using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Dual SMGs: two bullets per shot in a tight spread, at a fast rate.
    public class TwinVortexSMG : GunnerGun
    {
        protected override int BulletsPerShot => 2;
        protected override float SpreadDegrees => 6f;

        public override void SetDefaults() =>
            SetGunDefaults(28, 7, ItemRarityID.Pink, knockBack: 1.5f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 10)
                .AddIngredient(ItemID.SoulofSight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
