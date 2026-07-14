using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // The Crimson half of the evil-biome pair. Its critical hits siphon a little life back --
    // the survivalist's choice.
    public class CrimsonHunter : ArcherBow
    {
        public override void SetDefaults() =>
            SetBowDefaults(41, 15, ItemRarityID.LightRed, shootSpeed: 12f);

        public override void OnArrowHit(
            Projectile arrow, NPC target, Player player,
            bool perfect, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Crit && player.statLife < player.statLifeMax2)
            {
                player.statLife += 3;
                player.HealEffect(3);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CrimtaneBar, 12)
                .AddIngredient(ItemID.TissueSample, 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
