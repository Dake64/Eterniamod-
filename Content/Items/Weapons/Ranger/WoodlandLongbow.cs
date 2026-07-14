using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;

namespace Eternia.Content.Items.Weapons.Ranger
{
    // Sold by the Merchant after the Eye of Cthulhu (see ArcherShop). A precise longbow that
    // rewards distance -- your introduction to positioning.
    public class WoodlandLongbow : ArcherBow
    {
        public override void SetDefaults() =>
            SetBowDefaults(15, 20, ItemRarityID.Blue, shootSpeed: 12f, knockBack: 3f);

        // Extra damage against distant enemies (available to any wielder, not just a Perfect Shot).
        public override void ModifyArrowHit(
            Projectile arrow, NPC target, Player player, ref NPC.HitModifiers modifiers)
        {
            float blocks = Vector2.Distance(player.Center, target.Center) / 16f;

            if (blocks > 15f)
            {
                float bonus = MathHelper.Clamp((blocks - 15f) / 25f, 0f, 1f) * 0.20f;
                modifiers.SourceDamage *= 1f + bonus;
            }
        }
    }
}
