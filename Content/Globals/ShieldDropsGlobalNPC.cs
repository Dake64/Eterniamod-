using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;

using Eternia.Content.Items.Weapons.Guardian;

namespace Eternia.Content.Globals
{
    // Second obtention pass for the shield line: tie the tier shields to the boss/zone
    // they belong to instead of a plain material gate.
    //
    //  - Corrupt Shield has NO recipe -> the evil-boss drop is the only way to get it.
    //  - Hallowed Bulwark, Nightfall Barrier and Plaguebringer Wall stay craftable;
    //    these drops are a bonus path from the fitting boss/event.
    public class ShieldDropsGlobalNPC : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            switch (npc.type)
            {
                // Crimson evil boss -> the evil-biome shield (drop-only).
                // (The Corruption's Eater of Worlds is handled in OnKill, because a
                // worm boss would otherwise roll the drop once per severed head.)
                case NPCID.BrainofCthulhu:
                    npcLoot.Add(ItemDropRule.Common(
                        ModContent.ItemType<CorruptShield>(), 2));
                    break;

                // Mechanical bosses -> the Hallow shield (bonus; also craftable).
                case NPCID.TheDestroyer:
                case NPCID.Retinazer:
                case NPCID.Spazmatism:
                case NPCID.SkeletronPrime:
                    npcLoot.Add(ItemDropRule.Common(
                        ModContent.ItemType<HallowedBulwark>(), 4));
                    break;

                // Solar Eclipse -> the eclipse shield (bonus; also craftable).
                case NPCID.Mothron:
                    npcLoot.Add(ItemDropRule.Common(
                        ModContent.ItemType<NightfallBarrier>(), 4));
                    break;

                // Plantera -> the boss-tier shield (bonus; also craftable).
                case NPCID.Plantera:
                    npcLoot.Add(ItemDropRule.Common(
                        ModContent.ItemType<PlaguebringerWall>(), 3));
                    break;
            }
        }

        public override void OnKill(NPC npc)
        {
            // Eater of Worlds (Corruption): a worm boss. Only the death of its LAST
            // remaining segment is the true boss kill, so gate the drop on that.
            if (npc.type == NPCID.EaterofWorldsHead ||
                npc.type == NPCID.EaterofWorldsBody ||
                npc.type == NPCID.EaterofWorldsTail)
            {
                int remaining =
                    NPC.CountNPCS(NPCID.EaterofWorldsHead) +
                    NPC.CountNPCS(NPCID.EaterofWorldsBody) +
                    NPC.CountNPCS(NPCID.EaterofWorldsTail);

                if (remaining <= 1 && Main.rand.NextBool(2))
                {
                    Item.NewItem(
                        npc.GetSource_Loot(),
                        npc.Hitbox,
                        ModContent.ItemType<CorruptShield>());
                }
            }
        }
    }
}
