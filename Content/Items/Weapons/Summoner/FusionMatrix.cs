using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Projectiles.Summoner;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // FUSION: Construct + Soldier collapse into a Fusion Golem. First Hardmode legionnaire --
    // heavy, and it shatters armor. Both staves are consumed.
    public class FusionMatrix : LegionStaff
    {
        protected override int MinionType => ModContent.ProjectileType<FusionGolemMinion>();

        public override void SetDefaults() =>
            SetStaffDefaults(32, 32, ItemRarityID.LightPurple, mana: 12);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ConstructCore>(1)
                .AddIngredient<SpiritBanner>(1)
                .AddRecipeGroup("EterniaMythril", 10)
                .AddIngredient(ItemID.SoulofLight, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
