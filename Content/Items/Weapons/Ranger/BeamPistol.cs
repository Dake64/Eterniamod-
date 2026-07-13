using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Start of Hardmode. A short piercing beam -- punches through a couple of foes per shot.
    public class BeamPistol : EnergyWeapon
    {
        public override float HeatPerShot => 5f;

        protected override int Pierce => 2;
        protected override float ProjScale => 1.2f;

        public override void SetDefaults() =>
            SetEnergyDefaults(34, 12, ItemRarityID.LightRed, shootSpeed: 15f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("EterniaMythril", 10)
                .AddIngredient<PlasmaCore>(4)
                .AddIngredient(ItemID.SoulofLight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
