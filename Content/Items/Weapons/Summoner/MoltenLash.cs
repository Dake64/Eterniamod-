using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Projectiles.Summoner;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // The best pre-Hardmode whip. Sets tagged foes ablaze -- take it into the Wall of Flesh.
    public class MoltenLash : SummonerWhip
    {
        protected override int WhipProjectile =>
            ModContent.ProjectileType<MoltenWhipProjectile>();

        public override void SetDefaults() =>
            SetWhipDefaults(40, 24, ItemRarityID.LightRed, knockBack: 4f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HellstoneBar, 14)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
