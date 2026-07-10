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

            // Shared polished, fading resource bar over the player.
            EterniaUI.DrawFloatingResourceBar(
                Main.spriteBatch,
                player,
                "CRIMSON",
                crimson.CrimsonTrail,
                CrimsonTrailPlayer.MaxCrimsonTrail,
                new Color(205, 35, 45),
                ready,
                "Q: EXECUTE");

            return true;
        }
    }
}
