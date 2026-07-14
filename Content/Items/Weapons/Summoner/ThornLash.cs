using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Projectiles.Summoner;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // Jungle whip. Poisons what it tags -- great for wearing down a creature you mean to tame.
    public class ThornLash : SummonerWhip
    {
        protected override int WhipProjectile =>
            ModContent.ProjectileType<ThornWhipProjectile>();

        public override void SetDefaults() =>
            SetWhipDefaults(23, 26, ItemRarityID.Orange);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Stinger, 8)
                .AddIngredient(ItemID.Vine, 4)
                .AddIngredient(ItemID.JungleSpores, 6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
