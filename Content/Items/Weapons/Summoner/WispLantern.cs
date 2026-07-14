using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Projectiles.Summoner;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // THE SEED OF THE LEGION -- the only legion staff crafted from raw materials. Everything
    // stronger is FUSED from staves you already own, so keep making these: every fusion eats them.
    public class WispLantern : LegionStaff
    {
        protected override int MinionType => ModContent.ProjectileType<WispMinion>();

        public override void SetDefaults() =>
            SetStaffDefaults(8, 30, ItemRarityID.White);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Wood, 12)
                .AddIngredient(ItemID.Torch, 6)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
