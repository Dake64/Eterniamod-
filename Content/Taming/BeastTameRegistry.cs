using System;

using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Weapons.Summoner;

namespace Eternia.Content.Taming
{
    // One tameable beast: which wild creatures tame it, and the staff you earn for taming it.
    // The Beast Tamer obtains its pack by TAMING (weaken a creature to low health, then whip it)
    // rather than crafting. Vanilla lacks literal raptors/sabertooths, so a few sources are the
    // closest thematic fit -- tweak SourceNPCs freely, this table is the single source of truth.
    public class BeastTameEntry
    {
        public string Id;
        public string DisplayName;
        public int[] SourceNPCs;
        public Func<int> StaffType;
    }

    public static class BeastTameRegistry
    {
        public static readonly BeastTameEntry[] Entries =
        {
            new BeastTameEntry
            {
                Id = "Wolf",
                DisplayName = "Wolf",
                SourceNPCs = new int[] { NPCID.Wolf },
                StaffType = () => ModContent.ItemType<WolfFangTotem>()
            },
            new BeastTameEntry
            {
                Id = "Boar",
                DisplayName = "Boar",
                SourceNPCs = new int[] { NPCID.GraniteGolem, NPCID.GraniteFlyer },
                StaffType = () => ModContent.ItemType<BoarhideTotem>()
            },
            new BeastTameEntry
            {
                Id = "Raptor",
                DisplayName = "Raptor",
                SourceNPCs = new int[] { NPCID.GiantFlyingFox },
                StaffType = () => ModContent.ItemType<RaptorTalon>()
            },
            new BeastTameEntry
            {
                Id = "Bear",
                DisplayName = "Bear",
                SourceNPCs = new int[] { NPCID.Unicorn },
                StaffType = () => ModContent.ItemType<UrsineTotem>()
            },
            new BeastTameEntry
            {
                Id = "Sabertooth",
                DisplayName = "Sabertooth",
                SourceNPCs = new int[] { NPCID.Werewolf },
                StaffType = () => ModContent.ItemType<SabertoothFang>()
            },
            new BeastTameEntry
            {
                Id = "Wyvern",
                DisplayName = "Wyvern",
                SourceNPCs = new int[] { NPCID.WyvernHead },
                StaffType = () => ModContent.ItemType<Wyrmcaller>()
            }
        };

        public static BeastTameEntry ByNPC(int npcType)
        {
            foreach (BeastTameEntry entry in Entries)
            {
                foreach (int source in entry.SourceNPCs)
                {
                    if (source == npcType)
                    {
                        return entry;
                    }
                }
            }

            return null;
        }
    }
}
