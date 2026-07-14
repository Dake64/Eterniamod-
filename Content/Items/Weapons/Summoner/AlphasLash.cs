using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Projectiles.Summoner;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // The Beast Tamer's endgame whip: enormous reach and a crushing tag that turns the pack rabid.
    public class AlphasLash : SummonerWhip
    {
        protected override int WhipProjectile =>
            ModContent.ProjectileType<AlphaWhipProjectile>();

        public override void SetDefaults() =>
            SetWhipDefaults(110, 20, ItemRarityID.Red, knockBack: 5f, shootSpeed: 5f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LunarBar, 12)
                .AddIngredient(ItemID.FragmentSolar, 6)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
