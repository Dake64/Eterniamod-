using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // A bullet hose: very fast and a little inaccurate. The last pre-Hardmode Momentum machine
    // before the evil-biome upgrades.
    public class Chatterbox : GunnerGun
    {
        protected override float SpreadDegrees => 7f;

        public override void SetDefaults() =>
            SetGunDefaults(8, 5, ItemRarityID.Green, knockBack: 1f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("EterniaGold", 12)
                .AddIngredient(ItemID.IllegalGunParts, 1)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
