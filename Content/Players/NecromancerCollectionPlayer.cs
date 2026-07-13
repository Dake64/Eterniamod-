using System.Collections.Generic;
using System.Linq;

using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

using Eternia.Content.Necromancy;

namespace Eternia.Content.Players
{
    // The player's Grimoire of Death: how many of each enemy they have slain, and which
    // creatures they have dominated (unlocked). Persistent per character.
    public class NecromancerCollectionPlayer : ModPlayer
    {
        // NPC type -> kills.
        public Dictionary<int, int> Kills = new();

        // Grimoire creature Ids the player has dominated.
        public List<string> Unlocked = new();

        // The prepared loadout: which dominated creatures can be raised right now
        // (capped by the Grimoire rank's active-page limit). Ordered oldest-first.
        public List<string> ActivePages = new();

        // Which eligible creature the Grimoire currently points at.
        public int SelectedIndex;

        public void AddKill(int npcType)
        {
            if (Kills.ContainsKey(npcType))
            {
                Kills[npcType]++;
            }
            else
            {
                Kills[npcType] = 1;
            }
        }

        public int KillsForEntry(GrimoireEntry entry)
        {
            int total = 0;

            foreach (int src in entry.SourceNPCs)
            {
                if (Kills.TryGetValue(src, out int k))
                {
                    total += k;
                }
            }

            return total;
        }

        public bool IsUnlocked(GrimoireEntry entry)
        {
            return entry.DefaultUnlocked || Unlocked.Contains(entry.Id);
        }

        public bool Unlock(string id)
        {
            if (Unlocked.Contains(id))
            {
                return false;
            }

            Unlocked.Add(id);
            return true;
        }

        // Dominated creatures the current Grimoire rank allows (what you can cycle to).
        public List<GrimoireEntry> EligibleEntries()
        {
            int rank = GrimoireRank.Current();

            return GrimoireRegistry.Entries
                .Where(e => IsUnlocked(e) && e.RequiredRank <= rank)
                .ToList();
        }

        public int DominatedCount() => GrimoireRegistry.Entries.Count(IsUnlocked);

        public int TotalCount() => GrimoireRegistry.Entries.Count;

        // The un-dominated, rank-eligible creature closest to being unlocked, so the HUD
        // can nudge the player ("kill 30 more Zombies").
        public GrimoireEntry NextTarget()
        {
            int rank = GrimoireRank.Current();
            GrimoireEntry best = null;
            float bestRatio = -1f;

            foreach (GrimoireEntry e in GrimoireRegistry.Entries)
            {
                if (IsUnlocked(e) || e.RequiredRank > rank || e.KillThreshold <= 0)
                {
                    continue;
                }

                float ratio = (float)KillsForEntry(e) / e.KillThreshold;

                if (ratio > bestRatio)
                {
                    bestRatio = ratio;
                    best = e;
                }
            }

            return best;
        }

        public bool IsActivePage(string id) => ActivePages.Contains(id);

        // Make a creature part of the active loadout. If the pages are full, the oldest
        // active creature is dropped (and its undead crumble).
        public void EnsureActive(string id)
        {
            if (ActivePages.Contains(id))
            {
                ActivePages.Remove(id);
                ActivePages.Add(id); // most-recently used
                return;
            }

            int max = GrimoireRank.MaxActivePages();

            while (ActivePages.Count >= max && ActivePages.Count > 0)
            {
                string evicted = ActivePages[0];
                ActivePages.RemoveAt(0);
                DespawnCreature(evicted);
            }

            ActivePages.Add(id);
        }

        // Kill any active undead of a creature that just left the loadout.
        private void DespawnCreature(string id)
        {
            GrimoireEntry entry = GrimoireRegistry.ById(id);

            if (entry == null)
            {
                return;
            }

            int type = entry.MinionType();

            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.owner == Player.whoAmI && proj.type == type)
                {
                    proj.Kill();
                }
            }
        }

        public override void SaveData(TagCompound tag)
        {
            tag["NecroKillTypes"] = Kills.Keys.ToList();
            tag["NecroKillCounts"] = Kills.Values.ToList();
            tag["NecroUnlocked"] = Unlocked;
            tag["NecroActivePages"] = ActivePages;
        }

        public override void LoadData(TagCompound tag)
        {
            Kills = new Dictionary<int, int>();

            if (tag.ContainsKey("NecroKillTypes") && tag.ContainsKey("NecroKillCounts"))
            {
                var types = tag.GetList<int>("NecroKillTypes");
                var counts = tag.GetList<int>("NecroKillCounts");

                for (int i = 0; i < types.Count && i < counts.Count; i++)
                {
                    Kills[types[i]] = counts[i];
                }
            }

            Unlocked = tag.ContainsKey("NecroUnlocked")
                ? tag.GetList<string>("NecroUnlocked").ToList()
                : new List<string>();

            ActivePages = tag.ContainsKey("NecroActivePages")
                ? tag.GetList<string>("NecroActivePages").ToList()
                : new List<string>();
        }
    }
}
