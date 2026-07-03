using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

using Eternia.Content.Players;

namespace Eternia.Content.UI
{
    public class StatsUI : ModSystem
    {
        public static bool Visible;

        public override void UpdateUI(GameTime gameTime)
        {
            if (EterniaKeybinds.ToggleStatsUI.JustPressed)
            {
                Visible = !Visible;
            }
        }

        public override void ModifyInterfaceLayers(
            System.Collections.Generic.List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex =
                layers.FindIndex(
                    layer => layer.Name.Equals("Vanilla: Mouse Text")
                );

            if (mouseTextIndex != -1)
            {
                layers.Insert(
                    mouseTextIndex,
                    new LegacyGameInterfaceLayer(
                        "Eternia: Stats UI",
                        DrawUI,
                        InterfaceScaleType.UI
                    )
                );
            }
        }

        private bool DrawUI()
        {
            if (!Visible)
            {
                return true;
            }

            Player player = Main.LocalPlayer;

            var stats =
                player.GetModPlayer<EterniaStatsPlayer>();

            var level =
                player.GetModPlayer<EterniaLevelPlayer>();

            SpriteBatch spriteBatch = Main.spriteBatch;

            // =================================================
            // PANEL
            // =================================================

            Rectangle panel = new Rectangle(
                500,
                180,
                360,
                430
            );

            Texture2D texture =
                TextureAssets.MagicPixel.Value;

            spriteBatch.Draw(
                texture,
                panel,
                Color.Black * 0.75f
            );

            // =================================================
            // TITLE
            // =================================================

            Utils.DrawBorderString(
                spriteBatch,
                "ETERNIA STATS",
                new Vector2(panel.X + 80, panel.Y + 15),
                Color.Gold,
                1.1f
            );

            // =================================================
            // LEVEL
            // =================================================

            Utils.DrawBorderString(
                spriteBatch,
                $"Level: {level.level}",
                new Vector2(panel.X + 20, panel.Y + 55),
                Color.White
            );

            // =================================================
            // AVAILABLE POINTS
            // =================================================

            Utils.DrawBorderString(
                spriteBatch,
                $"Available Points: {stats.StatPoints}",
                new Vector2(panel.X + 20, panel.Y + 85),
                Color.LightGreen
            );
            Utils.DrawBorderString(
                spriteBatch,
                $"Passive Points: {level.passivePoints}",
                new Vector2(panel.X + 20, panel.Y + 110),
                Color.MediumPurple
            );

            // =================================================
            // STATS
            // =================================================

            int startY = panel.Y + 150;

            // ❤️ VITALITY

            DrawStat(
                spriteBatch,
                "Vitality",
                stats.Vitality,
                "+3 Max HP\n+0.1% Damage Reduction",
                panel.X + 20,
                startY,
                Color.Red
            );

            // ⚔ POWER

            DrawStat(
                spriteBatch,
                "Power",
                stats.Power,
                "+0.3% All Damage",
                panel.X + 20,
                startY + 55,
                Color.Orange
            );

            // 🎯 PRECISION

            DrawStat(
                spriteBatch,
                "Precision",
                stats.Precision,
                "+0.15% Critical Chance",
                panel.X + 20,
                startY + 110,
                Color.Yellow
            );

            // 💨 AGILITY

            DrawStat(
                spriteBatch,
                "Agility",
                stats.Agility,
                "+Movement Speed\n+Run Speed",
                panel.X + 20,
                startY + 165,
                Color.LimeGreen
            );

            // 🔵 FOCUS

            DrawStat(
                spriteBatch,
                "Focus",
                stats.Focus,
                "+3 Mana\n+Mana Regen",
                panel.X + 20,
                startY + 220,
                Color.Cyan
            );

            return true;
        }

        private void DrawStat(
            SpriteBatch spriteBatch,
            string name,
            int value,
            string description,
            int x,
            int y,
            Color color)
        {
            Player player = Main.LocalPlayer;

            var stats =
                player.GetModPlayer<EterniaStatsPlayer>();

            // =================================================
            // STAT NAME
            // =================================================

            Utils.DrawBorderString(
                spriteBatch,
                $"{name}: {value}",
                new Vector2(x, y),
                color
            );

            // =================================================
            // DESCRIPTION
            // =================================================

            Utils.DrawBorderString(
                spriteBatch,
                description,
                new Vector2(x + 20, y + 20),
                Color.LightGray,
                0.7f
            );

            // =================================================
            // PLUS BUTTON
            // =================================================

            Rectangle plusButton = new Rectangle(
                x + 220,
                y,
                24,
                24
            );

            Texture2D texture =
                TextureAssets.MagicPixel.Value;

            Color buttonColor =
                plusButton.Contains(Main.MouseScreen.ToPoint())
                ? Color.LightGreen
                : Color.DarkGreen;

            spriteBatch.Draw(
                texture,
                plusButton,
                buttonColor
            );

            Utils.DrawBorderString(
                spriteBatch,
                "+",
                new Vector2(
                    plusButton.X + 5,
                    plusButton.Y - 2
                ),
                Color.White
            );

            // =================================================
            // CLICK
            // =================================================

            if (plusButton.Contains(Main.MouseScreen.ToPoint()))
            {
                Main.LocalPlayer.mouseInterface = true;

                if (Main.mouseLeft &&
                    Main.mouseLeftRelease &&
                    stats.StatPoints > 0)
                {
                    stats.StatPoints--;

                    switch (name)
                    {
                        case "Vitality":
                            stats.Vitality++;
                            break;

                        case "Power":
                            stats.Power++;
                            break;

                        case "Precision":
                            stats.Precision++;
                            break;

                        case "Agility":
                            stats.Agility++;
                            break;

                        case "Focus":
                            stats.Focus++;
                            break;
                    }

                    SoundEngine.PlaySound(
                        SoundID.MenuTick
                    );
                }
            }
        }
    }
}