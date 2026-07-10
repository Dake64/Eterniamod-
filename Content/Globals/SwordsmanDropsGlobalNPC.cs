using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;

using Eternia.Content.Items.Weapons.Warrior;

namespace Eternia.Content.Globals
{
    // Second obtention pass for the Swordsman sword line: tie the tier-gating swords
    // to the boss / zone they belong to instead of a soft material gate.
    //
    //  - Dread Reaver and Bonewarden Sabre have NO recipe -> these drops are the only
    //    way to get them (Bonewarden also seeds Dungeon chests via SwordsmanChestLoot).
    //  - Thornrender and Crimson Requiem stay craftable; these drops are a bonus path.
    public class SwordsmanDropsGlobalNPC : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            switch (npc.type)
            {
                case NPCID.EyeofCthulhu:
                    npcLoot.Add(ItemDropRule.Common(
                        ModContent.ItemType<DreadReaver>(), 2));
                    break;

                case NPCID.QueenBee:
                    npcLoot.Add(ItemDropRule.Common(
                        ModContent.ItemType<Thornrender>(), 3));
                    break;

                case NPCID.Mothron:
                    npcLoot.Add(ItemDropRule.Common(
                        ModContent.ItemType<CrimsonRequiem>(), 4));
                    break;

                // The Dungeon's undead: a rare trickle so Bonewarden Sabre is
                // reachable on worlds generated before this update too.
                case NPCID.AngryBones:
                case NPCID.AngryBonesBig:
                case NPCID.AngryBonesBigHelmet:
                case NPCID.AngryBonesBigMuscle:
                case NPCID.DarkCaster:
                case NPCID.CursedSkull:
                    npcLoot.Add(ItemDropRule.Common(
                        ModContent.ItemType<BonewardenSabre>(), 60));
                    break;
            }
        }
    }
}
