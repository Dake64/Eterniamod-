using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace Eternia.Content.UI
{
    // The banner half of the Awakening. The ceremony (Content.Systems.AwakeningCeremony) plays the
    // build-up and the flash, then raises this banner as the flash clears. It names your subclass
    // AND the signature mechanic you were just handed -- the one thing nothing else explains.
    public class PromotionBannerUI : ModSystem
    {
        private const int DisplayTicks = 260;

        private static int remainingTicks;
        private static string subclassName = string.Empty;
        private static string mechanicLine = string.Empty;
        private static string creedLine = "A new path awakens";
        private static Color accent = Color.Gold;

        // Stash what to say without raising the banner yet -- the ceremony fires it at the burst.
        public static void Prepare(string subclass, string mechanic, string creed, Color accentColor)
        {
            subclassName = subclass;
            mechanicLine = mechanic ?? string.Empty;
            creedLine = string.IsNullOrEmpty(creed) ? "A new path awakens" : creed;
            accent = accentColor;
        }

        // Raise the already-prepared banner.
        public static void Fire()
        {
            remainingTicks = DisplayTicks;
        }

        // Prepare and raise in one call (instant banner, no ceremony).
        public static void Show(string subclass, string mechanic = "", string creed = "", Color? accentColor = null)
        {
            Prepare(subclass, mechanic, creed, accentColor ?? Color.Gold);
            Fire();
        }

        public override void Unload()
        {
            remainingTicks = 0;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (remainingTicks > 0)
            {
                remainingTicks--;
            }
        }

        public override void ModifyInterfaceLayers(
            List<GameInterfaceLayer> layers)
        {
            int index =
                layers.FindIndex(
                    layer => layer.Name.Equals("Vanilla: Mouse Text"));

            if (index != -1)
            {
                layers.Insert(
                    index,
                    new LegacyGameInterfaceLayer(
                        "Eternia: Promotion Banner",
                        DrawBanner,
                        InterfaceScaleType.UI));
            }
        }

        private bool DrawBanner()
        {
            if (remainingTicks <= 0)
            {
                return true;
            }

            Player player = Main.LocalPlayer;

            if (!EterniaUI.ShouldDrawPlayerUI(player))
            {
                return true;
            }

            int elapsed = DisplayTicks - remainingTicks;

            float alpha = 1f;

            if (elapsed < 14)
            {
                alpha = elapsed / 14f;
            }
            else if (remainingTicks < 50)
            {
                alpha = remainingTicks / 50f;
            }

            float pulse =
                0.5f + 0.5f * (float)System.Math.Sin(
                    Main.GlobalTimeWrappedHourly * 4f);

            float rise =
                (1f - System.Math.Min(1f, elapsed / 14f)) * 16f;

            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D pixel = TextureAssets.MagicPixel.Value;

            int width = 520;
            int height = 152;

            Rectangle panel =
                new Rectangle(
                    (Main.screenWidth - width) / 2,
                    (int)(Main.screenHeight * 0.20f - rise),
                    width,
                    height);

            // A faint wash of your colour over the whole screen.
            spriteBatch.Draw(
                pixel,
                new Rectangle(0, 0, Main.screenWidth, Main.screenHeight),
                accent * (0.08f * alpha));

            spriteBatch.Draw(pixel, panel, EterniaUI.PanelBackground * (0.93f * alpha));
            spriteBatch.Draw(pixel, new Rectangle(panel.X, panel.Y, panel.Width, 3), accent * (0.9f * alpha));
            spriteBatch.Draw(pixel, new Rectangle(panel.X, panel.Bottom - 3, panel.Width, 3), accent * (0.9f * alpha));
            EterniaUI.DrawBorder(spriteBatch, panel, accent * ((0.5f + 0.5f * pulse) * alpha), 2);

            EterniaUI.DrawCenteredText(
                spriteBatch,
                "THE SOUL HAS CHOSEN",
                new Rectangle(panel.X, panel.Y + 12, panel.Width, 20),
                EterniaUI.MutedText * alpha,
                0.55f);

            // Pulsing glow behind the subclass name.
            Rectangle nameRect =
                new Rectangle(panel.X, panel.Y + 34, panel.Width, 38);

            for (int i = 0; i < 6; i++)
            {
                float angle = MathHelper.TwoPi * i / 6f;
                Vector2 off = angle.ToRotationVector2() * (2f * pulse + 1f);

                EterniaUI.DrawCenteredText(
                    spriteBatch,
                    subclassName,
                    new Rectangle(
                        nameRect.X + (int)off.X,
                        nameRect.Y + (int)off.Y,
                        nameRect.Width,
                        nameRect.Height),
                    accent * (0.4f * alpha),
                    1.2f);
            }

            EterniaUI.DrawCenteredText(
                spriteBatch,
                subclassName,
                nameRect,
                accent * alpha,
                1.2f);

            // The mechanic you now carry -- the headline of the whole moment.
            if (mechanicLine.Length > 0)
            {
                EterniaUI.DrawCenteredText(
                    spriteBatch,
                    mechanicLine,
                    new Rectangle(panel.X, panel.Y + 84, panel.Width, 24),
                    Color.White * (0.92f * alpha),
                    0.72f);
            }

            EterniaUI.DrawCenteredText(
                spriteBatch,
                creedLine,
                new Rectangle(panel.X, panel.Y + 116, panel.Width, 20),
                EterniaUI.MutedText * alpha,
                0.55f);

            return true;
        }
    }
}
