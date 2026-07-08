using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace Eternia.Content.UI
{
    // A brief centered banner shown on level-up, instead of spamming the chat.
    public class LevelUpBannerUI : ModSystem
    {
        private const int DisplayTicks = 150;

        private static int remainingTicks;
        private static int bannerLevel;
        private static int pendingStats;
        private static int pendingPassives;

        public static void Show(int level, int statGain, int passiveGain)
        {
            // Multiple level-ups in one burst accumulate their rewards.
            if (remainingTicks > 0)
            {
                pendingStats += statGain;
                pendingPassives += passiveGain;
            }
            else
            {
                pendingStats = statGain;
                pendingPassives = passiveGain;
            }

            bannerLevel = level;
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
                        "Eternia: Level Up Banner",
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

            // Small upward rise as it appears.
            float rise =
                (1f - System.Math.Min(1f, elapsed / 12f)) * 12f;

            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D pixel = TextureAssets.MagicPixel.Value;

            Color accent = Color.Gold;

            int width = 340;
            int height = 98;

            Rectangle panel =
                new Rectangle(
                    (Main.screenWidth - width) / 2,
                    (int)(Main.screenHeight * 0.2f - rise),
                    width,
                    height);

            spriteBatch.Draw(
                pixel,
                panel,
                EterniaUI.PanelBackground * (0.92f * alpha));

            spriteBatch.Draw(
                pixel,
                new Rectangle(panel.X, panel.Y, panel.Width, 3),
                accent * (0.85f * alpha));

            EterniaUI.DrawBorder(
                spriteBatch,
                panel,
                accent * (0.7f * alpha));

            EterniaUI.DrawCenteredText(
                spriteBatch,
                "LEVEL UP!",
                new Rectangle(panel.X, panel.Y + 12, panel.Width, 30),
                accent * alpha,
                0.95f);

            EterniaUI.DrawCenteredText(
                spriteBatch,
                $"Level {bannerLevel}",
                new Rectangle(panel.X, panel.Y + 44, panel.Width, 24),
                Color.White * alpha,
                0.7f);

            EterniaUI.DrawCenteredText(
                spriteBatch,
                $"+{pendingStats} Stats      +{pendingPassives} Passives",
                new Rectangle(panel.X, panel.Y + 68, panel.Width, 22),
                new Color(150, 255, 150) * alpha,
                0.6f);

            return true;
        }
    }
}
