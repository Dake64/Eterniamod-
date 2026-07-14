using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Projectiles.Summoner;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // FUSION: a Spirit Soldier bound with a Wisp and forged into matter. The best pre-Hardmode
    // legionnaire. Both staves are consumed.
    public class ConstructCore : LegionStaff
    {
        protected override int MinionType => ModContent.ProjectileType<ArcaneConstructMinion>();

        public override void SetDefaults() =>
            SetStaffDefaults(21, 32, ItemRarityID.LightRed, mana: 12);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SpiritBanner>(1)
                .AddIngredient<WispLantern>(1)
                .AddIngredient(ItemID.HellstoneBar, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
