using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Projectiles.Summoner;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // FUSION: two Wisps become one Spirit Soldier. Both lanterns are consumed.
    public class SpiritBanner : LegionStaff
    {
        protected override int MinionType => ModContent.ProjectileType<SpiritSoldierMinion>();

        public override void SetDefaults() =>
            SetStaffDefaults(14, 32, ItemRarityID.Blue);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<WispLantern>(2)
                .AddIngredient(ItemID.Bone, 15)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
