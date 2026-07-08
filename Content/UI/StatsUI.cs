using Eternia.Content.Players;
using Eternia.Content.Progression;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

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

                if (Visible)
                {
                    EterniaUI.CloseMajorPanelsExcept(
                        EterniaUI.MajorPanel.Stats);
                }
            }
        }

        public override void Unload()
        {
            Visible = false;
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

            if (!EterniaUI.ShouldDrawPlayerUI(player))
            {
                return true;
            }

            var stats =
                player.GetModPlayer<EterniaStatsPlayer>();

            var level =
                player.GetModPlayer<EterniaLevelPlayer>();

            SpriteBatch spriteBatch =
                Main.spriteBatch;

            Rectangle panel =
                EterniaUI.GetCenteredPanel(486, 520, 32);

            if (panel.Contains(Main.MouseScreen.ToPoint()))
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            Color accent =
                Color.Gold;

            EterniaUI.DrawPanel(spriteBatch, panel, accent);
            EterniaUI.DrawHeader(
                spriteBatch,
                panel,
                "Character Stats",
                "Spend class stat points on permanent growth.",
                accent);

            if (EterniaUI.DrawCloseButton(spriteBatch, panel, accent))
            {
                Visible = false;
                return true;
            }

            Rectangle pointBar =
                new Rectangle(panel.X + 18, panel.Y + 78, panel.Width - 36, 36);

            Texture2D pixel =
                TextureAssets.MagicPixel.Value;

            spriteBatch.Draw(
                pixel,
                pointBar,
                EterniaUI.PanelSurface * 0.78f);

            EterniaUI.DrawBorder(
                spriteBatch,
                pointBar,
                EterniaUI.Border * 0.45f);

            DrawSummaryPills(
                spriteBatch,
                pointBar,
                level,
                stats);

            int rowsTop =
                panel.Y + 132;

            int rowGap =
                panel.Height < 460 ? 6 : 10;

            int rowWidth = panel.Width - 36;
            int x = panel.X + 18;

            StatRow[] rows =
            {
                new StatRow("Vitality", stats.Vitality, "+3 Max HP, +0.1% damage reduction", Color.IndianRed),
                new StatRow("Power", stats.Power, "+0.3% all damage", Color.Orange),
                new StatRow("Precision", stats.Precision, "+0.15% critical chance", Color.Yellow),
                new StatRow("Agility", stats.Agility, "+Movement speed and run speed", Color.LimeGreen),
                new StatRow("Focus", stats.Focus, "+3 Mana and mana regeneration", Color.Cyan)
            };

            int availableRowsHeight =
                System.Math.Max(
                    0,
                    panel.Bottom - rowsTop - 18);

            int fittedRowHeight =
                (availableRowsHeight - rowGap * (rows.Length - 1)) /
                rows.Length;

            int rowHeight =
                System.Math.Clamp(fittedRowHeight, 28, 66);

            for (int i = 0; i < rows.Length; i++)
            {
                int y =
                    rowsTop + i * (rowHeight + rowGap);

                int fittedHeight =
                    System.Math.Min(
                        rowHeight,
                        panel.Bottom - 12 - y);

                if (fittedHeight < 24)
                {
                    break;
                }

                StatRow row =
                    rows[i];

                DrawStatCard(
                    spriteBatch,
                    player,
                    new Rectangle(x, y, rowWidth, fittedHeight),
                    row.Name,
                    row.Value,
                    row.Description,
                    row.Color);
            }

            EterniaUI.DrawQueuedTooltip(spriteBatch);

            return true;
        }

        private static void DrawSummaryPills(
            SpriteBatch spriteBatch,
            Rectangle pointBar,
            EterniaLevelPlayer level,
            EterniaStatsPlayer stats)
        {
            string[] labels =
            {
                $"Level {level.level}",
                $"Stats {stats.StatPoints}",
                $"Passives {level.passivePoints}"
            };

            Color[] colors =
            {
                Color.DeepSkyBlue,
                Color.LightGreen,
                Color.MediumPurple
            };

            int gap = 8;
            int x = pointBar.X + 10;
            int width =
                System.Math.Max(
                    70,
                    (pointBar.Width - 20 - gap * (labels.Length - 1)) /
                    labels.Length);

            for (int i = 0; i < labels.Length; i++)
            {
                int pillRight =
                    i == labels.Length - 1
                        ? pointBar.Right - 10
                        : x + width;

                Rectangle pill =
                    new Rectangle(
                        x,
                        pointBar.Y + 6,
                        System.Math.Max(48, pillRight - x),
                        24);

                EterniaUI.DrawPill(
                    spriteBatch,
                    pill,
                    labels[i],
                    colors[i],
                    0.54f);

                x += width + gap;
            }
        }

        private static void DrawStatCard(
            SpriteBatch spriteBatch,
            Player player,
            Rectangle rect,
            string name,
            int value,
            string description,
            Color accent)
        {
            Texture2D pixel =
                TextureAssets.MagicPixel.Value;

            var stats =
                player.GetModPlayer<EterniaStatsPlayer>();

            bool canSpend =
                stats.StatPoints > 0;

            bool compact =
                rect.Height < 52;

            bool hover =
                rect.Contains(Main.MouseScreen.ToPoint());

            if (hover)
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            spriteBatch.Draw(
                pixel,
                rect,
                hover
                    ? EterniaUI.PanelSurfaceAlt * 0.94f
                    : EterniaUI.PanelSurface * 0.82f);

            EterniaUI.DrawBorder(
                spriteBatch,
                rect,
                accent * 0.4f);

            int actionWidth =
                System.Math.Clamp(rect.Width / 3, 88, 126);

            int buttonSize =
                compact ? 22 : 24;

            int actionY =
                rect.Y + System.Math.Max(4, (rect.Height - buttonSize) / 2);

            Rectangle button =
                new Rectangle(
                    rect.Right - 14 - buttonSize,
                    actionY,
                    buttonSize + 10,
                    buttonSize);

            Rectangle valuePill =
                new Rectangle(
                    rect.Right - actionWidth,
                    actionY,
                    System.Math.Max(34, actionWidth - button.Width - 8),
                    buttonSize);

            float titleScale =
                compact ? 0.58f : 0.68f;

            EterniaUI.DrawTrimmedText(
                spriteBatch,
                name,
                new Vector2(
                    rect.X + 14,
                    rect.Y + (compact ? 7 : 10)),
                rect.Width - actionWidth - 24,
                Color.White,
                titleScale);

            if (!compact)
            {
                EterniaUI.DrawTrimmedText(
                    spriteBatch,
                    description,
                    new Vector2(rect.X + 14, rect.Y + 34),
                    rect.Width - actionWidth - 26,
                    EterniaUI.MutedText,
                    0.52f);
            }

            EterniaUI.DrawPill(
                spriteBatch,
                valuePill,
                value.ToString(),
                accent,
                compact ? 0.56f : 0.66f);

            if (EterniaUI.DrawButton(
                spriteBatch,
                button,
                "+",
                accent,
                canSpend) &&
                ProgressionService.TrySpendStatPoint(
                    player,
                    GetStatId(name)))
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
            }

            if (hover)
            {
                EterniaUI.QueueTooltip(
                    name,
                    new[]
                    {
                        description,
                        canSpend
                            ? "Click + to spend one stat point."
                            : "No stat points available."
                    },
                    accent);
            }
        }

        private static StatId GetStatId(string name)
        {
            return name switch
            {
                "Vitality" => StatId.Vitality,
                "Power" => StatId.Power,
                "Precision" => StatId.Precision,
                "Agility" => StatId.Agility,
                _ => StatId.Focus
            };
        }

        private readonly struct StatRow
        {
            public StatRow(
                string name,
                int value,
                string description,
                Color color)
            {
                Name = name;
                Value = value;
                Description = description;
                Color = color;
            }

            public string Name { get; }

            public int Value { get; }

            public string Description { get; }

            public Color Color { get; }
        }
    }
}
