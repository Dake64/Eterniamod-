using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // The Corruption half of the evil-biome pair. Its arrows sap enemy defense (Ichor) --
    // the aggressor's choice.
    public class CorruptHunter : ArcherBow
    {
        public override void SetDefaults() =>
            SetBowDefaults(41, 15, ItemRarityID.LightRed, shootSpeed: 12f);

        public override void OnArrowHit(
            Projectile arrow, NPC target, Player player,
            bool perfect, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Ichor, 300);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DemoniteBar, 12)
                .AddIngredient(ItemID.ShadowScale, 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
