using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Terraria;

namespace Eternia.Content.Affixes
{
    // WEAPON AFFIXES. Every weapon you obtain rolls a rarity, and that rarity decides how many
    // sub-stats it carries. Same Common -> Nightmare ladder the enemy rarities use, so the scale
    // reads the same everywhere in the mod: a Legendary weapon is as special as a Legendary enemy.
    //
    // Rarity controls the NUMBER of sub-stats, not their size. That keeps a lucky Common from
    // out-rolling a Legendary, and makes a high rarity feel categorically better, not just bigger.
    public enum AffixTier : byte
    {
        Common,
        Uncommon,
        Rare,
        SuperRare,
        Legendary,
        Mythic,
        Ancient,
        Nightmare
    }

    public enum AffixKind : byte
    {
        Damage,
        Crit,
        ArmorPen,
        AttackSpeed,
        Knockback,
        MoveSpeed
    }

    public readonly struct RolledAffix
    {
        public RolledAffix(AffixKind kind, int value)
        {
            Kind = kind;
            Value = value;
        }

        public AffixKind Kind { get; }

        public int Value { get; }

        public string Describe()
        {
            return Kind switch
            {
                AffixKind.Damage => $"+{Value}% damage",
                AffixKind.Crit => $"+{Value}% critical chance",
                AffixKind.ArmorPen => $"+{Value} armor penetration",
                AffixKind.AttackSpeed => $"+{Value}% attack speed",
                AffixKind.Knockback => $"+{Value}% knockback",
                AffixKind.MoveSpeed => $"+{Value}% movement speed while held",
                _ => ""
            };
        }
    }

    public static class AffixTable
    {
        // Cumulative roll thresholds. Deliberately mirrors the (fixed) boss curve: Common is the
        // norm and a high roll is a real event.
        //   Common 55% | Uncommon 22% | Rare 12% | SuperRare 6% | Legendary 3% | Mythic 1.4%
        //   | Ancient 0.5% | Nightmare 0.1%
        private static readonly (float MaxRoll, AffixTier Tier)[] Odds =
        {
            (0.001f, AffixTier.Nightmare),
            (0.006f, AffixTier.Ancient),
            (0.02f, AffixTier.Mythic),
            (0.05f, AffixTier.Legendary),
            (0.11f, AffixTier.SuperRare),
            (0.23f, AffixTier.Rare),
            (0.45f, AffixTier.Uncommon),
            (1f, AffixTier.Common)
        };

        // Roll ranges per affix (inclusive). Values stay modest: several small sub-stats should add
        // up to a meaningful weapon, not replace the weapon's own tier.
        private static readonly Dictionary<AffixKind, (int Min, int Max)> Ranges = new()
        {
            { AffixKind.Damage, (3, 12) },
            { AffixKind.Crit, (2, 8) },
            { AffixKind.ArmorPen, (2, 7) },
            { AffixKind.AttackSpeed, (3, 10) },
            { AffixKind.Knockback, (5, 20) },
            { AffixKind.MoveSpeed, (2, 6) }
        };

        public static AffixTier RollTier()
        {
            float roll = Main.rand.NextFloat();

            foreach ((float maxRoll, AffixTier tier) in Odds)
            {
                if (roll <= maxRoll)
                {
                    return tier;
                }
            }

            return AffixTier.Common;
        }

        public static int AffixCount(AffixTier tier)
        {
            return tier switch
            {
                AffixTier.Uncommon => 1,
                AffixTier.Rare => 2,
                AffixTier.SuperRare => 2,
                AffixTier.Legendary => 3,
                AffixTier.Mythic => 4,
                AffixTier.Ancient => 5,
                AffixTier.Nightmare => 6,
                _ => 0
            };
        }

        // Picks distinct affixes -- a weapon never rolls the same sub-stat twice.
        public static List<RolledAffix> RollAffixes(int count)
        {
            List<RolledAffix> result = new List<RolledAffix>();

            if (count <= 0)
            {
                return result;
            }

            List<AffixKind> pool = new List<AffixKind>(Ranges.Keys);

            for (int i = 0; i < count && pool.Count > 0; i++)
            {
                int index = Main.rand.Next(pool.Count);
                AffixKind kind = pool[index];
                pool.RemoveAt(index);

                (int min, int max) = Ranges[kind];
                result.Add(new RolledAffix(kind, Main.rand.Next(min, max + 1)));
            }

            return result;
        }

        public static string TierName(AffixTier tier)
        {
            return tier switch
            {
                AffixTier.Uncommon => "Uncommon",
                AffixTier.Rare => "Rare",
                AffixTier.SuperRare => "Super Rare",
                AffixTier.Legendary => "Legendary",
                AffixTier.Mythic => "Mythic",
                AffixTier.Ancient => "Ancient",
                AffixTier.Nightmare => "Nightmare",
                _ => "Common"
            };
        }

        // Same colours as the enemy rarity badges, so the scale reads identically everywhere.
        public static Color TierColor(AffixTier tier)
        {
            return tier switch
            {
                AffixTier.Uncommon => Color.LightBlue,
                AffixTier.Rare => Color.LightGreen,
                AffixTier.SuperRare => Color.Gold,
                AffixTier.Legendary => Color.OrangeRed,
                AffixTier.Mythic => new Color(200, 70, 255),
                AffixTier.Ancient => new Color(60, 230, 210),
                AffixTier.Nightmare => new Color(210, 24, 44),
                _ => Color.LightGray
            };
        }
    }
}
