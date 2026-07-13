using System;
using System.Collections.Generic;

using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Souls;
using Eternia.Content.Projectiles.Necromancer;

namespace Eternia.Content.Necromancy
{
    // One page of the Grimoire of Death: a creature you can dominate. You unlock it by
    // killing enough of its source enemies, obtaining its Soul, and registering that
    // Soul in the Grimoire. Types are resolved lazily so this stays plain data.
    public class GrimoireEntry
    {
        public string Id;
        public string DisplayName;
        public string Category;
        public int[] SourceNPCs;
        public int KillThreshold;
        public bool DefaultUnlocked;

        // Grimoire rank needed to dominate/raise this creature (1..4). Higher categories
        // (Corrupt/Hallow/Boss/Lunar) require higher ranks.
        public int RequiredRank = 1;

        // Resolved via ModContent at use-time.
        public Func<int> SoulType;
        public Func<int> MinionType;
    }

    public static class GrimoireRegistry
    {
        public static readonly List<GrimoireEntry> Entries = new()
        {
            // --- Common ---
            new GrimoireEntry
            {
                Id = "Skeleton",
                DisplayName = "Skeleton",
                Category = "Common",
                SourceNPCs = new int[] { NPCID.Skeleton, NPCID.HeadacheSkeleton,
                    NPCID.MisassembledSkeleton, NPCID.PantlessSkeleton },
                KillThreshold = 0,
                DefaultUnlocked = true, // the starter undead
                SoulType = () => -1,
                MinionType = ModContent.ProjectileType<SkeletonMinion>
            },
            new GrimoireEntry
            {
                Id = "Zombie",
                DisplayName = "Zombie",
                Category = "Common",
                SourceNPCs = new int[] { NPCID.Zombie, NPCID.BaldZombie,
                    NPCID.PincushionZombie, NPCID.SlimedZombie, NPCID.SwampZombie,
                    NPCID.TwiggyZombie },
                KillThreshold = 100,
                SoulType = ModContent.ItemType<ZombieSoul>,
                MinionType = ModContent.ProjectileType<ZombieMinion>
            },
            new GrimoireEntry
            {
                Id = "DemonEye",
                DisplayName = "Demon Eye",
                Category = "Common",
                SourceNPCs = new int[] { NPCID.DemonEye, NPCID.DemonEye2,
                    NPCID.DemonEyeOwl, NPCID.DemonEyeSpaceship },
                KillThreshold = 75,
                SoulType = ModContent.ItemType<DemonEyeSoul>,
                MinionType = ModContent.ProjectileType<DemonEyeMinion>
            },

            // --- Boss (lesser echoes; kill the boss a few times to earn its Soul) ---
            new GrimoireEntry
            {
                Id = "GuardianSlime",
                DisplayName = "Guardian Slime",
                Category = "Boss",
                SourceNPCs = new int[] { NPCID.KingSlime },
                KillThreshold = 3,
                RequiredRank = 1,
                SoulType = ModContent.ItemType<KingSlimeSoul>,
                MinionType = ModContent.ProjectileType<GuardianSlimeMinion>
            },
            new GrimoireEntry
            {
                Id = "EyeSpirit",
                DisplayName = "Eye Spirit",
                Category = "Boss",
                SourceNPCs = new int[] { NPCID.EyeofCthulhu },
                KillThreshold = 3,
                RequiredRank = 1,
                SoulType = ModContent.ItemType<EyeOfCthulhuSoul>,
                MinionType = ModContent.ProjectileType<EyeSpiritMinion>
            }
        };

        public static GrimoireEntry ById(string id)
        {
            foreach (GrimoireEntry e in Entries)
            {
                if (e.Id == id)
                {
                    return e;
                }
            }

            return null;
        }

        // The creature whose source list contains this NPC type (for kill tracking /
        // soul drops), or null.
        public static GrimoireEntry ForSourceNPC(int npcType)
        {
            foreach (GrimoireEntry e in Entries)
            {
                foreach (int src in e.SourceNPCs)
                {
                    if (src == npcType)
                    {
                        return e;
                    }
                }
            }

            return null;
        }
    }
}
