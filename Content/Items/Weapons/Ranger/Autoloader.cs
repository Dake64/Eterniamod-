using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Extremely high fire rate at lower per-shot damage -- rockets Momentum to the top.
    public class Autoloader : GunnerGun
    {
        public override void SetDefaults() =>
            SetGunDefaults(22, 5, ItemRarityID.LightPurple, knockBack: 1.5f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("EterniaAdamantite", 12)
                .AddIngredient(ItemID.SoulofLight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
