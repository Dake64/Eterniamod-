using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;

using Eternia.Content.Items.Weapons.Ranger;

namespace Eternia.Content.Globals
{
    // Obtention for the drop-based Gunner guns. The rest register their own recipes.
    public class GunnerObtentionGlobalNPC : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            // Martian Saucer -> Storm Minigun.
            if (npc.type == NPCID.MartianSaucerCore)
            {
                npcLoot.Add(ItemDropRule.Common(
                    ModContent.ItemType<StormMinigun>(), 4));
            }
        }
    }
}
