using Eternia.Content.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Eternia.Content.UI
{
    public class ElementalistUI : UIState
    {
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            Player player =
                Main.LocalPlayer;

            if (!EterniaUI.ShouldDrawPlayerUI(player))
            {
                return;
            }

            var elementalist =
                player.GetModPlayer<ElementalistPlayer>();

            if (!elementalist.IsActiveElementalist())
            {
                return;
            }

            Color accent =
                GetElementColor(elementalist.CurrentElement);

            Rectangle panel =
                EterniaUI.GetBottomLeftPanel(306, 178, 20, 156);

            EterniaUI.DrawPanel(spriteBatch, panel, accent, 0.84f);

            EterniaUI.DrawText(
                spriteBatch,
                "Elementalist",
                new Vector2(panel.X + 14, panel.Y + 12),
                Color.White,
                0.68f);

            int y = panel.Y + 42;

            DrawElementPill(
                spriteBatch,
                new Rectangle(panel.X + 14, y, panel.Width - 28, 24),
                "Fire",
                elementalist.FireLevel,
                elementalist.FireAffinity,
                elementalist.CurrentElement == 0,
                Color.OrangeRed);

            y += 30;

            DrawElementPill(
                spriteBatch,
                new Rectangle(panel.X + 14, y, panel.Width - 28, 24),
                "Ice",
                elementalist.IceLevel,
                elementalist.IceAffinity,
                elementalist.CurrentElement == 1,
                Color.Cyan);

            y += 30;

            DrawElementPill(
                spriteBatch,
                new Rectangle(panel.X + 14, y, panel.Width - 28, 24),
                "Lightning",
                elementalist.LightningLevel,
                elementalist.LightningAffinity,
                elementalist.CurrentElement == 2,
                Color.Yellow);

            y += 36;

            float progress =
                (float)elementalist.ElementCharge /
                ElementalistPlayer.MaxElementCharge;

            bool ready =
                elementalist.ElementCharge >=
                ElementalistPlayer.MaxElementCharge;

            EterniaUI.DrawProgressBar(
                spriteBatch,
                new Rectangle(panel.X + 14, y, panel.Width - 28, 22),
                progress,
                ready ? Color.Gold : accent,
                ready
                    ? "Ultimate ready"
                    : $"Charge {elementalist.ElementCharge}/{ElementalistPlayer.MaxElementCharge}");
        }

        private static void DrawElementPill(
            SpriteBatch spriteBatch,
            Rectangle rect,
            string name,
            int level,
            int affinity,
            bool selected,
            Color color)
        {
            EterniaUI.DrawPill(
                spriteBatch,
                rect,
                $"{(selected ? "> " : "")}{name} Lv.{level}  Affinity {affinity}",
                selected ? color : Color.Gray,
                0.52f);
        }

        private static Color GetElementColor(int element)
        {
            return element switch
            {
                0 => Color.OrangeRed,
                1 => Color.Cyan,
                2 => Color.Yellow,
                _ => Color.White
            };
        }
    }
}
