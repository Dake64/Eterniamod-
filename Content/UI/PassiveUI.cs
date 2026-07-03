using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

using Eternia.Content.Passives;
using Eternia.Content.Players;

namespace Eternia.Content.UI
{
    public class PassiveUI : ModSystem
    {
        public static bool Visible;

        public override void UpdateUI(GameTime gameTime)
        {
            if (EterniaKeybinds.TogglePassiveUI.JustPressed)
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

            var stats =
                player.GetModPlayer<EterniaStatsPlayer>();

            var level =
                player.GetModPlayer<EterniaLevelPlayer>();

            var soulPlayer =
                player.GetModPlayer<EterniaPlayer>();

            SpriteBatch spriteBatch =
                Main.spriteBatch;

            Texture2D texture =
                TextureAssets.MagicPixel.Value;

            // =================================================
            // PANEL
            // =================================================

            Rectangle panel = new Rectangle(
                700,
                120,
                700,
                620
            );

            spriteBatch.Draw(
                texture,
                panel,
                Color.Black * 0.8f
            );

            // =================================================
            // TITLE
            // =================================================

            Utils.DrawBorderString(
                spriteBatch,
                "PASSIVE TREE",
                new Vector2(panel.X + 100, panel.Y + 20),
                Color.Gold,
                1.1f
            );

            // =================================================
            // PASSIVE POINTS
            // =================================================

            Utils.DrawBorderString(
                spriteBatch,
                $"Passive Points: {level.passivePoints}",
                new Vector2(panel.X + 20, panel.Y + 60),
                Color.MediumPurple
            );

            int affinityY = panel.Y + 90;

            // =================================================
            // CURRENT TREE
            // =================================================

            System.Collections.Generic.List<PassiveNode>
                currentTree = null;

            // =================================================
            // WARRIOR
            // =================================================

            if (soulPlayer.warriorSoul)
            {
                currentTree =
                    PassiveRegistry.WarriorPassives;

                Utils.DrawBorderString(
                    spriteBatch,
                    $"Bleed: {stats.BleedAffinity}",
                    new Vector2(panel.X + 220, affinityY),
                    Color.IndianRed,
                    0.7f
                );

                Utils.DrawBorderString(
                    spriteBatch,
                    $"Combo: {stats.ComboAffinity}",
                    new Vector2(panel.X + 220, affinityY + 18),
                    Color.Orange,
                    0.7f
                );

                Utils.DrawBorderString(
                    spriteBatch,
                    $"Defense: {stats.DefenseAffinity}",
                    new Vector2(panel.X + 220, affinityY + 36),
                    Color.LightBlue,
                    0.7f
                );

                Utils.DrawBorderString(
                    spriteBatch,
                    $"Precision: {stats.PrecisionAffinity}",
                    new Vector2(panel.X + 220, affinityY + 54),
                    Color.LimeGreen,
                    0.7f
                );

                Utils.DrawBorderString(
                    spriteBatch,
                    $"Rage: {stats.RageAffinity}",
                    new Vector2(panel.X + 220, affinityY + 72),
                    Color.Red,
                    0.7f
                );

                Utils.DrawBorderString(
                    spriteBatch,
                    $"Control: {stats.ControlAffinity}",
                    new Vector2(panel.X + 220, affinityY + 90),
                    Color.Gold,
                    0.7f
                );
            }

            // =================================================
            // RANGER
            // =================================================

            else if (soulPlayer.rangerSoul)
            {
                currentTree =
                    PassiveRegistry.RangerPassives;

                Utils.DrawBorderString(
                    spriteBatch,
                    $"Energy: {stats.EnergyAffinity}",
                    new Vector2(panel.X + 220, affinityY),
                    Color.Cyan,
                    0.7f
                );

                Utils.DrawBorderString(
                    spriteBatch,
                    $"Bow: {stats.BowAffinity}",
                    new Vector2(panel.X + 220, affinityY + 18),
                    Color.ForestGreen,
                    0.7f
                );

                Utils.DrawBorderString(
                    spriteBatch,
                    $"Gun: {stats.GunAffinity}",
                    new Vector2(panel.X + 220, affinityY + 36),
                    Color.Silver,
                    0.7f
                );

                Utils.DrawBorderString(
                    spriteBatch,
                    $"Music: {stats.MusicAffinity}",
                    new Vector2(panel.X + 220, affinityY + 54),
                    Color.MediumPurple,
                    0.7f
                );
            }

            // =================================================
            // MAGE
            // =================================================

            else if (soulPlayer.mageSoul)
            {
                currentTree =
                    PassiveRegistry.MagePassives;

                Utils.DrawBorderString(
                    spriteBatch,
                    $"Elemental: {stats.ElementalAffinity}",
                    new Vector2(panel.X + 220, affinityY),
                    Color.OrangeRed,
                    0.7f
                );

                Utils.DrawBorderString(
                    spriteBatch,
                    $"Card: {stats.CardAffinity}",
                    new Vector2(panel.X + 220, affinityY + 18),
                    Color.Gold,
                    0.7f
                );

                Utils.DrawBorderString(
                    spriteBatch,
                    $"Curse: {stats.CurseAffinity}",
                    new Vector2(panel.X + 220, affinityY + 36),
                    Color.MediumVioletRed,
                    0.7f
                );

                Utils.DrawBorderString(
                    spriteBatch,
                    $"Necro: {stats.NecroAffinity}",
                    new Vector2(panel.X + 220, affinityY + 54),
                    Color.DarkOliveGreen,
                    0.7f
                );

                Utils.DrawBorderString(
                    spriteBatch,
                    $"Infinity: {stats.InfinityAffinity}",
                    new Vector2(panel.X + 220, affinityY + 72),
                    Color.DeepSkyBlue,
                    0.7f
                );

                Utils.DrawBorderString(
                    spriteBatch,
                    $"Arcane: {stats.ArcaneAffinity}",
                    new Vector2(panel.X + 220, affinityY + 90),
                    Color.Plum,
                    0.7f
                );
            }

            // =================================================
            // SUMMONER
            // =================================================

            else if (soulPlayer.summonerSoul)
            {
                currentTree =
                    PassiveRegistry.SummonerPassives;

                Utils.DrawBorderString(
                    spriteBatch,
                    $"Beast: {stats.BeastAffinity}",
                    new Vector2(panel.X + 220, affinityY),
                    Color.SandyBrown,
                    0.7f
                );

                Utils.DrawBorderString(
                    spriteBatch,
                    $"Fusion: {stats.FusionAffinity}",
                    new Vector2(panel.X + 220, affinityY + 18),
                    Color.LightSteelBlue,
                    0.7f
                );

                Utils.DrawBorderString(
                    spriteBatch,
                    $"Tech: {stats.TechAffinity}",
                    new Vector2(panel.X + 220, affinityY + 36),
                    Color.LightGray,
                    0.7f
                );

                Utils.DrawBorderString(
                    spriteBatch,
                    $"Shadow: {stats.ShadowAffinity}",
                    new Vector2(panel.X + 220, affinityY + 54),
                    Color.MediumPurple,
                    0.7f
                );
            }

            // =================================================
            // PASSIVES
            // =================================================

            // =================================================
// DRAW CONNECTIONS
// =================================================

            if (currentTree != null)
            {
                foreach (var passive in currentTree)
                {
                    if (!string.IsNullOrEmpty(passive.RequiredPassive))
                    {
                        PassiveNode parent = currentTree.Find(
                            p => p.Name == passive.RequiredPassive
                        );

                        if (parent != null)
                        {
                            DrawConnection(
                                spriteBatch,
                                texture,
                                panel.X + parent.X + 180,
                                panel.Y + parent.Y + 55,

                                panel.X + passive.X + 180,
                                panel.Y + passive.Y
                            );
                        }
                    }
                }

                // =================================================
                // DRAW NODES
                // =================================================

                foreach (var passive in currentTree)
                {
                    DrawPassive(
                        spriteBatch,
                        passive,
                        panel.X + passive.X,
                        panel.Y + passive.Y,
                        stats,
                        level
                    );
                }
            }

            return true;
        }

        private void DrawPassive(
    SpriteBatch spriteBatch,
    PassiveNode passive,
    int x,
    int y,
    EterniaStatsPlayer stats,
    EterniaLevelPlayer level)
{
    Texture2D texture =
        TextureAssets.MagicPixel.Value;

    bool unlocked =
        stats.UnlockedPassives.Contains(
            passive.Name
        );

    // =================================================
    // NODE BACKGROUND
    // =================================================

    Rectangle bg = new Rectangle(
        x,
        y,
        90,
        90
    );

    spriteBatch.Draw(
        texture,
        bg,
        unlocked
        ? Color.DarkGreen
        : Color.DarkSlateGray
    );

    // =================================================
    // NODE BORDER
    // =================================================

    Rectangle border = new Rectangle(
        x - 2,
        y - 2,
        94,
        94
    );

    spriteBatch.Draw(
        texture,
        border,
        Color.Black * 0.4f
    );

    spriteBatch.Draw(
        texture,
        bg,
        unlocked
        ? Color.DarkGreen
        : Color.DarkSlateGray
    );

    // =================================================
    // NAME
    // =================================================

    Utils.DrawBorderString(
        spriteBatch,
        passive.Name,
        new Vector2(x + 6, y + 10),
        Color.White,
        0.5f
    );

    // =================================================
    // BUY BUTTON
    // =================================================

    Rectangle button = new Rectangle(
        x + 15,
        y + 58,
        60,
        20
    );

    spriteBatch.Draw(
        texture,
        button,
        unlocked
        ? Color.Gray
        : Color.DarkRed
    );

    Utils.DrawBorderString(
        spriteBatch,
        unlocked ? "OWNED" : "BUY",
        new Vector2(button.X + 8, button.Y + 2),
        Color.White,
        0.55f
    );

    // =================================================
    // TOOLTIP
    // =================================================

    if (bg.Contains(Main.MouseScreen.ToPoint()))
    {
        Main.LocalPlayer.mouseInterface = true;

        Main.hoverItemName =
            passive.Description;

        // =============================================
        // CLICK
        // =============================================

        if (button.Contains(Main.MouseScreen.ToPoint()))
        {
            if (Main.mouseLeft &&
                Main.mouseLeftRelease &&
                !unlocked &&
                level.passivePoints >= passive.Cost)
            {
                // =====================================
                // REQUIRED PASSIVE CHECK
                // =====================================

                bool requirementMet =
                    string.IsNullOrEmpty(
                        passive.RequiredPassive
                    )
                    ||
                    stats.UnlockedPassives.Contains(
                        passive.RequiredPassive
                    );

                if (!requirementMet)
                {
                    SoundEngine.PlaySound(
                        SoundID.MenuClose
                    );

                    return;
                }

                // =====================================
                // BUY PASSIVE
                // =====================================

                level.passivePoints -= passive.Cost;

                stats.UnlockedPassives.Add(
                    passive.Name
                );

                ApplyAffinity(
                    stats,
                    passive.AffinityType,
                    passive.AffinityAmount
                );

                SoundEngine.PlaySound(
                    SoundID.ResearchComplete
                );
            }
        }
    }
}
        // =================================================
// DRAW CONNECTION
// =================================================

        private void DrawConnection(
            SpriteBatch spriteBatch,
            Texture2D texture,
            int startX,
            int startY,
            int endX,
            int endY)
        {
            Rectangle line = new Rectangle(
                startX,
                startY,
                4,
                endY - startY
            );

            spriteBatch.Draw(
                texture,
                line,
                Color.DarkGray
            );
        }

        private void ApplyAffinity(
            EterniaStatsPlayer stats,
            string affinity,
            int amount)
        {
            switch (affinity)
            {
                // =================================================
                // WARRIOR
                // =================================================

                case "Bleed":
                    stats.BleedAffinity += amount;
                    break;

                case "Combo":
                    stats.ComboAffinity += amount;
                    break;

                case "Defense":
                    stats.DefenseAffinity += amount;
                    break;

                case "Precision":
                    stats.PrecisionAffinity += amount;
                    break;

                case "Rage":
                    stats.RageAffinity += amount;
                    break;

                case "Control":
                    stats.ControlAffinity += amount;
                    break;

                // =================================================
                // RANGER
                // =================================================

                case "Energy":
                    stats.EnergyAffinity += amount;
                    break;

                case "Bow":
                    stats.BowAffinity += amount;
                    break;

                case "Gun":
                    stats.GunAffinity += amount;
                    break;

                case "Music":
                    stats.MusicAffinity += amount;
                    break;

                // =================================================
                // MAGE
                // =================================================

                case "Elemental":
                    stats.ElementalAffinity += amount;
                    break;

                case "Card":
                    stats.CardAffinity += amount;
                    break;

                case "Curse":
                    stats.CurseAffinity += amount;
                    break;

                case "Necro":
                    stats.NecroAffinity += amount;
                    break;

                case "Infinity":
                    stats.InfinityAffinity += amount;
                    break;

                case "Arcane":
                    stats.ArcaneAffinity += amount;
                    break;

                // =================================================
                // SUMMONER
                // =================================================

                case "Beast":
                    stats.BeastAffinity += amount;
                    break;

                case "Fusion":
                    stats.FusionAffinity += amount;
                    break;

                case "Tech":
                    stats.TechAffinity += amount;
                    break;

                case "Shadow":
                    stats.ShadowAffinity += amount;
                    break;
            }
        }
    }
}