using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;

using Eternia.Content.Necromancy;
using Eternia.Content.Players;

namespace Eternia.Content.Globals
{
    // Tracks kills of Grimoire creatures and, once the threshold is met, lets the enemy
    // drop its Soul so the player can dominate it.
    public class NecromancerKillGlobalNPC : GlobalNPC
    {
        public override void OnKill(NPC npc)
        {
            GrimoireEntry entry = GrimoireRegistry.ForSourceNPC(npc.type);

            if (entry == null || entry.KillThreshold <= 0)
            {
                return;
            }

            // Attribute the kill to the local player (single-player focused).
            if (Main.LocalPlayer != null && Main.LocalPlayer.active)
            {
                Main.LocalPlayer
                    .GetModPlayer<NecromancerCollectionPlayer>()
                    .AddKill(npc.type);
            }
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            GrimoireEntry entry = GrimoireRegistry.ForSourceNPC(npc.type);

            if (entry == null || entry.KillThreshold <= 0)
            {
                return;
            }

            int soul = entry.SoulType();

            if (soul <= 0)
            {
                return;
            }

            // 1/5 chance once the kill threshold is met and the creature is not yet
            // dominated (SoulDropCondition).
            npcLoot.Add(ItemDropRule.ByCondition(
                new SoulDropCondition(entry.Id),
                soul,
                5));
        }
    }
}
