using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Post-Plantera. A rapid accelerator: modest damage, blistering rate. Reaches and holds the
    // critical zone effortlessly -- if you can keep it cool.
    public class PulseAccelerator : EnergyWeapon
    {
        public override float HeatPerShot => 6f;

        public override void SetDefaults() =>
            SetEnergyDefaults(42, 6, ItemRarityID.Lime, shootSpeed: 15f, knockBack: 2f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteBar, 12)
                .AddIngredient<EnergyCrystal>(16)
                .AddIngredient(ItemID.SoulofFright, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
