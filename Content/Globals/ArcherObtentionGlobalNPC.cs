using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;

using Eternia.Content.Items.Weapons.Ranger;

namespace Eternia.Content.Globals
{
    // Obtention for the drop- and shop-based Archer bows. Craftable bows register their own
    // recipes; these come from the boss / event / merchant the spec ties them to.
    public class ArcherObtentionGlobalNPC : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            switch (npc.type)
            {
                // Ice Mimic -> Frostpiercer.
                case NPCID.IceMimic:
                    npcLoot.Add(ItemDropRule.Common(
                        ModContent.ItemType<Frostpiercer>(), 3));
                    break;

                // Golem (guardian of the Jungle Temple) -> Temple Judgement.
                case NPCID.Golem:
                    npcLoot.Add(ItemDropRule.Common(
                        ModContent.ItemType<TempleJudgement>(), 3));
                    break;

                // Mothron (Solar Eclipse) -> Eclipse Recurve.
                case NPCID.Mothron:
                    npcLoot.Add(ItemDropRule.Common(
                        ModContent.ItemType<EclipseRecurve>(), 5));
                    break;

                // Duke Fishron -> Dragonbone Bow.
                case NPCID.DukeFishron:
                    npcLoot.Add(ItemDropRule.Common(
                        ModContent.ItemType<DragonboneBow>(), 4));
                    break;
            }
        }

        // Woodland Longbow: sold by the Merchant once the Eye of Cthulhu is down.
        public override void ModifyActiveShop(NPC npc, string shopName, Item[] items)
        {
            if (npc.type != NPCID.Merchant || !NPC.downedBoss1)
            {
                return;
            }

            for (int i = 0; i < items.Length - 1; i++)
            {
                if (items[i] == null || items[i].type == ItemID.None)
                {
                    items[i] = new Item(ModContent.ItemType<WoodlandLongbow>());
                    break;
                }
            }
        }
    }
}
