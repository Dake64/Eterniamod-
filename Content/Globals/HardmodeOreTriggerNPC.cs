using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Systems;

namespace Eternia.Content.Globals
{
    // Grows Eternia's Hardmode soul-metals into the world the moment the Wall of Flesh falls and
    // the world turns -- the same trigger vanilla uses for Cobalt/Mythril/Adamantite. Guarded by a
    // persisted world flag so it only ever happens once per world, and only on the server / in
    // singleplayer (world edits are authoritative there).
    public class HardmodeOreTriggerNPC : GlobalNPC
    {
        public override void OnKill(NPC npc)
        {
            if (npc.type != NPCID.WallofFlesh)
            {
                return;
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            EterniaOreGeneration.SeedHardmodeOres();
        }
    }
}
