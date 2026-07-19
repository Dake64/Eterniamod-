using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

using Eternia.Content.Players;

namespace Eternia.Content.UI
{
    // The Swordsman's exclusive Crimson Trail bar. Only drawn for an active
    // Swordsman; it lights up once there is enough resource to fire the technique.
    public class CrimsonTrailUI : ModSystem
    {
        public override void ModifyInterfaceLayers(
            List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex =
                layers.FindIndex(
                    layer => layer.Name.Equals("Vanilla: Mouse Text"));

            if (mouseTextIndex != -1)
            {
                layers.Insert(
                    mouseTextIndex,
                    new LegacyGameInterfaceLayer(
                        "Eternia: Crimson Trail UI",
                        DrawUI,
                        InterfaceScaleType.UI));
            }
        }

        private bool DrawUI()
        {
            Player player = Main.LocalPlayer;

            if (!EterniaUI.ShouldDrawWorldOverlay(player))
            {
                return true;
            }

            var swordsman =
                player.GetModPlayer<SwordsmanPlayer>();

            if (!swordsman.IsActiveSwordsman())
            {
                return true;
            }

            var crimson =
                player.GetModPlayer<CrimsonTrailPlayer>();

            // Read the REAL cost, so Merciless and the Hemorrhagic Frenzy keystone move the
            // fire line on the gauge instead of leaving it drawn at a number that no longer
            // applies.
            int cost =
                player.GetModPlayer<SwordsmanSkillPlayer>().EffectiveCost();

            bool ready = crimson.CrimsonTrail >= cost;

            // The bar never prints the skill key: players rebind it, and a printed key goes
            // stale without anyone noticing. The one thing worth surfacing is the genuinely
            // broken state -- tModLoader does not force a mod keybind's default onto an
            // existing controls profile, so it can sit unbound and the skill just "does
            // nothing". Only then does the bar say so.
            var keys = EterniaKeybinds.SkillKey?.GetAssignedKeys();
            string warning = keys == null || keys.Count == 0 ? "SET KEY" : null;

            // Blood-themed resource bar over the player: a clotted vessel of arterial blood
            // that ripples, trembles at the level line, and drips -- fitting the bleed fantasy.
            EterniaUI.DrawFloatingResourceBar(
                Main.spriteBatch,
                player,
                "CRIMSON",
                crimson.CrimsonTrail,
                CrimsonTrailPlayer.MaxCrimsonTrail,
                new Color(205, 35, 45),
                ready,
                warning,
                alwaysShow: true,
                bloodTheme: true,
                thresholdPercent:
                    cost / (float)CrimsonTrailPlayer.MaxCrimsonTrail);

            return true;
        }
    }
}
