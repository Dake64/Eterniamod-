using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Eternia.Content.Souls;

using Eternia.Content.NPCs;

namespace Eternia.Content.Players
{
    public class SwordsmanPlayer : ModPlayer
    {
        private const int MaxBleedStacks = 5;
        private const int BleedDurationTicks = 300;

        public override void OnHitNPCWithItem(
            Item item,
            NPC target,
            NPC.HitInfo hit,
            int damageDone)
        {
            if (!IsActiveSwordsman())
            {
                return;
            }

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

            if (bleedNPC.BleedStacks > MaxBleedStacks)
            {
                bleedNPC.BleedStacks = MaxBleedStacks;
            }

            bleedNPC.BleedTimer = HasActivePassive("Blood Flow")
                ? BleedDurationTicks + 120
                : BleedDurationTicks;
        }

        private bool HasActivePassive(string passiveName)
        {
            var soul =
                Player.GetModPlayer<EterniaPlayer>();

            return Player.GetModPlayer<EterniaStatsPlayer>()
                .HasActivePassive(
                    soul.ActiveSoul,
                    passiveName);
        }

        public bool IsActiveSwordsman()
        {
            var soul =
                Player.GetModPlayer<EterniaPlayer>();

            return soul.HasClassSoul &&
                soul.ActiveSoul == SoulId.Warrior &&
                Player.GetModPlayer<SubclassPlayer>().CurrentSubclass ==
                    "Swordsman";
        }
    }
}
