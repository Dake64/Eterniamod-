using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Start of Hardmode. Your first energy weapon: steady straight bolts, low heat -- the gun
    // to learn the Temperature mechanic on.
    public class LaserRifle : EnergyWeapon
    {
        public override float HeatPerShot => 4f;

        public override void SetDefaults() =>
            SetEnergyDefaults(30, 10, ItemRarityID.LightRed, shootSpeed: 13f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("EterniaMythril", 8)
                .AddIngredient<EnergyCrystal>(10)
                .AddIngredient(ItemID.SoulofLight, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
