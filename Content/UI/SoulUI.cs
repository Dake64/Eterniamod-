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
                        384,
                        500));

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

            int y = content.Y + 42;

            // CLASS: overall combat identity (this already folds in subclass bonuses).
            foreach (SoulMetric metric in GetClassMetrics(player, soulPlayer))
            {
                DrawMetric(
                    spriteBatch,
                    new Rectangle(content.X, y, content.Width, 42),
                    metric);

                y += 46;
            }

            // SUBCLASS: what the promotion specifically gives + its live resource.
            y += 6;

            DrawSectionLabel(
                spriteBatch,
                new Rectangle(content.X, y, content.Width, 18),
                $"SUBCLASS - {subclass}".ToUpperInvariant(),
                accent);

            y += 26;

            foreach (SoulMetric metric in GetSubclassMetrics(player, subclass))
            {
                DrawMetric(
                    spriteBatch,
                    new Rectangle(content.X, y, content.Width, 42),
                    metric);

                y += 46;
            }
        }

        private static void DrawSectionLabel(
            SpriteBatch spriteBatch,
            Rectangle rect,
            string text,
            Color accent)
        {
            EterniaUI.DrawText(
                spriteBatch,
                text,
                new Vector2(rect.X + 2, rect.Y),
                accent * 0.95f,
                0.5f);

            Texture2D pixel =
                Terraria.GameContent.TextureAssets.MagicPixel.Value;

            spriteBatch.Draw(
                pixel,
                new Rectangle(rect.X, rect.Bottom + 2, rect.Width, 1),
                accent * 0.30f);
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

        private static SoulMetric[] GetClassMetrics(
            Player player,
            EterniaPlayer soulPlayer)
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
                    new SoulMetric("Speed", $"{(int)(player.GetAttackSpeed(DamageClass.Melee) * 100)}% attack speed", Color.LightGreen)
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
                    new SoulMetric("Casting", $"{critChance}% crit | {(int)(player.GetAttackSpeed(DamageClass.Magic) * 100)}% cast speed", Color.Plum)
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
                    new SoulMetric("Cadence", $"{(int)(player.GetAttackSpeed(DamageClass.Ranged) * 100)}% attack speed", Color.LightGreen)
                };
            }

            float summonDamage =
                player.GetDamage(DamageClass.Summon).ApplyTo(100f) - 100f;

            return new[]
            {
                new SoulMetric("Survival", $"HP {player.statLifeMax2} | DEF {player.statDefense}", Color.MediumPurple),
                new SoulMetric("Damage", $"+{(int)summonDamage}% summon | +{(int)genericDamage}% all", Color.Orange),
                new SoulMetric("Command", $"{player.maxMinions} minions | {critChance}% crit", Color.SandyBrown),
                new SoulMetric("Whip", $"{(int)(player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) * 100)}% whip speed", Color.LightSteelBlue)
            };
        }

        private static SoulMetric[] GetSubclassMetrics(
            Player player,
            string subclass)
        {
            return new[]
            {
                new SoulMetric("Specialty", SubclassSpecialty(subclass), Color.Gold),
                new SoulMetric("Subclass bonus", SubclassBonus(subclass), Color.MediumPurple),
                new SoulMetric("Resource", SubclassResourceText(player, subclass), Color.SkyBlue)
            };
        }

        // Themed one-liner: what this subclass's mechanic is about.
        private static string SubclassSpecialty(string subclass) => subclass switch
        {
            "Warrior" => "Not promoted - build affinity to promote",
            "Mage" => "Not promoted - build affinity to promote",
            "Ranger" => "Not promoted - build affinity to promote",
            "Summoner" => "Not promoted - build affinity to promote",
            "Swordsman" => "Bleed swords + Crimson Execution",
            "Fighter" => "Aggressive brawler; passives shape the Combo",
            "Guardian" => "Shield Defensive Auras + heavy defense",
            "Yoyo Master" => "Precision stacks into True Strike",
            "Berserker" => "Rage builds into an Overrage frenzy",
            "Stunner" => "Charge melee to stun on hit",
            "Archer" => "Focus into an empowered Perfect Shot",
            "Gunner" => "Hit the sweet spot for Dead Eye",
            "Energy Gunner" => "Ride the 70-99% heat zone; don't overheat",
            "Virtuoso" => "Play note melodies for buffs",
            "Elementalist" => "Swap 5 elemental affinities that reshape all magic",
            "Cursed Mage" => "Cursed energy + risky corruption",
            "Infinity Mage" => "Overflow into free-cast Overload",
            "Arcane Bard" => "Keep the Crescendo momentum going",
            "Necromancer" => "Reserve life + mana to raise a soul-collected army",
            "Beast Tamer" => "Ferocity into a Primal Roar frenzy",
            "Advanced Summoner" => "Fill the roster, then Overclock the cap",
            "Tech Summoner" => "Charge a Power Core for Overdrive",
            _ => "-"
        };

        // The flat stat bonus the subclass grants (mirrors SubclassEffectsPlayer).
        private static string SubclassBonus(string subclass) => subclass switch
        {
            "Swordsman" => "+10% melee, +5 armor pen",
            "Fighter" => "+12% melee speed, +10% move",
            "Guardian" => "+8 defense, +8% damage reduction",
            "Yoyo Master" => "+10% melee crit, yoyo string",
            "Berserker" => "Scales as your HP drops",
            "Stunner" => "+2 knockback, +8% melee",
            "Archer" => "+15% arrow damage, +8% crit",
            "Gunner" => "+12% ranged speed, +8% damage",
            "Energy Gunner" => "+12% ranged, +5% crit",
            "Virtuoso" => "+20 max mana, +5% move",
            "Elementalist" => "+15% magic, +20 max mana",
            "Cursed Mage" => "+20% magic, -20 max HP",
            "Infinity Mage" => "-15% mana cost, +40 mana",
            "Arcane Bard" => "+8% move, +25 max mana",
            "Necromancer" => "+10% magic, +20 max mana",
            "Beast Tamer" => "+15% summon, +1 minion",
            "Advanced Summoner" => "+2 minions, +10% summon speed",
            "Tech Summoner" => "+12% summon, +5 defense",
            _ => "-"
        };

        // The subclass's live resource. Un-promoted base classes have no resource at
        // all (removed on purpose); each promotion shows its own mechanic's value.
        private static string SubclassResourceText(
            Player player,
            string subclass)
        {
            switch (subclass)
            {
                case "Warrior":
                case "Mage":
                case "Ranger":
                case "Summoner":
                    return "None - promote to gain one";

                case "Swordsman":
                    return $"Crimson Trail {player.GetModPlayer<CrimsonTrailPlayer>().CrimsonTrail}/100";
                case "Fighter":
                    var fighter = player.GetModPlayer<FighterPlayer>();
                    return $"Combo {fighter.Combo}/{fighter.EffectiveMaxCombo}";
                case "Guardian":
                    bool auraUp = player.ownedProjectileCounts[
                        ModContent.ProjectileType
                            <Eternia.Content.Projectiles.Guardian.DefensiveAuraProjectile>()] > 0;
                    return auraUp ? "Shield Aura: ON" : "Shield Aura: raise a shield";
                case "Yoyo Master":
                    return $"Precision {player.GetModPlayer<YoyoMasterPlayer>().precisionStacks}/5";
                case "Berserker":
                    var berserker = player.GetModPlayer<BerserkerPlayer>();
                    return berserker.Overrage
                        ? $"Rage {berserker.Rage} (OVERRAGE)"
                        : $"Rage {berserker.Rage}";
                case "Stunner":
                    var stunner = player.GetModPlayer<StunnerPlayer>();
                    return $"Charge {stunner.Charge}/{stunner.MaxCharge}";

                case "Archer":
                    return $"Focus {(int)player.GetModPlayer<ArcherPlayer>().Focus}/100";
                case "Gunner":
                    return $"Dead Eye {(int)player.GetModPlayer<GunnerPlayer>().DeadEyeEnergy}/100";
                case "Energy Gunner":
                    var eg = player.GetModPlayer<EnergyShooterPlayer>();
                    return eg.Overheated
                        ? "Heat: OVERHEATED"
                        : $"Heat {(int)eg.HeatPercent}% ({(eg.Zone == 2 ? "critical" : eg.Zone == 1 ? "hot" : "stable")})";
                case "Virtuoso":
                    return $"Notes {player.GetModPlayer<VirtuosoPlayer>().Notes.Count}/3";

                case "Elementalist":
                    var elementalist = player.GetModPlayer<ElementalistPlayer>();
                    return $"{elementalist.GetCurrentElement()} charge {elementalist.ElementCharge}/100";
                case "Cursed Mage":
                    return $"Cursed Energy {player.GetModPlayer<CursedMagePlayer>().CursedEnergy}/100";
                case "Infinity Mage":
                    return $"Overflow {(int)player.GetModPlayer<InfinityMagePlayer>().Overflow}/100";
                case "Arcane Bard":
                    return $"Crescendo {(int)player.GetModPlayer<ArcaneBardPlayer>().Crescendo}/100";

                case "Necromancer":
                    var necro = player.GetModPlayer<NecromancerPlayer>();
                    return $"Reserved life {(int)(necro.ReservedLifeFraction * 100)}% ({necro.ActiveNecroSummons} undead)";
                case "Beast Tamer":
                    return $"Ferocity {(int)player.GetModPlayer<BeastTamerPlayer>().Ferocity}/100";
                case "Advanced Summoner":
                    return $"Command {(int)player.GetModPlayer<AdvancedSummonerPlayer>().Command}/100";
                case "Tech Summoner":
                    return $"Power Core {(int)player.GetModPlayer<TechSummonerPlayer>().PowerCore}/100";

                default:
                    return "-";
            }
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
