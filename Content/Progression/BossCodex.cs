using System.Collections.Generic;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.NPCs.Bosses;

namespace Eternia.Content.Progression
{
    // The curated list of bosses the Boss Codex tracks, in progression order. Eternia has no
    // bosses of its own yet, so this is the vanilla ladder -- plus one locked teaser for the
    // Eternia final boss that the endgame gear already hints at.
    //
    // Each entry knows the NPC ids that make up that fight (worms and multi-part bosses have
    // several), so a kill is only logged as a completion when the LAST living piece falls.
    public readonly struct BossCodexEntry
    {
        public BossCodexEntry(
            int id,
            string name,
            bool hardmode,
            int[] group,
            string drops)
        {
            Id = id;
            Name = name;
            Hardmode = hardmode;
            Group = group;
            Drops = drops;
        }

        // Canonical NPC id -- also the persistence key. -1 for the locked teaser.
        public int Id { get; }

        public string Name { get; }

        public bool Hardmode { get; }

        // Every NPC id that belongs to this fight (the fight is done when none are active).
        public int[] Group { get; }

        public string Drops { get; }

        public bool IsMystery => Id < 0;
    }

    public static class BossCodex
    {
        private static BossCodexEntry[] entries;

        private static readonly Dictionary<int, int> MemberToIndex =
            new Dictionary<int, int>();

        private static bool built;

        public static BossCodexEntry[] Entries
        {
            get
            {
                EnsureBuilt();
                return entries;
            }
        }

        // Built lazily on first use. The Prototype entries need ModContent NPC types, which only
        // exist after the mod has loaded -- so this must NOT run in a static constructor.
        private static void EnsureBuilt()
        {
            if (built)
            {
                return;
            }

            built = true;

            int prototype = ModContent.NPCType<Prototype01>();

            List<BossCodexEntry> list = new List<BossCodexEntry>
            {
                new BossCodexEntry(NPCID.KingSlime, "King Slime", false,
                    new int[] { NPCID.KingSlime },
                    "Slime crown, Ninja gear, Solidifier"),
                new BossCodexEntry(NPCID.EyeofCthulhu, "Eye of Cthulhu", false,
                    new int[] { NPCID.EyeofCthulhu },
                    "Demonite / Crimtane ore, Unholy Arrows"),
                new BossCodexEntry(NPCID.EaterofWorldsHead, "Eater of Worlds", false,
                    new int[] { NPCID.EaterofWorldsHead, NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail },
                    "Shadow Scales, Demonite Ore"),
                new BossCodexEntry(NPCID.BrainofCthulhu, "Brain of Cthulhu", false,
                    new int[] { NPCID.BrainofCthulhu },
                    "Tissue Samples, Crimtane Ore"),
                new BossCodexEntry(NPCID.QueenBee, "Queen Bee", false,
                    new int[] { NPCID.QueenBee },
                    "Bee gear, Honey blocks, Bee Wax"),
                new BossCodexEntry(NPCID.SkeletronHead, "Skeletron", false,
                    new int[] { NPCID.SkeletronHead, NPCID.SkeletronHand },
                    "Book of Skulls, Skeletron Hand"),
                new BossCodexEntry(NPCID.Deerclops, "Deerclops", false,
                    new int[] { NPCID.Deerclops },
                    "Eye Bone, Pew-matic Horn, Weather Pain"),

                // Eternia's own boss: the failed Soul vessel, at the end of pre-Hardmode.
                new BossCodexEntry(prototype, "Prototype-01", false,
                    new int[] { prototype },
                    "Prototype Core, Soul Alloy, Soulforged Sabre"),

                new BossCodexEntry(NPCID.WallofFlesh, "Wall of Flesh", false,
                    new int[] { NPCID.WallofFlesh, NPCID.WallofFleshEye },
                    "Pwnhammer, Breaker Blade, Emblems  --  AWAKENS YOUR SUBCLASS"),

                new BossCodexEntry(NPCID.QueenSlimeBoss, "Queen Slime", true,
                    new int[] { NPCID.QueenSlimeBoss },
                    "Gelatinous gear, Volatile Gelatin"),
                new BossCodexEntry(NPCID.Retinazer, "The Twins", true,
                    new int[] { NPCID.Retinazer, NPCID.Spazmatism },
                    "Souls of Sight, Hallowed Bars"),
                new BossCodexEntry(NPCID.TheDestroyer, "The Destroyer", true,
                    new int[] { NPCID.TheDestroyer, NPCID.TheDestroyerBody, NPCID.TheDestroyerTail },
                    "Souls of Might, Hallowed Bars"),
                new BossCodexEntry(NPCID.SkeletronPrime, "Skeletron Prime", true,
                    new int[] { NPCID.SkeletronPrime, NPCID.PrimeCannon, NPCID.PrimeSaw, NPCID.PrimeVice, NPCID.PrimeLaser },
                    "Souls of Fright, Hallowed Bars"),
                new BossCodexEntry(NPCID.Plantera, "Plantera", true,
                    new int[] { NPCID.Plantera },
                    "Temple Key, Grenade Launcher, Nettle Burst"),
                new BossCodexEntry(NPCID.Golem, "Golem", true,
                    new int[] { NPCID.Golem, NPCID.GolemHead, NPCID.GolemHeadFree, NPCID.GolemFistLeft, NPCID.GolemFistRight },
                    "Picksaw, Stynger, Sun Stone, Beetle husks"),
                new BossCodexEntry(NPCID.DukeFishron, "Duke Fishron", true,
                    new int[] { NPCID.DukeFishron },
                    "Flairon, Razorblade Typhoon, Tsunami"),
                new BossCodexEntry(NPCID.HallowBoss, "Empress of Light", true,
                    new int[] { NPCID.HallowBoss },
                    "Terraprisma, Empress wings, Nightglow"),
                new BossCodexEntry(NPCID.CultistBoss, "Lunatic Cultist", true,
                    new int[] { NPCID.CultistBoss },
                    "Opens the Lunar Events"),
                new BossCodexEntry(NPCID.MoonLordCore, "Moon Lord", true,
                    new int[] { NPCID.MoonLordCore, NPCID.MoonLordHand, NPCID.MoonLordHead },
                    "Luminite, Meowmere, Terrarian, Celebration"),

                // The dangling thread: the endgame Ranger gear already crafts toward an Eternia
                // final boss. It does not exist yet, so it stays a locked mystery here.
                new BossCodexEntry(-1, "The Eternal  (not yet risen)", true,
                    new int[0],
                    "???"),
            };

            entries = list.ToArray();

            MemberToIndex.Clear();

            for (int i = 0; i < entries.Length; i++)
            {
                foreach (int member in entries[i].Group)
                {
                    MemberToIndex[member] = i;
                }
            }
        }

        // If the killed NPC belongs to a tracked fight AND no other piece of that fight is still
        // alive, returns the completed entry. Otherwise returns false (a mid-fight segment death,
        // or an untracked NPC).
        public static bool TryResolveCompletion(NPC killed, out BossCodexEntry entry)
        {
            entry = default;

            EnsureBuilt();

            if (!MemberToIndex.TryGetValue(killed.type, out int index))
            {
                return false;
            }

            entry = entries[index];

            foreach (int member in entry.Group)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC other = Main.npc[i];

                    if (i != killed.whoAmI &&
                        other.active &&
                        other.type == member)
                    {
                        // A piece of this boss is still alive -- not a completion yet.
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
