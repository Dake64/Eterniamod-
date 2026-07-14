using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // First Hardmode Gunner gun. A fast, reliable Momentum builder.
    public class MythrilRipper : GunnerGun
    {
        public override void SetDefaults() =>
            SetGunDefaults(26, 6, ItemRarityID.LightPurple, knockBack: 2f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("EterniaMythril", 12)
                .AddIngredient(ItemID.SoulofLight, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
