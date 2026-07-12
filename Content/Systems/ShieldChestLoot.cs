using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Weapons.Guardian;

namespace Eternia.Content.Systems
{
    // Seeds the Glacial Shield into underground chests at world generation, so a shield
    // can also be a find while exploring (it stays craftable too -- this is a bonus
    // path). Only affects newly generated worlds, like all vanilla chest loot.
    public class ShieldChestLoot : ModSystem
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
                // the Dungeon (reserved for the Swordsman's Bonewarden Sabre) or where
                // the fist Prospector's Gauntlets are seeded -- different roll, so a
                // chest can hold at most one.
                if (chest.y <= Main.worldSurface ||
                    chest.y >= Main.maxTilesY - 200 ||
                    Main.wallDungeon[Main.tile[chest.x, chest.y].WallType])
                {
                    continue;
                }

                // Roughly one in five qualifying chests, so it stays a find.
                if (!WorldGen.genRand.NextBool(5))
                {
                    continue;
                }

                for (int slot = 0; slot < chest.item.Length; slot++)
                {
                    if (chest.item[slot].type == ItemID.None)
                    {
                        chest.item[slot].SetDefaults(
                            ModContent.ItemType<GlacialShield>());
                        chest.item[slot].stack = 1;
                        placed++;
                        break;
                    }
                }
            }
        }
    }
}
