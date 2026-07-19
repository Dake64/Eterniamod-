using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

using Eternia.Content.Players;

namespace Eternia.Content.UI
{
    public class BerserkerUI : ModSystem
    {
        public override void ModifyInterfaceLayers(
            System.Collections.Generic.List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex =
                layers.FindIndex(
                    layer => layer.Name.Equals("Vanilla: Mouse Text")
                );

            if (mouseTextIndex != -1)
            {
                layers.Insert(
                    mouseTextIndex,
                    new LegacyGameInterfaceLayer(
                        "Eternia: Berserker UI",
                        DrawUI,
                        InterfaceScaleType.UI
                    )
                );
            }
        }

        private bool DrawUI()
        {
            Player player = Main.LocalPlayer;

            if (!EterniaUI.ShouldDrawWorldOverlay(player))
            {
                return true;
            }

            var berserker =
                player.GetModPlayer<BerserkerPlayer>();

            // =================================================
            // ONLY BERSERKER
            // =================================================

            if (!berserker.IsActiveBerserker())
            {
                return true;
            }

            SpriteBatch spriteBatch =
                Main.spriteBatch;

            // =================================================
            // PLAYER SCREEN POSITION
            // =================================================

            Vector2 drawPos =
                player.Top - Main.screenPosition;

            drawPos =
                EterniaUI.ClampWorldAnchored(drawPos, -70, -72, 184, 45);

            // =================================================
            // BAR POSITION
            // =================================================

            int barWidth = 140;

            int barHeight = 18;

            int x =
                (int)drawPos.X
                - (barWidth / 2);

            int y =
                (int)drawPos.Y
                - 70;

            Rectangle background =
                new Rectangle(
                    x,
                    y,
                    barWidth,
                    barHeight
                );

            float percent =
                berserker.Rage / 100f;

            Color barColor =
                Color.DarkRed;

            // =================================================
            // HIGH RAGE
            // =================================================

            if (berserker.Rage >= 70)
            {
                barColor = Color.Red;
            }

            // =================================================
            // OVERRAGE
            // =================================================

            if (berserker.Overrage)
            {
                barColor = Color.OrangeRed;
            }

            EterniaUI.DrawProgressBar(
                spriteBatch,
                background,
                percent,
                barColor,
                $"RAGE: {berserker.Rage}"
            );

            // =================================================
            // OVERRAGE TEXT
            // =================================================

            if (berserker.Overrage)
            {
                EterniaUI.DrawPill(
                    spriteBatch,
                    new Rectangle(x, y + 23, 84, 20),
                    "OVERRAGE",
                    Color.OrangeRed,
                    0.48f
                );
            }

            // =================================================
            // TIMER
            // =================================================

            float seconds =
                berserker.RageTimer / 60f;

            EterniaUI.DrawText(
                spriteBatch,
                $"{seconds:0.0}s",
                new Vector2(
                    x + barWidth + 10,
                    y - 2
                ),
                Color.LightGray,
                0.6f
            );

            return true;
        }
    }
}
