using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Players;
using Eternia.Content.Progression;

namespace Eternia.Content.Globals
{
    // Times boss fights and writes them into your personal Boss Codex. Each boss NPC remembers the
    // frame it spawned; when the fight ends (the last living piece falls) we log the duration, the
    // kill, and the rarity the boss rolled.
    //
    // Scope: singleplayer / the local player. In multiplayer this is a known gap -- the boss log is
    // per-character and there is no netcode to award it to every participant yet. It never runs on
    // a dedicated server (no local player, no UI).
    public class BossLogGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        private uint spawnTick;
        private bool stamped;

        public override void OnSpawn(NPC npc, Terraria.DataStructures.IEntitySource source)
        {
            if (npc.boss || IsTrackedPiece(npc.type))
            {
                spawnTick = Main.GameUpdateCount;
                stamped = true;
            }
        }

        public override void OnKill(NPC npc)
        {
            if (Main.dedServ)
            {
                return;
            }

            if (!BossCodex.TryResolveCompletion(npc, out BossCodexEntry entry))
            {
                return;
            }

            int durationTicks =
                stamped && Main.GameUpdateCount > spawnTick
                    ? (int)(Main.GameUpdateCount - spawnTick)
                    : 0;

            byte rarity =
                (byte)npc.GetGlobalNPC<EterniaGlobalNPC>().rarity;

            Main.LocalPlayer
                .GetModPlayer<BossLogPlayer>()
                .Record(entry.Id, rarity, durationTicks);
        }

        // Is this NPC type part of any tracked fight? Lets worm/segment pieces (which are not
        // flagged npc.boss) still get their spawn frame stamped. The entry list is tiny.
        private static bool IsTrackedPiece(int type)
        {
            foreach (BossCodexEntry entry in BossCodex.Entries)
            {
                foreach (int member in entry.Group)
                {
                    if (member == type)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
