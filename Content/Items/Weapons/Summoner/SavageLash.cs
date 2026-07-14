using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Projectiles.Summoner;

namespace Eternia.Content.Items.Weapons.Summoner
{
    // Post-Plantera Beast Tamer whip. Long reach, brutal tag, bleeding and poison.
    public class SavageLash : SummonerWhip
    {
        protected override int WhipProjectile =>
            ModContent.ProjectileType<SavageWhipProjectile>();

        public override void SetDefaults() =>
            SetWhipDefaults(84, 22, ItemRarityID.Lime, knockBack: 4.5f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteBar, 16)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
