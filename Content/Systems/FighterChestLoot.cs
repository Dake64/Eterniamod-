using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Weapons.Fighter;

namespace Eternia.Content.Systems
{
    // Seeds the Prospector's Gauntlets into underground chests at world generation, so
    // finding them means exploring the caverns rather than crafting. Only affects newly
    // generated worlds (like all vanilla chest loot); existing worlds fall back to the
    // Undead Miner trickle in FighterDropsGlobalNPC.
    public class FighterChestLoot : ModSystem
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

                // Underground only (below the surface, above the Underworld) and NOT in
                // the Dungeon (that space is the Swordsman's Bonewarden Sabre).
                if (chest.y <= Main.worldSurface ||
                    chest.y >= Main.maxTilesY - 200 ||
                    Main.wallDungeon[Main.tile[chest.x, chest.y].WallType])
                {
                    continue;
                }

                // Roughly one in four qualifying chests, so it stays a find.
                if (!WorldGen.genRand.NextBool(4))
                {
                    continue;
                }

                for (int slot = 0; slot < chest.item.Length; slot++)
                {
                    if (chest.item[slot].type == ItemID.None)
                    {
                        chest.item[slot].SetDefaults(
                            ModContent.ItemType<ProspectorsGauntlets>());
                        chest.item[slot].stack = 1;
                        placed++;
                        break;
                    }
                }
            }
        }
    }
}
