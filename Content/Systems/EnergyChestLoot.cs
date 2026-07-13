using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Eternia.Content.Items.Materials;

namespace Eternia.Content.Systems
{
    // Seeds Ancient Batteries into Dungeon chests at world generation, so the rarest tech
    // material is also a reward for delving the Dungeon rather than purely a grind drop.
    // Only affects newly generated worlds.
    public class EnergyChestLoot : ModSystem
    {
        public override void PostWorldGen()
        {
            int placed = 0;

            for (int i = 0; i < Main.maxChests && placed < 6; i++)
            {
                Chest chest = Main.chest[i];

                if (chest == null || !IsDungeonChest(chest))
                {
                    continue;
                }

                // Roughly half of Dungeon chests, so it stays a find.
                if (!WorldGen.genRand.NextBool(2))
                {
                    continue;
                }

                for (int slot = 0; slot < chest.item.Length; slot++)
                {
                    if (chest.item[slot].type == ItemID.None)
                    {
                        chest.item[slot].SetDefaults(
                            ModContent.ItemType<AncientBattery>());
                        chest.item[slot].stack = WorldGen.genRand.Next(2, 5);
                        placed++;
                        break;
                    }
                }
            }
        }

        // Sample the blocks around the chest for Dungeon bricks, robust to the exact style.
        private static bool IsDungeonChest(Chest chest)
        {
            int bricks = 0;

            for (int dx = -6; dx <= 6; dx += 3)
            {
                for (int dy = -6; dy <= 6; dy += 3)
                {
                    int x = chest.x + dx;
                    int y = chest.y + dy;

                    if (x < 0 || x >= Main.maxTilesX ||
                        y < 0 || y >= Main.maxTilesY)
                    {
                        continue;
                    }

                    Tile t = Main.tile[x, y];

                    if (!t.HasTile)
                    {
                        continue;
                    }

                    if (t.TileType == TileID.BlueDungeonBrick ||
                        t.TileType == TileID.GreenDungeonBrick ||
                        t.TileType == TileID.PinkDungeonBrick)
                    {
                        bricks++;
                    }
                }
            }

            return bricks >= 2;
        }
    }
}
