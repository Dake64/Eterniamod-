using System.Collections.Generic;
using System.Linq;
using Eternia.Content.Passives;
using Eternia.Content.Players;
using Eternia.Content.Progression;
using Eternia.Content.Souls;
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
    public class PassiveUI : ModSystem
    {
        private const int SidebarPreferredWidth = 238;
        private const int CompactSidebarHeight = 206;
        private const int HeaderHeight = 76;
        private const int NodeMinHeight = 58;
        private const int NodeMaxHeight = 72;
        private const int PanelInset = 18;

        public static bool Visible;

        private enum PassiveState
        {
            Locked,
            NoPoints,
            Available,
            Unlocked
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (EterniaKeybinds.TogglePassiveUI.JustPressed)
            {
                Visible = !Visible;

                if (Visible)
                {
                    EterniaUI.CloseMajorPanelsExcept(
                        EterniaUI.MajorPanel.Passive);
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
                        "Eternia: Passive UI",
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

            var soulPlayer =
                player.GetModPlayer<EterniaPlayer>();

            SpriteBatch spriteBatch =
                Main.spriteBatch;

            Color accent =
                GetSoulColor(soulPlayer.ActiveSoul);

            Rectangle panel =
                EterniaUI.GetCenteredPanel(1120, 628, 32);

            if (panel.Contains(Main.MouseScreen.ToPoint()))
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            EterniaUI.DrawPanel(spriteBatch, panel, accent);
            EterniaUI.DrawHeader(
                spriteBatch,
                panel,
                "Passive Tree",
                "Choose one direction per class and commit to its promotion path.",
                accent);

            if (EterniaUI.DrawCloseButton(spriteBatch, panel, accent))
            {
                Visible = false;
                return true;
            }

            GetPassiveAreas(
                panel,
                out Rectangle sidebar,
                out Rectangle treeArea,
                out bool compactLayout);

            if (!soulPlayer.HasClassSoul)
            {
                DrawNoClassState(spriteBatch, sidebar, treeArea);
                return true;
            }

            List<PassiveNode> currentTree =
                PassiveRegistry.GetPassivesForSoul(
                    soulPlayer.ActiveSoul);

            DrawSidebar(
                spriteBatch,
                sidebar,
                soulPlayer.ActiveSoul,
                stats,
                level,
                accent,
                compactLayout);

            if (currentTree == null ||
                currentTree.Count == 0)
            {
                DrawEmptyTree(spriteBatch, treeArea, accent);
                return true;
            }

            DrawTree(
                spriteBatch,
                treeArea,
                currentTree,
                stats,
                level);

            return true;
        }

        private static void DrawNoClassState(
            SpriteBatch spriteBatch,
            Rectangle sidebar,
            Rectangle treeArea)
        {
            EterniaUI.DrawPill(
                spriteBatch,
                new Rectangle(sidebar.X, sidebar.Y, sidebar.Width, 30),
                "No class Soul active",
                Color.Gray,
                0.58f);

            EterniaUI.DrawText(
                spriteBatch,
                "Craft and equip Warrior, Mage, Ranger or Summoner Soul before spending passives.",
                new Vector2(treeArea.X + 18, treeArea.Y + 28),
                EterniaUI.MutedText,
                0.66f);
        }

        private static void DrawEmptyTree(
            SpriteBatch spriteBatch,
            Rectangle treeArea,
            Color accent)
        {
            EterniaUI.DrawPanel(
                spriteBatch,
                new Rectangle(treeArea.X, treeArea.Y, treeArea.Width, 92),
                accent,
                0.66f);

            EterniaUI.DrawText(
                spriteBatch,
                "No passives registered for this Soul.",
                new Vector2(treeArea.X + 18, treeArea.Y + 30),
                EterniaUI.MutedText,
                0.7f);
        }

        private static void GetPassiveAreas(
            Rectangle panel,
            out Rectangle sidebar,
            out Rectangle treeArea,
            out bool compactLayout)
        {
            Rectangle content =
                new Rectangle(
                    panel.X + PanelInset,
                    panel.Y + HeaderHeight,
                    panel.Width - PanelInset * 2,
                    panel.Height - HeaderHeight - PanelInset);

            compactLayout =
                content.Width < 690;

            if (compactLayout)
            {
                int sidebarHeight =
                    System.Math.Min(
                        CompactSidebarHeight,
                        System.Math.Max(148, content.Height / 2));

                sidebar =
                    new Rectangle(
                        content.X,
                        content.Y,
                        content.Width,
                        sidebarHeight);

                int remainingTreeHeight =
                    System.Math.Max(
                        0,
                        content.Bottom - sidebar.Bottom - 14);

                treeArea =
                    new Rectangle(
                        content.X,
                        sidebar.Bottom + 14,
                        content.Width,
                        remainingTreeHeight);

                return;
            }

            int sidebarWidth =
                System.Math.Min(
                    SidebarPreferredWidth,
                    System.Math.Max(190, content.Width / 3));

            sidebar =
                new Rectangle(
                    content.X,
                    content.Y,
                    sidebarWidth,
                    content.Height);

            treeArea =
                new Rectangle(
                    sidebar.Right + 22,
                    content.Y,
                    System.Math.Max(180, content.Right - sidebar.Right - 22),
                    content.Height);
        }

        private static void DrawSidebar(
            SpriteBatch spriteBatch,
            Rectangle sidebar,
            SoulId soul,
            EterniaStatsPlayer stats,
            EterniaLevelPlayer level,
            Color accent,
            bool compactLayout)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;

            spriteBatch.Draw(
                pixel,
                sidebar,
                EterniaUI.PanelSurface * 0.72f);

            EterniaUI.DrawBorder(
                spriteBatch,
                sidebar,
                EterniaUI.Border * 0.5f);

            if (compactLayout)
            {
                DrawCompactSidebar(
                    spriteBatch,
                    sidebar,
                    soul,
                    stats,
                    level,
                    accent);

                return;
            }

            EterniaUI.DrawText(
                spriteBatch,
                "Class",
                new Vector2(sidebar.X + 14, sidebar.Y + 14),
                EterniaUI.MutedText,
                0.58f);

            EterniaUI.DrawPill(
                spriteBatch,
                new Rectangle(sidebar.X + 14, sidebar.Y + 36, sidebar.Width - 28, 30),
                SoulRules.GetDisplayName(soul),
                accent,
                0.66f);

            EterniaUI.DrawText(
                spriteBatch,
                "Passive points",
                new Vector2(sidebar.X + 14, sidebar.Y + 82),
                EterniaUI.MutedText,
                0.58f);

            EterniaUI.DrawPill(
                spriteBatch,
                new Rectangle(sidebar.X + 14, sidebar.Y + 104, sidebar.Width - 28, 30),
                level.passivePoints.ToString(),
                Color.MediumPurple,
                0.72f);

            EterniaUI.DrawDivider(
                spriteBatch,
                sidebar.X + 14,
                sidebar.Y + 154,
                sidebar.Width - 28,
                accent);

            EterniaUI.DrawText(
                spriteBatch,
                "Affinities",
                new Vector2(sidebar.X + 14, sidebar.Y + 170),
                Color.White,
                0.62f);

            int y = sidebar.Y + 198;

            foreach (AffinityInfo affinity in GetAffinities(soul, stats))
            {
                Rectangle bar =
                    new Rectangle(
                        sidebar.X + 14,
                        y,
                        sidebar.Width - 28,
                        24);

                EterniaUI.DrawProgressBar(
                    spriteBatch,
                    bar,
                    affinity.Value / 15f,
                    affinity.Color,
                    $"{affinity.Name}: {affinity.Value}");

                y += 31;
            }
        }

        private static void DrawCompactSidebar(
            SpriteBatch spriteBatch,
            Rectangle sidebar,
            SoulId soul,
            EterniaStatsPlayer stats,
            EterniaLevelPlayer level,
            Color accent)
        {
            EterniaUI.DrawText(
                spriteBatch,
                "Class",
                new Vector2(sidebar.X + 14, sidebar.Y + 12),
                EterniaUI.MutedText,
                0.58f);

            int pointWidth =
                System.Math.Min(126, System.Math.Max(82, sidebar.Width / 4));

            EterniaUI.DrawPill(
                spriteBatch,
                new Rectangle(
                    sidebar.X + 14,
                    sidebar.Y + 34,
                    sidebar.Width - pointWidth - 36,
                    30),
                SoulRules.GetDisplayName(soul),
                accent,
                0.66f);

            EterniaUI.DrawPill(
                spriteBatch,
                new Rectangle(
                    sidebar.Right - pointWidth - 14,
                    sidebar.Y + 34,
                    pointWidth,
                    30),
                $"Pts {level.passivePoints}",
                Color.MediumPurple,
                0.58f);

            EterniaUI.DrawDivider(
                spriteBatch,
                sidebar.X + 14,
                sidebar.Y + 78,
                sidebar.Width - 28,
                accent);

            EterniaUI.DrawText(
                spriteBatch,
                "Affinities",
                new Vector2(sidebar.X + 14, sidebar.Y + 92),
                Color.White,
                0.58f);

            AffinityInfo[] affinities =
                GetAffinities(soul, stats).ToArray();

            int columns =
                affinities.Length > 4 && sidebar.Width >= 360
                    ? 3
                    : sidebar.Width >= 520 ? 3 : 2;

            int affinityRows =
                (affinities.Length + columns - 1) / columns;

            int gap = 8;
            int barWidth =
                (sidebar.Width - 28 - gap * (columns - 1)) / columns;

            int affinityTop =
                sidebar.Y + 100;

            int affinityRowStep =
                affinityRows > 1
                    ? System.Math.Max(
                        22,
                        System.Math.Min(
                            26,
                            (sidebar.Bottom - affinityTop - 22) / (affinityRows - 1)))
                    : 26;

            for (int i = 0; i < affinities.Length; i++)
            {
                int column = i % columns;
                int row = i / columns;

                Rectangle bar =
                    new Rectangle(
                        sidebar.X + 14 + column * (barWidth + gap),
                        affinityTop + row * affinityRowStep,
                        barWidth,
                        20);

                EterniaUI.DrawProgressBar(
                    spriteBatch,
                    bar,
                    affinities[i].Value / 15f,
                    affinities[i].Color,
                    $"{affinities[i].Name}: {affinities[i].Value}");
            }
        }

        private static void DrawTree(
            SpriteBatch spriteBatch,
            Rectangle treeArea,
            List<PassiveNode> currentTree,
            EterniaStatsPlayer stats,
            EterniaLevelPlayer level)
        {
            List<AffinityGroup> groups =
                GroupPassivesByAffinity(currentTree);

            List<PassiveLayout> layouts =
                BuildLayouts(groups, treeArea);

            Dictionary<string, PassiveLayout> byName =
                layouts.ToDictionary(
                    layout => layout.Node.Name,
                    layout => layout);

            foreach (AffinityGroup group in groups)
            {
                Texture2D pixel = TextureAssets.MagicPixel.Value;

                spriteBatch.Draw(
                    pixel,
                    group.Bounds,
                    EterniaUI.PanelSurface * 0.58f);

                EterniaUI.DrawBorder(
                    spriteBatch,
                    group.Bounds,
                    group.Color * 0.35f);

                EterniaUI.DrawPill(
                    spriteBatch,
                    new Rectangle(
                        group.Bounds.X + 8,
                        group.Bounds.Y + 8,
                        group.Bounds.Width - 16,
                        24),
                    group.Name,
                    group.Color,
                    0.58f);
            }

            foreach (PassiveLayout layout in layouts)
            {
                PassiveNode passive = layout.Node;

                if (string.IsNullOrEmpty(passive.RequiredPassive) ||
                    !byName.TryGetValue(passive.RequiredPassive, out PassiveLayout parent))
                {
                    continue;
                }

                EterniaUI.DrawConnector(
                    spriteBatch,
                    new Point(parent.Rect.Center.X, parent.Rect.Bottom),
                    new Point(layout.Rect.Center.X, layout.Rect.Y),
                    layout.Color * 0.62f,
                    3);
            }

            foreach (PassiveLayout layout in layouts)
            {
                DrawPassiveNode(
                    spriteBatch,
                    layout.Node,
                    layout.Rect,
                    layout.Color,
                    stats,
                    level);
            }
        }

        private static List<AffinityGroup> GroupPassivesByAffinity(
            List<PassiveNode> passives)
        {
            return passives
                .GroupBy(passive => passive.AffinityType)
                .Select(group => new AffinityGroup(
                    group.Key,
                    group.ToList(),
                    GetAffinityColor(group.Key)))
                .ToList();
        }

        private static List<PassiveLayout> BuildLayouts(
            List<AffinityGroup> groups,
            Rectangle treeArea)
        {
            List<PassiveLayout> layouts =
                new List<PassiveLayout>();

            if (groups.Count == 0 ||
                treeArea.Width <= 0 ||
                treeArea.Height <= 0)
            {
                return layouts;
            }

            int rows =
                groups.Count > 4 ? 2 : 1;

            int columns =
                (groups.Count + rows - 1) / rows;

            int gapX = 14;
            int gapY = 16;
            int groupWidth =
                (treeArea.Width - gapX * (columns - 1)) / columns;

            int groupHeight =
                (treeArea.Height - gapY * (rows - 1)) / rows;

            for (int i = 0; i < groups.Count; i++)
            {
                int row = i / columns;
                int column = i % columns;

                AffinityGroup group = groups[i];

                Rectangle bounds =
                    new Rectangle(
                        treeArea.X + column * (groupWidth + gapX),
                        treeArea.Y + row * (groupHeight + gapY),
                        groupWidth,
                        groupHeight);

                group.Bounds = bounds;

                int nodeCount =
                    group.Nodes.Count;

                int nodeGap =
                    bounds.Height < 180 ? 6 : 8;

                int nodeTopOffset =
                    bounds.Height < 160 ? 34 : 40;

                int availableHeight =
                    System.Math.Max(
                        nodeCount,
                        bounds.Height - nodeTopOffset - (nodeCount - 1) * nodeGap);

                int fittedNodeHeight =
                    availableHeight / System.Math.Max(1, nodeCount);

                int maxFittingNodeHeight =
                    System.Math.Max(24, fittedNodeHeight);

                int nodeHeight =
                    System.Math.Clamp(fittedNodeHeight, 24, NodeMaxHeight);

                if (treeArea.Height >= 440 &&
                    maxFittingNodeHeight >= NodeMinHeight)
                {
                    nodeHeight =
                        System.Math.Max(
                            NodeMinHeight,
                            nodeHeight);
                }

                nodeHeight =
                    System.Math.Min(
                        nodeHeight,
                        maxFittingNodeHeight);

                int nodeWidth =
                    bounds.Width - 18;

                int x =
                    bounds.X + 9;

                int y =
                    bounds.Y + nodeTopOffset;

                foreach (PassiveNode node in group.Nodes)
                {
                    Rectangle rect =
                        new Rectangle(
                            x,
                            y,
                            nodeWidth,
                            nodeHeight);

                    layouts.Add(
                        new PassiveLayout(
                            node,
                            rect,
                            group.Color));

                    y += nodeHeight + nodeGap;
                }
            }

            return layouts;
        }

        private static void DrawPassiveNode(
            SpriteBatch spriteBatch,
            PassiveNode passive,
            Rectangle rect,
            Color accent,
            EterniaStatsPlayer stats,
            EterniaLevelPlayer level)
        {
            PassiveState state =
                GetPassiveState(passive, stats, level);

            Texture2D pixel =
                TextureAssets.MagicPixel.Value;

            Color fill =
                state switch
                {
                    PassiveState.Unlocked => Color.Lerp(accent, Color.Black, 0.35f),
                    PassiveState.Available => Color.Lerp(accent, EterniaUI.PanelSurface, 0.58f),
                    PassiveState.NoPoints => new Color(44, 38, 54),
                    _ => new Color(35, 39, 47)
                };

            spriteBatch.Draw(pixel, rect, fill * 0.94f);
            EterniaUI.DrawBorder(spriteBatch, rect, accent * 0.45f);

            bool compactNode =
                rect.Height < 50 ||
                rect.Width < 128;

            string label =
                state switch
                {
                    PassiveState.Unlocked => compactNode ? "Owned" : "Owned",
                    PassiveState.Available => compactNode ? "Open" : "Unlock",
                    PassiveState.NoPoints => compactNode ? "0 pt" : "No pts",
                    _ => compactNode ? "Lock" : "Locked"
                };

            if (compactNode)
            {
                bool textOnlyNode =
                    rect.Height < 42;

                int compactButtonHeight =
                    textOnlyNode
                        ? 0
                        : System.Math.Clamp(rect.Height - 34, 18, 22);

                EterniaUI.DrawTrimmedText(
                    spriteBatch,
                    passive.Name,
                    new Vector2(rect.X + 8, rect.Y + 7),
                    rect.Width - 16,
                    Color.White,
                    0.5f);

                bool compactClicked =
                    textOnlyNode
                        ? TryConsumeNodeClick(
                            rect,
                            state == PassiveState.Available)
                        : EterniaUI.DrawButton(
                            spriteBatch,
                            new Rectangle(
                                rect.X + 8,
                                rect.Bottom - compactButtonHeight - 6,
                                rect.Width - 16,
                                compactButtonHeight),
                            label,
                            accent,
                            state == PassiveState.Available);

                if (compactClicked)
                {
                    if (ProgressionService.TryUnlockPassive(
                        Main.LocalPlayer,
                        passive))
                    {
                        SoundEngine.PlaySound(SoundID.ResearchComplete);
                    }
                    else
                    {
                        SoundEngine.PlaySound(SoundID.MenuClose);
                    }
                }

                if (rect.Contains(Main.MouseScreen.ToPoint()))
                {
                    Main.LocalPlayer.mouseInterface = true;
                    EterniaUI.DrawTooltip(
                        spriteBatch,
                        passive.Name,
                        GetTooltipLines(passive, state),
                        accent);
                }

                return;
            }

            EterniaUI.DrawTrimmedText(
                spriteBatch,
                passive.Name,
                new Vector2(rect.X + 10, rect.Y + 8),
                rect.Width - 20,
                Color.White,
                0.58f);

            string effect =
                $"+{passive.AffinityAmount} {passive.AffinityType}";

            EterniaUI.DrawTrimmedText(
                spriteBatch,
                effect,
                new Vector2(rect.X + 10, rect.Y + 28),
                System.Math.Max(58, rect.Width - 94),
                EterniaUI.MutedText,
                0.5f);

            Rectangle cost =
                new Rectangle(
                    rect.Right - 52,
                    rect.Y + 27,
                    42,
                    18);

            EterniaUI.DrawPill(
                spriteBatch,
                cost,
                $"{passive.Cost} pt",
                Color.MediumPurple,
                0.46f);

            Rectangle button =
                new Rectangle(
                    rect.Right - 74,
                    rect.Bottom - 25,
                    64,
                    19);

            bool clicked =
                EterniaUI.DrawButton(
                    spriteBatch,
                    button,
                    label,
                    accent,
                    state == PassiveState.Available);

            if (clicked)
            {
                if (ProgressionService.TryUnlockPassive(
                    Main.LocalPlayer,
                    passive))
                {
                    SoundEngine.PlaySound(SoundID.ResearchComplete);
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.MenuClose);
                }
            }

            if (rect.Contains(Main.MouseScreen.ToPoint()))
            {
                Main.LocalPlayer.mouseInterface = true;
                EterniaUI.DrawTooltip(
                    spriteBatch,
                    passive.Name,
                    GetTooltipLines(passive, state),
                    accent);
            }
        }

        private static PassiveState GetPassiveState(
            PassiveNode passive,
            EterniaStatsPlayer stats,
            EterniaLevelPlayer level)
        {
            if (stats.UnlockedPassives.Contains(passive.Name))
            {
                return PassiveState.Unlocked;
            }

            if (!string.IsNullOrEmpty(passive.RequiredPassive) &&
                !stats.UnlockedPassives.Contains(passive.RequiredPassive))
            {
                return PassiveState.Locked;
            }

            if (level.passivePoints < passive.Cost)
            {
                return PassiveState.NoPoints;
            }

            return PassiveState.Available;
        }

        private static bool TryConsumeNodeClick(
            Rectangle rect,
            bool enabled)
        {
            if (!enabled ||
                !rect.Contains(Main.MouseScreen.ToPoint()) ||
                !Main.mouseLeft ||
                !Main.mouseLeftRelease)
            {
                return false;
            }

            Main.mouseLeftRelease = false;
            return true;
        }

        private static IEnumerable<string> GetTooltipLines(
            PassiveNode passive,
            PassiveState state)
        {
            yield return passive.Description;
            yield return $"Cost: {passive.Cost} passive point(s)";
            yield return $"Affinity reward: +{passive.AffinityAmount} {passive.AffinityType}";

            if (!string.IsNullOrEmpty(passive.RequiredPassive))
            {
                yield return $"Requires: {passive.RequiredPassive}";
            }

            yield return state switch
            {
                PassiveState.Unlocked => "Status: already unlocked",
                PassiveState.Available => "Status: ready to unlock",
                PassiveState.NoPoints => "Status: not enough passive points",
                _ => "Status: prerequisite locked"
            };
        }

        private static IEnumerable<AffinityInfo> GetAffinities(
            SoulId soul,
            EterniaStatsPlayer stats)
        {
            return soul switch
            {
                SoulId.Warrior => new[]
                {
                    new AffinityInfo("Bleed", stats.BleedAffinity, GetAffinityColor("Bleed")),
                    new AffinityInfo("Combo", stats.ComboAffinity, GetAffinityColor("Combo")),
                    new AffinityInfo("Defense", stats.DefenseAffinity, GetAffinityColor("Defense")),
                    new AffinityInfo("Precision", stats.PrecisionAffinity, GetAffinityColor("Precision")),
                    new AffinityInfo("Rage", stats.RageAffinity, GetAffinityColor("Rage")),
                    new AffinityInfo("Control", stats.ControlAffinity, GetAffinityColor("Control"))
                },
                SoulId.Mage => new[]
                {
                    new AffinityInfo("Elemental", stats.ElementalAffinity, GetAffinityColor("Elemental")),
                    new AffinityInfo("Curse", stats.CurseAffinity, GetAffinityColor("Curse")),
                    new AffinityInfo("Infinity", stats.InfinityAffinity, GetAffinityColor("Infinity")),
                    new AffinityInfo("Arcane", stats.ArcaneAffinity, GetAffinityColor("Arcane"))
                },
                SoulId.Ranger => new[]
                {
                    new AffinityInfo("Energy", stats.EnergyAffinity, GetAffinityColor("Energy")),
                    new AffinityInfo("Bow", stats.BowAffinity, GetAffinityColor("Bow")),
                    new AffinityInfo("Gun", stats.GunAffinity, GetAffinityColor("Gun")),
                    new AffinityInfo("Music", stats.MusicAffinity, GetAffinityColor("Music"))
                },
                SoulId.Summoner => new[]
                {
                    new AffinityInfo("Beast", stats.BeastAffinity, GetAffinityColor("Beast")),
                    new AffinityInfo("Fusion", stats.FusionAffinity, GetAffinityColor("Fusion")),
                    new AffinityInfo("Tech", stats.TechAffinity, GetAffinityColor("Tech")),
                    new AffinityInfo("Shadow", stats.ShadowAffinity, GetAffinityColor("Shadow"))
                },
                _ => new AffinityInfo[0]
            };
        }

        private static Color GetSoulColor(SoulId soul)
        {
            return soul switch
            {
                SoulId.Warrior => Color.OrangeRed,
                SoulId.Mage => Color.DeepSkyBlue,
                SoulId.Ranger => Color.LimeGreen,
                SoulId.Summoner => Color.MediumPurple,
                _ => Color.Gray
            };
        }

        private static Color GetAffinityColor(string affinity)
        {
            return affinity switch
            {
                "Bleed" => Color.IndianRed,
                "Combo" => Color.Orange,
                "Defense" => Color.LightSkyBlue,
                "Precision" => Color.LimeGreen,
                "Rage" => Color.Red,
                "Control" => Color.Gold,
                "Elemental" => Color.OrangeRed,
                "Curse" => Color.MediumVioletRed,
                "Infinity" => Color.DeepSkyBlue,
                "Arcane" => Color.Plum,
                "Energy" => Color.Cyan,
                "Bow" => Color.ForestGreen,
                "Gun" => Color.Silver,
                "Music" => Color.MediumPurple,
                "Beast" => Color.SandyBrown,
                "Fusion" => Color.LightSteelBlue,
                "Tech" => Color.LightGray,
                "Shadow" => Color.MediumPurple,
                _ => Color.White
            };
        }

        private sealed class AffinityGroup
        {
            public AffinityGroup(
                string name,
                List<PassiveNode> nodes,
                Color color)
            {
                Name = name;
                Nodes = nodes;
                Color = color;
            }

            public string Name { get; }

            public List<PassiveNode> Nodes { get; }

            public Color Color { get; }

            public Rectangle Bounds { get; set; }
        }

        private readonly struct PassiveLayout
        {
            public PassiveLayout(
                PassiveNode node,
                Rectangle rect,
                Color color)
            {
                Node = node;
                Rect = rect;
                Color = color;
            }

            public PassiveNode Node { get; }

            public Rectangle Rect { get; }

            public Color Color { get; }
        }

        private readonly struct AffinityInfo
        {
            public AffinityInfo(
                string name,
                int value,
                Color color)
            {
                Name = name;
                Value = value;
                Color = color;
            }

            public string Name { get; }

            public int Value { get; }

            public Color Color { get; }
        }
    }
}
