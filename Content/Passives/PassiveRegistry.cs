using System.Collections.Generic;

using Eternia.Content.Souls;

namespace Eternia.Content.Passives
{
    public static class PassiveRegistry
    {
        public static List<PassiveNode> GetPassivesForSoul(SoulId soul)
        {
            return soul switch
            {
                SoulId.Warrior => WarriorPassives,
                SoulId.Mage => MagePassives,
                SoulId.Ranger => RangerPassives,
                SoulId.Summoner => SummonerPassives,
                _ => null
            };
        }

        public static bool IsPassiveAllowedForSoul(
            SoulId soul,
            PassiveNode passive)
        {
            if (passive == null)
            {
                return false;
            }

            return IsPassiveAllowedForSoul(
                soul,
                passive.Name);
        }

        public static bool IsPassiveAllowedForSoul(
            SoulId soul,
            string passiveName)
        {
            List<PassiveNode> passives =
                GetPassivesForSoul(soul);

            if (passives == null)
            {
                return false;
            }

            return passives.Exists(
                node => node.Name == passiveName);
        }

        // =====================================================
        // DIAMOND-LATTICE STRUCTURE
        // =====================================================
        // The tree is non-linear. Within a branch, tiers alternate 1 node, then 2
        // nodes, and so on. A node's prerequisites are ALL nodes in the previous
        // tier, so the node after a 2-node tier requires BOTH of them (a merge/gate)
        // -- you must unlock two together to reach the next one.

        public static List<List<PassiveNode>> BuildTiers(
            List<PassiveNode> branch)
        {
            int style =
                branch.Count > 0 ? BranchStyle(branch[0].AffinityType) : 0;

            var tiers = new List<List<PassiveNode>>();

            int index = 0;
            int tier = 0;

            while (index < branch.Count)
            {
                int size = TierSize(style, tier);

                var group = new List<PassiveNode>();

                for (int j = 0; j < size && index < branch.Count; j++)
                {
                    group.Add(branch[index]);
                    index++;
                }

                tiers.Add(group);
                tier++;
            }

            return tiers;
        }

        // Each branch gets a deterministic shape so they don't all look identical:
        // some are diamond ladders, some straight lines, some diamond early / late.
        private static int BranchStyle(string affinityType)
        {
            int sum = 0;

            foreach (char c in affinityType)
            {
                sum += c;
            }

            return sum % 4;
        }

        // Tier size (1 or 2 nodes). A 2-node tier is always followed by a 1-node
        // tier, so every split merges back cleanly (no messy cross-links).
        private static int TierSize(int style, int tier)
        {
            // A periodic wide GATE: an isolated 3-node tier (split from one, merged
            // into one) whose successor requires ALL THREE -- so some nodes need
            // three prerequisites. Kept isolated so the shapes stay readable.
            int phase = tier % 6;

            if (phase == 4)
            {
                return 3;
            }

            if (phase == 3 || phase == 5)
            {
                return 1;
            }

            // Otherwise the branch's style decides. A 2-node tier is always followed
            // by a 1-node tier so every split merges cleanly; no style is a plain
            // straight line -- every branch gets diamonds on a different beat.
            switch (style)
            {
                case 0:
                    // Diamond ladder: split every other tier.
                    return tier % 2 == 0 ? 1 : 2;
                case 1:
                    // Diamonds starting one tier later.
                    return tier >= 2 && tier % 2 == 0 ? 2 : 1;
                case 2:
                    // Diamonds spaced out (every third tier).
                    return tier % 3 == 1 ? 2 : 1;
                default:
                    // Diamonds spaced out on a different beat.
                    return tier % 3 == 2 ? 2 : 1;
            }
        }

        public static List<PassiveNode> GetBranch(
            SoulId soul,
            string affinityType)
        {
            List<PassiveNode> all = GetPassivesForSoul(soul);

            if (all == null)
            {
                return new List<PassiveNode>();
            }

            return all.FindAll(node => node.AffinityType == affinityType);
        }

        public static List<string> GetPrerequisites(
            SoulId soul,
            PassiveNode node)
        {
            if (node == null)
            {
                return new List<string>();
            }

            List<List<PassiveNode>> tiers =
                BuildTiers(GetBranch(soul, node.AffinityType));

            for (int t = 0; t < tiers.Count; t++)
            {
                if (tiers[t].Exists(n => n.Name == node.Name))
                {
                    if (t == 0)
                    {
                        return new List<string>();
                    }

                    return tiers[t - 1].ConvertAll(n => n.Name);
                }
            }

            return new List<string>();
        }

        public static List<PassiveNode> WarriorPassives =
new List<PassiveNode>()
{
    // =================================================
    // SWORD
    // =================================================

    new PassiveNode(
        "Sword Mastery",
        "+5% melee damage",
        1,
        "Bleed",
        3,
        "",
        80,
        120
    ),

    new PassiveNode(
        "Blood Flow",
        "+3 melee armor pen, +2s bleed duration",
        1,
        "Bleed",
        4,
        "Sword Mastery",
        80,
        230
    ),

    new PassiveNode(
        "Execution",
        "+15% damage vs bleeding enemies",
        2,
        "Bleed",
        5,
        "Blood Flow",
        80,
        340
    ),

    new PassiveNode(
        "Hemorrhage",
        "+8% melee damage",
        2,
        "Bleed",
        6,
        "Execution",
        80,
        450
    ),

    new PassiveNode(
        "Crimson Reaper",
        "+10% melee crit",
        3,
        "Bleed",
        7,
        "Hemorrhage",
        80,
        560
    ),

    new PassiveNode(
        "Rupture",
        "+5 bleed damage",
        3,
        "Bleed",
        8,
        "Crimson Reaper",
        80,
        670
    ),

    new PassiveNode(
        "Hemoplague",
        "+2s bleed duration",
        3,
        "Bleed",
        9,
        "Rupture",
        80,
        780
    ),

    new PassiveNode(
        "Exsanguinate",
        "+25% damage vs bleeding enemies",
        4,
        "Bleed",
        10,
        "Hemoplague",
        80,
        890
    ),

    new PassiveNode(
        "Bloodthirst",
        "Heal when you strike bleeding foes",
        4,
        "Bleed",
        11,
        "Exsanguinate",
        80,
        1000
    ),

    // =================================================
    // FIGHTER
    // =================================================

    new PassiveNode(
        "Combo Instinct",
        "Each Combo point: +1% melee damage",
        1,
        "Combo",
        3,
        "",
        300,
        120
    ),

    new PassiveNode(
        "Adrenaline Rush",
        "+1.5s to the Combo window",
        1,
        "Combo",
        4,
        "Combo Instinct",
        300,
        230
    ),

    new PassiveNode(
        "Limit Breaker",
        "Each Combo point: +1% melee damage",
        2,
        "Combo",
        5,
        "Adrenaline Rush",
        300,
        340
    ),

    new PassiveNode(
        "Flow State",
        "Each Combo point: +0.6% attack speed",
        2,
        "Combo",
        6,
        "Limit Breaker",
        300,
        450
    ),

    new PassiveNode(
        "Perfect Rhythm",
        "Each Combo point: +0.5% move speed",
        3,
        "Combo",
        7,
        "Flow State",
        300,
        560
    ),

    new PassiveNode(
        "Rapid Blows",
        "Crit & point-blank hits build +1 Combo",
        3,
        "Combo",
        8,
        "Perfect Rhythm",
        300,
        670
    ),

    new PassiveNode(
        "Unbroken Chain",
        "+10 to your max Combo",
        3,
        "Combo",
        9,
        "Rapid Blows",
        300,
        780
    ),

    new PassiveNode(
        "Thousand Cuts",
        "Keep your Combo when you take damage",
        4,
        "Combo",
        10,
        "Unbroken Chain",
        300,
        890
    ),

    // =================================================
    // GUARDIAN
    // =================================================

    new PassiveNode(
        "Shield Training",
        "+3 defense - Escudero: +10% Shield Aura radius",
        1,
        "Defense",
        3,
        "",
        520,
        120
    ),

    new PassiveNode(
        "Iron Wall",
        "+6 defense - Escudero: +10% Shield Aura damage",
        1,
        "Defense",
        4,
        "Shield Training",
        520,
        230
    ),

    new PassiveNode(
        "Fortress Body",
        "+10% damage reduction - Escudero: +15% Shield Aura damage",
        2,
        "Defense",
        5,
        "Iron Wall",
        520,
        340
    ),

    new PassiveNode(
        "Bulwark",
        "+8 defense - Escudero: +15% Shield Aura radius",
        2,
        "Defense",
        6,
        "Fortress Body",
        520,
        450
    ),

    new PassiveNode(
        "Unbreakable",
        "+8% damage reduction - Escudero: Shield Aura pulses 15% faster",
        3,
        "Defense",
        7,
        "Bulwark",
        520,
        560
    ),

    new PassiveNode(
        "Aegis",
        "+8 defense - Escudero: +20% Shield Aura damage",
        3,
        "Defense",
        8,
        "Unbreakable",
        520,
        670
    ),

    new PassiveNode(
        "Stonewall",
        "+6% damage reduction - Escudero: Shield Aura pulses 15% faster",
        3,
        "Defense",
        9,
        "Aegis",
        520,
        780
    ),

    new PassiveNode(
        "Last Bastion",
        "+20 max life - Escudero: Shield Aura also heals you each pulse",
        4,
        "Defense",
        10,
        "Stonewall",
        520,
        890
    ),

    // =================================================
    // YOYO
    // =================================================

    new PassiveNode(
        "Precision Flow",
        "Infinite yoyo string",
        1,
        "Precision",
        3,
        "",
        80,
        500
    ),

    new PassiveNode(
        "Weakpoint Detection",
        "+8% yoyo crit",
        1,
        "Precision",
        4,
        "Precision Flow",
        80,
        610
    ),

    new PassiveNode(
        "True Strike",
        "+15% true damage",
        2,
        "Precision",
        5,
        "Weakpoint Detection",
        80,
        720
    ),

    new PassiveNode(
        "Keen Edge",
        "+8% melee crit",
        2,
        "Precision",
        6,
        "True Strike",
        80,
        830
    ),

    new PassiveNode(
        "Lethal Precision",
        "+12% melee damage",
        3,
        "Precision",
        7,
        "Keen Edge",
        80,
        940
    ),

    new PassiveNode(
        "Pinpoint",
        "+8% melee crit",
        3,
        "Precision",
        8,
        "Lethal Precision",
        80,
        1050
    ),

    new PassiveNode(
        "Exploit Weakness",
        "+10% melee damage",
        3,
        "Precision",
        9,
        "Pinpoint",
        80,
        1160
    ),

    new PassiveNode(
        "Perfect Aim",
        "+5 melee armor penetration",
        4,
        "Precision",
        10,
        "Exploit Weakness",
        80,
        1270
    ),

    // =================================================
    // BERSERKER
    // =================================================

    new PassiveNode(
        "Blood Rage",
        "+12% melee below 35% HP",
        1,
        "Rage",
        3,
        "",
        300,
        500
    ),

    new PassiveNode(
        "Savage Fury",
        "+10% attack speed below 50% HP",
        1,
        "Rage",
        4,
        "Blood Rage",
        300,
        610
    ),

    new PassiveNode(
        "Last Stand",
        "+20% damage below 25% HP",
        2,
        "Rage",
        5,
        "Savage Fury",
        300,
        720
    ),

    new PassiveNode(
        "Berserk Momentum",
        "+6% melee speed",
        2,
        "Rage",
        6,
        "Last Stand",
        300,
        830
    ),

    new PassiveNode(
        "Undying Wrath",
        "+10% melee damage",
        3,
        "Rage",
        7,
        "Berserk Momentum",
        300,
        940
    ),

    new PassiveNode(
        "Bloodlust",
        "+8% melee damage",
        3,
        "Rage",
        8,
        "Undying Wrath",
        300,
        1050
    ),

    new PassiveNode(
        "Frenzy",
        "+6% melee speed",
        3,
        "Rage",
        9,
        "Bloodlust",
        300,
        1160
    ),

    new PassiveNode(
        "Reckless Assault",
        "+10% melee crit",
        4,
        "Rage",
        10,
        "Frenzy",
        300,
        1270
    ),

    // =================================================
    // STUNNER
    // =================================================

    new PassiveNode(
        "Heavy Impact",
        "+10% knockback",
        1,
        "Control",
        3,
        "",
        520,
        500
    ),

    new PassiveNode(
        "Concussion",
        "+15% stun duration",
        1,
        "Control",
        4,
        "Heavy Impact",
        520,
        610
    ),

    new PassiveNode(
        "Tyrant Smash",
        "+20% charged attack damage",
        2,
        "Control",
        5,
        "Concussion",
        520,
        720
    ),

    new PassiveNode(
        "Crushing Blows",
        "+8% melee damage",
        2,
        "Control",
        6,
        "Tyrant Smash",
        520,
        830
    ),

    new PassiveNode(
        "Overwhelming Force",
        "+2 knockback",
        3,
        "Control",
        7,
        "Crushing Blows",
        520,
        940
    ),

    new PassiveNode(
        "Stagger",
        "+2 knockback",
        3,
        "Control",
        8,
        "Overwhelming Force",
        520,
        1050
    ),

    new PassiveNode(
        "Brutal Force",
        "+8% melee damage",
        3,
        "Control",
        9,
        "Stagger",
        520,
        1160
    ),

    new PassiveNode(
        "Shockwave",
        "+4 melee armor penetration",
        4,
        "Control",
        10,
        "Brutal Force",
        520,
        1270
    )
};
        public static List<PassiveNode> RangerPassives =
          new List<PassiveNode>()
         {
    // =================================================
    // ENERGY
    // =================================================

          // ENERGY

    new PassiveNode(
        "Energy Core",
        "+5% ranged damage",
        1,
        "Energy",
        3,
        "",
        40,
        140
    ),

    new PassiveNode(
        "Overcharge",
        "+8% crit chance",
        1,
        "Energy",
        4,
        "Energy Core",
        40,
        250
    ),

    new PassiveNode(
        "Plasma Reactor",
        "+12% energy damage",
        2,
        "Energy",
        5,
        "Overcharge",
        40,
        360
    ),

    new PassiveNode(
        "Ion Surge",
        "+8% ranged damage",
        2,
        "Energy",
        6,
        "Plasma Reactor",
        40,
        470
    ),

    new PassiveNode(
        "Fusion Cannon",
        "+10% ranged crit",
        3,
        "Energy",
        7,
        "Ion Surge",
        40,
        580
    ),

    new PassiveNode(
        "Overload",
        "+8% ranged damage",
        3,
        "Energy",
        8,
        "Fusion Cannon",
        40,
        690
    ),

    new PassiveNode(
        "Particle Beam",
        "+10% ranged crit",
        3,
        "Energy",
        9,
        "Overload",
        40,
        800
    ),

    new PassiveNode(
        "Reactor Core",
        "+6% ranged speed",
        4,
        "Energy",
        10,
        "Particle Beam",
        40,
        910
    ),

    // BOW

    new PassiveNode(
        "Bow Precision",
        "+5% arrow speed",
        1,
        "Bow",
        3,
        "",
        320,
        140
    ),

    new PassiveNode(
        "Eagle Eye",
        "+10% ranged crit",
        1,
        "Bow",
        4,
        "Bow Precision",
        320,
        250
    ),

    new PassiveNode(
        "Hunter Instinct",
        "+15% bow damage",
        2,
        "Bow",
        5,
        "Eagle Eye",
        320,
        360
    ),

    new PassiveNode(
        "Piercing Shot",
        "+5 ranged armor penetration",
        2,
        "Bow",
        6,
        "Hunter Instinct",
        320,
        470
    ),

    new PassiveNode(
        "Storm of Arrows",
        "+10% ranged speed",
        3,
        "Bow",
        7,
        "Piercing Shot",
        320,
        580
    ),

    new PassiveNode(
        "Marksman",
        "+8% ranged damage",
        3,
        "Bow",
        8,
        "Storm of Arrows",
        320,
        690
    ),

    new PassiveNode(
        "True Flight",
        "+8% ranged crit",
        3,
        "Bow",
        9,
        "Marksman",
        320,
        800
    ),

    new PassiveNode(
        "Volley",
        "+8% ranged speed",
        4,
        "Bow",
        10,
        "True Flight",
        320,
        910
    ),

    // GUN

    new PassiveNode(
        "Quick Trigger",
        "+4% gun speed",
        1,
        "Gun",
        3,
        "",
        600,
        140
    ),

    new PassiveNode(
        "Rapid Chamber",
        "+8% fire rate",
        1,
        "Gun",
        4,
        "Quick Trigger",
        600,
        250
    ),

    new PassiveNode(
        "Deadshot",
        "+15% crit damage",
        2,
        "Gun",
        5,
        "Rapid Chamber",
        600,
        360
    ),

    new PassiveNode(
        "Hair Trigger",
        "+8% ranged speed",
        2,
        "Gun",
        6,
        "Deadshot",
        600,
        470
    ),

    new PassiveNode(
        "Bullet Storm",
        "+12% ranged damage",
        3,
        "Gun",
        7,
        "Hair Trigger",
        600,
        580
    ),

    new PassiveNode(
        "Full Auto",
        "+8% ranged speed",
        3,
        "Gun",
        8,
        "Bullet Storm",
        600,
        690
    ),

    new PassiveNode(
        "Armor Piercing",
        "+5 ranged armor penetration",
        3,
        "Gun",
        9,
        "Full Auto",
        600,
        800
    ),

    new PassiveNode(
        "Executioner",
        "+10% ranged crit",
        4,
        "Gun",
        10,
        "Armor Piercing",
        600,
        910
    ),

    // MUSIC

    new PassiveNode(
        "Musical Soul",
        "+3% movement speed",
        1,
        "Music",
        3,
        "",
        880,
        140
    ),

    new PassiveNode(
        "Resonance",
        "+2% damage reduction",
        1,
        "Music",
        4,
        "Musical Soul",
        880,
        250
    ),

    new PassiveNode(
        "Symphony Master",
        "+5% damage",
        2,
        "Music",
        5,
        "Resonance",
        880,
        360
    ),

    new PassiveNode(
        "Battle Hymn",
        "+4% movement speed",
        2,
        "Music",
        6,
        "Symphony Master",
        880,
        470
    ),

    new PassiveNode(
        "Grand Finale",
        "+8% damage",
        3,
        "Music",
        7,
        "Battle Hymn",
        880,
        580
    ),

    new PassiveNode(
        "Encore",
        "+4% movement speed",
        3,
        "Music",
        8,
        "Grand Finale",
        880,
        690
    ),

    new PassiveNode(
        "War Anthem",
        "+6% damage",
        3,
        "Music",
        9,
        "Encore",
        880,
        800
    ),

    new PassiveNode(
        "Crescendo",
        "+8% ranged damage",
        4,
        "Music",
        10,
        "War Anthem",
        880,
        910
    )
};
        public static List<PassiveNode> MagePassives =
    new List<PassiveNode>()
{
    // =================================================
    // ELEMENTAL
    // =================================================

    new PassiveNode(
        "Elemental Control",
        "+5% magic damage",
        1,
        "Elemental",
        3,
        "",
        40,
        140
    ),

    new PassiveNode(
        "Elemental Surge",
        "+10 mana",
        1,
        "Elemental",
        4,
        "Elemental Control",
        40,
        250
    ),

    new PassiveNode(
        "Elemental Mastery",
        "+15% elemental damage",
        2,
        "Elemental",
        5,
        "Elemental Surge",
        40,
        360
    ),

    new PassiveNode(
        "Arcane Conductor",
        "+8% magic damage",
        2,
        "Elemental",
        6,
        "Elemental Mastery",
        40,
        470
    ),

    new PassiveNode(
        "Elemental Overload",
        "+10% magic crit",
        3,
        "Elemental",
        7,
        "Arcane Conductor",
        40,
        580
    ),

    new PassiveNode(
        "Pyroclasm",
        "+8% magic damage",
        3,
        "Elemental",
        8,
        "Elemental Overload",
        40,
        690
    ),

    new PassiveNode(
        "Frost Nova",
        "+8% magic crit",
        3,
        "Elemental",
        9,
        "Pyroclasm",
        40,
        800
    ),

    new PassiveNode(
        "Storm Caller",
        "+6% magic damage",
        4,
        "Elemental",
        10,
        "Frost Nova",
        40,
        910
    ),

    // CURSE

    new PassiveNode(
        "Dark Ritual",
        "+3% curse power",
        1,
        "Curse",
        3,
        "",
        600,
        140
    ),

    new PassiveNode(
        "Forbidden Hex",
        "+10% debuff duration",
        1,
        "Curse",
        4,
        "Dark Ritual",
        600,
        250
    ),

    new PassiveNode(
        "Cursed Blood",
        "+15% cursed damage",
        2,
        "Curse",
        5,
        "Forbidden Hex",
        600,
        360
    ),

    new PassiveNode(
        "Withering Curse",
        "+5 magic armor penetration",
        2,
        "Curse",
        6,
        "Cursed Blood",
        600,
        470
    ),

    new PassiveNode(
        "Doom Bringer",
        "+12% magic damage",
        3,
        "Curse",
        7,
        "Withering Curse",
        600,
        580
    ),

    new PassiveNode(
        "Soul Rot",
        "+5 magic armor penetration",
        3,
        "Curse",
        8,
        "Doom Bringer",
        600,
        690
    ),

    new PassiveNode(
        "Blight",
        "+8% magic damage",
        3,
        "Curse",
        9,
        "Soul Rot",
        600,
        800
    ),

    new PassiveNode(
        "Malediction",
        "+8% magic crit",
        4,
        "Curse",
        10,
        "Blight",
        600,
        910
    ),

    // INFINITY

    new PassiveNode(
        "Infinite Pages",
        "+4% mana efficiency",
        1,
        "Infinity",
        3,
        "",
        1160,
        140
    ),

    new PassiveNode(
        "Endless Wisdom",
        "+20 mana",
        1,
        "Infinity",
        4,
        "Infinite Pages",
        1160,
        250
    ),

    new PassiveNode(
        "Limit Break",
        "+15% magic damage",
        2,
        "Infinity",
        5,
        "Endless Wisdom",
        1160,
        360
    ),

    new PassiveNode(
        "Boundless Mana",
        "+30 mana",
        2,
        "Infinity",
        6,
        "Limit Break",
        1160,
        470
    ),

    new PassiveNode(
        "Eternal Flow",
        "+5% mana efficiency",
        3,
        "Infinity",
        7,
        "Boundless Mana",
        1160,
        580
    ),

    new PassiveNode(
        "Mana Font",
        "+30 mana",
        3,
        "Infinity",
        8,
        "Eternal Flow",
        1160,
        690
    ),

    new PassiveNode(
        "Overflow",
        "+5% mana efficiency",
        3,
        "Infinity",
        9,
        "Mana Font",
        1160,
        800
    ),

    new PassiveNode(
        "Infinite Well",
        "+4 mana regen",
        4,
        "Infinity",
        10,
        "Overflow",
        1160,
        910
    ),

    // ARCANE

    new PassiveNode(
        "Arcane Melody",
        "+3% buff power",
        1,
        "Arcane",
        3,
        "",
        1440,
        140
    ),

    new PassiveNode(
        "Mystic Chorus",
        "+8% support effects",
        1,
        "Arcane",
        4,
        "Arcane Melody",
        1440,
        250
    ),

    new PassiveNode(
        "Grand Orchestra",
        "+15% magical buffs",
        2,
        "Arcane",
        5,
        "Mystic Chorus",
        1440,
        360
    ),

    new PassiveNode(
        "Harmonic Field",
        "+4 mana regen",
        2,
        "Arcane",
        6,
        "Grand Orchestra",
        1440,
        470
    ),

    new PassiveNode(
        "Celestial Symphony",
        "+10% magic damage",
        3,
        "Arcane",
        7,
        "Harmonic Field",
        1440,
        580
    ),

    new PassiveNode(
        "Astral Resonance",
        "+4 mana regen",
        3,
        "Arcane",
        8,
        "Celestial Symphony",
        1440,
        690
    ),

    new PassiveNode(
        "Ley Line",
        "+8% magic damage",
        3,
        "Arcane",
        9,
        "Astral Resonance",
        1440,
        800
    ),

    new PassiveNode(
        "Cosmic Chord",
        "+25 mana",
        4,
        "Arcane",
        10,
        "Ley Line",
        1440,
        910
    )
};
        public static List<PassiveNode> SummonerPassives =
    new List<PassiveNode>()
{
    // =================================================
    // BEAST
    // =================================================

    new PassiveNode(
        "Wild Bond",
        "+5% summon damage",
        1,
        "Beast",
        3,
        "",
        40,
        140
    ),

    new PassiveNode(
        "Alpha Beast",
        "+10% summon knockback",
        1,
        "Beast",
        4,
        "Wild Bond",
        40,
        250
    ),

    new PassiveNode(
        "Primal Instinct",
        "+15% beast damage",
        2,
        "Beast",
        5,
        "Alpha Beast",
        40,
        360
    ),

    new PassiveNode(
        "Pack Leader",
        "+1 minion capacity",
        2,
        "Beast",
        6,
        "Primal Instinct",
        40,
        470
    ),

    new PassiveNode(
        "Savage Alpha",
        "+10% summon damage",
        3,
        "Beast",
        7,
        "Pack Leader",
        40,
        580
    ),

    new PassiveNode(
        "Feral Roar",
        "+8% summon damage",
        3,
        "Beast",
        8,
        "Savage Alpha",
        40,
        690
    ),

    new PassiveNode(
        "Bloodhound",
        "+8% summon crit",
        3,
        "Beast",
        9,
        "Feral Roar",
        40,
        800
    ),

    new PassiveNode(
        "Apex Predator",
        "+1 minion capacity",
        4,
        "Beast",
        10,
        "Bloodhound",
        40,
        910
    ),

    // FUSION

    new PassiveNode(
        "Fusion Mind",
        "+1 summon capacity",
        1,
        "Fusion",
        3,
        "",
        320,
        140
    ),

    new PassiveNode(
        "Perfect Fusion",
        "+10% summon attack speed",
        1,
        "Fusion",
        4,
        "Fusion Mind",
        320,
        250
    ),

    new PassiveNode(
        "Ultimate Fusion",
        "+15% fusion power",
        2,
        "Fusion",
        5,
        "Perfect Fusion",
        320,
        360
    ),

    new PassiveNode(
        "Synchronized Assault",
        "+8% summon speed",
        2,
        "Fusion",
        6,
        "Ultimate Fusion",
        320,
        470
    ),

    new PassiveNode(
        "Transcendent Fusion",
        "+12% summon damage",
        3,
        "Fusion",
        7,
        "Synchronized Assault",
        320,
        580
    ),

    new PassiveNode(
        "Hive Mind",
        "+1 minion capacity",
        3,
        "Fusion",
        8,
        "Transcendent Fusion",
        320,
        690
    ),

    new PassiveNode(
        "Overdrive",
        "+8% summon attack speed",
        3,
        "Fusion",
        9,
        "Hive Mind",
        320,
        800
    ),

    new PassiveNode(
        "Singularity",
        "+10% summon damage",
        4,
        "Fusion",
        10,
        "Overdrive",
        320,
        910
    ),

    // TECH

    new PassiveNode(
        "Tech Protocol",
        "+5% summon speed",
        1,
        "Tech",
        3,
        "",
        600,
        140
    ),

    new PassiveNode(
        "Combat AI",
        "+8% summon crit",
        1,
        "Tech",
        4,
        "Tech Protocol",
        600,
        250
    ),

    new PassiveNode(
        "War Machine",
        "+15% tech summon damage",
        2,
        "Tech",
        5,
        "Combat AI",
        600,
        360
    ),

    new PassiveNode(
        "Overclocked Core",
        "+10% summon crit",
        2,
        "Tech",
        6,
        "War Machine",
        600,
        470
    ),

    new PassiveNode(
        "Autonomous Legion",
        "+1 minion capacity",
        3,
        "Tech",
        7,
        "Overclocked Core",
        600,
        580
    ),

    new PassiveNode(
        "Targeting Array",
        "+8% summon crit",
        3,
        "Tech",
        8,
        "Autonomous Legion",
        600,
        690
    ),

    new PassiveNode(
        "Nanoswarm",
        "+8% summon damage",
        3,
        "Tech",
        9,
        "Targeting Array",
        600,
        800
    ),

    new PassiveNode(
        "Drone Fleet",
        "+1 minion capacity",
        4,
        "Tech",
        10,
        "Nanoswarm",
        600,
        910
    ),

    // NECROMANCER

    new PassiveNode(
        "Necrotic Pact",
        "+5% summon damage",
        1,
        "Shadow",
        3,
        "",
        880,
        140
    ),

    new PassiveNode(
        "Bone Conduit",
        "+1 necro slot scaling",
        1,
        "Shadow",
        4,
        "Necrotic Pact",
        880,
        250
    ),

    new PassiveNode(
        "Grave Legion",
        "+15% necromancy power",
        2,
        "Shadow",
        5,
        "Bone Conduit",
        880,
        360
    ),

    new PassiveNode(
        "Soul Harvest",
        "+8% summon damage",
        2,
        "Shadow",
        6,
        "Grave Legion",
        880,
        470
    ),

    new PassiveNode(
        "Legion of the Dead",
        "+1 minion capacity",
        3,
        "Shadow",
        7,
        "Soul Harvest",
        880,
        580
    ),

    new PassiveNode(
        "Dark Communion",
        "+8% summon damage",
        3,
        "Shadow",
        8,
        "Legion of the Dead",
        880,
        690
    ),

    new PassiveNode(
        "Wraith Form",
        "+8% summon crit",
        3,
        "Shadow",
        9,
        "Dark Communion",
        880,
        800
    ),

    new PassiveNode(
        "Undying Horde",
        "+1 minion capacity",
        4,
        "Shadow",
        10,
        "Wraith Form",
        880,
        910
    )
};

        // =====================================================
        // BRANCH PADDING
        // =====================================================
        // Each tree is hand-authored with its notable nodes, then padded up to
        // BranchTarget nodes per affinity branch with minor "path" nodes so the
        // tree is extensive for v1. Minor nodes grant affinity (feeding subclass
        // choice + milestones); the milestone system is the reward for filling in.

        public const int BranchTarget = 20;

        private static readonly string[] MasteryTitles =
        {
            "Adept", "Focus", "Discipline", "Insight", "Resolve",
            "Veteran", "Expert", "Ascendant", "Paragon", "Apex",
            "Pinnacle", "Zenith", "Sovereign", "Eternal"
        };

        static PassiveRegistry()
        {
            PadBranchesTo(WarriorPassives, BranchTarget);
            PadBranchesTo(RangerPassives, BranchTarget);
            PadBranchesTo(MagePassives, BranchTarget);
            PadBranchesTo(SummonerPassives, BranchTarget);
        }

        // Short themed description of what a branch's minor nodes give, matching the
        // generic affinity bonus in EterniaStatsPlayer.
        public static string AffinityEffectText(string affinity)
        {
            return affinity switch
            {
                "Bleed" => "+melee power",
                "Combo" => "+melee speed",
                "Defense" => "+toughness",
                "Precision" => "+melee crit",
                "Rage" => "+melee power",
                "Control" => "+knockback",
                "Energy" => "+ranged power",
                "Bow" => "+ranged crit",
                "Gun" => "+fire rate",
                "Music" => "+mobility",
                "Elemental" => "+magic power",
                "Curse" => "+magic power",
                "Infinity" => "+mana",
                "Arcane" => "+mana regen",
                "Beast" => "+summon power",
                "Fusion" => "+summon speed",
                "Tech" => "+summon crit",
                "Shadow" => "+summon power",
                _ => "+affinity"
            };
        }

        // The build-defining keystone at the end of each branch. Names are unique
        // (they never collide with notable/minor nodes). Effects live in
        // KeystonePlayer; these strings are just what the player reads.
        public static string KeystoneName(string affinity)
        {
            return affinity switch
            {
                "Bleed" => "Hemorrhagic Frenzy",
                "Combo" => "Perpetual Motion",
                "Defense" => "Immovable Object",
                "Precision" => "Assassin's Mark",
                "Rage" => "Death Wish",
                "Control" => "Juggernaut",
                "Energy" => "Overcharged Core",
                "Bow" => "Hawkeye",
                "Gun" => "Trigger Discipline",
                "Music" => "Grand Maestro",
                "Elemental" => "Cataclysm",
                "Curse" => "Pact of Ruin",
                "Infinity" => "Endless Well",
                "Arcane" => "Ley Conduit",
                "Beast" => "Apex Alpha",
                "Fusion" => "Living Swarm",
                "Tech" => "Combat Protocol",
                "Shadow" => "Lichdom",
                _ => affinity + " Keystone"
            };
        }

        public static string KeystoneDescription(string affinity)
        {
            return affinity switch
            {
                "Bleed" => "KEYSTONE: +20% melee damage, but -10% attack speed.",
                "Combo" => "KEYSTONE - FRENZY: while at max Combo, +15% melee damage, +10% attack speed and +8% damage reduction.",
                "Defense" => "KEYSTONE: +15% damage reduction, but -25% move speed.",
                "Precision" => "KEYSTONE: +30% melee crit, but -15% melee damage.",
                "Rage" => "KEYSTONE: +25% melee damage, but -40 max life.",
                "Control" => "KEYSTONE: +120% knockback and +12% melee, but -12% attack speed.",
                "Energy" => "KEYSTONE: +22% ranged damage, but -12% ranged crit.",
                "Bow" => "KEYSTONE: +30% ranged crit, but -12% ranged damage.",
                "Gun" => "KEYSTONE: +28% fire rate, but -14% ranged damage.",
                "Music" => "KEYSTONE: +14% damage, but -12% damage reduction.",
                "Elemental" => "KEYSTONE: +25% magic damage, but +25% mana cost.",
                "Curse" => "KEYSTONE: +30% magic damage, but -40 max life.",
                "Infinity" => "KEYSTONE: -30% mana cost, but -14% magic damage.",
                "Arcane" => "KEYSTONE: +8 mana regen, but -40 max mana.",
                "Beast" => "KEYSTONE: +30% summon damage, but -1 minion.",
                "Fusion" => "KEYSTONE: +2 minions, but -15% summon damage.",
                "Tech" => "KEYSTONE: +30% summon crit, but -12% summon attack speed.",
                "Shadow" => "KEYSTONE: +30% summon damage, but -40 max life.",
                _ => "KEYSTONE"
            };
        }

        private static void PadBranchesTo(List<PassiveNode> list, int perBranch)
        {
            var order = new List<string>();

            foreach (PassiveNode node in list)
            {
                if (!order.Contains(node.AffinityType))
                {
                    order.Add(node.AffinityType);
                }
            }

            var additions = new List<PassiveNode>();

            foreach (string affinity in order)
            {
                List<PassiveNode> branch =
                    list.FindAll(node => node.AffinityType == affinity);

                int have = branch.Count;
                string previous = branch[have - 1].Name;

                // Minor "path" nodes fill the branch up to one short of the target.
                // They always cost 1 and grant 2 affinity: they used to cost up to 2
                // for 1 affinity, which made the back half of every branch a dead
                // zone worth ~40x less per point than a notable node.
                for (int i = have; i < perBranch - 1; i++)
                {
                    int step = i - have;
                    string title = MasteryTitles[step % MasteryTitles.Length];
                    string name = affinity + " " + title;

                    PassiveNode minor =
                        new PassiveNode(
                            name,
                            AffinityEffectText(affinity)
                                + " (deepens your " + affinity + " path)",
                            1,
                            affinity,
                            2,
                            previous);
                    minor.Kind = PassiveKind.Minor;

                    additions.Add(minor);
                    previous = name;
                }

                // The branch's build-defining KEYSTONE caps it off.
                PassiveNode keystone =
                    new PassiveNode(
                        KeystoneName(affinity),
                        KeystoneDescription(affinity),
                        5,
                        affinity,
                        3,
                        previous);
                keystone.Kind = PassiveKind.Keystone;

                additions.Add(keystone);
            }

            list.AddRange(additions);
        }
    }
}
