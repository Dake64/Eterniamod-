using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;
using Eternia.Content.Projectiles.Summoner;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // The Tech Summoner's endgame whip. Mark one target and the whole fleet erases it.
    public class OmegaLash : SummonerWhip
    {
        protected override int WhipProjectile =>
            ModContent.ProjectileType<OmegaWhipProjectile>();

        public override void SetDefaults() =>
            SetWhipDefaults(100, 20, ItemRarityID.Red, knockBack: 5f, shootSpeed: 5f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LunarBar, 12)
                .AddIngredient<AncientBattery>(8)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
