using System.Collections.Generic;
using Eternia.Content.Progression;
using Eternia.Content.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Eternia.Content.Items.Weapons.Promotion
{
    public static class SubclassLockHelper
    {
        public static bool PlayerHasSubclass(
            Player player,
            string requiredSubclass)
        {
            var subclass =
                player.GetModPlayer<SubclassPlayer>();

            var soul =
                player.GetModPlayer<EterniaPlayer>();

            return soul.HasClassSoul &&
                ClassPromotionRules.IsPromotionForSoul(
                    soul.ActiveSoul,
                    requiredSubclass) &&
                subclass.CurrentSubclass == requiredSubclass;
        }

        public static void AddTooltip(
            Mod mod,
            List<TooltipLine> tooltips,
            string requiredSubclass)
        {
            var line = new TooltipLine(
                mod,
                "RequiredSubclass",
                $"Requires promotion: {requiredSubclass}"
            )
            {
                OverrideColor = Color.MediumPurple
            };

            tooltips.Add(line);
        }
    }
}
