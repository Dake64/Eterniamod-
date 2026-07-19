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

        // Path-of-Exile-style pannable canvas: the tree is a web of nodes on a big
        // canvas the player drags around. These track the pan offset and the drag
        // gesture (so a drag is not mistaken for a node click).
        private static int panX;
        private static int panY;
        private static bool dragging;
        private static bool dragMoved;
        private static Point dragStart;
        private static Point dragLast;
        private static bool prevLeft;

        // Default zoom sits near the top of the range so node labels are readable
        // the moment the tree opens; players wheel-zoom OUT to 0.4 for the overview.
        private static float zoom = 1f;
        private const float MinZoom = 0.3f;
        private const float MaxZoom = 1.4f;

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
                    panX = 0;
                    panY = 0;
                    zoom = 1f;
                    dragging = false;
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

            if (!soulPlayer.HasClassSoulNow)
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
                level,
                accent);

            EterniaUI.DrawQueuedTooltip(spriteBatch);

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

            // ---- Milestones: every 5 nodes grants a special class bonus ----
            var milestonePlayer =
                Main.LocalPlayer.GetModPlayer<MilestonePlayer>();

            int milestones = milestonePlayer.Milestones;
            int intoNext =
                stats.UnlockedPassives.Count % MilestonePlayer.NodesPerMilestone;

            y += 10;

            EterniaUI.DrawDivider(
                spriteBatch, sidebar.X + 14, y, sidebar.Width - 28, accent);

            y += 14;

            EterniaUI.DrawText(
                spriteBatch,
                "Milestones",
                new Vector2(sidebar.X + 14, y),
                Color.White,
                0.62f);

            y += 26;

            EterniaUI.DrawPill(
                spriteBatch,
                new Rectangle(sidebar.X + 14, y, sidebar.Width - 28, 28),
                $"{milestones} reached   ({intoNext}/{MilestonePlayer.NodesPerMilestone})",
                Color.Gold,
                0.6f);

            if (milestones > 0)
            {
                y += 34;

                string subclassNow =
                    Main.LocalPlayer.GetModPlayer<SubclassPlayer>().CurrentSubclass;

                EterniaUI.DrawTrimmedText(
                    spriteBatch,
                    $"+{milestones}x {MilestonePlayer.PerkLabel(subclassNow)}",
                    new Vector2(sidebar.X + 14, y),
                    sidebar.Width - 28,
                    new Color(210, 200, 170),
                    0.62f);
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
            EterniaLevelPlayer level,
            Color accent)
        {
            if (treeArea.Width <= 0 || treeArea.Height <= 0)
            {
                return;
            }

            List<AffinityGroup> groups =
                GroupPassivesByAffinity(currentTree);

            List<PassiveLayout> layouts =
                BuildLayouts(groups, treeArea);

            Dictionary<string, PassiveLayout> byName =
                layouts.ToDictionary(
                    layout => layout.Node.Name,
                    layout => layout);

            SoulId activeSoul =
                Main.LocalPlayer.GetModPlayer<EterniaPlayer>().ActiveSoul;

            // How far the farthest node sits from the hub (canvas space).
            int reach = 200;

            foreach (PassiveLayout layout in layouts)
            {
                reach = System.Math.Max(reach, System.Math.Abs(layout.Rect.Center.X));
                reach = System.Math.Max(reach, System.Math.Abs(layout.Rect.Center.Y));
            }

            // ---- Input: drag to pan, mouse wheel to zoom ----
            Point mouse = Main.MouseScreen.ToPoint();
            bool inArea = treeArea.Contains(mouse);

            if (inArea)
            {
                Main.LocalPlayer.mouseInterface = true;

                int wheel = Terraria.GameInput.PlayerInput.ScrollWheelDeltaForUI;
                if (wheel != 0)
                {
                    zoom = System.Math.Clamp(
                        zoom + wheel * 0.0008f, MinZoom, MaxZoom);
                }
            }

            bool left = Main.mouseLeft;
            bool pressStart = left && !prevLeft;

            if (pressStart && inArea)
            {
                dragging = true;
                dragMoved = false;
                dragStart = mouse;
                dragLast = mouse;
            }
            else if (left && dragging)
            {
                panX += mouse.X - dragLast.X;
                panY += mouse.Y - dragLast.Y;

                if (System.Math.Abs(mouse.X - dragStart.X)
                    + System.Math.Abs(mouse.Y - dragStart.Y) > 6)
                {
                    dragMoved = true;
                }

                dragLast = mouse;
            }

            if (!left)
            {
                dragging = false;
            }

            prevLeft = left;

            int scaledReach = (int)(reach * zoom) + 40;
            panX = System.Math.Clamp(panX, -scaledReach, scaledReach);
            panY = System.Math.Clamp(panY, -scaledReach, scaledReach);

            int ox = treeArea.Center.X + panX;
            int oy = treeArea.Center.Y + panY;
            float z = zoom;

            // Maps a canvas-space rect to its on-screen rect at the current pan+zoom.
            Rectangle ToScreen(Rectangle canvasRect)
            {
                int cx = ox + (int)(canvasRect.Center.X * z);
                int cy = oy + (int)(canvasRect.Center.Y * z);
                int w = System.Math.Max(1, (int)(canvasRect.Width * z));
                int h = System.Math.Max(1, (int)(canvasRect.Height * z));
                return new Rectangle(cx - w / 2, cy - h / 2, w, h);
            }

            Texture2D pixel = TextureAssets.MagicPixel.Value;

            // ---- Connectors: one line per prerequisite (or to the hub). ----
            foreach (PassiveLayout layout in layouts)
            {
                Rectangle childRect = ToScreen(layout.Rect);

                List<string> prereqs =
                    PassiveRegistry.GetPrerequisites(activeSoul, layout.Node);

                bool childOwned =
                    stats.UnlockedPassives.Contains(layout.Node.Name);

                bool ready =
                    prereqs.TrueForAll(
                        pr => stats.UnlockedPassives.Contains(pr));

                List<Point> parents = new List<Point>();

                if (prereqs.Count == 0)
                {
                    parents.Add(new Point(ox, oy));
                }
                else
                {
                    foreach (string prereq in prereqs)
                    {
                        if (byName.TryGetValue(prereq, out PassiveLayout parent))
                        {
                            parents.Add(ToScreen(parent.Rect).Center);
                        }
                    }
                }

                foreach (Point parentCenter in parents)
                {
                    if (!treeArea.Contains(childRect.Center) ||
                        !treeArea.Contains(parentCenter))
                    {
                        continue;
                    }

                    Vector2 pA = parentCenter.ToVector2();
                    Vector2 pC = childRect.Center.ToVector2();

                    if (childOwned)
                    {
                        EterniaUI.DrawLine(
                            spriteBatch, pA, pC, layout.Color * 0.30f, 7f * z);
                        EterniaUI.DrawLine(
                            spriteBatch, pA, pC,
                            Color.Lerp(layout.Color, Color.White, 0.35f) * 0.95f, 3f * z);
                    }
                    else if (ready)
                    {
                        float p =
                            0.5f + 0.5f * (float)System.Math.Sin(
                                Main.GlobalTimeWrappedHourly * 3f);

                        EterniaUI.DrawLine(
                            spriteBatch, pA, pC,
                            layout.Color * (0.35f + 0.4f * p), 3f * z);
                    }
                    else
                    {
                        EterniaUI.DrawLine(
                            spriteBatch, pA, pC, layout.Color * 0.28f, 2f * z);
                    }
                }
            }

            // ---- Central hub marker with an arcane glow ----
            if (treeArea.Contains(new Point(ox, oy)))
            {
                float hubPulse =
                    0.5f + 0.5f * (float)System.Math.Sin(
                        Main.GlobalTimeWrappedHourly * 2f);

                int hw = (int)(46 * z);
                int hh = (int)(22 * z);

                Rectangle glow =
                    new Rectangle(ox - (int)(hw * 1.35f), oy - (int)(hh * 1.55f),
                        (int)(hw * 2.7f), (int)(hh * 3.1f));
                spriteBatch.Draw(pixel, glow, accent * (0.12f + 0.08f * hubPulse));

                Rectangle hub =
                    new Rectangle(ox - hw, oy - hh, hw * 2, hh * 2);

                spriteBatch.Draw(pixel, hub, EterniaUI.PanelSurface * 0.96f);
                EterniaUI.DrawBorder(
                    spriteBatch, hub, accent * (0.6f + 0.35f * hubPulse), 2);
                EterniaUI.DrawCenteredText(
                    spriteBatch, "CORE", hub, accent * 0.92f, 0.62f * z);
            }

            // ---- Nodes: reuse the card (click/tooltip); drags suppress clicks ----
            foreach (PassiveLayout layout in layouts)
            {
                Rectangle rect = ToScreen(layout.Rect);

                if (treeArea.Contains(rect))
                {
                    DrawPassiveNode(
                        spriteBatch,
                        layout.Node,
                        rect,
                        layout.Color,
                        stats,
                        level,
                        !dragMoved,
                        z);
                }
            }

            EterniaUI.DrawText(
                spriteBatch,
                "Drag to pan  -  wheel to zoom",
                new Vector2(treeArea.X + 6, treeArea.Bottom - 20),
                EterniaUI.MutedText * 0.8f,
                0.5f);
        }

        // === v1 subclass gating =====================================================
        // For the v1 release only these affinity branches are shown in the passive tree
        // (and their affinity meters). The rest are HIDDEN, not deleted: the passive
        // data, player mechanics and weapons of the hidden subclasses all stay in the
        // code -- re-enabling one later is just adding its affinity back to this set.
        //   Warrior : Bleed (Swordsman), Combo (Fighter), Defense (Guardian/Escudero)
        //   Mage    : Elemental (5 element sub-branches Fire/Ice/Lightning/Wind/Earth),
        //             Curse, Infinity (the Infinity path promotes to the NECROMANCER)
        //             (Arcane / Arcane Bard hidden)
        //   Ranger  : Energy, Bow, Gun             (Music / Virtuoso hidden)
        //   Summoner: Beast, Fusion, Tech          (Shadow hidden -- the Necromancer
        //             moved to the Mage tree)
        private static readonly HashSet<string> V1VisibleAffinities =
            new HashSet<string>
            {
                "Bleed", "Combo", "Defense",
                // Elemental: the sidebar meter ("Elemental") plus its five element spokes.
                "Elemental", "Fire", "Ice", "Lightning", "Wind", "Earth",
                "Curse", "Infinity",
                "Energy", "Bow", "Gun",
                "Beast", "Fusion", "Tech"
            };

        private static bool IsAffinityVisible(string affinity) =>
            V1VisibleAffinities.Contains(affinity);

        private static List<AffinityGroup> GroupPassivesByAffinity(
            List<PassiveNode> passives)
        {
            return passives
                .Where(passive => IsAffinityVisible(passive.AffinityType))
                .GroupBy(passive => passive.AffinityType)
                .Select(group => new AffinityGroup(
                    group.Key,
                    group.ToList(),
                    GetAffinityColor(group.Key)))
                .ToList();
        }

        // Minor and Notable share one card size so the tree reads uniform; only the
        // Keystone is larger (it's the gold capstone). Styling still distinguishes
        // them, not size.
        private static int CardWidth(PassiveKind kind) => kind switch
        {
            PassiveKind.Keystone => 238,
            _ => 198
        };

        private static int CardHeight(PassiveKind kind) => kind switch
        {
            PassiveKind.Keystone => 58,
            _ => 40
        };

        // TierStep is a tier's full footprint (card diagonal + gap). Spacing two
        // adjacent tiers by half of each of their TierSteps guarantees the cards
        // can't overlap no matter which way the spoke points, because a card's
        // diagonal is the largest distance from its center to a corner.
        private static float TierStep(PassiveKind kind) => kind switch
        {
            PassiveKind.Keystone => 263f, // sqrt(238^2+58^2) + gap
            _ => 220f                     // sqrt(198^2+40^2) + gap
        };

        private static float LaneSpacing(PassiveKind kind) => kind switch
        {
            PassiveKind.Keystone => 259f,
            _ => 216f
        };

        // Grow node text WITHOUT widening the card: wrap the name across up to two
        // lines (the cards have spare vertical room) and center the block. A short
        // name stays one big centered line; a long two-word name splits instead of
        // shrinking or truncating.
        private static void DrawNodeLabel(
            SpriteBatch spriteBatch,
            string text,
            Rectangle rect,
            Color color,
            float baseScale,
            float uiScale)
        {
            float scale = baseScale * uiScale;
            int maxWidth = rect.Width - (int)(12 * uiScale);

            if (maxWidth <= 4 || string.IsNullOrEmpty(text))
            {
                return;
            }

            List<string> lines = EterniaUI.WrapText(text, maxWidth, scale);

            if (lines.Count == 0)
            {
                return;
            }

            if (lines.Count > 2)
            {
                lines = lines.GetRange(0, 2);
            }

            int lineStep = (int)(19 * scale);
            int blockHeight = lines.Count * lineStep;
            int startY = rect.Y + (rect.Height - blockHeight) / 2;

            for (int i = 0; i < lines.Count; i++)
            {
                float lineWidth =
                    FontAssets.MouseText.Value.MeasureString(lines[i]).X * scale;

                int x = rect.X + (int)((rect.Width - lineWidth) / 2f);

                EterniaUI.DrawText(
                    spriteBatch,
                    lines[i],
                    new Vector2(x, startY + i * lineStep),
                    color,
                    scale);
            }
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

            int branchCount = groups.Count;

            // Radial web. Each affinity branch is a spoke; nodes march outward by
            // tier, and each tier's spacing/size follows its node KIND -- Minor
            // "path" nodes are small dots packed tightly, Notables are cards, and the
            // Keystone at the end is large. Positions are canvas-space (hub at 0,0).
            float hubRadius = 150f;

            for (int b = 0; b < branchCount; b++)
            {
                double angle =
                    -System.Math.PI / 2.0
                    + 2.0 * System.Math.PI * b / branchCount;

                float dirX = (float)System.Math.Cos(angle);
                float dirY = (float)System.Math.Sin(angle);

                float perpX = -dirY;
                float perpY = dirX;

                List<List<PassiveNode>> tiers =
                    PassiveRegistry.BuildTiers(groups[b].Nodes);

                // Each tier's dominant kind drives both its own lane spacing and how
                // far the spoke must step to reach it, so a wide Keystone gets extra
                // clearance from the Minor tier before it (single-kind stepping used
                // to let the keystone overlap the last path node).
                PassiveKind[] tierKinds = new PassiveKind[tiers.Count];
                for (int t = 0; t < tiers.Count; t++)
                {
                    PassiveKind k = PassiveKind.Minor;
                    foreach (PassiveNode n in tiers[t])
                    {
                        if (n.Kind > k)
                        {
                            k = n.Kind;
                        }
                    }
                    tierKinds[t] = k;
                }

                float radius =
                    hubRadius + TierStep(tierKinds[0]) * 0.5f;

                for (int t = 0; t < tiers.Count; t++)
                {
                    List<PassiveNode> tierNodes = tiers[t];
                    int count = tierNodes.Count;

                    float laneSpacing = LaneSpacing(tierKinds[t]);

                    for (int j = 0; j < count; j++)
                    {
                        float laneOffset =
                            (j - (count - 1) / 2f) * laneSpacing;

                        int cx = (int)System.Math.Round(
                            dirX * radius + perpX * laneOffset);
                        int cy = (int)System.Math.Round(
                            dirY * radius + perpY * laneOffset);

                        int w = CardWidth(tierNodes[j].Kind);
                        int h = CardHeight(tierNodes[j].Kind);

                        Rectangle rect =
                            new Rectangle(cx - w / 2, cy - h / 2, w, h);

                        layouts.Add(
                            new PassiveLayout(
                                tierNodes[j], rect, groups[b].Color));
                    }

                    // Half of this tier's footprint + half of the next tier's:
                    // neither card can reach the other regardless of spoke angle.
                    if (t < tiers.Count - 1)
                    {
                        radius += 0.5f *
                            (TierStep(tierKinds[t]) + TierStep(tierKinds[t + 1]));
                    }
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
            EterniaLevelPlayer level,
            bool allowClicks,
            float uiScale)
        {
            if (passive.Kind == PassiveKind.Minor)
            {
                DrawMinorNode(
                    spriteBatch, passive, rect, accent, stats, level,
                    allowClicks, uiScale);
                return;
            }

            if (passive.Kind == PassiveKind.Keystone)
            {
                DrawKeystoneNode(
                    spriteBatch, passive, rect, accent, stats, level,
                    allowClicks, uiScale);
                return;
            }

            PassiveState state =
                GetPassiveState(passive, stats, level);

            Texture2D pixel =
                TextureAssets.MagicPixel.Value;

            // Brightness has to MEAN something: owned is brightest, then affordable, then
            // priced-out, then locked. It used to darken owned nodes while washing available
            // ones toward the panel, so the ladder was not monotonic and you could not read
            // your own progress off the tree at a glance.
            Color fill =
                state switch
                {
                    PassiveState.Unlocked => Color.Lerp(accent, Color.White, 0.10f),
                    PassiveState.Available => Color.Lerp(accent, EterniaUI.PanelSurface, 0.55f),

                    // Amber, deliberately a different HUE. "You qualify, you just cannot
                    // afford it" is a different kind of no from "not yet", and the old tint
                    // (44,38,54) sat so close to the locked (35,39,47) that the difference
                    // read as a rendering glitch instead of information.
                    PassiveState.NoPoints => new Color(74, 58, 30),

                    _ => new Color(28, 31, 38)
                };

            // RPG flourish: soft halo behind owned/available nodes, a gentle pulse
            // on nodes you can take right now, and a crafted double frame.
            float pulse =
                0.5f + 0.5f * (float)System.Math.Sin(
                    Main.GlobalTimeWrappedHourly * 3f + rect.X * 0.05f);

            int haloPad = System.Math.Max(2, (int)(5 * uiScale));

            if (state == PassiveState.Unlocked)
            {
                Rectangle halo = rect;
                halo.Inflate(haloPad, haloPad);
                spriteBatch.Draw(pixel, halo, accent * 0.30f);
            }
            else if (state == PassiveState.Available)
            {
                Rectangle halo = rect;
                halo.Inflate(haloPad, haloPad);
                spriteBatch.Draw(pixel, halo, accent * (0.14f + 0.18f * pulse));
            }

            spriteBatch.Draw(pixel, rect, fill * 0.96f);

            Rectangle innerBevel = rect;
            innerBevel.Inflate(-2, -2);
            EterniaUI.DrawBorder(spriteBatch, innerBevel, Color.White * 0.10f);

            Color frame =
                state switch
                {
                    // The pulse is what draws the eye to what you can take right now; it does
                    // the attention-grabbing so owned nodes don't have to compete for it.
                    PassiveState.Available => accent * (0.5f + 0.4f * pulse),
                    PassiveState.Unlocked => Color.Lerp(accent, Color.White, 0.25f),
                    PassiveState.NoPoints => new Color(214, 162, 74) * 0.75f,
                    _ => accent * 0.35f
                };

            EterniaUI.DrawBorder(spriteBatch, rect, frame);

            float unscaledHeight = rect.Height / System.Math.Max(0.01f, uiScale);
            float unscaledWidth = rect.Width / System.Math.Max(0.01f, uiScale);

            bool compactNode =
                unscaledHeight < 50f ||
                unscaledWidth < 128f;

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
                    unscaledHeight < 42f;

                int compactButtonHeight =
                    textOnlyNode
                        ? 0
                        : System.Math.Clamp(rect.Height - 34, 18, 22);

                DrawNodeLabel(
                    spriteBatch,
                    passive.Name,
                    rect,
                    Color.White,
                    0.95f,
                    uiScale);

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

                if (compactClicked && allowClicks)
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
                    EterniaUI.QueueTooltip(
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

            if (clicked && allowClicks)
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
                EterniaUI.QueueTooltip(
                    passive.Name,
                    GetTooltipLines(passive, state),
                    accent);
            }
        }

        private static void DrawMinorNode(
            SpriteBatch spriteBatch,
            PassiveNode passive,
            Rectangle rect,
            Color accent,
            EterniaStatsPlayer stats,
            EterniaLevelPlayer level,
            bool allowClicks,
            float uiScale)
        {
            PassiveState state = GetPassiveState(passive, stats, level);
            Texture2D pixel = TextureAssets.MagicPixel.Value;

            float pulse =
                0.5f + 0.5f * (float)System.Math.Sin(
                    Main.GlobalTimeWrappedHourly * 3f + rect.X * 0.05f);

            Color fill =
                state switch
                {
                    PassiveState.Unlocked => accent,
                    PassiveState.Available => Color.Lerp(accent, EterniaUI.PanelSurface, 0.4f),
                    PassiveState.NoPoints => new Color(52, 46, 64),
                    _ => new Color(40, 42, 52)
                };

            int glowPad = System.Math.Max(2, (int)(4 * uiScale));

            if (state == PassiveState.Unlocked)
            {
                Rectangle glow = rect;
                glow.Inflate(glowPad, glowPad);
                spriteBatch.Draw(pixel, glow, accent * 0.35f);
            }
            else if (state == PassiveState.Available)
            {
                Rectangle glow = rect;
                glow.Inflate(glowPad, glowPad);
                spriteBatch.Draw(pixel, glow, accent * (0.12f + 0.22f * pulse));
            }

            spriteBatch.Draw(pixel, rect, fill * 0.95f);

            Color frame =
                state == PassiveState.Available
                    ? accent * (0.5f + 0.4f * pulse)
                    : state == PassiveState.Unlocked
                        ? accent * 0.9f
                        : accent * 0.4f;

            EterniaUI.DrawBorder(spriteBatch, rect, frame);

            // Show the node name. The affinity prefix is dropped (it's obvious from
            // the branch) so "Bleed Adept" reads as just "Adept".
            string label =
                passive.Name.StartsWith(passive.AffinityType + " ")
                    ? passive.Name.Substring(passive.AffinityType.Length + 1)
                    : passive.Name;

            DrawNodeLabel(
                spriteBatch,
                label,
                rect,
                Color.White * 0.95f,
                0.95f,
                uiScale);

            if (TryConsumeNodeClick(rect, state == PassiveState.Available) && allowClicks)
            {
                if (ProgressionService.TryUnlockPassive(Main.LocalPlayer, passive))
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
                EterniaUI.QueueTooltip(passive.Name, GetTooltipLines(passive, state), accent);
            }
        }

        private static void DrawKeystoneNode(
            SpriteBatch spriteBatch,
            PassiveNode passive,
            Rectangle rect,
            Color accent,
            EterniaStatsPlayer stats,
            EterniaLevelPlayer level,
            bool allowClicks,
            float uiScale)
        {
            PassiveState state = GetPassiveState(passive, stats, level);
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            Color gold = Color.Gold;

            float pulse =
                0.5f + 0.5f * (float)System.Math.Sin(Main.GlobalTimeWrappedHourly * 3f);

            Color fill =
                state switch
                {
                    PassiveState.Unlocked => Color.Lerp(accent, Color.Black, 0.30f),
                    PassiveState.Available => Color.Lerp(accent, EterniaUI.PanelSurface, 0.45f),
                    PassiveState.NoPoints => new Color(50, 42, 60),
                    _ => new Color(38, 40, 50)
                };

            int glowPad = System.Math.Max(4, (int)(8 * uiScale));
            Rectangle glow = rect;
            glow.Inflate(glowPad, glowPad);
            float glowStrength = state == PassiveState.Locked ? 0.08f : 0.18f;
            spriteBatch.Draw(pixel, glow, gold * (glowStrength * (0.7f + 0.3f * pulse)));

            spriteBatch.Draw(pixel, rect, fill * 0.97f);

            Rectangle inner = rect;
            inner.Inflate(-3, -3);
            EterniaUI.DrawBorder(spriteBatch, inner, Color.White * 0.14f);
            EterniaUI.DrawBorder(spriteBatch, rect, gold * (0.6f + 0.4f * pulse), 2);

            Rectangle keystoneNameRect =
                new Rectangle(
                    rect.X,
                    rect.Y,
                    rect.Width,
                    rect.Height - (int)(20 * uiScale));

            DrawNodeLabel(
                spriteBatch,
                passive.Name,
                keystoneNameRect,
                Color.White,
                0.95f,
                uiScale);

            string keystoneTag = "KEYSTONE";
            float keystoneTagScale = 0.5f * uiScale;
            float keystoneTagWidth =
                FontAssets.MouseText.Value.MeasureString(keystoneTag).X * keystoneTagScale;

            EterniaUI.DrawText(
                spriteBatch,
                keystoneTag,
                new Vector2(
                    rect.X + (rect.Width - keystoneTagWidth) / 2f,
                    rect.Bottom - (int)(14 * uiScale)),
                gold * 0.85f,
                keystoneTagScale);

            if (TryConsumeNodeClick(rect, state == PassiveState.Available) && allowClicks)
            {
                if (ProgressionService.TryUnlockPassive(Main.LocalPlayer, passive))
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
                EterniaUI.QueueTooltip(passive.Name, GetTooltipLines(passive, state), accent);
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

            SoulId soul =
                Main.LocalPlayer.GetModPlayer<EterniaPlayer>().ActiveSoul;

            foreach (string prerequisite in
                PassiveRegistry.GetPrerequisites(soul, passive))
            {
                if (!stats.UnlockedPassives.Contains(prerequisite))
                {
                    return PassiveState.Locked;
                }
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

            SoulId soul =
                Main.LocalPlayer.GetModPlayer<EterniaPlayer>().ActiveSoul;

            var reqStats =
                Main.LocalPlayer.GetModPlayer<EterniaStatsPlayer>();

            List<string> prereqs =
                PassiveRegistry.GetPrerequisites(soul, passive);

            if (prereqs.Count == 1)
            {
                bool met = reqStats.UnlockedPassives.Contains(prereqs[0]);
                yield return "Requires: " + prereqs[0]
                    + (met ? " (owned)" : "");
            }
            else if (prereqs.Count > 1)
            {
                yield return "Requires ALL of:";

                foreach (string pr in prereqs)
                {
                    bool met = reqStats.UnlockedPassives.Contains(pr);
                    yield return "  - " + pr + (met ? " (owned)" : "");
                }
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
            AffinityInfo[] all = soul switch
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

            // v1: only the shipped subclass branches get an affinity meter.
            return all.Where(a => IsAffinityVisible(a.Name));
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
                "Fire" => Color.OrangeRed,
                "Ice" => Color.Cyan,
                "Lightning" => Color.Yellow,
                "Wind" => Color.PaleGreen,
                "Earth" => Color.SandyBrown,
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
