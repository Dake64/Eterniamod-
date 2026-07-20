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
            Passive,
            Bosses
        }

        // ETERNIA'S IDENTITY LIVES HERE.
        //
        // Every panel, border and label in the mod resolves to these five, so the whole UI
        // shares one violet chrome instead of the neutral blue-grey it grew into by accident.
        //
        // What is deliberately NOT unified: colours that carry MEANING. Enemy rarity, each
        // Soul's colour and the affinity branches stay as they are, because gold reads as
        // Legendary and red reads as Bleed. Tinting those violet would trade information the
        // player reads at a glance for decoration -- the same mistake as a passive node whose
        // colour said nothing. Identity belongs to the chrome; the content keeps its meaning.
        //
        // The luminance ladder matches the old palette step for step, so contrast is preserved
        // and nothing that was readable before becomes unreadable now.
        public static readonly Color PanelBackground =
            new Color(12, 8, 20);

        public static readonly Color PanelSurface =
            new Color(24, 18, 38);

        public static readonly Color PanelSurfaceAlt =
            new Color(36, 28, 54);

        public static readonly Color Border =
            new Color(104, 84, 148);

        public static readonly Color MutedText =
            new Color(186, 176, 208);

        // The mod's signature violet, for chrome that needs an accent of its own rather than
        // borrowing one from whatever content it happens to be framing.
        public static readonly Color Brand =
            new Color(150, 110, 230);

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

        // Places a panel inside a horizontally-centered row of total width rowWidth,
        // at offsetInRow from the row's left edge. Used to sit panels side by side.
        public static Rectangle GetTopRowPanel(
            int panelWidth,
            int height,
            int marginTop,
            int rowWidth,
            int offsetInRow)
        {
            int rowX =
                (Main.screenWidth - rowWidth) / 2;

            int panelHeight =
                Math.Min(height, Math.Max(1, Main.screenHeight - 24));

            int x =
                Math.Clamp(
                    rowX + offsetInRow,
                    6,
                    Math.Max(6, Main.screenWidth - panelWidth - 6));

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

        // Any of the mod's full-size panels being open. They cover the middle of the screen,
        // which is exactly where the floating over-the-player readouts live.
        public static bool AnyMajorPanelOpen()
        {
            return SoulUISystem.Visible ||
                StatsUI.Visible ||
                PassiveUI.Visible ||
                BossLogUI.Visible;
        }

        // "The player exists and is in the world." The major panels gate themselves on this,
        // so it must NOT consider whether a panel is open -- doing that made every panel hide
        // itself the instant it was opened.
        public static bool ShouldDrawPlayerUI(Player player)
        {
            return !Main.gameMenu &&
                player != null &&
                player.active &&
                !player.dead;
        }

        // Gate for readouts drawn over the player IN THE WORLD (resource bars and the like).
        // Those sit in the middle of the screen, so an open panel has to hide them: they were
        // being drawn on top of the Boss Codex, which drawing order alone never prevented.
        public static bool ShouldDrawWorldOverlay(Player player)
        {
            return ShouldDrawPlayerUI(player) && !AnyMajorPanelOpen();
        }

        public static void CloseAllMajorPanels()
        {
            SoulUISystem.Visible = false;
            StatsUI.Visible = false;
            PassiveUI.Visible = false;
            BossLogUI.Visible = false;
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

            if (panel != MajorPanel.Bosses)
            {
                BossLogUI.Visible = false;
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

        // Shared floating resource bar that hovers over the player. It fades in/out
        // with the value (so it never sits permanently at 0), draws a polished fill
        // with a bright leading edge + gloss, pulses a glow when nearly full, and can
        // show an optional warning in place of the value. Only one class/subclass resource is
        // ever active at a time, so one shared fade alpha is enough for all callers.
        private static float resourceBarAlpha;

        public static void DrawFloatingResourceBar(
            SpriteBatch spriteBatch,
            Player player,
            string label,
            int value,
            int max,
            Color color,
            bool ready = false,
            string warning = null,
            bool alwaysShow = false,
            bool bloodTheme = false,
            float thresholdPercent = -1f)
        {
            // alwaysShow keeps the gauge on screen even at 0, so a subclass's mechanic is
            // DISCOVERABLE -- the player sees the empty gauge and learns they need to fill it.
            float target = (value > 0 || alwaysShow) ? 1f : 0f;
            resourceBarAlpha += (target - resourceBarAlpha) * 0.12f;

            if (resourceBarAlpha < 0.02f)
            {
                return;
            }

            Texture2D pixel = TextureAssets.MagicPixel.Value;

            float a = resourceBarAlpha;
            int clamped = value < 0 ? 0 : (value > max ? max : value);
            float percent = max <= 0 ? 0f : clamped / (float)max;
            bool full = clamped >= max;
            bool hot = ready || full;

            float time = Main.GlobalTimeWrappedHourly;
            float pulse =
                0.5f + 0.5f * (float)System.Math.Sin(time * 5f);

            Color bright = Color.Lerp(color, Color.White, 0.4f);

            // Gauge geometry. Default is a short SEGMENTED energy meter; the blood theme uses a
            // slightly taller vessel so the liquid, its rippling surface and the drips can read.
            const int gaugeW = 78;
            const int segs = 13;
            const int segGap = 1;
            int segW = (gaugeW - (segs - 1) * segGap) / segs;
            int segTrueW = segs * segW + (segs - 1) * segGap;

            int trueW = bloodTheme ? gaugeW : segTrueW;
            int gaugeH = bloodTheme ? 9 : 6;

            // Compact side text: always YOUR resource number. It deliberately never shows the
            // skill key -- players rebind, and a printed key silently goes stale. "Ready" is
            // already unmistakable from the pulsing fire line, the glow and the hot colour.
            // The only exception is a genuine broken state passed in as a warning.
            string sideText =
                warning ?? (full ? "MAX" : clamped.ToString());

            float labelScale = 0.46f;
            float lw = FontAssets.MouseText.Value.MeasureString(label).X * labelScale;
            float sw = FontAssets.MouseText.Value.MeasureString(sideText).X * labelScale;

            int rowW = (int)lw + 6 + trueW + 6 + (int)sw;

            Vector2 anchor = player.Top - Main.screenPosition + new Vector2(0f, -52f);
            anchor = ClampWorldAnchored(anchor, -rowW / 2, -8, rowW, 20);

            int startX = (int)anchor.X - rowW / 2;
            int midY = (int)anchor.Y;
            int gaugeX = startX + (int)lw + 6;
            int gaugeY = midY - gaugeH / 2;

            // Label (left). Muted grey by default; the blood theme bleeds it toward the
            // resource colour so the whole readout feels of a piece.
            Color labelColor = bloodTheme
                ? Color.Lerp(MutedText, color, 0.55f)
                : MutedText;
            DrawText(
                spriteBatch, label,
                new Vector2(startX, midY - 6),
                labelColor * (0.9f * a), labelScale);

            // Soft glow behind the gauge when it's hot.
            if (hot)
            {
                spriteBatch.Draw(
                    pixel,
                    new Rectangle(gaugeX - 3, gaugeY - 3, trueW + 6, gaugeH + 6),
                    color * (0.18f * pulse * a));
            }

            if (bloodTheme)
            {
                DrawBloodGauge(
                    spriteBatch, pixel, gaugeX, gaugeY, trueW, gaugeH,
                    percent, color, bright, hot, pulse, time, thresholdPercent, a);
            }
            else
            {
                int fillW = (int)(trueW * percent);

                for (int i = 0; i < segs; i++)
                {
                    int sx = gaugeX + i * (segW + segGap);
                    int segStart = i * (segW + segGap);

                    // Empty cell.
                    spriteBatch.Draw(pixel, new Rectangle(sx, gaugeY, segW, gaugeH), PanelSurface * (0.9f * a));

                    // Filled portion of this cell (partial fill stays smooth across the notches).
                    int inSeg = System.Math.Clamp(fillW - segStart, 0, segW);

                    if (inSeg > 0)
                    {
                        Color cell = hot ? Color.Lerp(color, bright, pulse) : color;
                        spriteBatch.Draw(pixel, new Rectangle(sx, gaugeY, inSeg, gaugeH), cell * (0.95f * a));
                        // top gloss line
                        spriteBatch.Draw(pixel, new Rectangle(sx, gaugeY, inSeg, 1), Color.White * (0.22f * a));
                    }
                }

                // Bright leading edge where the fill ends.
                if (fillW > 0 && fillW < trueW)
                {
                    spriteBatch.Draw(pixel, new Rectangle(gaugeX + fillW - 1, gaugeY - 1, 2, gaugeH + 2), bright * (0.9f * a));
                }

                DrawBorder(
                    spriteBatch,
                    new Rectangle(gaugeX - 1, gaugeY - 1, trueW + 2, gaugeH + 2),
                    (hot ? Color.Lerp(color, Color.White, 0.35f) * (0.5f + 0.5f * pulse) : color * 0.5f) * a);
            }

            // Side text (right): the value, or a pulsing ready key.
            DrawText(
                spriteBatch, sideText,
                new Vector2(gaugeX + trueW + 6, midY - 6),
                (ready ? bright * (0.6f + 0.4f * pulse) : Color.Lerp(color, Color.White, 0.3f)) * a,
                labelScale);
        }

        // A visceral, blood-themed variant of the floating gauge -- built for the Swordsman's
        // Crimson Trail. A clotted vessel holds arterial liquid with a rippling wet surface, a
        // trembling level edge, and drips that bleed from the fill line. All motion is
        // time-driven (no per-frame RNG) so it stays smooth and save/net-agnostic.
        private static void DrawBloodGauge(
            SpriteBatch spriteBatch,
            Texture2D pixel,
            int x, int y, int w, int h,
            float percent,
            Color color,
            Color bright,
            bool hot,
            float pulse,
            float time,
            float threshold,
            float a)
        {
            // Clotted vessel behind the blood.
            Color vessel = new Color(34, 6, 9);
            spriteBatch.Draw(pixel, new Rectangle(x, y, w, h), vessel * (0.92f * a));

            int fillW = (int)System.Math.Round(w * percent);
            fillW = System.Math.Clamp(fillW, 0, w);

            if (fillW > 0)
            {
                // Vertical blood gradient: dark settled clot at the bottom, arterial body,
                // a brighter band just under the surface.
                Color bodyLo = Color.Lerp(color, new Color(58, 0, 6), 0.5f);
                Color bodyHi = Color.Lerp(color, new Color(150, 20, 24), 0.35f);

                spriteBatch.Draw(pixel, new Rectangle(x, y, fillW, h), bodyLo * (0.96f * a));
                spriteBatch.Draw(pixel, new Rectangle(x, y, fillW, System.Math.Max(1, h / 2)), bodyHi * (0.5f * a));
                spriteBatch.Draw(pixel, new Rectangle(x, y + h - 2, fillW, 2), new Color(40, 3, 6) * (0.7f * a));

                // Rippling wet surface line across the top of the fill.
                Color menisc = Color.Lerp(color, Color.White, hot ? 0.55f : 0.32f);
                for (int cx = 0; cx < fillW; cx++)
                {
                    float s = (float)System.Math.Sin(cx * 0.5f + time * 3.2f);
                    int yTop = y + (s > 0.25f ? 0 : 1);
                    spriteBatch.Draw(
                        pixel, new Rectangle(x + cx, yTop, 1, 1),
                        menisc * ((0.45f + 0.4f * (0.5f + 0.5f * s)) * a));
                }

                // Trembling leading edge -- the level where the blood currently sits.
                if (fillW < w)
                {
                    int edgeX = x + fillW - 1 + (int)System.Math.Round(System.Math.Sin(time * 4.5f));
                    spriteBatch.Draw(pixel, new Rectangle(edgeX, y, 2, h), bright * ((hot ? 0.9f : 0.7f) * a));
                }
            }

            // Drips bleeding from the fill line. Each has its own phase so they fall out of
            // sync; a drip only shows if the blood actually reaches its column.
            int dripCount = hot ? 4 : 3;
            float fall = h + 9f;
            for (int i = 0; i < dripCount; i++)
            {
                float frac = (i + 0.5f) / dripCount;
                if (frac > percent)
                {
                    continue;
                }

                int dripX = x + (int)(frac * w);
                float phaseRaw = time * (hot ? 1.1f : 0.7f) + i * 0.41f;
                float phase = phaseRaw - (float)System.Math.Floor(phaseRaw);

                int dripY = y + h - 1 + (int)(phase * fall);
                float dropA = (1f - phase) * a;

                Color drop = Color.Lerp(color, new Color(70, 0, 6), 0.4f);
                spriteBatch.Draw(pixel, new Rectangle(dripX, dripY, 1, 2), drop * (0.85f * dropA));
                spriteBatch.Draw(pixel, new Rectangle(dripX, dripY + 2, 1, 1), drop * (0.5f * dropA));
            }

            // The FIRE LINE etched on the vessel: the level the blood must reach before the
            // technique can be spent. Faint while out of reach, a bright wet pulse once the
            // blood rises past it -- so the cost is readable at a glance, not a hidden number.
            if (threshold > 0f && threshold < 1f)
            {
                int tx = x + (int)(threshold * w);
                bool reached = percent >= threshold;

                Color mark = reached
                    ? Color.Lerp(color, Color.White, 0.65f) * (0.6f + 0.4f * pulse)
                    : new Color(168, 168, 174) * 0.45f;

                spriteBatch.Draw(pixel, new Rectangle(tx, y - 1, 1, h + 2), mark * a);
                spriteBatch.Draw(pixel, new Rectangle(tx, y - 3, 1, 2), mark * a);
            }

            // Vessel outline -- clotted red, brightening to a wet pulse when ready.
            DrawBorder(
                spriteBatch,
                new Rectangle(x - 1, y - 1, w + 2, h + 2),
                (hot
                    ? Color.Lerp(color, Color.White, 0.35f) * (0.5f + 0.5f * pulse)
                    : new Color(120, 22, 26) * 0.7f) * a);
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

        // A straight (rotated) line between two points -- reads as a clean web edge,
        // unlike the right-angle DrawConnector which tangles on diagonal branches.
        public static void DrawLine(
            SpriteBatch spriteBatch,
            Vector2 start,
            Vector2 end,
            Color color,
            float thickness)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;

            Vector2 delta = end - start;
            float length = delta.Length();

            if (length < 0.5f)
            {
                return;
            }

            float rotation = (float)Math.Atan2(delta.Y, delta.X);

            // Scale is a multiplier on the texture size, so divide by the real
            // texture dimensions to get an exact length x thickness line (guards
            // against MagicPixel not being 1x1, which would draw huge wedges).
            spriteBatch.Draw(
                pixel,
                start,
                null,
                color,
                rotation,
                new Vector2(0f, pixel.Height / 2f),
                new Vector2(length / pixel.Width, thickness / pixel.Height),
                SpriteEffects.None,
                0f);
        }

        public static void DrawTooltip(
            SpriteBatch spriteBatch,
            string title,
            IEnumerable<string> lines,
            Color accent)
        {
            const int width = 408;
            const float scale = 0.7f;
            List<string> wrapped = new List<string>();

            foreach (string line in lines)
            {
                wrapped.AddRange(WrapText(line, width - 32, scale));
            }

            int desiredHeight =
                62 + wrapped.Count * 24;

            int maxHeight =
                Math.Max(96, Main.screenHeight - 32);

            int height =
                Math.Min(
                    maxHeight,
                    Math.Max(96, desiredHeight));

            Rectangle panel =
                ClampToScreen(
                    new Rectangle(
                        (int)Main.MouseScreen.X + 20,
                        (int)Main.MouseScreen.Y + 20,
                        width,
                        Math.Max(96, height)),
                    16);

            DrawPanel(spriteBatch, panel, accent, 0.96f);
            DrawText(spriteBatch, title, new Vector2(panel.X + 18, panel.Y + 15), Color.White, 0.84f);

            int y = panel.Y + 48;
            foreach (string line in wrapped)
            {
                if (y + 24 > panel.Bottom - 10)
                {
                    break;
                }

                DrawText(spriteBatch, line, new Vector2(panel.X + 18, y), MutedText, scale);
                y += 24;
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
