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

            if (!EterniaUI.ShouldDrawPlayerUI(player))
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

            bool ready =
                crimson.CrimsonTrail >= SwordsmanSkillPlayer.TechniqueCost;

            // Show the REAL bound key (not a hardcoded "Q"): tModLoader does not force a mod
            // keybind's default onto an existing controls profile, so it can sit unbound. When
            // it is, the bar says SET KEY -- telling the player exactly why the skill "does
            // nothing" instead of promising a key that was never assigned.
            var keys = EterniaKeybinds.SkillKey?.GetAssignedKeys();
            string keyLabel = keys != null && keys.Count > 0 ? keys[0] : "SET KEY";

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
                keyLabel + ": EXECUTE",
                alwaysShow: true,
                bloodTheme: true);

            return true;
        }
    }
}
