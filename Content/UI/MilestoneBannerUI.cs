using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace Eternia.Content.UI
{
    // A short gold banner celebrating a passive-tree milestone (every 5 nodes),
    // so the "you unlocked something special" moment is visible over the panel.
    public class MilestoneBannerUI : ModSystem
    {
        private const int DisplayTicks = 190;

        private static int remainingTicks;
        private static int milestone;

        public static void Show(int milestoneNumber)
        {
            milestone = milestoneNumber;
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
                        "Eternia: Milestone Banner",
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

            if (elapsed < 12)
            {
                alpha = elapsed / 12f;
            }
            else if (remainingTicks < 40)
            {
                alpha = remainingTicks / 40f;
            }

            float pulse =
                0.5f + 0.5f * (float)System.Math.Sin(
                    Main.GlobalTimeWrappedHourly * 4f);

            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            Color accent = Color.Gold;

            int width = 372;
            int height = 76;

            Rectangle panel =
                new Rectangle(
                    (Main.screenWidth - width) / 2,
                    (int)(Main.screenHeight * 0.16f),
                    width,
                    height);

            spriteBatch.Draw(pixel, panel, EterniaUI.PanelBackground * (0.92f * alpha));
            spriteBatch.Draw(pixel, new Rectangle(panel.X, panel.Y, panel.Width, 3), accent * (0.9f * alpha));
            spriteBatch.Draw(pixel, new Rectangle(panel.X, panel.Bottom - 3, panel.Width, 3), accent * (0.9f * alpha));
            EterniaUI.DrawBorder(spriteBatch, panel, accent * ((0.5f + 0.5f * pulse) * alpha), 2);

            EterniaUI.DrawCenteredText(
                spriteBatch,
                "MILESTONE " + milestone,
                new Rectangle(panel.X, panel.Y + 12, panel.Width, 32),
                accent * alpha,
                0.98f);

            EterniaUI.DrawCenteredText(
                spriteBatch,
                "Your mastery deepens",
                new Rectangle(panel.X, panel.Y + 46, panel.Width, 20),
                EterniaUI.MutedText * alpha,
                0.55f);

            return true;
        }
    }
}
