using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace Eternia.Content.UI
{
    // A dramatic centered banner shown when the player promotes into a subclass.
    public class PromotionBannerUI : ModSystem
    {
        private const int DisplayTicks = 220;

        private static int remainingTicks;
        private static string subclassName = string.Empty;

        public static void Show(string subclass)
        {
            subclassName = subclass;
            remainingTicks = DisplayTicks;
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
            Color accent = Color.Gold;

            int width = 420;
            int height = 112;

            Rectangle panel =
                new Rectangle(
                    (Main.screenWidth - width) / 2,
                    (int)(Main.screenHeight * 0.24f - rise),
                    width,
                    height);

            spriteBatch.Draw(pixel, panel, EterniaUI.PanelBackground * (0.93f * alpha));
            spriteBatch.Draw(pixel, new Rectangle(panel.X, panel.Y, panel.Width, 3), accent * (0.9f * alpha));
            spriteBatch.Draw(pixel, new Rectangle(panel.X, panel.Bottom - 3, panel.Width, 3), accent * (0.9f * alpha));
            EterniaUI.DrawBorder(spriteBatch, panel, accent * ((0.5f + 0.5f * pulse) * alpha), 2);

            EterniaUI.DrawCenteredText(
                spriteBatch,
                "PROMOTION!",
                new Rectangle(panel.X, panel.Y + 14, panel.Width, 34),
                accent * alpha,
                1.05f);

            // Pulsing glow behind the subclass name.
            Rectangle nameRect =
                new Rectangle(panel.X, panel.Y + 52, panel.Width, 32);

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
                    1f);
            }

            EterniaUI.DrawCenteredText(
                spriteBatch,
                subclassName,
                nameRect,
                Color.White * alpha,
                1f);

            EterniaUI.DrawCenteredText(
                spriteBatch,
                "A new path awakens",
                new Rectangle(panel.X, panel.Y + 88, panel.Width, 20),
                EterniaUI.MutedText * alpha,
                0.55f);

            return true;
        }
    }
}
