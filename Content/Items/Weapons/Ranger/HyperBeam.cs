using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Post-Golem. A near-continuous beam: very fast, deeply piercing bolts that read as a
    // sustained ray. Great sweeping damage.
    public class HyperBeam : EnergyWeapon
    {
        public override float HeatPerShot => 11f;

        protected override int Pierce => 6;
        protected override float ProjScale => 1.3f;

        public override void SetDefaults() =>
            SetEnergyDefaults(70, 7, ItemRarityID.Yellow, shootSpeed: 20f, knockBack: 1.5f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteBar, 16)
                .AddIngredient<PlasmaCore>(14)
                .AddIngredient<AncientBattery>(6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
