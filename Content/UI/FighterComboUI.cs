using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Eternia.Content.Players;

namespace Eternia.Content.UI
{
    public class FighterComboUI : ModSystem
    {
        public override void PostDrawInterface(
            SpriteBatch spriteBatch)
        {
            Player player = Main.LocalPlayer;

            if (!EterniaUI.ShouldDrawPlayerUI(player))
            {
                return;
            }

            var fighterPlayer =
                player.GetModPlayer<FighterPlayer>();

            // =================================================
            // ONLY FIGHTER
            // =================================================

            if (!fighterPlayer.IsActiveFighter())
            {
                return;
            }

            // =================================================
            // NO COMBO
            // =================================================

            if (fighterPlayer.Combo <= 0)
            {
                return;
            }

            // =================================================
            // POSITION
            // =================================================

            Vector2 drawPosition =
                player.MountedCenter
                - Main.screenPosition;

            drawPosition.Y -= 70f;

            drawPosition =
                EterniaUI.ClampWorldAnchored(drawPosition, -110, -18, 220, 58);

            // =================================================
            // COMBO TEXT
            // =================================================

            string comboText =
                fighterPlayer.Combo + " COMBO";

            // =================================================
            // COLOR
            // =================================================

            Color comboColor = Color.White;

            if (fighterPlayer.Combo >= 10)
            {
                comboColor = Color.Orange;
            }

            if (fighterPlayer.Combo >= 20)
            {
                comboColor = Color.Red;
            }

            if (fighterPlayer.Combo >= 30)
            {
                comboColor = Color.Gold;
            }

            // =================================================
            // SCALE
            // =================================================

            float scale = 1f;

            if (fighterPlayer.Combo >= 10)
            {
                scale = 1.2f;
            }

            if (fighterPlayer.Combo >= 20)
            {
                scale = 1.4f;
            }

            if (fighterPlayer.Combo >= 30)
            {
                scale = 1.6f;
            }

            EterniaUI.DrawCenteredText(
                spriteBatch,
                comboText,
                new Rectangle(
                    (int)drawPosition.X - 110,
                    (int)drawPosition.Y - 18,
                    220,
                    36),
                comboColor,
                scale
            );

            // =================================================
            // BAR
            // =================================================

            float progress =
                fighterPlayer.ComboTimer
                / 120f;

            // =================================================
            // BAR SIZE
            // =================================================

            int barWidth = 80;

            int barHeight = 10;

            // =================================================
            // BAR POSITION
            // =================================================

            Rectangle backBar =
                new Rectangle(
                    (int)drawPosition.X - 40,
                    (int)drawPosition.Y + 30,
                    barWidth,
                    barHeight
                );

            EterniaUI.DrawProgressBar(
                spriteBatch,
                backBar,
                progress,
                comboColor,
                ""
            );
        }
    }
}
