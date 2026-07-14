using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Projectiles.Summoner;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // The Beast Tamer's first Hardmode whip: tags foes for the pack and points your beasts at
    // the target.
    public class BeastcallerWhip : SummonerWhip
    {
        protected override int WhipProjectile =>
            ModContent.ProjectileType<BeastWhipProjectile>();

        public override void SetDefaults() =>
            SetWhipDefaults(52, 26, ItemRarityID.LightPurple);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("EterniaMythril", 8)
                .AddIngredient(ItemID.Leather, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
