using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Projectiles.Summoner;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // Early pre-Hardmode whip. Your first real tagging tool -- and your first taming tool.
    public class LeatherLash : SummonerWhip
    {
        protected override int WhipProjectile =>
            ModContent.ProjectileType<LeatherWhipProjectile>();

        public override void SetDefaults() =>
            SetWhipDefaults(15, 28, ItemRarityID.Blue);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Leather, 8)
                .AddRecipeGroup("IronBar", 6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
