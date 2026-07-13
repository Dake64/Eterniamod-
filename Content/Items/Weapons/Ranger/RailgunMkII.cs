using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Post-mechs. Slow, brutal precision shots that pierce and fly fast. Each pull spikes the
    // heat hard -- the trigger-discipline weapon.
    public class RailgunMkII : EnergyWeapon
    {
        public override float HeatPerShot => 8f;

        protected override int Pierce => 4;
        protected override float ProjScale => 1.4f;

        public override void SetDefaults() =>
            SetEnergyDefaults(95, 30, ItemRarityID.LightPurple, shootSpeed: 19f, knockBack: 6f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("EterniaAdamantite", 14)
                .AddIngredient<PlasmaCore>(8)
                .AddIngredient<AncientBattery>(3)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
