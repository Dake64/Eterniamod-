using Eternia.Content.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Eternia.Content.UI
{
    public class ExpBarUI : UIState
    {
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Player player = Main.LocalPlayer;

            if (!EterniaUI.ShouldDrawPlayerUI(player))
            {
                return;
            }

            var levelPlayer =
                player.GetModPlayer<EterniaLevelPlayer>();

            Rectangle panel =
                EterniaUI.GetTopRowPanel(
                    368,
                    56,
                    14,
                    750,
                    0);

            Color accent =
                Color.DeepSkyBlue;

            EterniaUI.DrawPanel(spriteBatch, panel, accent, 0.82f);

            float progress =
                (float)levelPlayer.currentExp /
                System.Math.Max(1, levelPlayer.expToNextLevel);

            string label =
                $"Level {levelPlayer.level}   {levelPlayer.currentExp}/{levelPlayer.expToNextLevel} EXP";

            EterniaUI.DrawProgressBar(
                spriteBatch,
                new Rectangle(panel.X + 12, panel.Y + 17, panel.Width - 24, 22),
                progress,
                accent,
                label);
        }
    }
}
