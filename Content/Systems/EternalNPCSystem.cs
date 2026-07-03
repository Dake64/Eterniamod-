using Terraria;
using Terraria.ModLoader;

namespace Eternia.Content.Systems
{
    public class EternalNPCSystem : ModSystem
    {
        public override void PostUpdateWorld()
        {
            // Si ya existe, no hacer nada
            if (NPC.AnyNPCs(ModContent.NPCType<Eternia.Content.NPCs.EternalNPC>()))
                return;

            // Spawnea cerca del jugador
            Player player = Main.LocalPlayer;

            if (player != null && player.active)
            {
                NPC.NewNPC(
                    player.GetSource_Misc("EternalSpawn"),
                    (int)player.Center.X + 100,
                    (int)player.Center.Y,
                    ModContent.NPCType<Eternia.Content.NPCs.EternalNPC>()
                );
            }
        }
    }
}