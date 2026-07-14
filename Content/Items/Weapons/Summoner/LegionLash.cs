using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Projectiles.Summoner;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // The Advanced Summoner's endgame whip. Mark one foe and the entire legion erases it.
    public class LegionLash : SummonerWhip
    {
        protected override int WhipProjectile =>
            ModContent.ProjectileType<LegionWhipProjectile>();

        public override void SetDefaults() =>
            SetWhipDefaults(100, 20, ItemRarityID.Red, knockBack: 5f, shootSpeed: 5f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LunarBar, 12)
                .AddIngredient(ItemID.FragmentNebula, 6)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
