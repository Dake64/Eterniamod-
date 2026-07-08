using Eternia.Content.Players;
using Eternia.Content.Progression;
using Eternia.Content.Souls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Eternia.Content.UI
{
    public class ClassProgressionUI : ModSystem
    {
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
                        "Eternia: Class Progression UI",
                        DrawUI,
                        InterfaceScaleType.UI
                    )
                );
            }
        }

        private bool DrawUI()
        {
            Player player = Main.LocalPlayer;

            if (!EterniaUI.ShouldDrawPlayerUI(player))
            {
                return true;
            }

            var soul =
                player.GetModPlayer<EterniaPlayer>();

            if (!soul.HasClassSoul)
            {
                return true;
            }

            var stats =
                player.GetModPlayer<EterniaStatsPlayer>();

            ClassAffinitySnapshot affinities =
                CreateAffinitySnapshot(stats);

            string baseClass =
                ClassPromotionRules.GetBaseClassName(soul.ActiveSoul);

            var subclassPlayer =
                player.GetModPlayer<SubclassPlayer>();

            string subclass =
                subclassPlayer.CurrentSubclass;

            string lockedPromotion =
                subclassPlayer.GetLockedPromotion(soul.ActiveSoul);

            bool pathLocked =
                ClassPromotionRules.IsPromotionForSoul(
                    soul.ActiveSoul,
                    lockedPromotion);

            string dominantAffinity =
                ClassPromotionRules.GetDominantAffinityName(
                    soul.ActiveSoul,
                    affinities);

            int dominantValue =
                ClassPromotionRules.GetDominantAffinityValue(
                    soul.ActiveSoul,
                    affinities);

            string status =
                GetPromotionStatus(
                    baseClass,
                    subclass,
                    dominantValue,
                    pathLocked);

            string detail =
                GetPromotionDetail(
                    dominantAffinity,
                    dominantValue,
                    lockedPromotion,
                    pathLocked);

            Color accent =
                GetSoulColor(soul.ActiveSoul);

            SpriteBatch spriteBatch =
                Main.spriteBatch;

            Rectangle panel =
                EterniaUI.GetTopCenterPanel(
                    368,
                    72,
                    76);

            EterniaUI.DrawPanel(spriteBatch, panel, accent, 0.82f);

            EterniaUI.DrawTrimmedText(
                spriteBatch,
                $"{baseClass} -> {subclass}",
                new Vector2(panel.X + 14, panel.Y + 11),
                panel.Width - 28,
                Color.White,
                0.66f);

            EterniaUI.DrawPill(
                spriteBatch,
                new Rectangle(panel.X + 14, panel.Y + 38, 128, 22),
                status,
                accent,
                0.48f);

            EterniaUI.DrawPill(
                spriteBatch,
                new Rectangle(panel.X + 152, panel.Y + 38, panel.Width - 166, 22),
                detail,
                pathLocked ? accent : GetAffinityColor(dominantAffinity),
                0.48f);

            return true;
        }

        private static ClassAffinitySnapshot CreateAffinitySnapshot(
            EterniaStatsPlayer stats)
        {
            return new ClassAffinitySnapshot(
                stats.BleedAffinity,
                stats.ComboAffinity,
                stats.DefenseAffinity,
                stats.PrecisionAffinity,
                stats.RageAffinity,
                stats.ControlAffinity,
                stats.EnergyAffinity,
                stats.BowAffinity,
                stats.GunAffinity,
                stats.MusicAffinity,
                stats.ElementalAffinity,
                stats.CurseAffinity,
                stats.InfinityAffinity,
                stats.ArcaneAffinity,
                stats.BeastAffinity,
                stats.FusionAffinity,
                stats.TechAffinity,
                stats.ShadowAffinity);
        }

        private static string GetPromotionStatus(
            string baseClass,
            string subclass,
            int dominantValue,
            bool pathLocked)
        {
            if (!Main.hardMode)
            {
                return "BASE CLASS";
            }

            if (pathLocked)
            {
                return "PATH LOCKED";
            }

            if (subclass == baseClass ||
                dominantValue <= 0)
            {
                return "PROMOTION WAITING";
            }

            return "PROMOTED";
        }

        private static string GetPromotionDetail(
            string dominantAffinity,
            int dominantValue,
            string lockedPromotion,
            bool pathLocked)
        {
            return pathLocked
                ? $"Locked: {lockedPromotion}"
                : $"{dominantAffinity}: {dominantValue}";
        }

        private static Color GetSoulColor(SoulId soul)
        {
            return soul switch
            {
                SoulId.Warrior => Color.OrangeRed,
                SoulId.Mage => Color.DeepSkyBlue,
                SoulId.Ranger => Color.LimeGreen,
                SoulId.Summoner => Color.MediumPurple,
                _ => Color.White
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
    }
}
