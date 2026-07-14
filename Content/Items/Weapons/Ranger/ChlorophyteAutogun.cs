using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Chlorophyte-fed autogun. High rate and solid damage -- a post-mech workhorse.
    public class ChlorophyteAutogun : GunnerGun
    {
        public override void SetDefaults() =>
            SetGunDefaults(40, 5, ItemRarityID.Lime, knockBack: 1.5f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteBar, 16)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
