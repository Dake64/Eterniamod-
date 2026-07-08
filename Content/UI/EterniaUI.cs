using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;

namespace Eternia.Content.UI
{
    public static class EterniaUI
    {
        public enum MajorPanel
        {
            Soul,
            Stats,
            Passive
        }

        public static readonly Color PanelBackground =
            new Color(8, 10, 14);

        public static readonly Color PanelSurface =
            new Color(18, 23, 31);

        public static readonly Color PanelSurfaceAlt =
            new Color(27, 34, 44);

        public static readonly Color Border =
            new Color(82, 95, 119);

        public static readonly Color MutedText =
            new Color(172, 182, 198);

        public static Rectangle GetCenteredPanel(
            int preferredWidth,
            int preferredHeight,
            int margin = 28)
        {
            int availableWidth =
                Math.Max(1, Main.screenWidth - margin * 2);

            int availableHeight =
                Math.Max(1, Main.screenHeight - margin * 2);

            int minWidth =
                Math.Min(320, availableWidth);

            int minHeight =
                Math.Min(220, availableHeight);

            int width =
                Math.Clamp(preferredWidth, minWidth, availableWidth);

            int height =
                Math.Clamp(preferredHeight, minHeight, availableHeight);

            return new Rectangle(
                (Main.screenWidth - width) / 2,
                (Main.screenHeight - height) / 2,
                width,
                height);
        }

        public static Rectangle GetBottomLeftPanel(
            int width,
            int height,
            int marginX,
            int marginBottom)
        {
            int availableWidth =
                Math.Max(1, Main.screenWidth - marginX * 2);

            int minWidth =
                Math.Min(260, availableWidth);

            int panelWidth =
                Math.Clamp(width, minWidth, availableWidth);

            int maxHeight =
                Math.Max(1, Main.screenHeight - 24);

            int panelHeight =
                Math.Min(height, maxHeight);

            int x =
                Math.Clamp(
                    marginX,
                    0,
                    Math.Max(0, Main.screenWidth - panelWidth));

            int preferredY =
                Main.screenHeight - marginBottom - panelHeight;

            int screenMaxY =
                Math.Max(12, Main.screenHeight - panelHeight - 12);

            int y =
                Math.Clamp(preferredY, 12, screenMaxY);

            return new Rectangle(
                x,
                y,
                panelWidth,
                panelHeight);
        }

        public static Rectangle GetTopCenterPanel(
            int width,
            int height,
            int marginTop)
        {
            int availableWidth =
                Math.Max(1, Main.screenWidth - 24);

            int minWidth =
                Math.Min(200, availableWidth);

            int panelWidth =
                Math.Clamp(width, minWidth, availableWidth);

            int panelHeight =
                Math.Min(height, Math.Max(1, Main.screenHeight - 24));

            int x =
                (Main.screenWidth - panelWidth) / 2;

            int y =
                Math.Clamp(
                    marginTop,
                    8,
                    Math.Max(8, Main.screenHeight - panelHeight - 8));

            return new Rectangle(x, y, panelWidth, panelHeight);
        }

        public static Rectangle ClampToScreen(
            Rectangle rect,
            int margin = 12)
        {
            int maxWidth =
                Math.Max(1, Main.screenWidth - margin * 2);

            int maxHeight =
                Math.Max(1, Main.screenHeight - margin * 2);

            int width =
                Math.Min(
                    rect.Width,
                    maxWidth);

            int height =
                Math.Min(
                    rect.Height,
                    maxHeight);

            int screenMarginX =
                Math.Min(
                    margin,
                    Math.Max(0, (Main.screenWidth - width) / 2));

            int screenMarginY =
                Math.Min(
                    margin,
                    Math.Max(0, (Main.screenHeight - height) / 2));

            int x =
                Math.Clamp(
                    rect.X,
                    screenMarginX,
                    Math.Max(screenMarginX, Main.screenWidth - width - screenMarginX));

            int y =
                Math.Clamp(
                    rect.Y,
                    screenMarginY,
                    Math.Max(screenMarginY, Main.screenHeight - height - screenMarginY));

            return new Rectangle(x, y, width, height);
        }

        // Shifts a player-anchored overlay so its full bounding box stays on
        // screen. `anchor` is the overlay draw position (player.Top - screen +
        // offset); offsetLeft/offsetTop/width/height describe the overlay content
        // bounds relative to that anchor (pills above the bar use a negative top).
        public static Vector2 ClampWorldAnchored(
            Vector2 anchor,
            int offsetLeft,
            int offsetTop,
            int width,
            int height,
            int margin = 6)
        {
            Rectangle bounds =
                new Rectangle(
                    (int)anchor.X + offsetLeft,
                    (int)anchor.Y + offsetTop,
                    Math.Max(1, width),
                    Math.Max(1, height));

            Rectangle clamped =
                ClampToScreen(bounds, margin);

            return new Vector2(
                anchor.X + (clamped.X - bounds.X),
                anchor.Y + (clamped.Y - bounds.Y));
        }

        public static bool ShouldDrawPlayerUI(Player player)
        {
            return !Main.gameMenu &&
                player != null &&
                player.active &&
                !player.dead;
        }

        public static void CloseMajorPanelsExcept(
            MajorPanel panel)
        {
            if (panel != MajorPanel.Soul)
            {
                SoulUISystem.CloseSoulPanel();
            }

            if (panel != MajorPanel.Stats)
            {
                StatsUI.Visible = false;
            }

            if (panel != MajorPanel.Passive)
            {
                PassiveUI.Visible = false;
            }
        }

        public static void DrawPanel(
            SpriteBatch spriteBatch,
            Rectangle rect,
            Color accent,
            float opacity = 0.92f)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;

            spriteBatch.Draw(
                pixel,
                new Rectangle(rect.X + 6, rect.Y + 7, rect.Width, rect.Height),
                Color.Black * 0.42f);

            spriteBatch.Draw(
                pixel,
                rect,
                PanelBackground * opacity);

            spriteBatch.Draw(
                pixel,
                new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, 52),
                PanelSurface * 0.98f);

            spriteBatch.Draw(
                pixel,
                new Rectangle(rect.X, rect.Y, 4, rect.Height),
                accent * 0.92f);

            spriteBatch.Draw(
                pixel,
                new Rectangle(rect.X, rect.Y, rect.Width, 2),
                accent * 0.74f);

            DrawBorder(spriteBatch, rect, Border * 0.82f);
        }

        public static void DrawBorder(
            SpriteBatch spriteBatch,
            Rectangle rect,
            Color color,
            int thickness = 1)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;

            spriteBatch.Draw(
                pixel,
                new Rectangle(rect.X, rect.Y, rect.Width, thickness),
                color);

            spriteBatch.Draw(
                pixel,
                new Rectangle(rect.X, rect.Bottom - thickness, rect.Width, thickness),
                color);

            spriteBatch.Draw(
                pixel,
                new Rectangle(rect.X, rect.Y, thickness, rect.Height),
                color);

            spriteBatch.Draw(
                pixel,
                new Rectangle(rect.Right - thickness, rect.Y, thickness, rect.Height),
                color);
        }

        public static void DrawHeader(
            SpriteBatch spriteBatch,
            Rectangle panel,
            string title,
            string subtitle,
            Color accent)
        {
            bool showBrandPill =
                panel.Width >= 208;

            int brandWidth =
                showBrandPill
                    ? Math.Clamp(panel.Width / 4, 58, 92)
                    : 0;

            int brandReservedWidth =
                showBrandPill
                    ? brandWidth + 52
                    : 0;

            int textMaxWidth =
                Math.Max(1, panel.Width - 44 - brandReservedWidth);

            DrawTrimmedText(
                spriteBatch,
                title,
                new Vector2(panel.X + 22, panel.Y + 14),
                textMaxWidth,
                Color.White,
                0.92f);

            if (!string.IsNullOrEmpty(subtitle))
            {
                DrawTrimmedText(
                    spriteBatch,
                    subtitle,
                    new Vector2(panel.X + 24, panel.Y + 36),
                    textMaxWidth - 2,
                    MutedText,
                    0.58f);
            }

            if (showBrandPill)
            {
                DrawPill(
                    spriteBatch,
                    new Rectangle(panel.Right - brandWidth - 26, panel.Y + 14, brandWidth, 24),
                    "ETERNIA",
                    accent,
                    0.56f);
            }
        }

        public static bool DrawButton(
            SpriteBatch spriteBatch,
            Rectangle rect,
            string label,
            Color accent,
            bool enabled = true)
        {
            bool hover =
                rect.Contains(Main.MouseScreen.ToPoint());

            if (hover)
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            Color fill =
                enabled
                    ? (hover ? accent : accent * 0.78f)
                    : new Color(58, 63, 72);

            Texture2D pixel = TextureAssets.MagicPixel.Value;

            spriteBatch.Draw(pixel, rect, fill);
            DrawBorder(spriteBatch, rect, enabled ? Color.White * 0.22f : Border * 0.45f);

            DrawCenteredText(
                spriteBatch,
                label,
                rect,
                enabled ? Color.White : MutedText,
                0.56f);

            bool clicked =
                enabled &&
                hover &&
                Main.mouseLeft &&
                Main.mouseLeftRelease;

            if (clicked)
            {
                Main.mouseLeftRelease = false;
            }

            return clicked;
        }

        public static bool DrawCloseButton(
            SpriteBatch spriteBatch,
            Rectangle panel,
            Color accent)
        {
            Rectangle closeRect =
                new Rectangle(
                    panel.Right - 44,
                    panel.Y + 44,
                    26,
                    22);

            Color closeAccent =
                Color.Lerp(accent, Color.IndianRed, 0.55f);

            return DrawButton(
                spriteBatch,
                closeRect,
                "X",
                closeAccent);
        }

        public static void DrawPill(
            SpriteBatch spriteBatch,
            Rectangle rect,
            string label,
            Color accent,
            float textScale = 0.56f)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;

            spriteBatch.Draw(
                pixel,
                rect,
                PanelSurfaceAlt * 0.9f);

            spriteBatch.Draw(
                pixel,
                new Rectangle(rect.X, rect.Y, 3, rect.Height),
                accent);

            DrawBorder(spriteBatch, rect, accent * 0.38f);

            DrawTrimmedText(
                spriteBatch,
                label,
                new Vector2(rect.X + 8, rect.Y + 5),
                rect.Width - 14,
                Color.White,
                textScale);
        }

        public static void DrawProgressBar(
            SpriteBatch spriteBatch,
            Rectangle rect,
            float progress,
            Color accent,
            string label)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            float clamped = Math.Clamp(progress, 0f, 1f);

            spriteBatch.Draw(pixel, rect, PanelSurfaceAlt * 0.95f);

            if (clamped > 0f)
            {
                spriteBatch.Draw(
                    pixel,
                    new Rectangle(
                        rect.X,
                        rect.Y,
                        Math.Max(2, (int)(rect.Width * clamped)),
                        rect.Height),
                    accent * 0.88f);
            }

            DrawBorder(spriteBatch, rect, Border * 0.7f);
            DrawCenteredText(spriteBatch, label, rect, Color.White, 0.56f);
        }

        public static void DrawConnector(
            SpriteBatch spriteBatch,
            Point start,
            Point end,
            Color color,
            int thickness = 3)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            int half = Math.Max(1, thickness / 2);
            int midY = (start.Y + end.Y) / 2;

            DrawRectLine(
                spriteBatch,
                pixel,
                new Point(start.X, start.Y),
                new Point(start.X, midY),
                color,
                thickness,
                half);

            DrawRectLine(
                spriteBatch,
                pixel,
                new Point(start.X, midY),
                new Point(end.X, midY),
                color,
                thickness,
                half);

            DrawRectLine(
                spriteBatch,
                pixel,
                new Point(end.X, midY),
                new Point(end.X, end.Y),
                color,
                thickness,
                half);
        }

        public static void DrawTooltip(
            SpriteBatch spriteBatch,
            string title,
            IEnumerable<string> lines,
            Color accent)
        {
            const int width = 330;
            const float scale = 0.58f;
            List<string> wrapped = new List<string>();

            foreach (string line in lines)
            {
                wrapped.AddRange(WrapText(line, width - 28, scale));
            }

            int desiredHeight =
                52 + wrapped.Count * 18;

            int maxHeight =
                Math.Max(88, Main.screenHeight - 32);

            int height =
                Math.Min(
                    maxHeight,
                    Math.Max(88, desiredHeight));

            Rectangle panel =
                ClampToScreen(
                    new Rectangle(
                        (int)Main.MouseScreen.X + 18,
                        (int)Main.MouseScreen.Y + 18,
                        width,
                        Math.Max(88, height)),
                    16);

            DrawPanel(spriteBatch, panel, accent, 0.96f);
            DrawText(spriteBatch, title, new Vector2(panel.X + 16, panel.Y + 14), Color.White, 0.68f);

            int y = panel.Y + 42;
            foreach (string line in wrapped)
            {
                if (y + 18 > panel.Bottom - 10)
                {
                    break;
                }

                DrawText(spriteBatch, line, new Vector2(panel.X + 16, y), MutedText, scale);
                y += 18;
            }
        }

        private static string pendingTooltipTitle;
        private static IEnumerable<string> pendingTooltipLines;
        private static Color pendingTooltipAccent;

        // Queue a tooltip to be drawn at the very end of the UI pass (on top of
        // everything), so later-drawn rows/panels don't cover it. Pair with
        // DrawQueuedTooltip called last in the panel's draw.
        public static void QueueTooltip(
            string title,
            IEnumerable<string> lines,
            Color accent)
        {
            pendingTooltipTitle = title;
            pendingTooltipLines = lines;
            pendingTooltipAccent = accent;
        }

        public static void DrawQueuedTooltip(SpriteBatch spriteBatch)
        {
            if (pendingTooltipTitle == null)
            {
                return;
            }

            DrawTooltip(
                spriteBatch,
                pendingTooltipTitle,
                pendingTooltipLines,
                pendingTooltipAccent);

            pendingTooltipTitle = null;
            pendingTooltipLines = null;
        }

        public static void DrawDivider(
            SpriteBatch spriteBatch,
            int x,
            int y,
            int width,
            Color accent)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;

            spriteBatch.Draw(
                pixel,
                new Rectangle(x, y, width, 1),
                Border * 0.45f);

            spriteBatch.Draw(
                pixel,
                new Rectangle(x, y, Math.Max(24, width / 5), 1),
                accent * 0.82f);
        }

        public static void DrawText(
            SpriteBatch spriteBatch,
            string text,
            Vector2 position,
            Color color,
            float scale = 0.72f)
        {
            Utils.DrawBorderString(
                spriteBatch,
                text,
                position,
                color,
                scale);
        }

        public static void DrawWrappedText(
            SpriteBatch spriteBatch,
            string text,
            Rectangle bounds,
            Color color,
            float scale = 0.62f,
            int lineHeight = 18)
        {
            int y = bounds.Y;

            foreach (string line in WrapText(text, bounds.Width, scale))
            {
                if (y + lineHeight > bounds.Bottom)
                {
                    return;
                }

                DrawTrimmedText(
                    spriteBatch,
                    line,
                    new Vector2(bounds.X, y),
                    bounds.Width,
                    color,
                    scale);

                y += lineHeight;
            }
        }

        public static void DrawCenteredText(
            SpriteBatch spriteBatch,
            string text,
            Rectangle rect,
            Color color,
            float scale = 0.62f)
        {
            string fitted =
                FitText(
                    text,
                    Math.Max(4, rect.Width - 8),
                    scale);

            Vector2 size =
                FontAssets.MouseText.Value.MeasureString(fitted) * scale;

            DrawText(
                spriteBatch,
                fitted,
                new Vector2(
                    rect.X + (rect.Width - size.X) / 2f,
                    rect.Y + (rect.Height - size.Y) / 2f - 1f),
                color,
                scale);
        }

        public static void DrawTrimmedText(
            SpriteBatch spriteBatch,
            string text,
            Vector2 position,
            int maxWidth,
            Color color,
            float scale = 0.62f)
        {
            if (maxWidth <= 4 ||
                string.IsNullOrEmpty(text))
            {
                return;
            }

            string output = text;
            float width =
                FontAssets.MouseText.Value.MeasureString(output).X * scale;

            while (output.Length > 3 &&
                width > maxWidth)
            {
                output = output.Substring(0, output.Length - 4) + "...";
                width = FontAssets.MouseText.Value.MeasureString(output).X * scale;
            }

            if (width > maxWidth)
            {
                return;
            }

            DrawText(spriteBatch, output, position, color, scale);
        }

        private static string FitText(
            string text,
            int maxWidth,
            float scale)
        {
            if (maxWidth <= 4 ||
                string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            string output = text;
            float width =
                FontAssets.MouseText.Value.MeasureString(output).X * scale;

            while (output.Length > 3 &&
                width > maxWidth)
            {
                output = output.Substring(0, output.Length - 4) + "...";
                width = FontAssets.MouseText.Value.MeasureString(output).X * scale;
            }

            return width <= maxWidth
                ? output
                : string.Empty;
        }

        public static List<string> WrapText(
            string text,
            int maxWidth,
            float scale)
        {
            List<string> lines = new List<string>();

            if (string.IsNullOrWhiteSpace(text))
            {
                return lines;
            }

            if (maxWidth <= 4)
            {
                return lines;
            }

            string[] words =
                text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            string current = "";

            foreach (string word in words)
            {
                float wordWidth =
                    FontAssets.MouseText.Value.MeasureString(word).X * scale;

                if (wordWidth > maxWidth)
                {
                    if (!string.IsNullOrEmpty(current))
                    {
                        lines.Add(current);
                        current = string.Empty;
                    }

                    string fittedWord =
                        FitText(word, maxWidth, scale);

                    if (!string.IsNullOrEmpty(fittedWord))
                    {
                        lines.Add(fittedWord);
                    }

                    continue;
                }

                string candidate =
                    string.IsNullOrEmpty(current)
                        ? word
                        : current + " " + word;

                float width =
                    FontAssets.MouseText.Value.MeasureString(candidate).X * scale;

                if (width <= maxWidth ||
                    string.IsNullOrEmpty(current))
                {
                    current = candidate;
                    continue;
                }

                lines.Add(current);
                current = word;
            }

            if (!string.IsNullOrEmpty(current))
            {
                lines.Add(current);
            }

            return lines;
        }

        private static void DrawRectLine(
            SpriteBatch spriteBatch,
            Texture2D pixel,
            Point start,
            Point end,
            Color color,
            int thickness,
            int half)
        {
            if (start.X == end.X)
            {
                int y = Math.Min(start.Y, end.Y);
                int height = Math.Abs(end.Y - start.Y);

                spriteBatch.Draw(
                    pixel,
                    new Rectangle(start.X - half, y, thickness, Math.Max(thickness, height)),
                    color);
            }
            else
            {
                int x = Math.Min(start.X, end.X);
                int width = Math.Abs(end.X - start.X);

                spriteBatch.Draw(
                    pixel,
                    new Rectangle(x, start.Y - half, Math.Max(thickness, width), thickness),
                    color);
            }
        }
    }
}
