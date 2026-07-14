using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Projectiles.Summoner;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // THE FINAL FUSION: Sentinel + Golem collapse into a Singularity Wraith, which halves the
    // defenses of whatever it touches. A field full of them is the Advanced Summoner's whole
    // fantasy. Both staves are consumed.
    public class SingularityCore : LegionStaff
    {
        protected override int MinionType =>
            ModContent.ProjectileType<SingularityWraithMinion>();

        public override void SetDefaults() =>
            SetStaffDefaults(58, 32, ItemRarityID.Red, mana: 16);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SentinelBeacon>(1)
                .AddIngredient<FusionMatrix>(1)
                .AddIngredient(ItemID.LunarBar, 12)
                .AddIngredient(ItemID.FragmentNebula, 8)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
