using System.Collections.Generic;

namespace Eternia.Content.Passives
{
    public static class PassiveRegistry
    {
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
        "+10% bleed duration",
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

    // =================================================
    // FIGHTER
    // =================================================

    new PassiveNode(
        "Combo Instinct",
        "+5% melee speed",
        1,
        "Combo",
        3,
        "",
        300,
        120
    ),

    new PassiveNode(
        "Adrenaline Rush",
        "+8% combo duration",
        1,
        "Combo",
        4,
        "Combo Instinct",
        300,
        230
    ),

    new PassiveNode(
        "Limit Breaker",
        "+15% combo damage",
        2,
        "Combo",
        5,
        "Adrenaline Rush",
        300,
        340
    ),

    // =================================================
    // GUARDIAN
    // =================================================

    new PassiveNode(
        "Shield Training",
        "+3 defense",
        1,
        "Defense",
        3,
        "",
        520,
        120
    ),

    new PassiveNode(
        "Iron Wall",
        "+6 defense",
        1,
        "Defense",
        4,
        "Shield Training",
        520,
        230
    ),

    new PassiveNode(
        "Fortress Body",
        "+10% damage reduction",
        2,
        "Defense",
        5,
        "Iron Wall",
        520,
        340
    ),

    // =================================================
    // YOYO
    // =================================================

    new PassiveNode(
        "Precision Flow",
        "+10% yoyo range",
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

    // =================================================
    // BERSERKER
    // =================================================

    new PassiveNode(
        "Blood Rage",
        "+5% low HP damage",
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

    // MUSIC

    new PassiveNode(
        "Musical Soul",
        "+3% support power",
        1,
        "Music",
        3,
        "",
        880,
        140
    ),

    new PassiveNode(
        "Resonance",
        "+5% buff duration",
        1,
        "Music",
        4,
        "Musical Soul",
        880,
        250
    ),

    new PassiveNode(
        "Symphony Master",
        "+10% ally buffs",
        2,
        "Music",
        5,
        "Resonance",
        880,
        360
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

    // CARD

    new PassiveNode(
        "Card Knowledge",
        "+5 max mana",
        1,
        "Card",
        3,
        "",
        320,
        140
    ),

    new PassiveNode(
        "Arcane Deck",
        "+8% magic crit",
        1,
        "Card",
        4,
        "Card Knowledge",
        320,
        250
    ),

    new PassiveNode(
        "Royal Flush",
        "+15% card power",
        2,
        "Card",
        5,
        "Arcane Deck",
        320,
        360
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

    // NECRO

    new PassiveNode(
        "Necrotic Energy",
        "+1 minion slot",
        1,
        "Necro",
        3,
        "",
        880,
        140
    ),

    new PassiveNode(
        "Soul Harvest",
        "+10% summon damage",
        1,
        "Necro",
        4,
        "Necrotic Energy",
        880,
        250
    ),

    new PassiveNode(
        "Army of Death",
        "+2 max minions",
        2,
        "Necro",
        5,
        "Soul Harvest",
        880,
        360
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

    // SHADOW

    new PassiveNode(
        "Shadow Dominion",
        "+5% soul damage",
        1,
        "Shadow",
        3,
        "",
        880,
        140
    ),

    new PassiveNode(
        "Soul Extraction",
        "+10% shadow damage",
        1,
        "Shadow",
        4,
        "Shadow Dominion",
        880,
        250
    ),

    new PassiveNode(
        "Monarch Awakening",
        "+15% shadow power",
        2,
        "Shadow",
        5,
        "Soul Extraction",
        880,
        360
    )
};
        
    }
}