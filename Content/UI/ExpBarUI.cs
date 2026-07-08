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

            var stats =
                player.GetModPlayer<EterniaStatsPlayer>();

            int statPoints = stats.StatPoints;
            int passivePoints = levelPlayer.passivePoints;
            bool hasPoints = statPoints > 0 || passivePoints > 0;

            Color accent =
                Color.DeepSkyBlue;

            Rectangle panel =
                EterniaUI.GetTopRowPanel(
                    368,
                    hasPoints ? 82 : 56,
                    14,
                    750,
                    0);

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

            if (hasPoints)
            {
                string hint;

                if (statPoints > 0 && passivePoints > 0)
                {
                    hint = $"{statPoints} Stat & {passivePoints} Passive points to spend";
                }
                else if (statPoints > 0)
                {
                    hint = $"{statPoints} Stat point{(statPoints == 1 ? "" : "s")} to spend";
                }
                else
                {
                    hint = $"{passivePoints} Passive point{(passivePoints == 1 ? "" : "s")} to spend";
                }

                float pulse =
                    0.6f + 0.4f * (float)System.Math.Sin(
                        Main.GlobalTimeWrappedHourly * 5f);

                Color hintColor =
                    Color.Lerp(Color.Gold, Color.White, pulse);

                EterniaUI.DrawCenteredText(
                    spriteBatch,
                    hint,
                    new Rectangle(panel.X, panel.Y + 48, panel.Width, 24),
                    hintColor,
                    0.56f);
            }
        }
    }
}
