using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;

using Eternia.Content.Items.Weapons.Magic;

namespace Eternia.Content.Globals
{
    // Obtention for the Elemental Mage arsenal: tie the drop-only staves to the boss /
    // event / biome they belong to. The craftable staves are unaffected.
    public class ElementalDropsGlobalNPC : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            switch (npc.type)
            {
                // King Slime -> Spark Rod (Lightning).
                case NPCID.KingSlime:
                    npcLoot.Add(ItemDropRule.Common(
                        ModContent.ItemType<SparkRod>(), 2));
                    break;

                // Skeletron -> Elemental Sage Scepter (best pre-Hardmode magic weapon).
                case NPCID.SkeletronHead:
                    npcLoot.Add(ItemDropRule.Common(
                        ModContent.ItemType<ElementalSageScepter>(), 2));
                    break;

                // Granite biome enemies -> Granite Core (Earth), a rare trickle.
                case NPCID.GraniteFlyer:
                case NPCID.GraniteGolem:
                    npcLoot.Add(ItemDropRule.Common(
                        ModContent.ItemType<GraniteCore>(), 12));
                    break;

                // Pirate Invasion -> Hurricane Staff (Wind).
                case NPCID.PirateCaptain:
                case NPCID.PirateShip:
                    npcLoot.Add(ItemDropRule.Common(
                        ModContent.ItemType<HurricaneStaff>(), 3));
                    break;

                // Golem -> Seismic Scepter (Earth).
                case NPCID.Golem:
                    npcLoot.Add(ItemDropRule.Common(
                        ModContent.ItemType<SeismicScepter>(), 2));
                    break;

                // Nebula Pillar -> Cataclysm Staff.
                case NPCID.LunarTowerNebula:
                    npcLoot.Add(ItemDropRule.Common(
                        ModContent.ItemType<CataclysmStaff>(), 2));
                    break;
            }
        }
    }
}
