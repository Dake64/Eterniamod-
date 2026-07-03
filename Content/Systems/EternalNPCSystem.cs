using Eternia.Content.Souls;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Eternia.Content.Systems
{
    public class EternalNPCSystem : ModSystem
    {
        public override void PostUpdateWorld()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            if (NPC.AnyNPCs(ModContent.NPCType<Eternia.Content.NPCs.EternalNPC>()))
            {
                return;
            }

            if (!TryFindPlayerNeedingSoul(out Player player))
            {
                return;
            }

            NPC.NewNPC(
                player.GetSource_Misc("EternalSpawn"),
                (int)player.Center.X + 100,
                (int)player.Center.Y,
                ModContent.NPCType<Eternia.Content.NPCs.EternalNPC>()
            );
        }

        private static bool TryFindPlayerNeedingSoul(
            out Player player)
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player candidate = Main.player[i];

                if (candidate == null ||
                    !candidate.active ||
                    candidate.dead ||
                    SoulInventory.HasAnySoulItem(candidate))
                {
                    continue;
                }

                player = candidate;
                return true;
            }

            player = null;
            return false;
        }
    }
}
