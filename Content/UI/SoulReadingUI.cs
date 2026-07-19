using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace Eternia.Content.UI
{
    // "Read my soul" used to dump four lines into chat, which is where combat spam and item
    // pickups live -- the one place in the game that explains your build read like noise.
    //
    // It is a panel now, and the tier is drawn as PIPS rather than described in words: three
    // diamonds, filled up to where you stand. That is the part a player takes in at a glance,
    // and it makes "there is more after this" impossible to miss.
    //
    // Unlike the Awakening (a one-time, easily-missed moment that still goes to chat so it can
    // be scrolled back to), this reading is repeatable on demand -- so a panel that fades is
    // the right shape for it.
    public class SoulReadingUI : ModSystem
    {
        private const int DisplayTicks = 600; // ~10s, long enough to actually read

        private static int remaining;
        private static string subclass = string.Empty;
        private static string mechanic = string.Empty;
        private static string sealedBy = string.Empty;
        private static string growth = string.Empty;
        private static int tier = 1;
        private static int maxTier = 3;
        private static Color accent = Color.MediumPurple;

        public static void Show(
            string subclassName,
            string mechanicName,
            string sealedByLine,
            string growthLine,
            int currentTier,
            int highestTier,
            Color accentColor)
        {
            subclass = subclassName ?? string.Empty;
            mechanic = mechanicName ?? string.Empty;
            sealedBy = sealedByLine ?? string.Empty;
            growth = growthLine ?? string.Empty;
            tier = currentTier;
            maxTier = highestTier;
            accent = accentColor;
            remaining = DisplayTicks;
        }

        public override void Unload() => remaining = 0;

        public override void UpdateUI(GameTime gameTime)
        {
            if (remaining > 0)
            {
                remaining--;
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
                        "Eternia: Soul Reading",
                        Draw,
                        InterfaceScaleType.UI));
            }
        }

        private bool Draw()
        {
            if (remaining <= 0)
            {
                return true;
            }

            int elapsed = DisplayTicks - remaining;

            float alpha = 1f;

            if (elapsed < 12)
            {
                alpha = elapsed / 12f;
            }
            else if (remaining < 45)
            {
                alpha = remaining / 45f;
            }

            float pulse =
                0.5f + 0.5f * (float)System.Math.Sin(
                    Main.GlobalTimeWrappedHourly * 3f);

            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D pixel = TextureAssets.MagicPixel.Value;

            // Sized around the type, not the other way round: the reading is meant to be read
            // from a normal seating distance, so every line got a size bump and the panel grew
            // to match rather than cramming bigger text into the old box.
            const int width = 680;
            const int height = 268;

            var panel = new Rectangle(
                (Main.screenWidth - width) / 2,
                (int)(Main.screenHeight * 0.16f),
                width,
                height);

            spriteBatch.Draw(pixel, panel, EterniaUI.PanelBackground * (0.94f * alpha));
            spriteBatch.Draw(
                pixel,
                new Rectangle(panel.X, panel.Y, panel.Width, 3),
                accent * (0.9f * alpha));

            EterniaUI.DrawBorder(spriteBatch, panel, accent * (0.55f * alpha), 2);

            EterniaUI.DrawCenteredText(
                spriteBatch,
                "THE ETERNAL READS YOUR SOUL",
                new Rectangle(panel.X, panel.Y + 14, panel.Width, 22),
                EterniaUI.MutedText * alpha,
                0.62f);

            EterniaUI.DrawCenteredText(
                spriteBatch,
                subclass,
                new Rectangle(panel.X, panel.Y + 42, panel.Width, 38),
                accent * alpha,
                1.2f);

            EterniaUI.DrawCenteredText(
                spriteBatch,
                sealedBy,
                new Rectangle(panel.X, panel.Y + 84, panel.Width, 22),
                EterniaUI.MutedText * alpha,
                0.62f);

            // --- The mechanic and its rung, drawn as pips -------------------------------
            EterniaUI.DrawCenteredText(
                spriteBatch,
                mechanic,
                new Rectangle(panel.X, panel.Y + 116, panel.Width, 28),
                Color.White * (0.95f * alpha),
                0.88f);

            DrawTierPips(spriteBatch, pixel, panel, alpha, pulse);

            // --- What the next milestone brings ----------------------------------------
            int y = panel.Y + 192;

            foreach (string line in EterniaUI.WrapText(growth, panel.Width - 64, 0.64f))
            {
                EterniaUI.DrawCenteredText(
                    spriteBatch,
                    line,
                    new Rectangle(panel.X, y, panel.Width, 22),
                    Color.Lerp(accent, Color.White, 0.55f) * (0.92f * alpha),
                    0.64f);

                y += 22;
            }

            return true;
        }

        // Three diamonds, filled up to your rung. The last filled one breathes, so the eye is
        // drawn to where you actually are rather than to the empty ones ahead.
        private static void DrawTierPips(
            SpriteBatch spriteBatch,
            Texture2D pixel,
            Rectangle panel,
            float alpha,
            float pulse)
        {
            const int pipSize = 13;
            const int gap = 20;

            int totalW = maxTier * pipSize + (maxTier - 1) * gap;
            int startX = panel.X + (panel.Width - totalW) / 2;
            int pipY = panel.Y + 156;

            for (int i = 0; i < maxTier; i++)
            {
                int x = startX + i * (pipSize + gap);
                bool earned = i < tier;
                bool current = i == tier - 1;

                Color pip = earned
                    ? (current
                        ? Color.Lerp(accent, Color.White, 0.35f + 0.35f * pulse)
                        : accent)
                    : EterniaUI.MutedText * 0.5f;

                // A diamond: rows narrowing out from the middle.
                for (int row = 0; row < pipSize; row++)
                {
                    int spread = pipSize / 2 - System.Math.Abs(row - pipSize / 2);
                    int w = spread * 2 + 1;

                    spriteBatch.Draw(
                        pixel,
                        new Rectangle(x + pipSize / 2 - spread, pipY + row, w, 1),
                        pip * alpha);
                }

                // A connecting thread between pips, so they read as one ladder.
                if (i < maxTier - 1)
                {
                    spriteBatch.Draw(
                        pixel,
                        new Rectangle(x + pipSize, pipY + pipSize / 2, gap, 1),
                        (i + 1 < tier ? accent : EterniaUI.MutedText * 0.4f) * (0.7f * alpha));
                }
            }
        }
    }
}
