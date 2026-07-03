using Eternia.Content.Souls;

namespace Eternia.Content.Progression
{
    public readonly record struct ClassAffinitySnapshot(
        int BleedAffinity,
        int ComboAffinity,
        int DefenseAffinity,
        int PrecisionAffinity,
        int RageAffinity,
        int ControlAffinity,
        int EnergyAffinity,
        int BowAffinity,
        int GunAffinity,
        int MusicAffinity,
        int ElementalAffinity,
        int CurseAffinity,
        int InfinityAffinity,
        int ArcaneAffinity,
        int BeastAffinity,
        int FusionAffinity,
        int TechAffinity,
        int ShadowAffinity)
    {
        public static ClassAffinitySnapshot Empty => default;
    }

    public static class ClassPromotionRules
    {
        public const string None = "None";
        public const string Warrior = "Warrior";
        public const string Mage = "Mage";
        public const string Ranger = "Ranger";
        public const string Summoner = "Summoner";

        public static string ResolveSubclass(
            SoulId activeSoul,
            bool hardMode,
            ClassAffinitySnapshot affinities)
        {
            string baseClass = GetBaseClassName(activeSoul);

            if (baseClass == None ||
                !hardMode)
            {
                return baseClass;
            }

            return activeSoul switch
            {
                SoulId.Warrior => ResolveWarriorPromotion(
                    affinities,
                    baseClass),
                SoulId.Mage => ResolveMagePromotion(
                    affinities,
                    baseClass),
                SoulId.Ranger => ResolveRangerPromotion(
                    affinities,
                    baseClass),
                SoulId.Summoner => ResolveSummonerPromotion(
                    affinities,
                    baseClass),
                _ => baseClass
            };
        }

        public static string ResolveSubclass(
            SoulId activeSoul,
            bool hardMode,
            ClassAffinitySnapshot affinities,
            string lockedPromotion)
        {
            string baseClass = GetBaseClassName(activeSoul);

            if (baseClass == None ||
                !hardMode)
            {
                return baseClass;
            }

            if (IsPromotionForSoul(activeSoul, lockedPromotion))
            {
                return lockedPromotion;
            }

            return ResolveSubclass(
                activeSoul,
                hardMode,
                affinities);
        }

        public static bool IsPromotionForSoul(
            SoulId activeSoul,
            string promotion)
        {
            return activeSoul switch
            {
                SoulId.Warrior => promotion == "Swordsman" ||
                    promotion == "Fighter" ||
                    promotion == "Guardian" ||
                    promotion == "Yoyo Master" ||
                    promotion == "Berserker" ||
                    promotion == "Stunner",
                SoulId.Mage => promotion == "Elementalist" ||
                    promotion == "Cursed Mage" ||
                    promotion == "Infinity Mage" ||
                    promotion == "Arcane Bard",
                SoulId.Ranger => promotion == "Energy Gunner" ||
                    promotion == "Archer" ||
                    promotion == "Gunner" ||
                    promotion == "Virtuoso",
                SoulId.Summoner => promotion == "Beast Tamer" ||
                    promotion == "Advanced Summoner" ||
                    promotion == "Tech Summoner" ||
                    promotion == "Necromancer",
                _ => false
            };
        }

        public static string GetBaseClassName(SoulId activeSoul)
        {
            return activeSoul switch
            {
                SoulId.Warrior => Warrior,
                SoulId.Mage => Mage,
                SoulId.Ranger => Ranger,
                SoulId.Summoner => Summoner,
                _ => None
            };
        }

        public static string GetDominantAffinityName(
            SoulId activeSoul,
            ClassAffinitySnapshot affinities)
        {
            return activeSoul switch
            {
                SoulId.Warrior => GetHighestName(
                    ("Bleed", affinities.BleedAffinity),
                    ("Combo", affinities.ComboAffinity),
                    ("Defense", affinities.DefenseAffinity),
                    ("Precision", affinities.PrecisionAffinity),
                    ("Rage", affinities.RageAffinity),
                    ("Control", affinities.ControlAffinity)),
                SoulId.Mage => GetHighestName(
                    ("Elemental", affinities.ElementalAffinity),
                    ("Curse", affinities.CurseAffinity),
                    ("Infinity", affinities.InfinityAffinity),
                    ("Arcane", affinities.ArcaneAffinity)),
                SoulId.Ranger => GetHighestName(
                    ("Energy", affinities.EnergyAffinity),
                    ("Bow", affinities.BowAffinity),
                    ("Gun", affinities.GunAffinity),
                    ("Music", affinities.MusicAffinity)),
                SoulId.Summoner => GetHighestName(
                    ("Shadow", affinities.ShadowAffinity),
                    ("Beast", affinities.BeastAffinity),
                    ("Fusion", affinities.FusionAffinity),
                    ("Tech", affinities.TechAffinity)),
                _ => None
            };
        }

        public static int GetDominantAffinityValue(
            SoulId activeSoul,
            ClassAffinitySnapshot affinities)
        {
            return activeSoul switch
            {
                SoulId.Warrior => Highest(
                    affinities.BleedAffinity,
                    affinities.ComboAffinity,
                    affinities.DefenseAffinity,
                    affinities.PrecisionAffinity,
                    affinities.RageAffinity,
                    affinities.ControlAffinity),
                SoulId.Mage => Highest(
                    affinities.ElementalAffinity,
                    affinities.CurseAffinity,
                    affinities.InfinityAffinity,
                    affinities.ArcaneAffinity),
                SoulId.Ranger => Highest(
                    affinities.EnergyAffinity,
                    affinities.BowAffinity,
                    affinities.GunAffinity,
                    affinities.MusicAffinity),
                SoulId.Summoner => Highest(
                    affinities.BeastAffinity,
                    affinities.FusionAffinity,
                    affinities.TechAffinity,
                    affinities.ShadowAffinity),
                _ => 0
            };
        }

        private static string ResolveWarriorPromotion(
            ClassAffinitySnapshot affinities,
            string fallback)
        {
            int highest = Highest(
                affinities.BleedAffinity,
                affinities.ComboAffinity,
                affinities.DefenseAffinity,
                affinities.PrecisionAffinity,
                affinities.RageAffinity,
                affinities.ControlAffinity);

            if (highest <= 0)
            {
                return fallback;
            }

            if (highest == affinities.BleedAffinity)
            {
                return "Swordsman";
            }

            if (highest == affinities.ComboAffinity)
            {
                return "Fighter";
            }

            if (highest == affinities.DefenseAffinity)
            {
                return "Guardian";
            }

            if (highest == affinities.PrecisionAffinity)
            {
                return "Yoyo Master";
            }

            if (highest == affinities.RageAffinity)
            {
                return "Berserker";
            }

            return "Stunner";
        }

        private static string ResolveRangerPromotion(
            ClassAffinitySnapshot affinities,
            string fallback)
        {
            int highest = Highest(
                affinities.EnergyAffinity,
                affinities.BowAffinity,
                affinities.GunAffinity,
                affinities.MusicAffinity);

            if (highest <= 0)
            {
                return fallback;
            }

            if (highest == affinities.EnergyAffinity)
            {
                return "Energy Gunner";
            }

            if (highest == affinities.BowAffinity)
            {
                return "Archer";
            }

            if (highest == affinities.GunAffinity)
            {
                return "Gunner";
            }

            return "Virtuoso";
        }

        private static string ResolveMagePromotion(
            ClassAffinitySnapshot affinities,
            string fallback)
        {
            int highest = Highest(
                affinities.ElementalAffinity,
                affinities.CurseAffinity,
                affinities.InfinityAffinity,
                affinities.ArcaneAffinity);

            if (highest <= 0)
            {
                return fallback;
            }

            if (highest == affinities.ElementalAffinity)
            {
                return "Elementalist";
            }

            if (highest == affinities.CurseAffinity)
            {
                return "Cursed Mage";
            }

            if (highest == affinities.InfinityAffinity)
            {
                return "Infinity Mage";
            }

            return "Arcane Bard";
        }

        private static string ResolveSummonerPromotion(
            ClassAffinitySnapshot affinities,
            string fallback)
        {
            int highest = Highest(
                affinities.BeastAffinity,
                affinities.FusionAffinity,
                affinities.TechAffinity,
                affinities.ShadowAffinity);

            if (highest <= 0)
            {
                return fallback;
            }

            if (highest == affinities.ShadowAffinity)
            {
                return "Necromancer";
            }

            if (highest == affinities.BeastAffinity)
            {
                return "Beast Tamer";
            }

            if (highest == affinities.FusionAffinity)
            {
                return "Advanced Summoner";
            }

            if (highest == affinities.TechAffinity)
            {
                return "Tech Summoner";
            }

            return "Tech Summoner";
        }

        private static int Highest(params int[] values)
        {
            int highest = 0;

            foreach (int value in values)
            {
                if (value > highest)
                {
                    highest = value;
                }
            }

            return highest;
        }

        private static string GetHighestName(
            params (string Name, int Value)[] values)
        {
            string name = None;
            int highest = 0;

            foreach ((string valueName, int value) in values)
            {
                if (value > highest)
                {
                    highest = value;
                    name = valueName;
                }
            }

            return name;
        }
    }
}
