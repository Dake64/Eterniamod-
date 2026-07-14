using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Crafted from jungle materials. Poisons its targets, and your arrows bite harder into
    // anything already poisoned -- superb against tanky foes.
    public class JungleStinger : ArcherBow
    {
        public override void SetDefaults() =>
            SetBowDefaults(34, 16, ItemRarityID.Orange, shootSpeed: 12f);

        public override void ModifyArrowHit(
            Projectile arrow, NPC target, Player player, ref NPC.HitModifiers modifiers)
        {
            if (target.HasBuff(BuffID.Poisoned))
            {
                modifiers.SourceDamage *= 1.15f;
            }
        }

        public override void OnArrowHit(
            Projectile arrow, NPC target, Player player,
            bool perfect, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Poisoned, 300);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Stinger, 8)
                .AddIngredient(ItemID.JungleSpores, 6)
                .AddIngredient(ItemID.Vine, 3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
