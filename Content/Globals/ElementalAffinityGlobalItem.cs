using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.Globals
{
    // Elemental Affinity, item side: Lightning and Wind make the Elementalist's magic
    // projectiles fly faster. Mana cost, cast speed, damage and defense are handled in
    // ElementalistPlayer; on-hit effects in ElementalAffinityGlobalProjectile.
    public class ElementalAffinityGlobalItem : GlobalItem
    {
        public override void ModifyShootStats(
            Item item,
            Player player,
            ref Vector2 position,
            ref Vector2 velocity,
            ref int type,
            ref int damage,
            ref float knockback)
        {
            var ele = player.GetModPlayer<ElementalistPlayer>();

            if (!ele.IsActiveElementalist() ||
                !item.DamageType.CountsAsClass(DamageClass.Magic))
            {
                return;
            }

            // Lightning (2) and Wind (3): faster projectiles. Tempest Winds makes Wind
            // projectiles faster still.
            if (ele.CurrentElement == 2)
            {
                velocity *= 1.25f;
            }
            else if (ele.CurrentElement == 3)
            {
                velocity *= ele.HasElementNode("Tempest Winds") ? 1.4f : 1.25f;
            }
        }
    }
}
