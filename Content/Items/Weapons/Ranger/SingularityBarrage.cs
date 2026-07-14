using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // The Gunner's endgame minigun. An absurd fire rate that reaches Dead Eye in a heartbeat and
    // shreds through it -- the pinnacle of rapid-fire.
    public class SingularityBarrage : GunnerGun
    {
        protected override float SpreadDegrees => 4f;

        public override void SetDefaults() =>
            SetGunDefaults(78, 3, ItemRarityID.Red, knockBack: 1.5f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LunarBar, 18)
                .AddIngredient(ItemID.FragmentVortex, 14)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
