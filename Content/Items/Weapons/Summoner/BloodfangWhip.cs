using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Projectiles.Summoner;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // Evil-biome whip. Makes tagged foes bleed out.
    public class BloodfangWhip : SummonerWhip
    {
        protected override int WhipProjectile =>
            ModContent.ProjectileType<BloodfangWhipProjectile>();

        public override void SetDefaults() =>
            SetWhipDefaults(31, 25, ItemRarityID.LightRed);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("EterniaEvilBar", 10)
                .AddRecipeGroup("EterniaEvilScale", 6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
