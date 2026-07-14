using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Projectiles.Summoner;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // The Tech Summoner's first Hardmode whip. Paints a target for the fleet to focus.
    public class CircuitLash : SummonerWhip
    {
        protected override int WhipProjectile =>
            ModContent.ProjectileType<TechWhipProjectile>();

        public override void SetDefaults() =>
            SetWhipDefaults(52, 26, ItemRarityID.LightPurple);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("EterniaMythril", 8)
                .AddIngredient<DamagedCircuit>(10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
