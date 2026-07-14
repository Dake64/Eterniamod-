using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Projectiles.Summoner;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // FUSION: Golem + Construct, refined into arc energy. Post-mechs -- blistering speed, its
    // strikes electrify. Both staves are consumed.
    public class SentinelBeacon : LegionStaff
    {
        protected override int MinionType => ModContent.ProjectileType<ArcSentinelMinion>();

        public override void SetDefaults() =>
            SetStaffDefaults(42, 30, ItemRarityID.Pink, mana: 14);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<FusionMatrix>(1)
                .AddIngredient<ConstructCore>(1)
                .AddIngredient(ItemID.HallowedBar, 12)
                .AddIngredient(ItemID.SoulofSight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
