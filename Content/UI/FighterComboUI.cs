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

            if (player == null
                || !player.active)
            {
                return;
            }

            var subclassPlayer =
                player.GetModPlayer<SubclassPlayer>();

            // =================================================
            // ONLY FIGHTER
            // =================================================

            if (subclassPlayer.CurrentSubclass
                != "Fighter")
            {
                return;
            }

            var fighterPlayer =
                player.GetModPlayer<FighterPlayer>();

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

            // =================================================
            // DRAW TEXT
            // =================================================

            Utils.DrawBorderString(
                spriteBatch,
                comboText,
                drawPosition,
                comboColor,
                scale,
                0.5f,
                0.5f
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

            Rectangle frontBar =
                new Rectangle(
                    backBar.X,
                    backBar.Y,
                    (int)(barWidth * progress),
                    barHeight
                );

            // =================================================
            // DRAW BACK
            // =================================================

            spriteBatch.Draw(
                TextureAssets.MagicPixel.Value,
                backBar,
                Color.Black
            );

            // =================================================
            // DRAW FRONT
            // =================================================

            spriteBatch.Draw(
                TextureAssets.MagicPixel.Value,
                frontBar,
                comboColor
            );
        }
    }
}