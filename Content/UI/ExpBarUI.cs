using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using Eternia.Content.Players;

namespace Eternia.Content.UI
{
    public class ExpBarUI : UIState
    {
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Player player = Main.LocalPlayer;

            var levelPlayer =
                player.GetModPlayer<EterniaLevelPlayer>();

            // =====================================================
            // PANEL
            // =====================================================

            Rectangle backPanel = new Rectangle(
                20,
                Main.screenHeight - 60,
                300,
                30
            );

            spriteBatch.Draw(
                TextureAssets.MagicPixel.Value,
                backPanel,
                Color.Black * 0.7f
            );

            // =====================================================
            // XP %
            // =====================================================

            float progress =
                (float)levelPlayer.currentExp /
                levelPlayer.expToNextLevel;

            Rectangle expBar = new Rectangle(
                20,
                Main.screenHeight - 60,
                (int)(300 * progress),
                30
            );

            spriteBatch.Draw(
                TextureAssets.MagicPixel.Value,
                expBar,
                Color.DeepSkyBlue
            );

            // =====================================================
            // BORDER
            // =====================================================

            DrawBorder(spriteBatch, backPanel);

            // =====================================================
            // TEXT
            // =====================================================

            string text =
                $"LVL {levelPlayer.level}  |  " +
                $"{levelPlayer.currentExp} / {levelPlayer.expToNextLevel} EXP";

            Utils.DrawBorderString(
                spriteBatch,
                text,
                new Vector2(30, Main.screenHeight - 58),
                Color.White
            );
        }

        // =====================================================
        // BORDER
        // =====================================================

        private void DrawBorder(SpriteBatch spriteBatch, Rectangle rect)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;

            spriteBatch.Draw(pixel,
                new Rectangle(rect.X, rect.Y, rect.Width, 2),
                Color.Black);

            spriteBatch.Draw(pixel,
                new Rectangle(rect.X, rect.Bottom, rect.Width, 2),
                Color.Black);

            spriteBatch.Draw(pixel,
                new Rectangle(rect.X, rect.Y, 2, rect.Height),
                Color.Black);

            spriteBatch.Draw(pixel,
                new Rectangle(rect.Right, rect.Y, 2, rect.Height + 2),
                Color.Black);
        }
    }
}