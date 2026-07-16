using System.Collections.Generic;

using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Eternia.Content.Players
{
    // Your personal boss record. For each boss you have beaten it remembers: how many times, your
    // fastest clear, and the highest enemy-rarity you have ever faced it at. Keyed by the boss's
    // canonical NPC id, saved with the character.
    public class BossLogPlayer : ModPlayer
    {
        public struct BossRecord
        {
            public int Kills;
            public int BestKillTicks; // 0 = never timed
            public byte HighestRarity; // EterniaGlobalNPC.EnemyRarity value
        }

        private readonly Dictionary<int, BossRecord> records =
            new Dictionary<int, BossRecord>();

        public bool TryGet(int bossId, out BossRecord record)
        {
            return records.TryGetValue(bossId, out record);
        }

        public int DefeatedCount => records.Count;

        // Logs one completed boss fight. kills accumulate, best time keeps the fastest, rarity
        // keeps the highest ever seen. durationTicks <= 0 means "not timed" and is ignored.
        public void Record(int bossId, byte rarity, int durationTicks)
        {
            records.TryGetValue(bossId, out BossRecord record);

            record.Kills++;

            if (rarity > record.HighestRarity)
            {
                record.HighestRarity = rarity;
            }

            if (durationTicks > 0 &&
                (record.BestKillTicks == 0 || durationTicks < record.BestKillTicks))
            {
                record.BestKillTicks = durationTicks;
            }

            records[bossId] = record;
        }

        public override void SaveData(TagCompound tag)
        {
            List<int> ids = new List<int>();
            List<int> kills = new List<int>();
            List<int> best = new List<int>();
            List<int> rarity = new List<int>();

            foreach (KeyValuePair<int, BossRecord> pair in records)
            {
                ids.Add(pair.Key);
                kills.Add(pair.Value.Kills);
                best.Add(pair.Value.BestKillTicks);
                rarity.Add(pair.Value.HighestRarity);
            }

            tag["BossLogIds"] = ids;
            tag["BossLogKills"] = kills;
            tag["BossLogBest"] = best;
            tag["BossLogRarity"] = rarity;
        }

        public override void LoadData(TagCompound tag)
        {
            records.Clear();

            if (!tag.ContainsKey("BossLogIds"))
            {
                return;
            }

            List<int> ids = tag.Get<List<int>>("BossLogIds");
            List<int> kills = tag.Get<List<int>>("BossLogKills");
            List<int> best = tag.Get<List<int>>("BossLogBest");
            List<int> rarity = tag.Get<List<int>>("BossLogRarity");

            for (int i = 0; i < ids.Count; i++)
            {
                records[ids[i]] = new BossRecord
                {
                    Kills = Safe(kills, i),
                    BestKillTicks = Safe(best, i),
                    HighestRarity = (byte)Safe(rarity, i)
                };
            }
        }

        private static int Safe(List<int> list, int index)
        {
            return list != null && index < list.Count
                ? list[index]
                : 0;
        }
    }
}
