using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Magic
{
    // Piercing bolts, medium cost. Each foe pierced refunds energy, so cutting through a
    // line makes the next cast effectively cheaper.
    public class VoidRod : CurseWeapon
    {
        public override int EnergyCost => 6;
        protected override int RefundOnHit => 2;
        protected override int Pierce => 3;

        public override void SetDefaults() =>
            SetCurseDefaults(20, 24, ItemRarityID.Blue, 12f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("EterniaEvilBar", 8)
                .AddIngredient(ItemID.Cobweb, 15)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
