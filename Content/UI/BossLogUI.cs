using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ReLogic.Content;

using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

using Eternia.Content.Players;
using Eternia.Content.Progression;

namespace Eternia.Content.UI
{
    // THE BOSS CODEX. A master/detail bestiary: a scrollable, filterable list of every boss with
    // its portrait on the left, and a full detail card on the right -- portrait, progress, your
    // best clear, highest rarity faced, and drops. Toggle with the Boss Codex key (default N).
    //
    // Everything is drawn immediate-mode with the shared EterniaUI helpers. Rows and tabs are
    // clickable; the list scrolls by whole rows so nothing ever spills past the panel.
    public class BossLogUI : ModSystem
    {
        private enum Filter
        {
            All,
            PreHardmode,
            Hardmode,
            Defeated
        }

        public static bool Visible;

        private static readonly Color Accent = new Color(230, 180, 90);

        private const int RowHeight = 46;

        private static Filter filter = Filter.All;
        private static int selectedEntry = -1;
        private static int scrollRow;

        // Real drop data for the selected boss, pulled from the game's drop database and cached so
        // it is only computed when the selection changes.
        private static int cachedDropEntry = -2;
        private static List<DropRateInfo> cachedDrops = new List<DropRateInfo>();

        private static Asset<Texture2D> soulIcon;

        private static Texture2D SoulIcon =>
            (soulIcon ??= ModContent.Request<Texture2D>(
                "ETERNIA/Content/Items/Souls/EmptySoul",
                AssetRequestMode.ImmediateLoad)).Value;

        // Each panel owns how it opens; the hub tab only decides WHICH one.
        internal static void OpenPanel()
        {
            Visible = true;

            EterniaUI.CloseMajorPanelsExcept(
                EterniaUI.MajorPanel.Bosses);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (!Visible)
            {
                return;
            }

            Rectangle panel = GetPanelRect();

            if (panel.Contains(Main.MouseScreen.ToPoint()))
            {
                float delta = PlayerInput.ScrollWheelDeltaForUI;

                if (delta != 0f)
                {
                    scrollRow -= (int)System.Math.Round(delta / 120f);
                }
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(l => l.Name.Equals("Vanilla: Mouse Text"));

            if (index != -1)
            {
                layers.Insert(
                    index,
                    new LegacyGameInterfaceLayer(
                        "Eternia: Boss Codex",
                        () =>
                        {
                            if (Visible)
                            {
                                Draw(Main.spriteBatch);
                            }

                            return true;
                        },
                        InterfaceScaleType.UI));
            }
        }

        private static Rectangle GetPanelRect()
        {
            return EterniaUI.GetCenteredPanel(896, 628);
        }

        // ==============================================================================
        // Draw
        // ==============================================================================

        private static void Draw(SpriteBatch spriteBatch)
        {
            Rectangle panel = GetPanelRect();
            Point mouse = Main.MouseScreen.ToPoint();

            if (panel.Contains(mouse))
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            DrawCodexBackground(spriteBatch, panel);

            BossLogPlayer log = Main.LocalPlayer.GetModPlayer<BossLogPlayer>();

            int total = BossCodex.Entries.Length - 1; // exclude the mystery teaser from the count
            int beaten = log.DefeatedCount;

            DrawTitleBand(spriteBatch, panel);

            // Overall progress bar, with a soft glow behind it.
            Rectangle progress =
                new Rectangle(panel.X + 20, panel.Y + 62, panel.Width - 40, 18);

            spriteBatch.Draw(
                TextureAssets.MagicPixel.Value,
                new Rectangle(progress.X - 3, progress.Y - 3, progress.Width + 6, progress.Height + 6),
                Accent * 0.12f);

            EterniaUI.DrawProgressBar(
                spriteBatch, progress,
                total <= 0 ? 0f : beaten / (float)total,
                Accent,
                $"{beaten} / {total} bosses defeated");

            // Filter tabs.
            DrawTabs(spriteBatch, new Rectangle(panel.X + 20, panel.Y + 90, panel.Width - 40, 27), mouse);

            int contentTop = panel.Y + 128;
            int contentBottom = panel.Bottom - 18;
            int leftW = System.Math.Clamp((int)(panel.Width * 0.40f), 250, 360);

            Rectangle listRect =
                new Rectangle(panel.X + 18, contentTop, leftW, contentBottom - contentTop);

            Rectangle detailRect =
                new Rectangle(
                    listRect.Right + 22, contentTop,
                    panel.Right - 18 - (listRect.Right + 22), contentBottom - contentTop);

            List<int> filtered = Filtered(log);

            EnsureSelection(filtered);

            DrawList(spriteBatch, listRect, filtered, log, mouse);
            DrawDetail(spriteBatch, detailRect, log, mouse);

            // Frame flourishes drawn over everything: corner brackets + a soft glowing border.
            DrawCornerBrackets(spriteBatch, panel, Accent, 20, 3);

            for (int i = 3; i >= 1; i--)
            {
                EterniaUI.DrawBorder(
                    spriteBatch,
                    new Rectangle(panel.X - i, panel.Y - i, panel.Width + i * 2, panel.Height + i * 2),
                    Accent * (0.05f * i));
            }

            EterniaUI.DrawBorder(spriteBatch, panel, Accent * 0.6f, 1);

            if (EterniaUI.DrawCloseButton(spriteBatch, panel, Accent))
            {
                Visible = false;
            }

            EterniaUI.DrawQueuedTooltip(spriteBatch);
        }

        // --- Decorative frame ---------------------------------------------------------------

        private static void DrawCodexBackground(SpriteBatch spriteBatch, Rectangle panel)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;

            // Drop shadow.
            spriteBatch.Draw(pixel, new Rectangle(panel.X + 8, panel.Y + 11, panel.Width, panel.Height), EterniaUI.PanelBackground * 0.55f);

            // Vertical gradient body.
            DrawVGradient(spriteBatch, panel, new Color(20, 24, 32), new Color(8, 10, 14));
        }

        private static void DrawTitleBand(SpriteBatch spriteBatch, Rectangle panel)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;

            Rectangle band = new Rectangle(panel.X + 1, panel.Y + 1, panel.Width - 2, 50);

            // A gold-tinted gradient band sets the header apart from the body.
            DrawVGradient(spriteBatch, band, new Color(58, 46, 24), new Color(22, 25, 32));

            spriteBatch.Draw(pixel, new Rectangle(band.X, band.Bottom - 2, band.Width, 2), Accent);
            spriteBatch.Draw(pixel, new Rectangle(band.X, band.Bottom, band.Width, 6), Accent * 0.14f);

            EterniaUI.DrawText(spriteBatch, "BOSS CODEX", new Vector2(panel.X + 24, panel.Y + 12), Color.White, 1.1f);

            EterniaUI.DrawPill(
                spriteBatch, new Rectangle(panel.Right - 120, panel.Y + 14, 94, 24), "ETERNIA", Accent, 0.56f);
        }

        private static void DrawCornerBrackets(SpriteBatch spriteBatch, Rectangle r, Color color, int len, int th)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;

            spriteBatch.Draw(pixel, new Rectangle(r.X, r.Y, len, th), color);
            spriteBatch.Draw(pixel, new Rectangle(r.X, r.Y, th, len), color);

            spriteBatch.Draw(pixel, new Rectangle(r.Right - len, r.Y, len, th), color);
            spriteBatch.Draw(pixel, new Rectangle(r.Right - th, r.Y, th, len), color);

            spriteBatch.Draw(pixel, new Rectangle(r.X, r.Bottom - th, len, th), color);
            spriteBatch.Draw(pixel, new Rectangle(r.X, r.Bottom - len, th, len), color);

            spriteBatch.Draw(pixel, new Rectangle(r.Right - len, r.Bottom - th, len, th), color);
            spriteBatch.Draw(pixel, new Rectangle(r.Right - th, r.Bottom - len, th, len), color);
        }

        private static void DrawVGradient(SpriteBatch spriteBatch, Rectangle rect, Color top, Color bottom)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;

            const int steps = 22;
            int h = System.Math.Max(1, rect.Height / steps);

            for (int i = 0; i < steps; i++)
            {
                int y = rect.Y + i * h;
                int hh = i == steps - 1 ? rect.Bottom - y : h;

                spriteBatch.Draw(pixel, new Rectangle(rect.X, y, rect.Width, hh),
                    Color.Lerp(top, bottom, i / (float)(steps - 1)));
            }
        }

        private static void DrawHGradient(SpriteBatch spriteBatch, Rectangle rect, Color left, Color right)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;

            const int steps = 20;
            int w = System.Math.Max(1, rect.Width / steps);

            for (int i = 0; i < steps; i++)
            {
                int x = rect.X + i * w;
                int ww = i == steps - 1 ? rect.Right - x : w;

                spriteBatch.Draw(pixel, new Rectangle(x, rect.Y, ww, rect.Height),
                    Color.Lerp(left, right, i / (float)(steps - 1)));
            }
        }

        // A framed inset region (used for the list and the detail card) with a top accent line and
        // corner ticks -- reads as a designed sub-panel, not a plain rectangle.
        private static void DrawInsetPanel(SpriteBatch spriteBatch, Rectangle rect, Color accent)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;

            spriteBatch.Draw(pixel, rect, new Color(6, 8, 11) * 0.85f);
            spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, rect.Width, 2), accent * 0.6f);
            EterniaUI.DrawBorder(spriteBatch, rect, EterniaUI.Border * 0.55f);
            DrawCornerBrackets(spriteBatch, rect, accent * 0.8f, 10, 2);
        }

        // --- Filter tabs --------------------------------------------------------------------

        private static void DrawTabs(SpriteBatch spriteBatch, Rectangle bar, Point mouse)
        {
            (Filter f, string label)[] tabs =
            {
                (Filter.All, "All"),
                (Filter.PreHardmode, "Pre-HM"),
                (Filter.Hardmode, "Hardmode"),
                (Filter.Defeated, "Defeated")
            };

            int gap = 8;
            int tabW = (bar.Width - gap * (tabs.Length - 1)) / tabs.Length;

            for (int i = 0; i < tabs.Length; i++)
            {
                Rectangle rect = new Rectangle(bar.X + i * (tabW + gap), bar.Y, tabW, bar.Height);
                bool active = filter == tabs[i].f;
                bool hover = rect.Contains(mouse);

                Texture2D pixel = TextureAssets.MagicPixel.Value;

                // Active tab: dark gold-tinted fill with bright text and accent bars top+bottom, so
                // the selected filter reads clearly (black-on-gold was unreadable).
                Color fill =
                    active ? Color.Lerp(EterniaUI.PanelSurface, Accent, 0.22f) :
                    hover ? EterniaUI.PanelSurfaceAlt : EterniaUI.PanelSurface * 0.85f;

                spriteBatch.Draw(pixel, rect, fill);

                if (active)
                {
                    spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, rect.Width, 2), Accent);
                    spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Bottom - 2, rect.Width, 2), Accent);
                }
                else
                {
                    spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Bottom - 2, rect.Width, 2), Accent * 0.3f);
                }

                Color textColor =
                    active ? Color.White : (hover ? Color.White : EterniaUI.MutedText);

                EterniaUI.DrawCenteredText(spriteBatch, tabs[i].label, rect, textColor, 0.64f);

                if (hover)
                {
                    Main.LocalPlayer.mouseInterface = true;
                }

                if (hover && Main.mouseLeft && Main.mouseLeftRelease)
                {
                    Main.mouseLeftRelease = false;
                    filter = tabs[i].f;
                    scrollRow = 0;
                }
            }
        }

        // --- List ---------------------------------------------------------------------------

        private static void DrawList(
            SpriteBatch spriteBatch, Rectangle listRect, List<int> filtered, BossLogPlayer log, Point mouse)
        {
            DrawInsetPanel(spriteBatch, listRect, Accent);

            int visibleRows = System.Math.Max(1, (listRect.Height - 6) / RowHeight);
            int maxScroll = System.Math.Max(0, filtered.Count - visibleRows);
            scrollRow = System.Math.Clamp(scrollRow, 0, maxScroll);

            if (filtered.Count == 0)
            {
                EterniaUI.DrawCenteredText(
                    spriteBatch, "No bosses match this filter", listRect, EterniaUI.MutedText, 0.6f);
                return;
            }

            for (int r = 0; r < visibleRows; r++)
            {
                int fi = scrollRow + r;

                if (fi >= filtered.Count)
                {
                    break;
                }

                int entryIndex = filtered[fi];
                BossCodexEntry entry = BossCodex.Entries[entryIndex];

                Rectangle row =
                    new Rectangle(listRect.X + 4, listRect.Y + r * RowHeight + 3, listRect.Width - 8, RowHeight - 6);

                DrawListRow(spriteBatch, entry, entryIndex, log, row, mouse);
            }

            DrawScrollbar(spriteBatch, listRect, filtered.Count, visibleRows);
        }

        private static void DrawListRow(
            SpriteBatch spriteBatch, BossCodexEntry entry, int entryIndex,
            BossLogPlayer log, Rectangle row, Point mouse)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;

            bool selected = entryIndex == selectedEntry;
            bool hover = row.Contains(mouse);

            BossLogPlayer.BossRecord record = default;
            bool defeated = !entry.IsMystery && log.TryGet(entry.Id, out record);

            Color accent =
                entry.IsMystery ? Accent * 0.6f :
                defeated ? RarityColor(record.HighestRarity) :
                (entry.Hardmode ? new Color(210, 90, 90) : new Color(110, 170, 120));

            if (selected)
            {
                // Selected: a colour wash fading to the right + a pulsing accent edge.
                DrawHGradient(spriteBatch, row, accent * 0.34f, accent * 0.05f);

                float pulse = 0.55f + 0.45f * (float)System.Math.Sin(Main.GlobalTimeWrappedHourly * 4f);
                spriteBatch.Draw(pixel, new Rectangle(row.X, row.Y, 4, row.Height), accent * pulse);
                EterniaUI.DrawBorder(spriteBatch, row, accent * 0.5f);
            }
            else
            {
                spriteBatch.Draw(pixel, row, hover ? EterniaUI.PanelSurfaceAlt * 0.85f : EterniaUI.PanelSurface * 0.4f);
                spriteBatch.Draw(pixel, new Rectangle(row.X, row.Y, 3, row.Height), accent * (hover ? 0.95f : 0.7f));
            }

            // Portrait.
            DrawPortrait(spriteBatch, entry, new Rectangle(row.X + 8, row.Y + 4, 30, row.Height - 8));

            // Name + tier.
            Color nameColor =
                entry.IsMystery ? EterniaUI.MutedText * 0.7f :
                defeated ? Color.White : new Color(210, 214, 222);

            EterniaUI.DrawTrimmedText(
                spriteBatch, entry.Name, new Vector2(row.X + 46, row.Y + 4),
                row.Width - 74, nameColor, 0.78f);

            EterniaUI.DrawText(
                spriteBatch, entry.Hardmode ? "Hardmode" : "Pre-Hardmode",
                new Vector2(row.X + 46, row.Y + 24),
                (entry.Hardmode ? new Color(230, 120, 120) : new Color(130, 195, 140)), 0.6f);

            // Defeated marker on the right: a filled gem in the rarity colour (font-glyph-safe).
            if (defeated)
            {
                Rectangle gem = new Rectangle(row.Right - 24, row.Y + row.Height / 2 - 6, 12, 12);
                spriteBatch.Draw(pixel, gem, RarityColor(record.HighestRarity));
                EterniaUI.DrawBorder(spriteBatch, gem, Color.White * 0.3f);
            }
            else if (entry.IsMystery)
            {
                EterniaUI.DrawText(spriteBatch, "?", new Vector2(row.Right - 20, row.Y + 8), EterniaUI.MutedText * 0.8f, 0.62f);
            }

            if (hover)
            {
                Main.LocalPlayer.mouseInterface = true;

                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    Main.mouseLeftRelease = false;
                    selectedEntry = entryIndex;
                }
            }
        }

        // --- Detail card --------------------------------------------------------------------

        private static void DrawDetail(SpriteBatch spriteBatch, Rectangle pane, BossLogPlayer log, Point mouse)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;

            DrawInsetPanel(spriteBatch, pane, Accent);

            if (selectedEntry < 0 || selectedEntry >= BossCodex.Entries.Length)
            {
                EterniaUI.DrawCenteredText(spriteBatch, "Select a boss", pane, EterniaUI.MutedText, 0.66f);
                return;
            }

            BossCodexEntry entry = BossCodex.Entries[selectedEntry];

            BossLogPlayer.BossRecord record = default;
            bool defeated = !entry.IsMystery && log.TryGet(entry.Id, out record);

            Color accent =
                entry.IsMystery ? Accent * 0.7f :
                defeated ? RarityColor(record.HighestRarity) : Accent;

            // A faint rarity wash across the top of the card.
            DrawHGradient(
                spriteBatch, new Rectangle(pane.X + 2, pane.Y + 2, pane.Width - 4, 108),
                accent * 0.12f, Color.Transparent);

            // Portrait, framed with a rarity glow + corner brackets.
            Rectangle portrait = new Rectangle(pane.X + 16, pane.Y + 16, 88, 88);

            for (int g = 4; g >= 1; g--)
            {
                spriteBatch.Draw(pixel,
                    new Rectangle(portrait.X - g * 2, portrait.Y - g * 2, portrait.Width + g * 4, portrait.Height + g * 4),
                    accent * (0.05f * g));
            }

            spriteBatch.Draw(pixel, portrait, EterniaUI.PanelSurface * 0.95f);
            EterniaUI.DrawBorder(spriteBatch, portrait, accent * 0.7f);
            DrawCornerBrackets(spriteBatch, portrait, accent, 12, 2);
            DrawPortrait(spriteBatch, entry, new Rectangle(portrait.X + 10, portrait.Y + 10, portrait.Width - 20, portrait.Height - 20));

            int textX = portrait.Right + 18;

            EterniaUI.DrawTrimmedText(
                spriteBatch, entry.Name, new Vector2(textX, pane.Y + 20),
                pane.Right - textX - 12, Color.White, 1.12f);

            // Tier pill.
            EterniaUI.DrawPill(
                spriteBatch, new Rectangle(textX, pane.Y + 50, 138, 26),
                entry.Hardmode ? "HARDMODE" : "PRE-HARDMODE",
                entry.Hardmode ? new Color(210, 90, 90) : new Color(110, 170, 120), 0.56f);

            // Status.
            string status =
                entry.IsMystery ? "Not yet risen" :
                defeated ? "DEFEATED" : "Not defeated";

            EterniaUI.DrawText(
                spriteBatch, status, new Vector2(textX + 150, pane.Y + 54),
                defeated ? RarityColor(record.HighestRarity) : EterniaUI.MutedText, 0.64f);

            EterniaUI.DrawDivider(spriteBatch, pane.X + 16, pane.Y + 112, pane.Width - 32, accent);

            // Stat tiles.
            int tileY = pane.Y + 124;
            int tileW = (pane.Width - 32 - 16) / 3;
            int tileH = 62;

            DrawStatTile(spriteBatch, new Rectangle(pane.X + 16, tileY, tileW, tileH),
                "KILLS", defeated ? record.Kills.ToString() : "0", Color.White);

            DrawStatTile(spriteBatch, new Rectangle(pane.X + 16 + tileW + 8, tileY, tileW, tileH),
                "BEST TIME",
                defeated && record.BestKillTicks > 0 ? FormatTime(record.BestKillTicks) : "--",
                Color.White);

            DrawStatTile(spriteBatch, new Rectangle(pane.X + 16 + (tileW + 8) * 2, tileY, tileW, tileH),
                "TOP RARITY",
                defeated ? RarityName(record.HighestRarity) : "--",
                defeated ? RarityColor(record.HighestRarity) : EterniaUI.MutedText);

            // Drops -- real item icons + drop rates, straight from the game's drop database.
            int dropsY = tileY + tileH + 16;

            EterniaUI.DrawText(spriteBatch, "DROPS", new Vector2(pane.X + 16, dropsY), Accent, 0.66f);
            EterniaUI.DrawDivider(spriteBatch, pane.X + 78, dropsY + 8, pane.Width - 94, accent);

            EnsureDrops(entry);

            if (entry.IsMystery || cachedDrops.Count == 0)
            {
                EterniaUI.DrawWrappedText(
                    spriteBatch, entry.Drops,
                    new Rectangle(pane.X + 16, dropsY + 24, pane.Width - 32, pane.Bottom - (dropsY + 24) - 10),
                    EterniaUI.MutedText, 0.62f, 22);

                return;
            }

            // A grid of item slots -- reads like loot, not a spreadsheet. Each slot shows the item
            // icon with its drop rate underneath; hover for the name + quantity.
            const int slot = 54;
            const int gap = 10;
            int cellH = slot + 20;

            int gridTop = dropsY + 26;
            int gridLeft = pane.X + 16;
            int gridWidth = pane.Width - 30;

            int cols = System.Math.Max(1, (gridWidth + gap) / (slot + gap));
            int availH = pane.Bottom - 6 - gridTop;
            int maxRows = System.Math.Max(1, (availH - 18) / cellH);
            int capacity = cols * maxRows;
            int shown = System.Math.Min(cachedDrops.Count, capacity);

            for (int i = 0; i < shown; i++)
            {
                int c = i % cols;
                int r = i / cols;

                Rectangle slotRect =
                    new Rectangle(gridLeft + c * (slot + gap), gridTop + r * cellH, slot, slot);

                DrawDropSlot(spriteBatch, cachedDrops[i], slotRect, mouse);
            }

            if (cachedDrops.Count > shown)
            {
                EterniaUI.DrawText(
                    spriteBatch, $"+{cachedDrops.Count - shown} more",
                    new Vector2(gridLeft, gridTop + maxRows * cellH), EterniaUI.MutedText, 0.55f);
            }
        }

        private static void EnsureDrops(BossCodexEntry entry)
        {
            if (cachedDropEntry == selectedEntry)
            {
                return;
            }

            cachedDropEntry = selectedEntry;
            cachedDrops = new List<DropRateInfo>();

            if (entry.IsMystery || entry.Id < 0)
            {
                return;
            }

            List<DropRateInfo> raw = new List<DropRateInfo>();
            DropRateInfoChainFeed feed = new DropRateInfoChainFeed(1f);

            foreach (IItemDropRule rule in Main.ItemDropsDB.GetRulesForNPCID(entry.Id, false))
            {
                rule.ReportDroprates(raw, feed);
            }

            // The DB can list the same item several times (normal vs expert, per-condition). Keep
            // one row per item at its best rate, most likely first.
            Dictionary<int, DropRateInfo> best = new Dictionary<int, DropRateInfo>();

            foreach (DropRateInfo d in raw)
            {
                if (!best.TryGetValue(d.itemId, out DropRateInfo cur) || d.dropRate > cur.dropRate)
                {
                    best[d.itemId] = d;
                }
            }

            cachedDrops = best.Values.OrderByDescending(d => d.dropRate).ToList();
        }

        private static void DrawDropSlot(SpriteBatch spriteBatch, DropRateInfo info, Rectangle slotRect, Point mouse)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;

            bool hover = slotRect.Contains(mouse);
            Color rateColor = RateColor(info.dropRate);

            // Slot background + rate-tinted border (bright on hover).
            spriteBatch.Draw(pixel, slotRect, EterniaUI.PanelSurfaceAlt * (hover ? 1f : 0.85f));
            EterniaUI.DrawBorder(spriteBatch, slotRect, hover ? rateColor : rateColor * 0.55f, hover ? 2 : 1);

            // Item icon, centered with a little padding.
            DrawItemIcon(spriteBatch, info.itemId,
                new Rectangle(slotRect.X + 8, slotRect.Y + 6, slotRect.Width - 16, slotRect.Height - 16));

            // Quantity badge (e.g. "8-15") in the corner when it drops in a stack.
            if (info.stackMax > 1)
            {
                string qty = info.stackMax == info.stackMin ? info.stackMax.ToString() : $"{info.stackMin}-{info.stackMax}";
                spriteBatch.Draw(pixel, new Rectangle(slotRect.X + 1, slotRect.Bottom - 15, slotRect.Width - 2, 14), EterniaUI.PanelBackground * 0.8f);
                EterniaUI.DrawText(spriteBatch, qty, new Vector2(slotRect.X + 4, slotRect.Bottom - 15), Color.White, 0.48f);
            }

            // Drop rate under the slot.
            string rate = RateText(info.dropRate);
            EterniaUI.DrawCenteredText(
                spriteBatch, rate,
                new Rectangle(slotRect.X - 4, slotRect.Bottom + 2, slotRect.Width + 8, 16),
                rateColor, 0.58f);

            if (hover)
            {
                Main.LocalPlayer.mouseInterface = true;

                string name = Lang.GetItemNameValue(info.itemId);
                List<string> lines = new List<string> { $"Drop rate: {rate}" };

                if (info.stackMax > 1)
                {
                    lines.Add($"Quantity: {info.stackMin}-{info.stackMax}");
                }

                EterniaUI.QueueTooltip(name, lines, rateColor);
            }
        }

        private static void DrawItemIcon(SpriteBatch spriteBatch, int itemId, Rectangle box)
        {
            Main.instance.LoadItem(itemId);

            Texture2D tex = TextureAssets.Item[itemId].Value;

            Rectangle frame =
                Main.itemAnimations[itemId] != null
                    ? Main.itemAnimations[itemId].GetFrame(tex)
                    : tex.Bounds;

            float scale = System.Math.Min(box.Width / (float)frame.Width, box.Height / (float)frame.Height);

            if (scale > 1f)
            {
                scale = 1f;
            }

            spriteBatch.Draw(
                tex, box.Center.ToVector2(), frame, Color.White, 0f,
                new Vector2(frame.Width / 2f, frame.Height / 2f), scale, SpriteEffects.None, 0f);
        }

        private static string RateText(float rate)
        {
            if (rate >= 0.999f)
            {
                return "100%";
            }

            if (rate >= 0.1f)
            {
                return $"{rate * 100f:0}%";
            }

            if (rate <= 0f)
            {
                return "--";
            }

            return $"1/{(int)System.Math.Round(1f / rate)}";
        }

        private static Color RateColor(float rate)
        {
            return
                rate >= 0.999f ? new Color(120, 220, 140) :
                rate >= 0.34f ? Color.White :
                rate >= 0.1f ? new Color(150, 200, 255) :
                Color.Gold;
        }

        private static void DrawStatTile(SpriteBatch spriteBatch, Rectangle rect, string label, string value, Color valueColor)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;

            spriteBatch.Draw(pixel, rect, EterniaUI.PanelSurfaceAlt * 0.85f);
            spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, rect.Width, 2), Accent * 0.5f);
            EterniaUI.DrawBorder(spriteBatch, rect, EterniaUI.Border * 0.6f);

            EterniaUI.DrawCenteredText(spriteBatch, label,
                new Rectangle(rect.X, rect.Y + 9, rect.Width, 14), EterniaUI.MutedText, 0.54f);

            EterniaUI.DrawCenteredText(spriteBatch, value,
                new Rectangle(rect.X, rect.Y + 28, rect.Width, 24), valueColor, 0.84f);
        }

        // --- Shared bits --------------------------------------------------------------------

        private static void DrawPortrait(SpriteBatch spriteBatch, BossCodexEntry entry, Rectangle box)
        {
            Texture2D head = HeadTexture(entry);

            if (head == null)
            {
                // Placeholder soul icon for the mystery entry or a boss with no head slot.
                head = SoulIcon;
            }

            float scale = System.Math.Min(box.Width / (float)head.Width, box.Height / (float)head.Height);
            Vector2 center = box.Center.ToVector2();

            Color tint = entry.IsMystery ? EterniaUI.MutedText * 0.5f : Color.White;

            spriteBatch.Draw(
                head, center, null, tint, 0f,
                new Vector2(head.Width / 2f, head.Height / 2f), scale, SpriteEffects.None, 0f);
        }

        private static Texture2D HeadTexture(BossCodexEntry entry)
        {
            if (entry.IsMystery || entry.Id < 0 || entry.Id >= NPCID.Sets.BossHeadTextures.Length)
            {
                return null;
            }

            int slot = NPCID.Sets.BossHeadTextures[entry.Id];

            if (slot < 0 || slot >= TextureAssets.NpcHeadBoss.Length)
            {
                return null;
            }

            Asset<Texture2D> asset = TextureAssets.NpcHeadBoss[slot];

            return asset != null && asset.IsLoaded ? asset.Value : null;
        }

        private static void DrawScrollbar(SpriteBatch spriteBatch, Rectangle listRect, int total, int visible)
        {
            if (total <= visible)
            {
                return;
            }

            Texture2D pixel = TextureAssets.MagicPixel.Value;

            Rectangle track = new Rectangle(listRect.Right - 5, listRect.Y + 2, 3, listRect.Height - 4);
            spriteBatch.Draw(pixel, track, EterniaUI.Border * 0.4f);

            float frac = visible / (float)total;
            int handleH = System.Math.Max(24, (int)(track.Height * frac));
            int maxScroll = System.Math.Max(1, total - visible);
            float t = scrollRow / (float)maxScroll;
            int handleY = track.Y + (int)((track.Height - handleH) * t);

            spriteBatch.Draw(pixel, new Rectangle(track.X, handleY, 3, handleH), Accent * 0.85f);
        }

        private static List<int> Filtered(BossLogPlayer log)
        {
            List<int> result = new List<int>();

            for (int i = 0; i < BossCodex.Entries.Length; i++)
            {
                BossCodexEntry e = BossCodex.Entries[i];

                bool match = filter switch
                {
                    Filter.PreHardmode => !e.Hardmode,
                    Filter.Hardmode => e.Hardmode,
                    Filter.Defeated => !e.IsMystery && log.TryGet(e.Id, out _),
                    _ => true
                };

                if (match)
                {
                    result.Add(i);
                }
            }

            return result;
        }

        private static void EnsureSelection(List<int> filtered)
        {
            if (filtered.Contains(selectedEntry))
            {
                return;
            }

            selectedEntry = filtered.Count > 0 ? filtered[0] : -1;
        }

        private static string FormatTime(int ticks)
        {
            int totalSeconds = ticks / 60;
            return $"{totalSeconds / 60}:{totalSeconds % 60:00}";
        }

        // Mirrors EterniaGlobalNPC.EnemyRarity ordering (0..7).
        private static string RarityName(byte rarity)
        {
            return rarity switch
            {
                1 => "Uncommon",
                2 => "Rare",
                3 => "Super Rare",
                4 => "Legendary",
                5 => "Mythic",
                6 => "Ancient",
                7 => "Nightmare",
                _ => "Common"
            };
        }

        private static Color RarityColor(byte rarity)
        {
            return rarity switch
            {
                1 => Color.LightBlue,
                2 => Color.LightGreen,
                3 => Color.Gold,
                4 => Color.OrangeRed,
                5 => new Color(200, 70, 255),
                6 => new Color(60, 230, 210),
                7 => new Color(210, 24, 44),
                _ => Color.LightGray
            };
        }
    }
}
