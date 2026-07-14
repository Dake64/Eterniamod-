using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Projectiles.Summoner;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // Post-mechs Beast Tamer whip. Heavier tag, longer reach, and it leaves foes bleeding.
    public class FeralLash : SummonerWhip
    {
        protected override int WhipProjectile =>
            ModContent.ProjectileType<FeralWhipProjectile>();

        public override void SetDefaults() =>
            SetWhipDefaults(68, 24, ItemRarityID.Pink, knockBack: 4f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 12)
                .AddIngredient(ItemID.SoulofMight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
