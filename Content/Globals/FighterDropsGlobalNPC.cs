using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;

using Eternia.Content.Items.Weapons.Fighter;

namespace Eternia.Content.Globals
{
    // Second obtention pass for the Peleador fist line: tie the tier fists to the boss
    // / zone they belong to instead of a plain material gate.
    //
    //  - Bloodfeast Gauntlets and Prospector's Gauntlets have NO recipe -> these drops
    //    (and the Prospector's chest seed in FighterChestLoot) are the only way to get
    //    them.
    //  - Thornfist, Nightfall and Plaguebringer stay craftable; these drops are a
    //    bonus path tied to the fitting boss/event.
    public class FighterDropsGlobalNPC : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            switch (npc.type)
            {
                // Crimson evil boss -> the final pre-Hardmode fists (drop-only).
                // (The Corruption's Eater of Worlds is handled in OnKill because a worm
                // boss would otherwise roll the drop once per severed head.)
                case NPCID.BrainofCthulhu:
                    npcLoot.Add(ItemDropRule.Common(
                        ModContent.ItemType<BloodfeastGauntlets>(), 2));
                    break;

                // Jungle boss -> the jungle fists (bonus; also craftable).
                case NPCID.QueenBee:
                    npcLoot.Add(ItemDropRule.Common(
                        ModContent.ItemType<ThornfistGauntlets>(), 3));
                    break;

                // Solar Eclipse -> the eclipse fists (bonus; also craftable).
                case NPCID.Mothron:
                    npcLoot.Add(ItemDropRule.Common(
                        ModContent.ItemType<NightfallGauntlets>(), 4));
                    break;

                // Plantera -> the boss-tier fists (bonus; also craftable).
                case NPCID.Plantera:
                    npcLoot.Add(ItemDropRule.Common(
                        ModContent.ItemType<PlaguebringerFists>(), 3));
                    break;

                // Undead Miner (a prospector's ghost): a rare trickle so the
                // Prospector's Gauntlets are reachable on old worlds too.
                case NPCID.UndeadMiner:
                    npcLoot.Add(ItemDropRule.Common(
                        ModContent.ItemType<ProspectorsGauntlets>(), 8));
                    break;
            }
        }

        public override void OnKill(NPC npc)
        {
            // Eater of Worlds (Corruption): a worm boss. Only the death of its LAST
            // remaining segment is the true boss kill, so gate the drop on that to
            // avoid one drop per severed head.
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
                        ModContent.ItemType<BloodfeastGauntlets>());
                }
            }
        }
    }
}
