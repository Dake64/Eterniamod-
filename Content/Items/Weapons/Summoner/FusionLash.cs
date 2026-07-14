using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Projectiles.Summoner;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // The Advanced Summoner's first Hardmode whip. Tags a foe for the whole legion.
    public class FusionLash : SummonerWhip
    {
        protected override int WhipProjectile =>
            ModContent.ProjectileType<FusionWhipProjectile>();

        public override void SetDefaults() =>
            SetWhipDefaults(52, 26, ItemRarityID.LightPurple);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("EterniaMythril", 8)
                .AddIngredient(ItemID.SoulofLight, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
