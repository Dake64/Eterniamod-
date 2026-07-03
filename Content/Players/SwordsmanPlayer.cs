using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.NPCs;

namespace Eternia.Content.Players
{
    public class SwordsmanPlayer : ModPlayer
    {
        public override void OnHitNPCWithItem(
            Item item,
            NPC target,
            NPC.HitInfo hit,
            int damageDone)
        {
            // =============================================
            // ONLY SWORDS
            // =============================================

            if (!item.DamageType.CountsAsClass(
                    DamageClass.Melee))
            {
                return;
            }

            if (item.useStyle != ItemUseStyleID.Swing)
            {
                return;
            }

            // =============================================
            // APPLY BLEED
            // =============================================

            BleedGlobalNPC bleedNPC =
                target.GetGlobalNPC<BleedGlobalNPC>();

            bleedNPC.BleedStacks++;

            if (bleedNPC.BleedStacks > 5)
            {
                bleedNPC.BleedStacks = 5;
            }

            bleedNPC.BleedTimer = 300;
        }
    }
}