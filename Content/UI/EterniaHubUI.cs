using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace Eternia.Content.UI
{
    // One door into everything. The mod had grown four separate panels behind four separate
    // keys (Soul, Stats, Passives, Codex), which meant remembering four bindings and closing
    // one to open another.
    //
    // This is the strip of tabs that turns them into pages of a single book. It is deliberately
    // a LAYER OVER the existing panels rather than a rewrite of them: each panel keeps its own
    // drawing, scrolling and click handling untouched, so consolidating the navigation cannot
    // break the contents. The old per-panel keys still work as direct shortcuts.
    public class EterniaHubUI : ModSystem
    {
        // Which page the hub key reopens. Remembering it means the key behaves like "resume
        // what I was doing" instead of always dumping you on the same page.
        private static EterniaUI.MajorPanel lastPage = EterniaUI.MajorPanel.Stats;

        public static bool IsOpen => EterniaUI.AnyMajorPanelOpen();

        public static void Toggle()
        {
            if (IsOpen)
            {
                EterniaUI.CloseAllMajorPanels();
                return;
            }

            Open(lastPage);
        }

        public static void Open(EterniaUI.MajorPanel page)
        {
            lastPage = page;

            // Every panel owns its own opening: the Soul page has to push a UIState or it
            // renders empty, and the tree has to recentre or you reopen it staring at blank
            // canvas. The hub only decides WHICH page, never how one is opened.
            switch (page)
            {
                case EterniaUI.MajorPanel.Soul:
                    SoulUISystem.OpenSoulPanel();
                    break;
                case EterniaUI.MajorPanel.Stats:
                    StatsUI.OpenPanel();
                    break;
                case EterniaUI.MajorPanel.Passive:
                    PassiveUI.OpenPanel();
                    break;
                case EterniaUI.MajorPanel.Bosses:
                    BossLogUI.OpenPanel();
                    break;
            }
        }

        // Anchored to the TALLEST page, so the strip does not jump as you change tabs -- a
        // navigation bar that moves is worse than no navigation bar.
        public static Rectangle TabStrip()
        {
            Rectangle tallest =
                EterniaUI.GetCenteredPanel(1120, 628, 32);

            int width =
                System.Math.Min(680, Main.screenWidth - 40);

            const int height = 32;

            return new Rectangle(
                (Main.screenWidth - width) / 2,
                System.Math.Max(4, tallest.Y - height - 8),
                width,
                height);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (EterniaKeybinds.ToggleEterniaMenu != null &&
                EterniaKeybinds.ToggleEterniaMenu.JustPressed)
            {
                Toggle();
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
                        "Eternia: Hub Tabs",
                        Draw,
                        InterfaceScaleType.UI));
            }
        }

        private bool Draw()
        {
            if (!IsOpen || !EterniaUI.ShouldDrawPlayerUI(Main.LocalPlayer))
            {
                return true;
            }

            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D pixel = TextureAssets.MagicPixel.Value;

            Rectangle bar = TabStrip();
            Point mouse = new Point(Main.mouseX, Main.mouseY);

            // The strip is NAVIGATION, so it wears the mod's colour rather than the page's. It
            // is the one element present on every page, which makes it the thing that says
            // "this is Eternia" -- if it changed hue per tab it would be the least consistent
            // part of the most consistent surface.
            (EterniaUI.MajorPanel page, string label, bool open)[] tabs =
            {
                (EterniaUI.MajorPanel.Soul, "SOUL", SoulUISystem.Visible),
                (EterniaUI.MajorPanel.Stats, "STATS", StatsUI.Visible),
                (EterniaUI.MajorPanel.Passive, "PASSIVES", PassiveUI.Visible),
                (EterniaUI.MajorPanel.Bosses, "CODEX", BossLogUI.Visible)
            };

            const int gap = 6;
            int tabW = (bar.Width - gap * (tabs.Length - 1)) / tabs.Length;


            for (int i = 0; i < tabs.Length; i++)
            {
                var rect = new Rectangle(
                    bar.X + i * (tabW + gap), bar.Y, tabW, bar.Height);

                bool active = tabs[i].open;
                bool hover = rect.Contains(mouse);
                Color brand = EterniaUI.Brand;

                Color fill =
                    active ? Color.Lerp(EterniaUI.PanelSurface, brand, 0.30f) :
                    hover ? Color.Lerp(EterniaUI.PanelSurfaceAlt, brand, 0.18f)
                          : EterniaUI.PanelSurface * 0.9f;

                spriteBatch.Draw(pixel, rect, fill);

                // The active tab is underlined into the page below it; the rest get a faint
                // baseline so the strip still reads as one connected row.
                spriteBatch.Draw(
                    pixel,
                    new Rectangle(rect.X, rect.Bottom - 2, rect.Width, 2),
                    active ? brand : brand * 0.30f);

                if (active)
                {
                    spriteBatch.Draw(
                        pixel, new Rectangle(rect.X, rect.Y, rect.Width, 2), brand);
                }

                EterniaUI.DrawCenteredText(
                    spriteBatch,
                    tabs[i].label,
                    rect,
                    active || hover ? Color.White : EterniaUI.MutedText,
                    0.6f);

                if (hover)
                {
                    Main.LocalPlayer.mouseInterface = true;

                    if (Main.mouseLeft && Main.mouseLeftRelease)
                    {
                        Main.mouseLeftRelease = false;
                        Open(tabs[i].page);
                    }
                }
            }

            return true;
        }
    }
}
