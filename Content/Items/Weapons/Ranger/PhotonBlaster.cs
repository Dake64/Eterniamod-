using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Post-Plantera. Heavy photon bolts with real knockback -- a hard-hitting workhorse.
    public class PhotonBlaster : EnergyWeapon
    {
        public override float HeatPerShot => 9f;

        public override void SetDefaults() =>
            SetEnergyDefaults(78, 11, ItemRarityID.Lime, shootSpeed: 15f, knockBack: 4f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteBar, 14)
                .AddIngredient<PlasmaCore>(10)
                .AddIngredient<AncientBattery>(5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
