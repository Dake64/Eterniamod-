using Eternia.Content.Players;
using Eternia.Content.Souls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Eternia.Content.UI
{
    public class SoulUI : UIState
    {
        private bool dragging;
        private Vector2 offset;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Player player = Main.LocalPlayer;

            if (!EterniaUI.ShouldDrawPlayerUI(player))
            {
                dragging = false;
                return;
            }

            var soulPlayer =
                player.GetModPlayer<EterniaPlayer>();

            if (!soulPlayer.HasAnySoul)
            {
                dragging = false;
                return;
            }

            Vector2 panelPos =
                soulPlayer.soulUIPosition;

            Rectangle panel =
                EterniaUI.ClampToScreen(
                    new Rectangle(
                        (int)panelPos.X,
                        (int)panelPos.Y,
                        368,
                        402));

            panelPos =
                new Vector2(panel.X, panel.Y);

            if (!Main.mouseLeft)
            {
                dragging = false;
            }

            if (dragging)
            {
                panel =
                    EterniaUI.ClampToScreen(
                        new Rectangle(
                            (int)(Main.MouseScreen.X - offset.X),
                            (int)(Main.MouseScreen.Y - offset.Y),
                            panel.Width,
                            panel.Height));

                soulPlayer.soulUIPosition =
                    new Vector2(panel.X, panel.Y);

                panelPos =
                    new Vector2(panel.X, panel.Y);
            }

            if (panel.Contains(Main.MouseScreen.ToPoint()) ||
                dragging)
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            Color accent =
                GetSoulColor(soulPlayer.ActiveSoul);

            EterniaUI.DrawPanel(spriteBatch, panel, accent);
            EterniaUI.DrawHeader(
                spriteBatch,
                panel,
                SoulRules.GetDisplayName(soulPlayer.ActiveSoul),
                "Soul status and combat identity.",
                accent);

            if (EterniaUI.DrawCloseButton(spriteBatch, panel, accent))
            {
                dragging = false;
                SoulUISystem.CloseSoulPanel();
                return;
            }

            Rectangle dragHandle =
                new Rectangle(
                    panel.X,
                    panel.Y,
                    panel.Width - 48,
                    64);

            if (dragHandle.Contains(Main.MouseScreen.ToPoint()) &&
                Main.mouseLeft &&
                Main.mouseLeftRelease &&
                !dragging)
            {
                Main.mouseLeftRelease = false;
                dragging = true;
                offset = Main.MouseScreen - panelPos;
            }

            Rectangle content =
                new Rectangle(
                    panel.X + 18,
                    panel.Y + 74,
                    panel.Width - 36,
                    panel.Height - 92);

            string subclass =
                player.GetModPlayer<SubclassPlayer>().CurrentSubclass;

            EterniaUI.DrawPill(
                spriteBatch,
                new Rectangle(content.X, content.Y, content.Width, 28),
                $"Subclass: {subclass}",
                accent,
                0.58f);

            if (!soulPlayer.HasClassSoul)
            {
                EterniaUI.DrawWrappedText(
                    spriteBatch,
                    "Empty Soul is equipped. Choose a base class to enable EXP, passives and weapon rules.",
                    new Rectangle(
                        content.X,
                        content.Y + 48,
                        content.Width,
                        content.Height - 48),
                    EterniaUI.MutedText,
                    0.62f);
                return;
            }

            var baseClass =
                player.GetModPlayer<BaseClassPlayer>();

            int y = content.Y + 44;

            foreach (SoulMetric metric in GetMetrics(player, soulPlayer, baseClass, subclass))
            {
                DrawMetric(
                    spriteBatch,
                    new Rectangle(content.X, y, content.Width, 42),
                    metric);

                y += 48;
            }
        }

        private static void DrawMetric(
            SpriteBatch spriteBatch,
            Rectangle rect,
            SoulMetric metric)
        {
            Texture2D pixel =
                Terraria.GameContent.TextureAssets.MagicPixel.Value;

            spriteBatch.Draw(
                pixel,
                rect,
                EterniaUI.PanelSurface * 0.76f);

            EterniaUI.DrawBorder(
                spriteBatch,
                rect,
                metric.Color * 0.35f);

            EterniaUI.DrawText(
                spriteBatch,
                metric.Label,
                new Vector2(rect.X + 10, rect.Y + 7),
                EterniaUI.MutedText,
                0.52f);

            EterniaUI.DrawTrimmedText(
                spriteBatch,
                metric.Value,
                new Vector2(rect.X + 10, rect.Y + 22),
                rect.Width - 20,
                Color.White,
                0.62f);
        }

        private static SoulMetric[] GetMetrics(
            Player player,
            EterniaPlayer soulPlayer,
            BaseClassPlayer baseClass,
            string subclass)
        {
            float genericDamage =
                player.GetDamage(DamageClass.Generic).ApplyTo(100f) - 100f;

            float critChance =
                player.GetCritChance(DamageClass.Generic);

            if (soulPlayer.warriorSoul)
            {
                float meleeDamage =
                    player.GetDamage(DamageClass.Melee).ApplyTo(100f) - 100f;

                return new[]
                {
                    new SoulMetric("Survival", $"HP {player.statLifeMax2} | DEF {player.statDefense}", Color.IndianRed),
                    new SoulMetric("Damage", $"+{(int)meleeDamage}% melee | +{(int)genericDamage}% all", Color.OrangeRed),
                    new SoulMetric("Combat", $"{critChance}% crit | {player.GetArmorPenetration(DamageClass.Melee)} armor pen", Color.Gold),
                    new SoulMetric("Speed", $"{(int)(player.GetAttackSpeed(DamageClass.Melee) * 100)}% attack speed", Color.LightGreen),
                    new SoulMetric("Base resource", GetResourceText(subclass == "Warrior", "Momentum", baseClass.WarriorMomentum), Color.Orange)
                };
            }

            if (soulPlayer.mageSoul)
            {
                float magicDamage =
                    player.GetDamage(DamageClass.Magic).ApplyTo(100f) - 100f;

                return new[]
                {
                    new SoulMetric("Survival", $"HP {player.statLifeMax2} | DEF {player.statDefense}", Color.DeepSkyBlue),
                    new SoulMetric("Mana", $"{player.statManaMax2} max | {player.manaRegen} regen", Color.Cyan),
                    new SoulMetric("Damage", $"+{(int)magicDamage}% magic | +{(int)genericDamage}% all", Color.DeepSkyBlue),
                    new SoulMetric("Casting", $"{critChance}% crit | {(int)(player.GetAttackSpeed(DamageClass.Magic) * 100)}% cast speed", Color.Plum),
                    new SoulMetric("Base resource", GetResourceText(subclass == "Mage", "Charge", baseClass.MageCharge), Color.Cyan)
                };
            }

            if (soulPlayer.rangerSoul)
            {
                float rangedDamage =
                    player.GetDamage(DamageClass.Ranged).ApplyTo(100f) - 100f;

                return new[]
                {
                    new SoulMetric("Survival", $"HP {player.statLifeMax2} | DEF {player.statDefense}", Color.LimeGreen),
                    new SoulMetric("Damage", $"+{(int)rangedDamage}% ranged | +{(int)genericDamage}% all", Color.LimeGreen),
                    new SoulMetric("Aim", $"{critChance}% crit | ammo save {(player.ammoCost80 ? 20 : 0)}%", Color.Gold),
                    new SoulMetric("Cadence", $"{(int)(player.GetAttackSpeed(DamageClass.Ranged) * 100)}% attack speed", Color.LightGreen),
                    new SoulMetric("Base resource", GetResourceText(subclass == "Ranger", "Focus", baseClass.RangerFocus), Color.ForestGreen)
                };
            }

            float summonDamage =
                player.GetDamage(DamageClass.Summon).ApplyTo(100f) - 100f;

            return new[]
            {
                new SoulMetric("Survival", $"HP {player.statLifeMax2} | DEF {player.statDefense}", Color.MediumPurple),
                new SoulMetric("Damage", $"+{(int)summonDamage}% summon | +{(int)genericDamage}% all", Color.Orange),
                new SoulMetric("Command", $"{player.maxMinions} minions | {critChance}% crit", Color.SandyBrown),
                new SoulMetric("Whip", $"{(int)(player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) * 100)}% whip speed", Color.LightSteelBlue),
                new SoulMetric("Base resource", GetResourceText(subclass == "Summoner", "Bond", baseClass.SummonerBond), Color.MediumPurple)
            };
        }

        private static string GetResourceText(
            bool visible,
            string name,
            int value)
        {
            return visible
                ? $"{name} {value}/100"
                : "Inactive after promotion";
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

        private readonly struct SoulMetric
        {
            public SoulMetric(
                string label,
                string value,
                Color color)
            {
                Label = label;
                Value = value;
                Color = color;
            }

            public string Label { get; }

            public string Value { get; }

            public Color Color { get; }
        }
    }
}
