using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Weapons.Warrior;

namespace Eternia.Content.Systems
{
    // Seeds the Bonewarden Sabre into Dungeon chests at world generation, so finding
    // it means exploring the Dungeon rather than crafting. Only affects newly
    // generated worlds (like all vanilla chest loot); existing worlds fall back to
    // the Dungeon-undead drop in SwordsmanDropsGlobalNPC.
    public class SwordsmanChestLoot : ModSystem
    {
        public override void PostWorldGen()
        {
            int placed = 0;

            for (int i = 0; i < Main.maxChests && placed < 3; i++)
            {
                Chest chest = Main.chest[i];

                if (chest == null)
                {
                    continue;
                }

                // Only chests set into Dungeon-brick walls.
                if (!Main.wallDungeon[Main.tile[chest.x, chest.y].WallType])
                {
                    continue;
                }

                // Roughly one in three dungeon chests, so it stays a find.
                if (!WorldGen.genRand.NextBool(3))
                {
                    continue;
                }

                for (int slot = 0; slot < chest.item.Length; slot++)
                {
                    if (chest.item[slot].type == ItemID.None)
                    {
                        chest.item[slot].SetDefaults(
                            ModContent.ItemType<BonewardenSabre>());
                        chest.item[slot].stack = 1;
                        placed++;
                        break;
                    }
                }
            }
        }
    }
}
